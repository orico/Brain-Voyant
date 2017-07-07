using System; 
using System.IO; 
using System.Text;
using libSVMWrapper; 
using System.Timers; 
using System.Diagnostics; 
using AsyncPipes;
using System.Text.RegularExpressions;
using System.Collections; 

namespace OriBrainLearnerCore
{
    /// <summary>
    ///  some functions that 
    /// </summary>
    public partial class RawReader
    {

        private void getFilePaths(string extension)
        {
            string WorkingDir = GuiPreferences.Instance.WorkDirectory;
            string FileName = GuiPreferences.Instance.FileName;
            //find how many files are there
            string[] filePaths = Directory.GetFiles(WorkingDir, extension);
            Preferences.Instance.filesInWorkingDir = new string[filePaths.Length];
            Array.Copy(filePaths, Preferences.Instance.filesInWorkingDir, filePaths.Length);
        }

        public void loadSVM()
        {
            Preferences.Instance.ProblemOriginal.Reload(GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + ".libsvm");
            GuiPreferences.Instance.setLog("Import Succeeded: Loaded SVM file");
        }

        // short int **tGetTimeCourseData() 
        // save current volume to disk - we store first mini-header of dims (3 x 4-byte int) followed by volume (dim_xyz x 2-byte short int)
        // ofstream vdatfile;
        // vdatfile.open(vdat_filename, ofstream::out | ofstream::binary | ofstream::trunc);
        // vdatfile.write((char *)&dim_x, 4*sizeof(unsigned char));
        // vdatfile.write((char *)&dim_y, 4*sizeof(unsigned char));
        // vdatfile.write((char *)&dim_z, 4*sizeof(unsigned char));
        // vdatfile.write((char *)TimeCourseData, dim_xyz*2*sizeof(unsigned char));
        public void loadRawData()
        {
            string fileName;
            int i;
            byte[] buffer; //will be of size because each two bytes are one short new byte[dim_xyz * 2]; 

            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event     
            trPosition = 0;
            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
            GC.Collect();

            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;

            //loading just the amount of data we need, so we assign accordingly, not this>Program.mainForm.getMaxAvailableTrs()
            Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(totalToProcess);

            //progress bar stuff            
            // DLL GUI Seperation
            //Program.mainForm.progressBar1.Maximum = totalToProcess;
            GuiPreferences.Instance.ProgressBarMax = totalToProcess;

            //go over all the files in the dir            
            for (i = GuiPreferences.Instance.FromTR; i <= GuiPreferences.Instance.ToTR; i++)
            {
                //report progress      
                // DLL GUI Seperation
                //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1 ;
                GuiPreferences.Instance.ProgressBarValue = i - GuiPreferences.Instance.FromTR + 1;

                fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + i.ToString() + ".vdat";
                BinaryReader binReader = new BinaryReader(File.Open(fileName, FileMode.Open));
                Preferences.Instance.dim_x = binReader.ReadInt32();
                Preferences.Instance.dim_y = binReader.ReadInt32();
                Preferences.Instance.dim_z = binReader.ReadInt32();
                int dim_xyz = Preferences.Instance.dim_x * Preferences.Instance.dim_y * Preferences.Instance.dim_z;
                buffer = new byte[dim_xyz * 2];
                binReader.Read(buffer, 0, dim_xyz * 2);

                short[] data = new short[dim_xyz];
                System.Buffer.BlockCopy(buffer, 0, data, 0, buffer.Length);

                //the first brain will be used as the brain in the plot
                if (i == GuiPreferences.Instance.FromTR)
                {
                    Preferences.Instance.BrainRawValueForPlot = new short[dim_xyz];
                    System.Buffer.BlockCopy(buffer, 0, Preferences.Instance.BrainRawValueForPlot, 0, buffer.Length);
                }

                int _bufferLength = buffer.Length;
                buffer = null;
                short _threshold = Int16.Parse(GuiPreferences.Instance.NudThreshold.ToString());
                int _label = getLabelImport(i);
                int nonFiltered = Preferences.Instance.ProblemOriginal.AddSample(data, data.Length, _label, _threshold);
                GuiPreferences.Instance.setLog("Vector " + i.ToString() + " Total Bytes:" + _bufferLength.ToString() + " x: " + Preferences.Instance.dim_x.ToString() + " y: " + Preferences.Instance.dim_y.ToString() + " z: " + Preferences.Instance.dim_z.ToString() + " added: " + nonFiltered.ToString());
            }
        }

        //just assigns labels according to the chosen classes.
        public int getLabelClasses(int i)
        {
            IntIntStr e = Preferences.Instance.events.eventList[i - 1];
            if (GuiPreferences.Instance.CbMultiClassChecked)
            {
                return GuiPreferences.Instance.indexOfLabel(e.var3) + 1;
            }
            else
            {
                if (GuiPreferences.Instance.indexOfLabel(e.var3) == GuiPreferences.Instance.indexOfSlectedLabel1())
                    return 1;
                else if (GuiPreferences.Instance.indexOfLabel(e.var3) == GuiPreferences.Instance.indexOfSlectedLabel2())
                    return -1;
                else GuiPreferences.Instance.setLog("label failed: unknown label in binary mode");

            }
            return 999;
        }

        //double *tGetBetaMaps()
        //bdatfile.write((char *)&n_predictors_current, 4*sizeof(unsigned char));
        //bdatfile.write((char *)&dim_x, 4*sizeof(unsigned char));
        //bdatfile.write((char *)&dim_y, 4*sizeof(unsigned char));
        //bdatfile.write((char *)&dim_z, 4*sizeof(unsigned char));
        //bdatfile.write((char *)BetasData, n_predictors_current*dim_xyz*8*sizeof(unsigned char)); // "8" since betas are stored in double precision
        public void loadBetaData()
        {
            string fileName;
            int i;
            byte[] buffer; //will be of size because each two bytes are one short new byte[dim_xyz * 2];
            byte[] dimConversionArray;
            int n_predictors_current;

            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event
            trPosition = 0;

            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(GuiPreferences.Instance.getMaxAvailableTrs());

            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;
            //Program.mainForm.progressBar1.Maximum = totalToProcess;
            GuiPreferences.Instance.ProgressBarMax = totalToProcess;

            //this is how you work with background worker!
            //http://www.dotnetperls.com/backgroundworker
            //Program.mainForm.backgroundWorker1.RunWorkerAsync();

            //go over all the files in the dir
            for (i = GuiPreferences.Instance.FromTR; i <= GuiPreferences.Instance.ToTR; i++)
            {
                //report progress
                //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1;
                GuiPreferences.Instance.ProgressBarValue = i - GuiPreferences.Instance.FromTR + 1;

                fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + i.ToString() + ".bdat";
                buffer = File.ReadAllBytes(fileName);
                dimConversionArray = new byte[4];
                dimConversionArray[0] = buffer[0];
                dimConversionArray[1] = buffer[1];
                dimConversionArray[2] = buffer[2];
                dimConversionArray[3] = buffer[3];
                n_predictors_current = BitConverter.ToInt32(dimConversionArray, 0);
                dimConversionArray[0] = buffer[4];
                dimConversionArray[1] = buffer[5];
                dimConversionArray[2] = buffer[6];
                dimConversionArray[3] = buffer[7];
                Preferences.Instance.dim_x = BitConverter.ToInt32(dimConversionArray, 0);
                dimConversionArray[0] = buffer[8];
                dimConversionArray[1] = buffer[9];
                dimConversionArray[2] = buffer[10];
                dimConversionArray[3] = buffer[11];
                Preferences.Instance.dim_y = BitConverter.ToInt32(dimConversionArray, 0);
                dimConversionArray[0] = buffer[12];
                dimConversionArray[1] = buffer[13];
                dimConversionArray[2] = buffer[14];
                dimConversionArray[3] = buffer[15];
                Preferences.Instance.dim_z = BitConverter.ToInt32(dimConversionArray, 0);
                GuiPreferences.Instance.setLog("Vector " + i.ToString() + " Total Bytes:" + buffer.Length.ToString() + " predictors: " + n_predictors_current.ToString() + " x: " + Preferences.Instance.dim_x.ToString() + " y: " + Preferences.Instance.dim_y.ToString() + " z: " + Preferences.Instance.dim_z.ToString());
                int dim_xyz = Preferences.Instance.dim_x * Preferences.Instance.dim_y * Preferences.Instance.dim_z;
                double[] data = new double[dim_xyz * n_predictors_current];
                System.Buffer.BlockCopy(buffer, 16, data, 0, buffer.Length - 16);
                buffer = null;
                //int _threshold = Int32.Parse(Program.mainForm.nudBetaThreshold.Value.ToString()); 
                int _threshold = Int32.Parse(GuiPreferences.Instance.NudBetaThreshold.ToString());
                int _label = getLabelImport(i);
                double[] betaPredictorData = new double[dim_xyz];
                //!!!!! verify this block copy calculation!!!!!!
                System.Buffer.BlockCopy(data, (_label - 1) * dim_xyz, betaPredictorData, 0, dim_xyz);
                data = null;
                //!!!!! verify this block copy calculation!!!!!!
                Preferences.Instance.ProblemOriginal.AddSample(betaPredictorData, betaPredictorData.Length, _label, _threshold);
            }
        }

        //float *tGetContrastMaps()
        //mdatfile.write((char *)&n_contrast_maps, 4*sizeof(unsigned char));
        //mdatfile.write((char *)&dim_x, 4*sizeof(unsigned char));
        //mdatfile.write((char *)&dim_y, 4*sizeof(unsigned char));
        //mdatfile.write((char *)&dim_z, 4*sizeof(unsigned char));
        //mdatfile.write((char *)MapsData, n_contrast_maps*dim_xyz*4*sizeof(unsigned char));
        public void loadTContrastData()
        {
            string fileName;
            int i;
            byte[] buffer; //will be of size because each two bytes are one short new byte[dim_xyz * 2];
            byte[] dimConversionArray;
            int dim_x, dim_y, dim_z;
            int n_contrast_maps;

            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event
            trPosition = 0;

            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(GuiPreferences.Instance.getMaxAvailableTrs());

            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;
            // DLL GUI
            //Program.mainForm.progressBar1.Maximum = totalToProcess;
            GuiPreferences.Instance.ProgressBarMax = totalToProcess;

            //go over all the files in the dir
            for (i = GuiPreferences.Instance.FromTR; i <= GuiPreferences.Instance.ToTR; i++)
            {
                //report progress
                //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1;                
                GuiPreferences.Instance.ProgressBarValue = i - GuiPreferences.Instance.FromTR + 1;

                fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + i.ToString() + ".mdat";
                buffer = File.ReadAllBytes(fileName);
                dimConversionArray = new byte[4];
                dimConversionArray[0] = buffer[0];
                dimConversionArray[1] = buffer[1];
                dimConversionArray[2] = buffer[2];
                dimConversionArray[3] = buffer[3];
                n_contrast_maps = BitConverter.ToInt32(dimConversionArray, 0);
                dimConversionArray[0] = buffer[4];
                dimConversionArray[1] = buffer[5];
                dimConversionArray[2] = buffer[6];
                dimConversionArray[3] = buffer[7];
                dim_x = BitConverter.ToInt32(dimConversionArray, 0);
                dimConversionArray[0] = buffer[8];
                dimConversionArray[1] = buffer[9];
                dimConversionArray[2] = buffer[10];
                dimConversionArray[3] = buffer[11];
                dim_y = BitConverter.ToInt32(dimConversionArray, 0);
                dimConversionArray[0] = buffer[12];
                dimConversionArray[1] = buffer[13];
                dimConversionArray[2] = buffer[14];
                dimConversionArray[3] = buffer[15];
                dim_z = BitConverter.ToInt32(dimConversionArray, 0);
                GuiPreferences.Instance.setLog("Vector " + i.ToString() + " Total Bytes:" + buffer.Length.ToString() + " contrast maps: " + n_contrast_maps.ToString() + " x: " + dim_x.ToString() + " y: " + dim_y.ToString() + " z: " + dim_z.ToString());
                int dim_xyz = dim_x * dim_y * dim_z;
                float[] data = new float[dim_xyz * n_contrast_maps];
                System.Buffer.BlockCopy(buffer, 16, data, 0, buffer.Length - 16);
                buffer = null;
                //int _threshold = Int32.Parse(Program.mainForm.nudTConThreshold.Value.ToString());
                int _threshold = Int32.Parse(GuiPreferences.Instance.NudTConThreshold.ToString());
                int _label = getLabelImport(i);
                double[] TContrastMapPredictorData = new double[dim_xyz];
                //!!!!! verify this block copy calculation!!!!!!
                System.Buffer.BlockCopy(data, (_label - 1) * dim_xyz, TContrastMapPredictorData, 0, dim_xyz);
                data = null;
                //!!!!! verify this block copy calculation!!!!!!
                Preferences.Instance.ProblemOriginal.AddSample(TContrastMapPredictorData, TContrastMapPredictorData.Length, _label, _threshold);
            }
        }


        public void loadRawDataUsing_UDP(IAsyncResult ar)
        {

            if (shouldStop)
                return;

            byte[] data = Preferences.Instance.udp.EndReceive(ar);

            if (data.Length > 0)
            {
                string text = Encoding.ASCII.GetString(data);
                int loc1 = text.LastIndexOf("\\");
                int loc2 = text.LastIndexOf(".");
                int loc3 = text.LastIndexOf("-");
                int loc4 = text.LastIndexOf(",");

                if (Preferences.Instance.ProblemOriginal.samples == null)
                {
                    Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
                    GC.Collect();
                    int totalToProcess = Int32.Parse(text.Substring(loc4 + 1, text.Length - (loc4 + 1)));
                    GuiPreferences.Instance.ProgressBarMax = totalToProcess;
                    //loading just the amount of data we need, so we assign accordingly, not this>Program.mainForm.getMaxAvailableTrs()
                    Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(totalToProcess);
                }

                string path = text.Substring(0, loc1 + 1);
                fileName = text.Substring(loc1 + 1, loc2 - loc1 - 1);
                last_index = Int32.Parse(text.Substring(loc3 + 1, loc2 - loc3 - 1));

                if (last_index <= GuiPreferences.Instance.ToTR)
                {
                    //report progress      
                    // DLL GUI Seperation
                    //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1 ;
                    GuiPreferences.Instance.ProgressBarValue = last_index;
                    fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + last_index.ToString() + ".vdat";
                    loadRawFileName(fileName);
                    Training_OLDProcessing.OfflineProcess();
                    //Preferences.Instance.pipeClient.Send(fileName);
                    GuiPreferences.Instance.setLog("Sending Vector " + last_index.ToString());
                    last_index++;
                }
                else shouldStop = true;


                //updategui();
                // shoot event 
                //send the recieved data somewhere 
            }

            if (!shouldStop)
                Preferences.Instance.udp.BeginReceive();
        }

        public void loadRawDataUsingPipes_SendDataTimer()
        {
            string fileName = "";
            byte[] buffer = null; //will be of size because each two bytes are one short new byte[dim_xyz * 2]; 
            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event     
            trPosition = 0;
            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
            GC.Collect();

            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;

            //loading just the amount of data we need, so we assign accordingly, not this>Program.mainForm.getMaxAvailableTrs()
            Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(totalToProcess);

            //progress bar stuff            
            // DLL GUI Seperation
            //Program.mainForm.progressBar1.Maximum = totalToProcess;
            GuiPreferences.Instance.ProgressBarMax = totalToProcess;

            t = new Timer(2000);
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            last_index = GuiPreferences.Instance.FromTR;
            t.Start();
            //go over all the files in the dir            
            for (int i = GuiPreferences.Instance.FromTR; i <= GuiPreferences.Instance.ToTR; i++)
            {

            }
        }

        private Timer t;
        private int last_index = -1;

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (last_index <= GuiPreferences.Instance.ToTR)
            {
                //report progress      
                // DLL GUI Seperation
                //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1 ;
                GuiPreferences.Instance.ProgressBarValue = last_index - GuiPreferences.Instance.FromTR + 1;

                string fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + last_index.ToString() + ".vdat";
                Preferences.Instance.pipeClient.Send(fileName);
                GuiPreferences.Instance.setLog("Sending Vector " + last_index.ToString());
                last_index++;
            }
            else
            {
                t.Stop();
            }
        }

        public void loadRawDataUsingPipes_ReceiveData(object sender, MessageEventArgs args)
        {
            string fileName = "";
            IMessage mess = AsyncPipes.MessageSerializers.DeserializeMessage(args.Message);
            if (mess.MessageType == typeof(String))
            {
                fileName = System.Text.Encoding.UTF8.GetString(mess.Payload);
                loadRawFileName(fileName);
            }
            else
            {
                GuiPreferences.Instance.setLog("filename not of type string");
            }
        }

        public string GetFileProcessName(string filePath)
        {

            Process[] procs = Process.GetProcesses();
            string fileName = Path.GetFileName(filePath);

            foreach (Process proc in procs)
            {
                if (proc.MainWindowHandle != new IntPtr(0) && !proc.HasExited)
                {
                    ProcessModule[] arr = new ProcessModule[proc.Modules.Count];
                    foreach (ProcessModule pm in proc.Modules)
                    {
                        if (pm.ModuleName == fileName)
                            return proc.ProcessName;
                    }
                }
            }


            return "unknown";
        }

        public void getprocname(string fileName)
        {
            Process tool = new Process();
            tool.StartInfo.FileName = "handle.exe";
            tool.StartInfo.Arguments = fileName + " /accepteula";
            tool.StartInfo.UseShellExecute = false;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.Start();
            tool.WaitForExit();
            string outputTool = tool.StandardOutput.ReadToEnd();

            string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            foreach (Match match in Regex.Matches(outputTool, matchPattern))
            {
                GuiPreferences.Instance.setLog(Process.GetProcessById(int.Parse(match.Value)).ToString());//.Kill();
            }
        }

        /// <summary>
        /// gets the current working processes (doesnt work)
        /// </summary>
        /// <param name="strFile"></param>
        private void getFileProcesses(string strFile)
        {
            ArrayList myProcessArray = new ArrayList();
            Process myProcess;

            myProcessArray.Clear();
            Process[] processes = Process.GetProcesses();
            int i = 0;
            for (i = 0; i <= processes.GetUpperBound(0) - 1; i++)
            {
                myProcess = processes[i];
                //if (!myProcess.HasExited) //This will cause an "Access is denied" error
                if (myProcess.Threads.Count > 0)
                {
                    try
                    {
                        ProcessModuleCollection modules = myProcess.Modules;
                        int j = 0;
                        for (j = 0; j <= modules.Count - 1; j++)
                        {
                            if ((modules[j].FileName.ToLower().CompareTo(strFile.ToLower()) == 0))
                            {
                                myProcessArray.Add(myProcess);
                                break;
                                // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        //MsgBox(("Error : " & exception.Message)) 
                    }
                }
            }

            GuiPreferences.Instance.setLog(myProcessArray.Count.ToString());
            foreach (Process p in myProcessArray)
            {
                GuiPreferences.Instance.setLog(p.ProcessName);
            }
        } 

        public void loadRawDataText()
        {
            string fileName;
            int i;
            byte[] buffer; //will be of size because each two bytes are one short new byte[dim_xyz * 2];
            char[] dimConversionArray;

            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event     
            trPosition = 0;
            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
            GC.Collect();
            Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(GuiPreferences.Instance.getMaxAvailableTrs());

            //progress bar stuff
            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;
            // DLL GUI Separation
            GuiPreferences.Instance.ProgressBarMax = totalToProcess;

            //go over all the files in the dir            
            for (i = GuiPreferences.Instance.FromTR; i <= GuiPreferences.Instance.ToTR; i++)
            {
                //report progress 
                // DLL GUI Seperation
                //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1;
                GuiPreferences.Instance.ProgressBarValue = i - GuiPreferences.Instance.FromTR + 1;


                fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + i.ToString() + ".vdat";
                //string text = File.ReadAllText(fileName);
                buffer = File.ReadAllBytes(fileName);
                char[] charList = System.Text.Encoding.UTF8.GetChars(buffer);
                dimConversionArray = new char[4];
                dimConversionArray[0] = charList[0];
                dimConversionArray[1] = charList[1];
                dimConversionArray[2] = charList[2];
                dimConversionArray[3] = charList[3];
                Preferences.Instance.dim_x = Convert.ToInt32(charList[0] + charList[1] + charList[2] + charList[3]);
                dimConversionArray[0] = charList[4];
                dimConversionArray[1] = charList[5];
                dimConversionArray[2] = charList[6];
                dimConversionArray[3] = charList[7];
                Preferences.Instance.dim_y = Convert.ToInt32(charList[4] + charList[5] + charList[6] + charList[7]);
                dimConversionArray[0] = charList[8];
                dimConversionArray[1] = charList[9];
                dimConversionArray[2] = charList[10];
                dimConversionArray[3] = charList[11];
                Preferences.Instance.dim_z = Convert.ToInt32(charList[8] + charList[9] + charList[10] + charList[11]);
                GuiPreferences.Instance.setLog("Vector " + i.ToString() + " Total Bytes:" + buffer.Length.ToString() + " x: " + Preferences.Instance.dim_x.ToString() + " y: " + Preferences.Instance.dim_y.ToString() + " z: " + Preferences.Instance.dim_z.ToString());
                int dim_xyz = Preferences.Instance.dim_x * Preferences.Instance.dim_y * Preferences.Instance.dim_z;
                short[] data = new short[dim_xyz];
                System.Buffer.BlockCopy(charList, 12, data, 0, charList.Length - 12);
                buffer = null;
                // DLL GUI
                //short _threshold = Int16.Parse(Program.mainForm.nudThreshold.Value.ToString()); 
                short _threshold = Int16.Parse(GuiPreferences.Instance.NudThreshold.ToString());
                int _label = getLabelImport(i);
                //will add from index 0 into samples array, even if real index is 300.
                Preferences.Instance.ProblemOriginal.AddSample(data, data.Length, _label, _threshold);
            }
        }


        public void loadRawDataUsingPipes_SendData()
        {
            string fileName = "";
            byte[] buffer = null; //will be of size because each two bytes are one short new byte[dim_xyz * 2]; 
            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event     
            trPosition = 0;
            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
            GC.Collect();

            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;

            //loading just the amount of data we need, so we assign accordingly, not this>Program.mainForm.getMaxAvailableTrs()
            Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(totalToProcess);

            //progress bar stuff            
            // DLL GUI Seperation
            //Program.mainForm.progressBar1.Maximum = totalToProcess;
            GuiPreferences.Instance.ProgressBarMax = totalToProcess;

            //go over all the files in the dir            
            for (int i = GuiPreferences.Instance.FromTR; i <= GuiPreferences.Instance.ToTR; i++)
            {
                //report progress      
                // DLL GUI Seperation
                //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1 ;
                GuiPreferences.Instance.ProgressBarValue = i - GuiPreferences.Instance.FromTR + 1;

                fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + i.ToString() + ".vdat";
                Preferences.Instance.pipeClient.Send(fileName);
                GuiPreferences.Instance.setLog("Sending Vector " + i.ToString());
                //System.Threading.Thread.Sleep(2000);
            }
        }

    }
}
