using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms; 
using System.Reflection;
using LibSVMScale;
using libSVMWrapper;
using OriBrainLearnerCore;
using weka.core;
using TreeNode = System.Windows.Forms.TreeNode;


namespace OriBrainLearner
{
    public partial class Form1 : Form
    {
        //public Preferences pref;
        private AsyncNamedPipeServer _pipeServer;
        PipeLineTestFunctions t = new PipeLineTestFunctions();

        public Form1()
        {
            InitializeComponent();
        }

        public void UnhandledThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            this.HandleUnhandledException(e.Exception);
        }

        public void HandleUnhandledException(Exception e)
        {
            // do what you want here.
            if (MessageBox.Show(e.ToString() + "An unexpected error has occurred. Continue?",
                "My application", MessageBoxButtons.YesNo, MessageBoxIcon.Stop,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pipeServer != null)
            {
                if (!_pipeServer.isEmpty())
                {
                    _pipeServer.PipeMessage -= new DelegateMessage(MessageHandler);
                    _pipeServer.DataUpdateEvent -= new EventHandler<DataChangeEventArg>(Instance_DataUpdateEvent);
                    _pipeServer = null;
                }
            }
        }

        /// <summary>
        /// handle async pipe call back output messeges
        /// </summary>
        /// <param name="message"></param>
        private void MessageHandler(string message)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new DelegateMessage(MessageHandler), message);
                }
                else
                {
                    TbSVMLog = message;
                }
            }
            catch (Exception ex)
            {

                if (this.InvokeRequired)
                {
                    this.Invoke(new DelegateMessage(MessageHandler), ex.Message);
                }
                else
                {
                    TbSVMLog = message;
                } 
            } 
        }

        /// <summary>
        /// handle async pipe call back data 
        /// </summary>
        /// <param name="message"></param>
        private void PipeDataHandler(byte[] data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new DelegateBuffer(PipeDataHandler), data);
                }
                else
                {
                   // PublicMethods.binary.loadRawDataMultiRun_Pipe(data);
                }
            } 
            catch (Exception ex)
            { 
                this.Invoke(new DelegateMessage(MessageHandler), ex.Message); 
            }
        }  

        private void Instance_DataUpdateEvent(object sender, DataChangeEventArg t)
        {
            if (InvokeRequired)
            {
                Invoke(new Instance_DataUpdateEventDelegate(Instance_DataUpdateEvent), new object[2] { sender, t });
            }
            else
            {
                TrainingTesting_SharedVariables.binary.loadRawDataMultiRun_Pipe(t._x, t._y, t._z, t._data, t._currentTR, t._totalTRs);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (Preferences.Instance == null)
            {
                TbSVMLog = "===> null pref";
            }

            //testSVM.testSVMWrapperPackage(); 

            nudFromTR.Value = GuiPreferences.Instance.FromTR;
            nudFromTR.Minimum = GuiPreferences.Instance.MinTR;
            nudFromTR.Maximum = GuiPreferences.Instance.MaxTR;

            nudToTR.Value = GuiPreferences.Instance.ToTR;
            nudToTR.Minimum = GuiPreferences.Instance.MinTR;
            nudToTR.Maximum = GuiPreferences.Instance.MaxTR;
            
            // events for window 1.            
            GuiPreferences.Instance.FileNameUpdateEvent += new EventHandler(Instance_FileNameUpdateEvent);
            GuiPreferences.Instance.workDirectoryUpdateEvent += new EventHandler(Instance_workDirectoryUpdateEvent);
            GuiPreferences.Instance.ProtocolFileUpdateEvent += new EventHandler(Instance_ProtocolFileUpdateEvent);
            GuiPreferences.Instance.contrastFileUpdateEvent += new EventHandler(Instance_contrastFileUpdateEvent);
            GuiPreferences.Instance.tbTRUpdateEvent += new EventHandler(Instance_tbTRUpdateEvent);
            
            // events for window 2.
            GuiPreferences.Instance.PreOnsetUpdateEvent += new EventHandler(Instance_PreOnsetUpdateEvent);
            GuiPreferences.Instance.PostOnsetUpdateEvent += new EventHandler(Instance_PostOnsetUpdateEvent);            
            GuiPreferences.Instance.TrUpdateEvent += new EventHandler<GuiPreferences.TrChangeEventArg>(Instance_TrUpdateEvent);

            // events for window 3.
            GuiPreferences.Instance.FileTypeUpdateEvent += new EventHandler(Instance_FileTypeUpdateEvent);

            GuiPreferences.Instance.nudBetaThresholdEvent +=new EventHandler(Instance_nudBetaThresholdEvent);
            GuiPreferences.Instance.nudTConThresholdEvent += new EventHandler(Instance_nudTConThresholdEvent);
            GuiPreferences.Instance.nudThresholdEvent += new EventHandler(Instance_nudThresholdEvent);
            GuiPreferences.Instance.nudSVMThresholdEvent += new EventHandler(Instance_nudSVMThresholdEvent);
            GuiPreferences.Instance.nudEyeSliceFirstLinesEvent += new EventHandler(Instance_nudEyeSliceFirstLinesEvent);
            GuiPreferences.Instance.nudFilterEyeSlicesEvent += new EventHandler(Instance_nudFilterEyeSlicesEvent);


            // events for window X.

            // events for window 4.
            GuiPreferences.Instance.NormalizationTypeUpdateEvent += new EventHandler(Instance_NormalizationTypeUpdateEvent);
            GuiPreferences.Instance.IGTypeUpdateEvent += new EventHandler(Instance_IGTypeUpdateEvent);
           
            GuiPreferences.Instance.unprocessed += new EventHandler(Instance_unprocessed);

            GuiPreferences.Instance.nudExtractFromTREvent += new EventHandler(Instance_nudExtractFromTREvent);
            GuiPreferences.Instance.nudExtractToTREvent += new EventHandler(Instance_nudExtractToTREvent);
            GuiPreferences.Instance.nudClassifyUsingTREvent += new EventHandler(Instance_nudClassifyUsingTREvent);
            GuiPreferences.Instance.nudIGThresholdEvent += new EventHandler(Instance_nudIGThresholdEvent);
            GuiPreferences.Instance.nudIGVoxelAmountEvent += new EventHandler(Instance_nudIGVoxelAmountEvent);
            GuiPreferences.Instance.nudMovingWindowEvent += new EventHandler(Instance_nudMovingWindowEvent);

            // events for window 5.
            GuiPreferences.Instance.featureProcessJobsUpdateEvent += new EventHandler(Instance_featureProcessJobsUpdateEvent);
            GuiPreferences.Instance.KvaluesUpdateEvent += new EventHandler(Instance_KvaluesUpdateEvent);

            // events for window 6.
            GuiPreferences.Instance.cbMultiClassCheckedEvent += new EventHandler(Instance_cbMultiClassCheckedEvent);
            GuiPreferences.Instance.cbOverwriteProcessedFilesCheckedEvent += new EventHandler(Instance_cbOverwriteProcessedFilesCheckedEvent);
            GuiPreferences.Instance.cbPeekHigherTRsIGCheckedEvent += new EventHandler(Instance_cbPeekHigherTRsIGCheckedEvent);
            
            GuiPreferences.Instance.cbSMOCheckedUpdateEvent += new EventHandler(Instance_cbSMOCheckedEvent);
            GuiPreferences.Instance.cbSVMCheckedUpdateEvent += new EventHandler(Instance_cbSVMCheckedEvent);
            GuiPreferences.Instance.cmbClass1Event += new EventHandler(Instance_cmbClass1Event);
            GuiPreferences.Instance.cmbClass2Event += new EventHandler(Instance_cmbClass2Event);
            GuiPreferences.Instance.labelsUpdateEvent += new EventHandler(Instance_labelsUpdateEvent);
            GuiPreferences.Instance.nudCVFoldsEvent += new EventHandler(Instance_nudCVFoldsEvent);
            GuiPreferences.Instance.nudGridFoldsEvent += new EventHandler(Instance_nudGridFoldsEvent);
            GuiPreferences.Instance.nudRFEFoldsEvent += new EventHandler(Instance_nudRFEFoldsEvent);
            GuiPreferences.Instance.nudTrainTestSplitEvent += new EventHandler(Instance_nudTrainTestSplitEvent);

            GuiPreferences.Instance.TrainTypeUpdateEvent += new EventHandler(Instance_TrainTypeUpdateEvent);

            // events for window 7.

            // events for window 8.
            GuiPreferences.Instance.ProgressBarUpdateEvent += new EventHandler<GuiPreferences.ProgressBarChangeEventArg>(Instance_ProgressBarUpdateEvent);            
            GuiPreferences.Instance.LoggerUpdateEvent += new EventHandler(Instance_LoggerUpdateEvent);

            GuiPreferences.Instance.LogMessage += new DelegateMessage(MessageHandler);

            //init fast vector, prepared for real-time testing as a base container for all possible

            initGuiVariables();
            initFolderTreeView();
            LoadUDPConfig();
            Preferences.Instance.udp.setLocal(LocalIP.Text, int.Parse(LocalPort.Text));
            Preferences.Instance.udp.setRemote(UnityIP.Text, int.Parse(UnityPort.Text));
            
            //if port is changed because one application is already open, add +1 to port and change gui.
            Preferences.Instance.udp.initUdpSocket();
            LocalPort.Text = Preferences.Instance.udp.localPortNumberStatic.ToString();
            //Preferences.Instance.udp.setLocal(LocalIP.Text, Preferences.Instance.udp.localPortNumberStatic);
            TbSVMLog = "WARNING: Increasing Local Port, because application is already open once. (port is:" + Preferences.Instance.udp.localPortNumberStatic.ToString() + ")";

        }

        void LoadUDPConfig()
        {
            string[] lines = File.ReadAllLines("udp.cfg");
            UnityIP.Text = lines[0].Split(',')[1];
            UnityPort.Text = lines[1].Split(',')[1];
            LocalIP.Text = lines[2].Split(',')[1];
            LocalPort.Text = lines[3].Split(',')[1]; 
        }
        
        void Instance_nudEyeSliceFirstLinesEvent(object sender, EventArgs e)
        {
            nudEyeSliceFirstLines.Value = GuiPreferences.Instance.NudEyeSliceFirstLines;
        }

        void Instance_nudFilterEyeSlicesEvent(object sender, EventArgs e)
        {
            nudFilterEyeSlices.Value = GuiPreferences.Instance.NudFilterEyeSlices;
        }

        void Instance_nudTrainTestSplitEvent(object sender, EventArgs e)
        {
            nudTrainTestSplit.Value = GuiPreferences.Instance.NudTrainTestSplit;
        }

        void Instance_cbMultiClassCheckedEvent(object sender, EventArgs e)
        {
            cbMultiClass.Checked = GuiPreferences.Instance.CbMultiClassChecked;   
        }

        void Instance_cbOverwriteProcessedFilesCheckedEvent(object sender, EventArgs e)
        {
            cbOverwriteProcessedFiles.Checked = GuiPreferences.Instance.CbOverwriteProcessedFilesChecked;
        }

        void Instance_cbPeekHigherTRsIGCheckedEvent(object sender, EventArgs e)
        {
            cbPeekHigherTRsIG.Checked = GuiPreferences.Instance.CbPeekHigherTRsIGChecked;
        } 

        void Instance_cbSVMCheckedEvent(object sender, EventArgs e)
        {
            cbSVM.Checked = GuiPreferences.Instance.CbSVMChecked;
        }
         
        void Instance_cbSMOCheckedEvent(object sender, EventArgs e)
        {
            cbSMO.Checked = GuiPreferences.Instance.CbSMOChecked;
        }

        void Instance_labelsUpdateEvent(object sender, EventArgs e)
        {
            List<string> temp = GuiPreferences.Instance.getLabels();
            cmbClass1.Items.Clear();
            cmbClass2.Items.Clear();
            for (int t = 0; t < temp.Count; t++)
            {
                cmbClass1.Items.Add(temp[t]);
                cmbClass2.Items.Add(temp[t]);
            }
            if (temp.Count > GuiPreferences.Instance.CmbClass1Selected)
            {
                cmbClass1.SelectedIndex = GuiPreferences.Instance.CmbClass1Selected;
            }

            if (temp.Count > GuiPreferences.Instance.CmbClass2Selected)
            {
                cmbClass2.SelectedIndex = GuiPreferences.Instance.CmbClass2Selected;
            }
        }
        
        void Instance_cmbClass2Event(object sender, EventArgs e)
        {
            cmbClass2.SelectedIndex = GuiPreferences.Instance.CmbClass2Selected;
        }

        void Instance_cmbClass1Event(object sender, EventArgs e)
        {
            cmbClass1.SelectedIndex = GuiPreferences.Instance.CmbClass1Selected;
        }

        void Instance_PostOnsetUpdateEvent(object sender, EventArgs e)
        {
            Program.mainForm.nudPostOnset.Value = GuiPreferences.Instance.PostOnset;
        }

        void Instance_PreOnsetUpdateEvent(object sender, EventArgs e)
        {
            Program.mainForm.nudPreOnset.Value = GuiPreferences.Instance.PreOnset;
        }

        void Instance_KvaluesUpdateEvent(object sender, EventArgs e)
        {
            List<int> temp = GuiPreferences.Instance.getKvalues();
            int si = lbK.SelectedIndex;
            lbK.Items.Clear();
            for (int t = 0; t < temp.Count; t++)
            {
                lbK.Items.Add(temp[t]);
            }
            lbK.SelectedIndex = si;
        }

        void Instance_featureProcessJobsUpdateEvent(object sender, EventArgs e)
        {
            List<string> temp = GuiPreferences.Instance.getFeatureProcessJobs();
            lbFeatureProcessFlow.Items.Clear();
            for (int t = 0; t < temp.Count; t++)
            {
                lbFeatureProcessFlow.Items.Add(temp[t]);
            }
        }

        void Instance_TrainTypeUpdateEvent(object sender, EventArgs e)
        {
            switch (GuiPreferences.Instance.TrainType)
            {
                case TrainingType.TrainTestSplit:
                    rbTrainTestSplit.Checked = true;
                    break;
                case TrainingType.CrossValidation:
                    rbCrossValidation.Checked = true;
                    break;
                case TrainingType.GridSearch:
                    rbGridSearch.Checked = true;
                    break;
                case TrainingType.RFE:
                    rbRFE.Checked = true;
                    break;
            }
        }

        void Instance_FileTypeUpdateEvent(object sender, EventArgs e)
        {
            switch (GuiPreferences.Instance.FileType)
            {
                case DataType.rawValue:
                    rbRawValues.Checked = true;
                    break;
                case DataType.betaValue:
                    rbBetaValues.Checked = true;
                    break;
                case DataType.tContrastValue:
                    rbTcontrastValues.Checked = true;
                    break;
                case DataType.SVMFile:
                    rbSVM.Checked = true;
                    break;
            } 
        }

        void Instance_IGTypeUpdateEvent(object sender, EventArgs e)
        {
            switch (GuiPreferences.Instance.IgSelectionType)
            {
                case IGType.Threshold:
                    rbIGThreshold.Checked = true;
                    break;
                case IGType.Voxels:
                    rbIGVoxelCount.Checked = true;
                    break;
            }
        }

        void Instance_NormalizationTypeUpdateEvent(object sender, EventArgs e)
        {
            switch (GuiPreferences.Instance.NormalizedType)
            {
                case NormalizationType.None:
                    rbNoNorm.Checked = true;
                    break;
                case NormalizationType.MinMax:
                    rbMinMax.Checked = true;
                    break;
                case NormalizationType.RawMinusMed:
                    rbRawMinusMed.Checked = true;
                    break;
                case NormalizationType.RawDivMed:
                    rbRawDivMed.Checked = true;
                    break;
                case NormalizationType.RawMinusMedDivMed:
                    rbRawMinusMedDivMed.Checked = true;
                    break;
                case NormalizationType.RawMinusMedWin:
                    rdRawMinusMedWin.Checked = true;
                    break;
                case NormalizationType.RawMinusMedWinDivIqrWin:
                    rdRawMinusMedWinIqrWin.Checked = true;
                    break;
            }
        }

        void Instance_nudCVFoldsEvent(object sender, EventArgs e)
        {
            nudCvFolds.Value = GuiPreferences.Instance.NudCVFolds;
        }

        void Instance_nudRFEFoldsEvent(object sender, EventArgs e)
        {
            numericUpDown2.Value = GuiPreferences.Instance.NudRFEFolds;
        }

        void Instance_nudGridFoldsEvent(object sender, EventArgs e)
        {
            nudGridFolds.Value = GuiPreferences.Instance.NudGridFolds;
        }

        void Instance_nudSVMThresholdEvent(object sender, EventArgs e)
        {
            nudThresholdSVM.Value = GuiPreferences.Instance.NudSVMThreshold;
        }

        void Instance_nudThresholdEvent(object sender, EventArgs e)
        {
            nudThreshold.Value = GuiPreferences.Instance.NudThreshold;
        }

        void Instance_nudTConThresholdEvent(object sender, EventArgs e)
        {
            nudTConThreshold.Value = GuiPreferences.Instance.NudTConThreshold;
        }

        void Instance_nudBetaThresholdEvent(object sender, EventArgs e)
        {
            nudBetaThreshold.Value = GuiPreferences.Instance.NudBetaThreshold;
        }

        void Instance_unprocessed(object sender, EventArgs e)
        {
            cbFeatureProcessingUnprocessed.Checked = GuiPreferences.Instance.UnprocessedChecked;
        }

        void Instance_nudExtractFromTREvent(object sender, EventArgs e)
        {
            nudExtractFromTR.Value = GuiPreferences.Instance.NudExtractFromTR;
        }

        void Instance_nudExtractToTREvent(object sender, EventArgs e)
        {
            nudExtractToTR.Value = GuiPreferences.Instance.NudExtractToTR;
        }

        void Instance_nudClassifyUsingTREvent(object sender, EventArgs e)
        {
            nudClassifyUsingTR.Value = GuiPreferences.Instance.NudClassifyUsingTR;
        }

        void Instance_nudMovingWindowEvent(object sender, EventArgs e)
        {
            nudMovingWindow.Value = GuiPreferences.Instance.NudMovingWindow;
        }

        void Instance_nudIGThresholdEvent(object sender, EventArgs e)
        {
            nudIGThreshold.Value = GuiPreferences.Instance.NudIGThreshold;
        } 

        void Instance_nudIGVoxelAmountEvent(object sender, EventArgs e)
        {
            nudIGVoxelAmount.Value = GuiPreferences.Instance.NudIGVoxelAmount;
        }

        void Instance_contrastFileUpdateEvent(object sender, EventArgs e)
        {
            tbContrastFileName.Text = GuiPreferences.Instance.ContrastFile;                
        }

        void Instance_ProtocolFileUpdateEvent(object sender, EventArgs e)
        {
            tbProtocolFileName.Text = GuiPreferences.Instance.ProtocolFile;                
        }

        void Instance_workDirectoryUpdateEvent(object sender, EventArgs e)
        {
            tbWorkingDir.Text = GuiPreferences.Instance.WorkDirectory;    
        }

        void Instance_FileNameUpdateEvent(object sender, EventArgs e)
        {
            tbFileName.Text = GuiPreferences.Instance.FileName;  
        }

        void Instance_tbTRUpdateEvent(object sender, EventArgs e)
        {
            tbTRs.Text = GuiPreferences.Instance.TbTRs.ToString();
        }

        void Instance_LoggerUpdateEvent(object sender, EventArgs e)
        {            
            List<string> temp = GuiPreferences.Instance.getLog();
            for (int t = 0; t < temp.Count; t++)
            {
                TbSVMLog = temp[t];
            }
        }

        private delegate void Instance_ProgressBarUpdateEventDelegate(object sender, GuiPreferences.ProgressBarChangeEventArg t);

        private void Instance_ProgressBarUpdateEvent(object sender, GuiPreferences.ProgressBarChangeEventArg t)
        {
            if (InvokeRequired)
            {
                Invoke(new Instance_ProgressBarUpdateEventDelegate(Instance_ProgressBarUpdateEvent), new object[2]{sender,t});
            }
            else
            {
                progressBar1.Maximum = t._max;
                progressBar1.Value = t._value;
            }
        }

        private void Instance_TrUpdateEvent(object sender, GuiPreferences.TrChangeEventArg t)
        {
            nudFromTR.Value = t._fromTR;
            nudFromTR.Minimum = t._minTR;
            nudFromTR.Maximum = t._maxTr;

            nudToTR.Value = t._toTR;
            nudToTR.Minimum = t._minTR;
            nudToTR.Maximum = t._maxTr;
        }

        private void bSelectFiles_Click(object sender, EventArgs e)
        {
            SelectWorkingDirFiles();
        }


        private void initFolderTreeView()
        {
            treeView1.Nodes.Clear();
            string[] drives = System.IO.Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeNode rootnode = new TreeNode(drive);
                treeView1.Nodes.Add(rootnode);
                if (!FillChildNodes(rootnode))
                {
                    treeView1.Nodes.Remove(rootnode);
                }
            }
            treeView1.Nodes[0].Expand();
        }
        
        private void bLoadProtocol_Click(object sender, EventArgs e)
        {
            selectProtocolFile();
        }


        private void cbTrainAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbCrossValidation.Checked)
            {
                lFolds.Enabled = false;
                nudCvFolds.Enabled = false;
            }
            else
            {
                lFolds.Enabled = true;
                nudCvFolds.Enabled = true;
            }
        }

        private void cbMultiClass_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.CbMultiClassChecked = !GuiPreferences.Instance.CbMultiClassChecked;
            if (GuiPreferences.Instance.CbMultiClassChecked)
            {
                lClass1.Enabled = false;
                lClass2.Enabled = false;
                cmbClass1.Enabled = false;
                cmbClass2.Enabled = false;
            }
            else
            {
                lClass1.Enabled = true;
                lClass2.Enabled = true;
                cmbClass1.Enabled = true;
                cmbClass2.Enabled = true;
            }
        }


        private void bImport_Click(object sender, EventArgs e)
        {
            importMultiRun(getDirectoryListByCheckedIndex(2));
            if (cbAutoAll.Checked)
            {
                GuiPreferences.Instance.setLog("Automatically doing the following: 1. importing 2. processing & 3. training.");
                ProcessClick();
                TrainClick();
            } 
            Sound.PlayEverythingFinished();
        }


        private void prepareFilteredEyeVoxels()
        {
            //prepare lookup dictionary for filtered voxels that belong to the eyes, based on first N eye slices and first M lines in each slice.
            Dictionary<int, int> EyeVoxels = new Dictionary<int, int>();

            //if the user decided to filter the eyes, we will add relevant eye voxels to be filtered.
            if (GuiPreferences.Instance.NudFilterEyeSlices > 0)
            {
                for (int slice = 1; slice <= GuiPreferences.Instance.NudFilterEyeSlices; slice++)
                {
                    for (int line = 1; line <= GuiPreferences.Instance.NudEyeSliceFirstLines; line++)
                    {
                        for (int vox = 1; vox <= 80; vox++)
                        {
                            int index = (slice - 1)*80*80 + (line - 1)*80 + vox;
                            EyeVoxels.Add(index, -1);
                        }
                    }
                }
            }

            Preferences.Instance.EyeVoxels = EyeVoxels;
        }

        private void importMultiRun(List<string> dirList)
        {
            Preferences.Instance.svmLoaded = false;

            if (!Preferences.Instance.protocolLoaded)
            {
                showMessageBox("Please Load A Protocol First.");
                return;
            }

            if (dataGridView1.Rows.Count <=0)
            {
                showMessageBox("please select at least one folder");
                return;
            }

            if (GuiPreferences.Instance.CbOverwriteProcessedFilesChecked && MessageBox.Show("Do you realy want to overwrite previously processed LIBSVM-ARFF files?\n",
                "OBLML", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                GuiPreferences.Instance.CbOverwriteProcessedFilesChecked = false;
            } 

            // uncomment if changing type in between loading is acceptable, if we only have one type, no need to add support like reloading the datagrid with *.bdat etc etc
            /* if (GuiPreferences.Instance.changedDataType)
            {
                showMessageBox("type select since last load, please reselect folders");
                return;
            }*/

            GuiPreferences.Instance.changedDataType = false; // should this be up ? 
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FromTR = 1;

            //assigned after we know what to assign from the protocol
            //PublicMethods.setClassesLabels();
            GuiPreferences.Instance.CmbClass1Selected = 1; //left
            GuiPreferences.Instance.CmbClass2Selected = 2; //right 

            //delete all files that are going to be created, in order to prevent anomaly vectors.
            /*string[] deleteFiles =
                    {
                        "TrainSet.libsvm",
                        "TrainSet_3th_vectors.libsvm",
                        "TrainSet_3th_vectors_scale_paramCS.libsvm",
                        "TrainSet_3th_vectors_scaledCS.libsvm",
                        "TrainSet_3th_vectors_scaledCS.libsvm.arff",
                        "TrainSet_3th_vectors_scaledCS_filteredIG.arff",
                        "TrainSet_3th_vectors_scaledCS_filteredIG.model",
                        "TrainSet_3th_vectors_scaledCS_filteredIG_indices.xml",
                        "TrainSet_4th_vectors.libsvm",
                        "TrainSet_4th_vectors_scale_paramCS.libsvm",
                        "TrainSet_4th_vectors_scaledCS.libsvm",
                        "TrainSet_4th_vectors_scaledCS.libsvm.arff"
                    };*/

            Preferences.Instance.TrainingBaselineMedians = new List<StatisticsFeatures>();
            Preferences.Instance.TrainingBaselineMediansRunningWindow = new List<StatisticsFeatures>();

            for (int i = 0; i < dirList.Count; i++)
            {
                //add one struct to the list, initialization of this struct is done when the first vdat file is read from the disk binaryreader.loadrawdatamultirun
                //NOTE: used to keep the min/max values in training stage
                Preferences.Instance.MinMax.Add(new Dictionary<int, MinMax>());

                //init this huge ram sucking monster that keeps track of all values per feature per tr per run... 

                Preferences.Instance.currentProcessedRun = i; //0-based index (run 0 is the first run)
                Preferences.Instance.TrainingEventStartLocationsPerTR.Add(i,new Dictionary<string, List<int>>());

                //NOTE: used to keep median of ONLY a limited amount of TRs (the baseline events)
                Preferences.Instance.TrainingBaselineMedians.Add(new StatisticsFeatures(80 * 80 * 32, Preferences.Instance.events.eventList[0].var2, null, true));
                Preferences.Instance.TrainingBaselineMediansRunningWindow.Add(new StatisticsFeatures(80 * 80 * 32, Preferences.Instance.events.EventListLastTr, null, true));

                GuiPreferences.Instance.WorkDirectory = dirList[i];

                /*if (GuiPreferences.Instance.CbOverwriteProcessedFilesChecked)
                {
                    foreach (string fileName in deleteFiles)
                    {
                        PublicMethods.DeleteFile(GuiPreferences.Instance.WorkDirectory + fileName);
                    }
                }*/

                //get all files in the path with this extention
                GuiManager.getFilePaths("*.vdat");

                //update certain info
                GuiManager.updateFilePaths();

                TbSVMLog = "Preparing Filtered Eye Voxels. " + GuiPreferences.Instance.NudEyeSliceFirstLines +
                           " first lines in " + GuiPreferences.Instance.NudFilterEyeSlices.ToString() +
                           " slices are ignored."; 
                prepareFilteredEyeVoxels();

                if (GuiPreferences.Instance.FileType == DataType.rawValue)
                {
                    TrainingTesting_SharedVariables.binary.loadRawDataMultiRun(GuiPreferences.Instance.WorkDirectory);
                }
                else if (GuiPreferences.Instance.FileType == DataType.betaValue)
                {
                    //PublicMethods.binary.loadBetaData();
                    showMessageBox("TODO: implement multi run loading for beta values");
                }
                else if (GuiPreferences.Instance.FileType == DataType.tContrastValue)
                {
                    //PublicMethods.binary.loadTContrastData();
                    showMessageBox("TODO: implement multi run loading for contrast T values");
                }
                //Preferences.Instance.Medians[Preferences.Instance.Medians.Count - 1].clearMatrixMemory();
            }
            TbSVMLog = "Finished Importing.";
            Sound.beep(Sound.beepType.Asterisk);
        } 

        private void bImport_Click_singleRun(object sender, EventArgs e)
        {
            Preferences.Instance.svmLoaded = false;
            if (GuiPreferences.Instance.changedDataType || (GuiPreferences.Instance.WorkDirectory == null || GuiPreferences.Instance.WorkDirectory == ""))
                if (!SelectWorkingDirFiles())
                    return;
                else
                {
                    GuiPreferences.Instance.changedDataType = false; // should this be up ?

                    //svm is a special case and needs to be loaded before the second dialog because only after we load it we know how many TRs we have.
                    if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                    {
                        TrainingTesting_SharedVariables.binary.loadSVM();
                        Preferences.Instance.svmLoaded = true;
                        //Preferences.Instance.inputFileType = dataType.SVMFile;
                        GuiPreferences.Instance.TbTRs = 1;
                        // then we change to the new values                
                        if (GuiPreferences.Instance.FromTR == 0)
                        {
                            GuiPreferences.Instance.FromTR = 1;
                            GuiPreferences.Instance.MaxTR = GuiPreferences.Instance.getMaxAvailableTrs();
                            GuiPreferences.Instance.ToTR = GuiPreferences.Instance.getMaxAvailableTrs();
                            GuiPreferences.Instance.MinTR = 1;
                        }
                        else
                        {
                            GuiPreferences.Instance.MaxTR = GuiPreferences.Instance.getMaxAvailableTrs();
                            GuiPreferences.Instance.FromTR = 1;
                            GuiPreferences.Instance.ToTR = GuiPreferences.Instance.getMaxAvailableTrs();
                            GuiPreferences.Instance.MinTR = 1;
                        }
                    }
                    // commented out, this loaded a new form that let you choose your min and max, ever since i have 32GB + 64b, i dont need to limit my TRs for calculation
                    /*Form getTRvalues = new Form2();
                    getTRvalues.StartPosition = FormStartPosition.CenterParent;
                    getTRvalues.ShowDialog();
                    getTRvalues.Close();*/
                }

            if (!Preferences.Instance.protocolLoaded)
            {
                Preferences.Instance.protocolLoaded = selectProtocolFile();
                if (!Preferences.Instance.protocolLoaded)
                    return;
            }

            if (GuiPreferences.Instance.FileType == DataType.rawValue)
            {
                TrainingTesting_SharedVariables.binary.loadRawData();
                //binary.loadRawDataText();
                //GuiPreferences.Instance.FileType = dataType.rawValue;
            }
            else if (GuiPreferences.Instance.FileType == DataType.betaValue)
            {
                TrainingTesting_SharedVariables.binary.loadBetaData();
                //Preferences.Instance.inputFileType = dataType.betaValue;
            }
            else if (GuiPreferences.Instance.FileType == DataType.tContrastValue)
            {
                TrainingTesting_SharedVariables.binary.loadTContrastData();
                //Preferences.Instance.inputFileType = dataType.tContrastValue;
            }
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            ExportMultiRun(getDirectoryListByCheckedIndex(2));
        }

        private void ExportMultiRun(List<string> dirList)
        {
            showMessageBox("export code untested");
            for (int i = 0; i < dirList.Count;i++ )
            {
                if (Preferences.Instance.ProblemOriginalMulti[i].samples == null)
                {
                    TbSVMLog = "Export Failed: Problem has no samples!";
                    return;
                }

                string trainFileName = dirList[i] /*+ GuiPreferences.Instance.FileName */ + "TrainSet";

                if (cbFeatureProcessingUnprocessed.Checked) // unprocessed
                {

                    //todo add proper named to saved files, check if null is logical at all.
                    if ((Preferences.Instance.ProblemOriginalMulti[i].samples != null) && (rbSparse.Checked))
                    {
                        Preferences.Instance.ProblemOriginalMulti[i].Save(trainFileName + ".libsvm");
                        TbSVMLog = "saved Original Problem LibSVM file: " + trainFileName + ".libsvm";
                    }
                    else if ((Preferences.Instance.ProblemOriginalMulti[i].samples != null) && (rbCSV.Checked))
                    {
                        if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                            Preferences.Instance.ProblemOriginalMulti[i].UpdateMaximumIndex();
                        Preferences.Instance.ProblemOriginalMulti[i].SaveCSV(trainFileName + ".csv");
                        TbSVMLog = "saved Original Problem CSV file: " + trainFileName + ".csv";
                    }
                }
                else
                {
                    if (Preferences.Instance.ProblemFinalMulti[i] == null)
                    {
                        TbSVMLog = "Export Failed: Final Problem has no samples!";
                        return;
                    }

                    //todo add proper named to saved files, check if null is logical at all.
                    if ((Preferences.Instance.ProblemFinalMulti[i].samples != null) && (rbSparse.Checked))
                    {
                        Preferences.Instance.ProblemFinalMulti[i].Save(trainFileName + ".libsvm");
                        TbSVMLog = "saved Final Problem LibSVM file: " + trainFileName + ".libsvm";
                    }
                    else if ((Preferences.Instance.ProblemFinalMulti[i].samples != null) && (rbCSV.Checked))
                    {
                        if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                            Preferences.Instance.ProblemFinalMulti[i].UpdateMaximumIndex();
                        Preferences.Instance.ProblemFinalMulti[i].SaveCSV(trainFileName + ".csv");
                        TbSVMLog = "saved Final Problem CSV file: " + trainFileName + ".csv";
                    }
                    //process files before saving them.
                }
            }
        }

        private void Export()
        {
            if (Preferences.Instance.ProblemOriginal.samples == null)
            {
                TbSVMLog = "Export Failed: Problem has no samples!";
                return;
            }

            string trainFileName = GuiPreferences.Instance.WorkDirectory /*+ GuiPreferences.Instance.FileName*/ + "TrainSet";

            if (cbFeatureProcessingUnprocessed.Checked) // unprocessed
            {

                //todo add proper named to saved files, check if null is logical at all.
                if ((Preferences.Instance.ProblemOriginal.samples != null) && (rbSparse.Checked))
                {
                    //string _filename = GuiPreferences.Instance.WorkDirectory;
                    //_filename = _filename.Substring(_filename.TrimStart((char[])"H:\\My_Dropbox\\VERE\MRI_data\\"));

                    //_filename = _filename.Replace(@"\", "_");
                    // = @"H:\My_Dropbox\VERE\MRI_data\Dona\20121113.null.class\07\rtp\";
                    Preferences.Instance.ProblemOriginal.Save(trainFileName + ".libsvm");
                    TbSVMLog = "saved Original Problem LibSVM file: " + trainFileName + ".libsvm";
                }
                else if ((Preferences.Instance.ProblemOriginal.samples != null) && (rbCSV.Checked))
                {
                    if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                        Preferences.Instance.ProblemOriginal.UpdateMaximumIndex();
                    Preferences.Instance.ProblemOriginal.SaveCSV(trainFileName + ".csv");
                    TbSVMLog = "saved Original Problem CSV file: " + trainFileName + ".csv";
                }
            }
            else
            {
                if (Preferences.Instance.ProblemFinal == null)
                {
                    TbSVMLog = "Export Failed: Final Problem has no samples!";
                    return;
                }

                //todo add proper named to saved files, check if null is logical at all.
                if ((Preferences.Instance.ProblemFinal.samples != null) && (rbSparse.Checked))
                {
                    Preferences.Instance.ProblemFinal.Save(trainFileName + ".libsvm");
                    TbSVMLog = "saved Final Problem LibSVM file: " + trainFileName + ".libsvm";
                }
                else if ((Preferences.Instance.ProblemFinal.samples != null) && (rbCSV.Checked))
                {
                    if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                        Preferences.Instance.ProblemFinal.UpdateMaximumIndex();
                    Preferences.Instance.ProblemFinal.SaveCSV(trainFileName + ".csv");
                    TbSVMLog = "saved Final Problem CSV file: " + trainFileName + ".csv";
                }
                //process files before saving them.
            }
        }

        private void tbSVMLog_TextChanged(object sender, EventArgs e)
        {
            //Get the last text position  
            tbSVMLog.SelectionStart = tbSVMLog.Text.Length;
            tbSVMLog.ScrollToCaret();
            tbSVMLog.Refresh();
        }

        private void cbFeatureReductionUnprocessed_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFeatureProcessingUnprocessed.Checked)
            {
                gbFeatureProcessFlow.Enabled = false;
                bRight.Enabled = false;
                bLeft.Enabled = false;
                lbFeatureProcessOptions.Enabled = false;
            }
            else
            {
                gbFeatureProcessFlow.Enabled = true;
                bRight.Enabled = true;
                bLeft.Enabled = true;
                lbFeatureProcessOptions.Enabled = true;
            }
        }


        private void nudFromTR_ValueChanged(object sender, EventArgs e)
        {
            if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                return;

            int changedValue = Convert.ToInt32(nudFromTR.Value);
            GuiPreferences.Instance.FromTR = changedValue;
            if (GuiPreferences.Instance.FromTR != changedValue)
                nudFromTR.Value = GuiPreferences.Instance.FromTR;
        }

        private void nudToTR_ValueChanged(object sender, EventArgs e)
        {
            if (GuiPreferences.Instance.FileType == DataType.SVMFile)
                return;

            int changedValue = Convert.ToInt32(nudToTR.Value);
            GuiPreferences.Instance.ToTR = changedValue;
            if (GuiPreferences.Instance.ToTR != changedValue)
                nudToTR.Value = GuiPreferences.Instance.ToTR;
        }



        private void bTrain_Click(object sender, EventArgs e)
        {  
            TrainClick(); 
            Sound.PlayTrainingFinished();
        } 

        private void TrainClick()
        {
            Training_MultiRunTraining.TrainMultiRun(getDirectoryListByCheckedIndex(2));
            reloadDirectoryStructure();
        }
        //-----------


        private void button6_Click_1(object sender, EventArgs e)
        { 
            Training_MultiRunPlotting.hardCodedIGPlot();
        }

        private void bProcess_Click(object sender, EventArgs e)
        {
            ProcessClick();
            Sound.PlayProcessingFinished();
        }

        private void ProcessClick()
        {
            
            if (GuiPreferences.Instance.NudClassifyUsingTR >= GuiPreferences.Instance.NudExtractToTR && GuiPreferences.Instance.CbPeekHigherTRsIGChecked)
            {
                showMessageBox("You are trying to peek onto IG balues from TR that will not be saved/used, please increase 'Extract To TR' or decrease 'Classify Using TR'");
                return;
            }

            if (GuiPreferences.Instance.NudClassifyUsingTR > GuiPreferences.Instance.NudExtractToTR && !GuiPreferences.Instance.CbPeekHigherTRsIGChecked)
            {
                showMessageBox("You are trying to train on a TR that will not be saved/used, please increase 'Extract To TR' or decrease 'Classify Using TR'");
                return;
            }

            //minmax, raw-med, etc..
            LoadNormalizationFormula();

            Training_MultiRunProcessing.ProcessMultiRuns(getDirectoryListByCheckedIndex(2));
        }

        //-----------

        private List<string> getDirectoryListByCheckedIndex(int index)
        {
            List<string> dirList = new List<string>(); 
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[index].Value.ToString() == "True")
                    dirList.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
            }
            return dirList;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, 100);
        }

        //private void clbFeatureRedEnG_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        //{
        //    // This item is going to be toggled to the checked state if this is true.
        //    if (e.NewValue == CheckState.Checked)
        //    {
        //        lbFeatureRedEng.BeginUpdate();
        //        // Uncheck all checked items.
        //        foreach (int index in lbFeatureRedEng.inIndices)
        //        {
        //            lbFeatureRedEng.SetItemChecked(index, false);
        //        }
        //        lbFeatureRedEng.EndUpdate();
        //    }
        //}

        private void bUp_Click(object sender, EventArgs e)
        {
            clickUP();
        }


        private void bDown_Click(object sender, EventArgs e)
        {
            if (this.lbFeatureProcessFlow.SelectedIndex == -1 || this.lbFeatureProcessFlow.SelectedIndex == lbFeatureProcessFlow.Items.Count - 1)
                return;
            
            // feature processing selection manipulation
            Object select, next, temp;
            select = lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex]; 
            next = lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex + 1];   
            int selectedIndex = lbFeatureProcessFlow.SelectedIndex;
            temp = select;
            select = next;
            next = temp;  
            lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex] = select;
            lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex + 1] = next; 
            lbFeatureProcessFlow.SelectedIndices.Clear(); 
            lbFeatureProcessFlow.SelectedIndex = selectedIndex + 1;      

            // selection manipulation for K
            Object selectK, nextK, tempK;
            selectK = lbK.Items[selectedIndex];
            nextK = lbK.Items[selectedIndex + 1];   
            tempK = selectK;
            selectK = nextK;
            nextK = tempK;
            lbK.Items[selectedIndex] = selectK;
            lbK.Items[selectedIndex + 1] = nextK;
        }

        private void initGuiVariables()
        {
            GuiPreferences.Instance.WorkDirectory = @"";// @"\My_Dropbox\UProjects\OriBrainLearner\Data";
            GuiPreferences.Instance.FileName = @"";//"";
            GuiPreferences.Instance.ProtocolFile = @"";//@"\My_Dropbox\VERE\MRI_data\Tirosh\20112006.3directions_NamedDir.prt";
            GuiPreferences.Instance.ContrastFile = @"";//@"\My_Dropbox\VERE\MRI_data\Tirosh\tirosh_body_3dir.ctr";
            //GuiPreferences.Instance.PreOnset = 1;
            //GuiPreferences.Instance.PostOnset = 8;
            GuiPreferences.Instance.CbOnsetFromProtocolChecked = true;
            GuiPreferences.Instance.FileType = DataType.rawValue;
            GuiPreferences.Instance.changedDataType = false;
            GuiPreferences.Instance.NudThreshold = 400;
            GuiPreferences.Instance.NudTConThreshold = 0;
            GuiPreferences.Instance.NudBetaThreshold = 0;
            GuiPreferences.Instance.NudSVMThreshold = 100;
            GuiPreferences.Instance.NudTrainTestSplit = (decimal) 0.5f;
            GuiPreferences.Instance.NudCVFolds = 10;
            GuiPreferences.Instance.NudGridFolds = 2;
            GuiPreferences.Instance.NudRFEFolds = 10;
            GuiPreferences.Instance.NudExtractFromTR = 3;
            GuiPreferences.Instance.NudExtractToTR = 4;
            GuiPreferences.Instance.NudClassifyUsingTR = 3;
            GuiPreferences.Instance.NudMovingWindow = 40;
            GuiPreferences.Instance.NudIGThreshold = 0.1M; //0.15 dona, keren 0.10
            GuiPreferences.Instance.NudIGVoxelAmount = 1024; //0.15 dona, keren 0.10
            GuiPreferences.Instance.NudFilterEyeSlices = 13; // 13 dona, keren 10
            GuiPreferences.Instance.CbPeekHigherTRsIGChecked = false;
            GuiPreferences.Instance.NudEyeSliceFirstLines = 80; //dona eye band = 27 but 80 is better accuracy.


            JobManager.addFeatureProcess("Glm");
            JobManager.addFeatureProcess("Voxelize");
            /*PublicMethods.addFeatureProcess("Voxelize");
            PublicMethods.addFeatureProcess("Voxelize");*/
         
        }

        private void bRight_Click(object sender, EventArgs e)
        { 
            foreach (var item in lbFeatureProcessOptions.SelectedItems)
            {
                JobManager.addFeatureProcess(item.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JobManager.clearFeatureProcesses();
            //lbFeatureProcessFlow.Items.Clear();
            //lbK.Items.Clear();
        }

        private void bLeft_Click(object sender, EventArgs e)
        {
            for (int i = lbFeatureProcessFlow.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int _pos = lbFeatureProcessFlow.SelectedIndices[i];
                //lbFeatureProcessFlow.Items.RemoveAt(_pos);
                //lbK.Items.RemoveAt(_pos);
                GuiPreferences.Instance.RemoveAtKvalues(_pos);
                GuiPreferences.Instance.RemoveAtFeatureProcessJobs(_pos);
            }

            if (lbFeatureProcessFlow.Items.Count <= ((lbFeatureProcessFlow.Size.Height - 4) / lbFeatureProcessFlow.ItemHeight))
            {
                if (lbFeatureProcessFlow.Items.Count > 0)
                {
                    lbFeatureProcessFlow.SelectedIndex = 0;
                    lbFeatureProcessFlow.SelectedIndex = -1;
                }
            }

        }

        private void cmbClass1_SelectedIndexChanged(object sender, EventArgs e)      
        {
            if (cmbClass1.SelectedIndex == cmbClass2.SelectedIndex) 
                cmbClass1.SelectedIndex = GuiPreferences.Instance.CmbClass1Selected;
            GuiPreferences.Instance.CmbClass1Selected = cmbClass1.SelectedIndex;
            //GuiPreferences.Instance.CmbClass1Text = cmbClass1.Text;
        }

        private void cmbClass2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbClass2.SelectedIndex == cmbClass1.SelectedIndex)
                cmbClass2.SelectedIndex = GuiPreferences.Instance.CmbClass2Selected;
            GuiPreferences.Instance.CmbClass2Selected = cmbClass2.SelectedIndex;
            //GuiPreferences.Instance.CmbClass2Text = cmbClass2.Text;
            //Program.mainForm.cmbClass1.selectedText;
        }

        private void bLoadModel_Click(object sender, EventArgs e)
        {
            WekaTrainingMethods.loadModel();
        }

        private void bSaveModel_Click(object sender, EventArgs e)
        {
            if (GuiPreferences.Instance.TrainType != TrainingType.CrossValidation)
                WekaTrainingMethods.saveModel();
            else
            {
                GuiPreferences.Instance.setLog("Model cant be saved in cross validation (has 10 models which are not saved!)");
            }
        }

        private void bPlot_Click(object sender, EventArgs e)
        {
            PlotManager.plotBrainDicomViewer();
            //PlotManager.plotBrainMatlab();
        }

        private void bPlus_Click(object sender, EventArgs e)
        {
            if (this.lbK.SelectedIndex == -1)
                return;

            //Object select;
            GuiPreferences.Instance.incKvalues(lbK.SelectedIndex);
            /*select = lbK.Items[lbK.SelectedIndex];
            lbK.Items[lbK.SelectedIndex] = (Convert.ToInt32(select) + 1).ToString(); */
        }
         
        private void bMinus_Click(object sender, EventArgs e)
        {
            if (this.lbK.SelectedIndex == -1)
                return;

            GuiPreferences.Instance.decKvalues(lbK.SelectedIndex);
            /*Object select;
            select = lbK.Items[lbK.SelectedIndex];
            if (Convert.ToInt32(select) == 0) return;
            lbK.Items[lbK.SelectedIndex] = (Convert.ToInt32(select) - 1).ToString(); */
        }

        private void lbFeatureProcessFlow_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbK.ClearSelected();
            lbK.SelectedIndex = lbFeatureProcessFlow.SelectedIndex; 
        }

        private void lbK_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lbFeatureProcessFlow.ClearSelected();
            //lbFeatureProcessFlow.SelectedIndex = lbK.SelectedIndex;
        }

        private void nudThreshold_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudThreshold = nudThreshold.Value;
        }
        
        private void nudBetaThreshold_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudBetaThreshold = nudBetaThreshold.Value;
        }

        private void nudTConThreshold_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudTConThreshold = nudTConThreshold.Value;
        } 

        private void tbWorkingDir_TextChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.WorkDirectory = tbWorkingDir.Text;
        }

        private void tbFileName_TextChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.FileName = tbFileName.Text;
        }

        private void tbProtocolFileName_TextChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.ProtocolFile = tbProtocolFileName.Text;
        }

        private void tbContrastFileName_TextChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.ContrastFile = tbContrastFileName.Text;
        }

        private void nudPreOnset_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.PreOnset = (int) nudPreOnset.Value;
        }

        private void nudPostOnset_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.PreOnset = (int) nudPostOnset.Value;  
        }

        private void cbOnsetFromProtocol_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.CbOnsetFromProtocolChecked = cbOnsetFromProtocol.Checked;
        }

        private void rbRawValues_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRawValues.Checked) 
                GuiPreferences.Instance.FileType = DataType.rawValue;
        }

        private void rbBetaValues_CheckedChanged(object sender, EventArgs e)
        { 
            if (rbBetaValues.Checked) 
                GuiPreferences.Instance.FileType = DataType.betaValue;
        }

        private void rbTcontrastValues_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTcontrastValues.Checked) 
                GuiPreferences.Instance.FileType = DataType.tContrastValue;
        }

        private void rbSVM_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSVM.Checked) 
                GuiPreferences.Instance.FileType = DataType.SVMFile;
        }

        private void rbSparse_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.RbSvmOutputChecked = rbSparse.Checked;
        }

        private void rbCSV_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.RbCSVOutputChecked = rbCSV.Checked;
        }

        private void cbPerEvent_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.PerEventMapChecked = cbPerEvent.Checked;
        }

        private void rbTrainTestSplit_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTrainTestSplit.Checked) 
                GuiPreferences.Instance.TrainType = TrainingType.TrainTestSplit;
        }

        private void rbCrossValidation_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCrossValidation.Checked) 
                GuiPreferences.Instance.TrainType = TrainingType.CrossValidation;
        }

        private void rbGridSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGridSearch.Checked) 
                GuiPreferences.Instance.TrainType = TrainingType.GridSearch;
        }

        private void rbRFE_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRFE.Checked) 
                GuiPreferences.Instance.TrainType = TrainingType.RFE;
        }

        private void nudThresholdSVM_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudSVMThreshold = nudThresholdSVM.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudTrainTestSplit = nudTrainTestSplit.Value;
        }

        private void nudCvFolds_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudCVFolds = nudCvFolds.Value ;
        }

        private void nudGridFolds_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudGridFolds = nudGridFolds.Value; 
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudRFEFolds = numericUpDown2.Value ;
        }
        
        private void RTServer_Click(object sender, EventArgs e)
        {
            if (!Preferences.Instance.protocolLoaded)
            {
                showMessageBox("Please Load A Protocol First.");
                return;
            }

            if (dataGridView1.Rows.Count == 1 && dataGridView1.Rows[0].Cells[3].Value.ToString() == "True")
            {}
            else
            {
                showMessageBox("please select EXACTLY ONE folder (contains Model and IG xml)");
                return;
            }

            /*if  (dataGridView1.Rows[0].Cells[3].Value.ToString() == "True" && dataGridView1.Rows[1].Cells[4].Value.ToString() == "True") ||
                (dataGridView1.Rows[1].Cells[3].Value.ToString() == "True" && dataGridView1.Rows[0].Cells[4].Value.ToString() == "True"))
            {   
            }
            else
            {
                showMessageBox("please select ONLY one Test folder");
                return;
            }*/

            //inits a udp connection that listens for filenames (from unity or some sender), after receiving them, we load the raw filename, process the data, send classification to unity.
            Preferences.Instance.RealTimeTestingSingleVector = true;
            GuiPreferences.Instance.WorkDirectory = dataGridView1.Rows[0].Cells[0].Value.ToString();
            
            //NOTE: vdat folder used to hold the vdat files folder when we passed files around from tbv to obl, but at the moment its not needed as we moved to pipes.
            // at some point we will need to remove all commented lines relate to vdatfolder as they are no longer needed.
            string vdatDataFolder = null;
            string testDataFolder;
            if (dataGridView1.Rows[0].Cells[4].Value.ToString()=="True")
            {
                //vdatDataFolder = dataGridView1.Rows[0].Cells[0].Value.ToString();
                testDataFolder = dataGridView1.Rows[1].Cells[0].Value.ToString();
            }
            else
            { 
                //vdatDataFolder = dataGridView1.Rows[1].Cells[0].Value.ToString();
                testDataFolder = dataGridView1.Rows[0].Cells[0].Value.ToString();
            }


            GuiPreferences.Instance.setLog("Loading Config File");
            ConfigManager.loadConfigFile();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // we load the configuration file for min/max for all features - ONCE.

            //minmax, raw-med, etc..
            LoadNormalizationFormula();

            Preferences.Instance.TestingBaselineMediansRunningWindow = new StatisticsFeatures(80 * 80 * 32, Preferences.Instance.events.EventListLastTr, null, true);
            //TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_PercentageChange();

            Preferences.Instance.RealTimeModelDirectory = testDataFolder; // assigning the model directory so that we can load the param file
            if (!TrainingTesting_SharedVariables._svmscaleTesting.ConfigFileLoaded )//&& GuiPreferences.Instance.NormalizedType == NormalizationType.MinMax) 
            {
                //load normalization configuration from training stage.
                string commandLine = "-l 0 " +
                                        "-r " + Preferences.Instance.RealTimeModelDirectory +
                                        "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString()+ "th_vectors_scale_paramCS.libsvm " +
                                        Preferences.Instance.RealTimeModelDirectory + "nofile";

                TrainingTesting_SharedVariables._svmscaleTesting.initSingleVectorConfig(commandLine.Split(' '), 80 * 80 * 32);
                TbSVMLog = "Loaded Min/Max Normalization Values - Done Automatically, Normalization method is written above.";

                GuiPreferences.Instance.setLog("Loading Median Range - Done Automatically, Normalization method is written above.");
                Preferences.Instance.medianRange = XMLSerializer.DeserializeFile<MinMax>(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_MedianRangeFromBaseline.xml");
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            

            //load and test if pipe is working 
            try
            {
                TbSVMLog = "Pipe Loading is Done Inside Form1, only in Testing Mode";
                _pipeServer = new AsyncNamedPipeServer();
                _pipeServer.PipeMessage += new DelegateMessage(MessageHandler);
                //_pipeServer.PipeData += new DelegateBuffer(PipeDataHandler);
                _pipeServer.DataUpdateEvent += new EventHandler<DataChangeEventArg>(Instance_DataUpdateEvent);

                _pipeServer.Listen("TestPipe");
                TbSVMLog = "'TestPipe' Pipe Always Active & Listening - OK";
            }
            catch (Exception)
            {
                TbSVMLog = "Pipe Error Listening";
            }

            // prepare eye voxels to be filtered. based on guipreferences values                
            TbSVMLog = "Preparing Filtered Eye Voxels. " + GuiPreferences.Instance.NudEyeSliceFirstLines +
            " first lines in " + GuiPreferences.Instance.NudFilterEyeSlices.ToString() +
            " slices are ignored."; 
            prepareFilteredEyeVoxels();

            TbSVMLog = "Pipe Loading is Done Inside Form1, only in Testing Mode";



            //////////////////////////////// initializations... were taken from udowekasmomultirun()            ///////////////////////
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.DataType.rawValue;

            //assigned after we know what to assign from the protocol
            //PublicMethods.setClassesLabels();
            GuiPreferences.Instance.CmbClass1Selected = 1; //left
            GuiPreferences.Instance.CmbClass2Selected = 2; //right                                

            //load a model for testing
            GuiPreferences.Instance.WorkDirectory = testDataFolder;
            //next line moved to form.cs
            //Preferences.Instance.RealTimeModelDirectory = ModelDataFolder; // assigning the model directory so that we can load the param file later on in the RealTimeProcess() func.
            GuiPreferences.Instance.TrainType = TrainingType.Weka;
            
            GuiPreferences.Instance.setLog("Deserializing Model");
            WekaTrainingMethods.loadModel();

            //IG features are 0-based
            TrainingTesting_SharedVariables._trainTopIGFeatures = XMLSerializer.DeserializeFile<int[]>(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG_indices.xml");
            GuiPreferences.Instance.setLog("Loading IG Features: #" + TrainingTesting_SharedVariables._trainTopIGFeatures.Length.ToString());

            //store minmax per TR. add one data structure to the list. same thing is done for each training set in the training.
            Preferences.Instance.MinMax.Add(new Dictionary<int, MinMax>());

            //sanity check: for all the IGs checking if the ranges are negative.
            int lowrange = 0;
            int highrange = 0;
            for (int k = 0; k < TrainingTesting_SharedVariables._trainTopIGFeatures.Length; k++)
            {
                if (Preferences.Instance.medianRange.feature_max[TrainingTesting_SharedVariables._trainTopIGFeatures[k] + 1] <= 0)
                    highrange++;
                if (Preferences.Instance.medianRange.feature_min[TrainingTesting_SharedVariables._trainTopIGFeatures[k] + 1] <= 0)
                    lowrange++;

            }
            GuiPreferences.Instance.setLog("High Range < 0 : " + highrange.ToString() + " && Low Range < 0 : " + lowrange.ToString() +" (both should be zero)");

            //PublicMethods._trainTopIGFeatures = PublicMethods.DeserializeArrayToFile(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG_indices.xml");

            //this should be done ONCE - move elsewhere.
            Preferences.Instance.fastvector = RealTimeProcessing.CreateFastVector(TrainingTesting_SharedVariables._trainTopIGFeatures.Length);
            
            //all of the above were inside this function>
            RealTimeProcessing.UdpWekaSMOMultiRun(vdatDataFolder, testDataFolder);            
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
        }
        
        private  void LoadNormalizationFormula()
        {
            switch (GuiPreferences.Instance.NormalizedType)
            {
                case NormalizationType.None:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_None();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_None();
                        GuiPreferences.Instance.setLog(
                            "Not Normalizing");
                        break;
                    }
                case NormalizationType.MinMax:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_MinMax();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_Range();
                        GuiPreferences.Instance.setLog(
                            "MinMax Formula assigned to training and Range assigned to testing");
                        break;
                    }
                case NormalizationType.RawMinusMed:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_RawMinusMed();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_RawMinusMed();
                        GuiPreferences.Instance.setLog(
                            "Normalize_RawMinusMed Formula assigned to training and testing");
                        break;
                    }
                case NormalizationType.RawDivMed:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_RawDivMed();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_RawDivMed();
                        GuiPreferences.Instance.setLog(
                            "Normalize_RawDivMed Formula assigned to training and testing");
                        break;
                    }
                case NormalizationType.RawMinusMedDivMed:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_RawMinusMedDivMed();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_RawMinusMedDivMed();
                        GuiPreferences.Instance.setLog(
                            "Normalize_RawMinusMedDivMed Formula assigned to training and testing");
                        break;
                    }
                case NormalizationType.RawMinusMedWin:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_RawMinusMedWin();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_RawMinusMedWin();
                        GuiPreferences.Instance.setLog(
                            @"Detrending/Normalization Normalize_RawMinusMedWin Formula assigned to training and testing, Window: " + GuiPreferences.Instance.NudMovingWindow.ToString());
                        break;
                    }
                case NormalizationType.RawMinusMedWinDivIqrWin:
                    {
                        // Train
                        NormalizationManager.svmscale = new Normalize_RawMinusMedWinIqrWin();
                        // Test
                        TrainingTesting_SharedVariables._svmscaleTesting = new Normalize_RawMinusMedWinIqrWin();
                        GuiPreferences.Instance.setLog(
                            @"Detrending/Normalization Normalize_RawMinusMedWinIqrWin Formula assigned to training and testing, Window: " + GuiPreferences.Instance.NudMovingWindow.ToString());
                        break;
                    }
            }
        }

        private void UDP_Click(object sender, EventArgs e)
        {  
            //inits a udp connection that listens for filenames (from unity or some sender), after receiving them, we load the raw filename, process the data, send classification to unity.
            Preferences.Instance.RealTimeTestingSingleVector = true; 
            t.testUdpWekaSMO();


            //for debugging purposes, if we dont want to use unity as client that sends file names, we can enabled this line to send filenames every X time.
            UDPServerLoop();


            //t.testGLM();
        }

        
        private void button5_Click(object sender, EventArgs e)
        {
            UDPServerLoop();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // trains ML for real time, no stops along the way.
            Preferences.Instance.RealTimeTestingSingleVector = false;
            t.QuickProcessWekaPipeline(1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            t.TestIronPython();
        }

        private void cbSVM_CheckedChanged(object sender, EventArgs e)
        {

            GuiPreferences.Instance.CbSVMChecked = !GuiPreferences.Instance.CbSVMChecked;
        }

        private void cbSMO_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.CbSMOChecked = !GuiPreferences.Instance.CbSMOChecked;
        }

        /// <summary>
        /// populate treeview with all the folders in all available drives.
        /// </summary>
        /// <param name="node"></param>
        bool FillChildNodes(TreeNode node)
        {
           try
           {
               DirectoryInfo dirs = new DirectoryInfo(node.FullPath); 
               if (!dirs.Exists) return false;
               foreach (DirectoryInfo dir in dirs.GetDirectories())
               { 
                    TreeNode newnode = new TreeNode(dir.Name);
                    node.Nodes.Add(newnode);
                    newnode.Nodes.Add("*"); 
               }
           }
           catch(Exception ex)
           {
               MessageBox.Show(node.FullPath.ToString() + " " + ex.Message.ToString());
           }
           return true;
        }

        /// <summary>
        /// before expanding nodes populate with folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
           if (e.Node.Nodes[0].Text == "*")
           {
               e.Node.Nodes.Clear();
               FillChildNodes(e.Node);
           }
        }

        private bool FindByText(string text)
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            foreach (TreeNode n in nodes)
            {
                bool res = FindRecursive(n, text);
                if (res)
                    return res;
            }
            return false;
        }

        private bool FindRecursive(TreeNode treeNode,string text)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                // if the text properties match, color the item
                if (tn.Text == text)
                    return true;

                FindRecursive(tn,text);
            }
            return false;
        }
        
        /// <summary>
        /// after node is checked: add to datagridview or delete if unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                int trainCount = countFilesInFolder(e.Node.FullPath,"*.vdat");
                if (trainCount == Preferences.Instance.events.EventListLastTr)
                {
                    dataGridView1.Rows.Add(e.Node.FullPath + @"\", trainCount, true, false, false);
                    return;
                }

                if (trainCount > 0)
                {
                    //error uncheck in tree
                    showMessageBox("Amount of files in folder doesnt match the number of trs in protocol file");
                    e.Node.Checked = false;
                    return;
                }

                int testCount = countFilesInFolder(e.Node.FullPath, "*.model");
                if (testCount == 1)
                {
                    dataGridView1.Rows.Add(e.Node.FullPath + @"\", testCount, false, true, false);
                    return;
                }

                //empty dir is acceptable as a data holder for RT testing
                /*testCount = countFilesInFolder(e.Node.FullPath, "*.*");
                if (testCount == 0)
                {
                    dataGridView1.Rows.Add(e.Node.FullPath + @"\", testCount, false, false, true);
                    return;
                } */
                //error uncheck in tree 
                showMessageBox("*.vdat or Model files not found");
                e.Node.Checked = false;  
            }
            else
            {
                for (int i=0;i<dataGridView1.Rows.Count;i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null)                    
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString() == (e.Node.FullPath + @"\"))
                        {
                            dataGridView1.Rows.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
        }

        public void CheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = true;
                CheckChildren(node, true);
            }
        }

        public void UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = false;
                CheckChildren(node, false);
            }
        }

        private void CheckChildren(TreeNode rootNode, bool isChecked)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, isChecked);
                node.Checked = isChecked;
            }
        }

        /// <summary>
        /// count files in a specified folder
        /// </summary>
        private int countFilesInFolder(string folder, string extension)
        { 
            //find how many files are there
            string[] files = Directory.GetFiles(folder, extension);
            if (files.Length==0)
            {
                return 0;
            }
            Preferences.Instance.filesInWorkingDir = new string[files.Length];
            Array.Copy(files, Preferences.Instance.filesInWorkingDir, files.Length);
            return files.Length;
        }

        private void showMessageBox(string ex)
        {
            MessageBox.Show(ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private List<string> getCurrentFolderList()
        {
            List<string> CurrentFolderList = new List<string>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                CurrentFolderList.Add(dataGridView1.Rows[i].Cells[0].Value.ToString().Substring(0,dataGridView1.Rows[i].Cells[0].Value.ToString().Length - 1));
            }
            return CurrentFolderList;
        }

        private void reloadDirectoryStructure()
        {
            List<string> CurrentFolderList = getCurrentFolderList();
            UncheckAllNodes(treeView1.Nodes);
            initFolderTreeView();
            setCheckedTreeViewNodes(CurrentFolderList,treeView1.Nodes);
        }
        
        private void setCheckedTreeViewNodes(List<string> CurrentFolderList, TreeNodeCollection nodes)
        {
            foreach (string fullpath in CurrentFolderList)
            {
                foreach (TreeNode n in nodes)
                {
                    if (n.FullPath.Equals(fullpath))
                        n.Checked = true;
                    setRecoursiveCheckedTreeViewNodes(fullpath, n);
                }
            }
        } 

        private void setRecoursiveCheckedTreeViewNodes(string fullPath, TreeNode node)
        {
            foreach (TreeNode n in node.Nodes)
            {
                if (n.FullPath.Equals(fullPath))
                    n.Checked = true;
                setRecoursiveCheckedTreeViewNodes(fullPath, n);
            }
        }

        private void cbOverwriteProcessedFiles_CheckedChanged(object sender, EventArgs e)
        {
           // GuiPreferences.Instance.CbOverwriteProcessedFilesChecked = !GuiPreferences.Instance.CbOverwriteProcessedFilesChecked;
        }

        private void nudExtractFromTR_ValueChanged(object sender, EventArgs e)
        { 
            GuiPreferences.Instance.NudExtractFromTR = nudExtractFromTR.Value;
        }

        private void nudExtractToTR_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudExtractToTR = nudExtractToTR.Value;
        }

        private void nudClassifyUsingTR_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudClassifyUsingTR = nudClassifyUsingTR.Value;
        }

        private void nudMovingWindow_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudMovingWindow = nudMovingWindow.Value;
        }

        private void nudIGThreshold_ValueChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.NudIGThreshold = nudIGThreshold.Value;
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {

            GuiPreferences.Instance.NudIGVoxelAmount = nudIGVoxelAmount.Value;
        } 

        private void cbPeekHigherTRsIG_CheckedChanged(object sender, EventArgs e)
        {   
            GuiPreferences.Instance.CbPeekHigherTRsIGChecked = cbPeekHigherTRsIG.Checked;
        }

        private void nudFilterEyeSlices_ValueChanged(object sender, EventArgs e)
        { 
            GuiPreferences.Instance.NudFilterEyeSlices = nudFilterEyeSlices.Value;
        }

        private void nudEyeSliceFirstLines_ValueChanged(object sender, EventArgs e)
        { 
            GuiPreferences.Instance.NudEyeSliceFirstLines = nudEyeSliceFirstLines.Value;
        }

        private void bStats_Click(object sender, EventArgs e)
        {
            StatisticsAccuracy.generateStats();
            TbSVMLog = TrainingTesting_SharedVariables.binary.getMeanAverage().ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //loads a test set that was normalized offline stand alone and uses the IG xml produced by the training runs to create a csv file that only has the features that were selected in IG.
            //used to compare the normalization formula of offline against the one produced by the online formula

            //for normalized
            Instances data;
            data = WekaTrainingMethods.loadDataSetFile(@"c:\compareOfflineOnlineNormalization\test_Set_offline_normalized_tr4.arff");

            //and for raw
            Instances data2;
            data2 = WekaTrainingMethods.loadDataSetFile(@"c:\compareOfflineOnlineNormalization\test_Set_offline_raw_tr4.libsvm");

            int[] topIGFeatures;
            topIGFeatures = XMLSerializer.DeserializeFile<int[]>(@"c:\compareOfflineOnlineNormalization\training_ig_indices.xml");

            data = WekaTrainingMethods.useRemoveFilter(data, topIGFeatures, true);
            WekaCommonFileOperation.SaveCSV(data,@"c:\compareOfflineOnlineNormalization\test_set_filteredIG_CSV.arff");

            data2 = WekaTrainingMethods.useRemoveFilter(data2, topIGFeatures, true);
            WekaCommonFileOperation.SaveCSV(data2, @"c:\compareOfflineOnlineNormalization\test_set_filteredIG_RAW_CSV.arff");

        }

        private void rbMinMax_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMinMax.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.MinMax;
        }

        private void rbRawMinusMed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRawMinusMed.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.RawMinusMed;
        }

        private void rbRawDivMed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRawDivMed.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.RawDivMed;
        }

        private void rbRawMinusMedDivMed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRawMinusMedDivMed.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.RawMinusMedDivMed;
        }

        private void rdRawMinusMedWinIqrWin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdRawMinusMedWinIqrWin.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.RawMinusMedWinDivIqrWin;
        }

        private void rbNoNorm_CheckedChanged(object sender, EventArgs e)
        { 
            if (rbNoNorm.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.None; 
        }

        private void rdRawMinusMedWin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdRawMinusMedWin.Checked)
                GuiPreferences.Instance.NormalizedType = NormalizationType.RawMinusMedWin;
        } 

        private void rbIGThreshold_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIGThreshold.Checked)
                GuiPreferences.Instance.IgSelectionType = IGType.Threshold;

        }

        private void rbVoxelCount_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIGVoxelCount.Checked)
                GuiPreferences.Instance.IgSelectionType = IGType.Voxels;
        }

        private void cbSoundON_CheckedChanged(object sender, EventArgs e)
        {
            GuiPreferences.Instance.CbSoundONChecked = !GuiPreferences.Instance.CbSoundONChecked; 
        }



    }
}
