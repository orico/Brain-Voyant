using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using libSVMWrapper;
using System.Drawing.Imaging;


namespace OriBrainLearnerCore
{
    public partial class RawReader
    { 
        libSVM_Parameter Parameter = new libSVM_Parameter();
        libSVM_Grid grid = new libSVM_Grid(); // C = 1.0 there is no grid search.
        int trPosition = 0;
        private double MeanAverage = 0;

        public double getMeanAverage()
        {
            return MeanAverage / Preferences.Instance.events.EventListLastTr;
        }

        public RawReader()
        { 
            Parameter.svm_type = SVM_TYPE.C_SVC;
            Parameter.kernel_type = KERNEL_TYPE.LINEAR;
            //var form = Form.ActiveForm as Form1;
            //form.TbSVMLog = "binary file reader->raw value reader supports usage of from-to-TR. if from=300 then its at. samples[i=0]";

        }

        //just assigns labels according to the event the TR is at.
        public int getLabelImport(int j)
        {
            IntIntStr e;
            for (int i = trPosition; i < Preferences.Instance.events.eventList.Count; i++)
            {
                e = Preferences.Instance.events.eventList[i];
                if ((j >= e.var1) && (j <= e.var2))
                    return GuiPreferences.Instance.indexOfLabel(e.var3) + 1;
            }
            return 9999999;
        }

        /// <summary>
        /// This method is called when training, loads many raw data runs into problem using treeview and datagridview
        /// </summary>
        public void loadRawDataMultiRun(string folder)
        {
            string fileName;
            int i;
            byte[] buffer; //will be of size because each two bytes are one short new byte[dim_xyz * 2]; 

            // we keep a position so that we dont need to always search from scratch the current TR in the eventlabel list to know the event     
            trPosition = 0;
            //prepare labels and dictionary of the amount of files from this filetype.
            Preferences.Instance.ProblemOriginalMulti.Add(new libSVM_ExtendedProblem());
            GC.Collect();

            int totalToProcess = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;

            //loading just the amount of data we need, so we assign accordingly, not this>Program.mainForm.getMaxAvailableTrs()
            Preferences.Instance.ProblemOriginalMulti[Preferences.Instance.ProblemOriginalMulti.Count - 1].PrepareSamplesLabelsSize(totalToProcess);

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
                //Preferences.Instance.currentClassifiedVector = i;
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
                
                //init minmax vectors once for each run. placed here because dimxyz is only known first at this point.
                
                //brain size including class and taking into account that its 1-based. zero cell is ignored.
                int relativeTR = Preferences.Instance.events.findConditionsRelativeTrBasedOnTr(i);

                //add new relative tr with an empty minmax.
                if (relativeTR!=-1 && !Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1].ContainsKey(relativeTR))
                {
                    Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1].Add(relativeTR, new MinMax(new double[dim_xyz + 2], new double[dim_xyz + 2]));
                }
                else
                {
                    int unknown  = 0;
                } 

                int nonFiltered = Preferences.Instance.ProblemOriginalMulti[Preferences.Instance.ProblemOriginalMulti.Count - 1].AddSample(data, data.Length, _label, _threshold);
                GuiPreferences.Instance.setLog("Vector " + i.ToString() + " Total Bytes:" + _bufferLength.ToString() + " x: " + Preferences.Instance.dim_x.ToString() + " y: " + Preferences.Instance.dim_y.ToString() + " z: " + Preferences.Instance.dim_z.ToString() + " added: " + nonFiltered.ToString());
            }
        }

        /// <summary>
        /// this method is called back in real-time testing and will load a .vdat file 
        /// </summary>
        /// <param name="ar"></param>
        public void loadRawDataMultiRun_Pipe(int x, int y, int z, byte[] data, int current, int total)
        {

            if (shouldStop)
                return;
             
            if (data.Length > 0)
            {
                if (Preferences.Instance.ProblemOriginal.samples == null)
                {
                    Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
                    GC.Collect(); 
                    GuiPreferences.Instance.ProgressBarMax = total;
                    Preferences.Instance.ProblemOriginal.PrepareSamplesLabelsSize(total);  
                }


                if (current <= Preferences.Instance.events.EventListLastTr)
                {
                    bool corrupted = false;
                    GuiPreferences.Instance.ProgressBarValue = current;
                    ProcessRawBuffer(x, y, z, data, current);
                    RealTimeProcessing.RealTimeProcess(); 
                    GuiPreferences.Instance.setLog("Processing Vector " + current.ToString()); 
                }
                else shouldStop = true;
                 
            }

            if (!shouldStop)
                Preferences.Instance.udp.BeginReceive();
        }

        //processing the data sent from tbv
        public void ProcessRawBuffer(int x, int y, int z, byte[] buffer, int current)
        {
            lock (this)
            {
                TrainingTesting_SharedVariables.time = DateTime.Now.TimeOfDay.TotalMilliseconds;
                GuiPreferences.Instance.setLog("received TR:" + current);
                Preferences.Instance.cumulativeTR = current;

                Preferences.Instance.dim_x = x;
                Preferences.Instance.dim_y = y;
                Preferences.Instance.dim_z = z;
                int dim_xyz = Preferences.Instance.dim_x * Preferences.Instance.dim_y * Preferences.Instance.dim_z;

                short[] data = new short[dim_xyz];
                System.Buffer.BlockCopy(buffer, 0, data, 0, buffer.Length);

                //the first brain will be used as the brain in the plot
                if (current == GuiPreferences.Instance.FromTR)
                {
                    Preferences.Instance.BrainRawValueForPlot = new short[dim_xyz];
                    System.Buffer.BlockCopy(buffer, 0, Preferences.Instance.BrainRawValueForPlot, 0, buffer.Length);
                }

                int _bufferLength = buffer.Length;
                buffer = null;
                short _threshold = Int16.Parse(GuiPreferences.Instance.NudThreshold.ToString());
                int _label = getLabelImport(current);

                //init minmax vectors once for each run. placed here because dimxyz is only known first at this point.

                //brain size including class and taking into account that its 1-based. zero cell is ignored.
                int relativeTR = Preferences.Instance.events.findConditionsRelativeTrBasedOnTr(current);

                //add new relative tr with an empty minmax.
                if (relativeTR != -1 && !Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1].ContainsKey(relativeTR))
                {
                    Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1].Add(relativeTR, new MinMax(new double[dim_xyz + 2], new double[dim_xyz + 2]));
                }
                else
                {
                    int unknown = 0;
                } 

                //add sample if not corrupted (the check is inside add sample)
                int nonFiltered = Preferences.Instance.ProblemOriginal.AddSample(data, data.Length, _label, _threshold);

                Preferences.Instance.currentUDPVector++;

                if (Preferences.Instance.currentUDPVector!=current)
                {
                    GuiPreferences.Instance.setLog("incremented currentUDPVector != current (from pipe)");
                }

                GuiPreferences.Instance.setLog("Vector " + current.ToString() + " Total Bytes:" + _bufferLength.ToString() + " x: " + Preferences.Instance.dim_x.ToString() + " y: " + Preferences.Instance.dim_y.ToString() + " z: " + Preferences.Instance.dim_z.ToString() + " added: " + nonFiltered.ToString());

            }
        } 

        public  bool shouldStop = false;
        private string fileName;

        /// <summary>
        /// this method is called back in real-time testing and will load a .vdat file 
        /// </summary>
        /// <param name="ar"></param>
        public void loadRawDataMultiRun_UDP(IAsyncResult ar)
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

                if (last_index <= Preferences.Instance.events.EventListLastTr)
                {
                    //report progress      
                    // DLL GUI Seperation
                    //Program.mainForm.progressBar1.Value = i - GuiPreferences.Instance.FromTR + 1 ;
                    GuiPreferences.Instance.ProgressBarValue = last_index;
                    fileName = GuiPreferences.Instance.WorkDirectory + GuiPreferences.Instance.FileName + last_index.ToString() + ".vdat";
                    loadRawFileName(fileName);
                    //PublicMethods.ProcessMultiRuns(null); 
                    RealTimeProcessing.RealTimeProcess();
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

        public void loadRawFileName(string fileName)
        {
            lock (this)
            {
                TrainingTesting_SharedVariables.time = DateTime.Now.TimeOfDay.TotalMilliseconds;
                byte[] buffer = null;
                int i = -1;
                int _locationLastHyphen = fileName.LastIndexOf("-");
                i = Convert.ToInt32(fileName.Substring(_locationLastHyphen + 1, fileName.LastIndexOf(".") - (_locationLastHyphen + 1)));
                GuiPreferences.Instance.setLog("received " + fileName);
                BinaryReader binReader = null;
                Exception filenotfound = null;
                bool doLoop = true;

                /* //will try to see if the files are in the directory before, but wont see them.
                try
                {
                    DirectoryInfo di = new DirectoryInfo(@"U:\ori_rtp");
                    GuiPreferences.Instance.setLog(di.GetFiles().Length.ToString());
                    for (int t = 0; t < di.GetFiles().Length; t++)
                    {
                        GuiPreferences.Instance.setLog(di.GetFiles()[t].ToString());
                    }

                }
                catch (Exception e)
                {
                    GuiPreferences.Instance.setLog(e.ToString());
                }
                */

                //brute force attempt at opening the file. this code was placed here in order to bypass a filenotfound and other io exceptions.
                //probably due to some process holding the tirosh-?.vdat files for a few milliseconds


                while (doLoop)
                {
                    try
                    {
                        binReader = new BinaryReader(File.Open(fileName, FileMode.Open));
                        doLoop = false;
                    }
                    catch (Exception e)
                    {
                        getFileProcesses(fileName);
                        //GuiPreferences.Instance.setLog("File not found! not adding sample" + e.ToString());
                        //Preferences.Instance.ProblemOriginal.AddSample(new short[Preferences.Instance.dim_x * Preferences.Instance.dim_y * Preferences.Instance.dim_z], 0, -1, Int16.Parse(GuiPreferences.Instance.NudThreshold.ToString()));
                        Preferences.Instance.currentUDPVector++;

                        //       return;
                    }
                }
                if (binReader == null)
                {
                    return;
                }
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
                Preferences.Instance.currentUDPVector++;

                GuiPreferences.Instance.setLog("Vector " + i.ToString() + " Total Bytes:" + _bufferLength.ToString() + " x: " + Preferences.Instance.dim_x.ToString() + " y: " + Preferences.Instance.dim_y.ToString() + " z: " + Preferences.Instance.dim_z.ToString() + " added: " + nonFiltered.ToString());

            }
        } 

        public void setCGridSearchLong()
        {
            grid = libSVM_Grid.C();
        }

        public void setGammaGridSearchLong()
        {
            grid = libSVM_Grid.gamma();
        }

        public void setPGridSearchLong()
        {
            grid = libSVM_Grid.p();
        }

        public void setNuGridSearchLong()
        {
            grid = libSVM_Grid.nu();
        }

        public void setCoef0GridSearchLong()
        {
            grid = libSVM_Grid.coef0();
        }

        public void setDegreeGridSearchLong()
        {
            grid = libSVM_Grid.degree();
        }

        //svm.TrainAuto(10, Problem, Parameter, libSVM_Grid.C(), libSVM_Grid.gamma(), libSVM_Grid.p(), libSVM_Grid.nu(), libSVM_Grid.coef0(), libSVM_Grid.degree());
    }
}
