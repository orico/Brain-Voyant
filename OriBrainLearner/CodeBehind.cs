using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using libSVMWrapper;
//using MathWorks.MATLAB.NET.Arrays;
using OriBrainLearnerCore;

namespace OriBrainLearner
{
    public partial class Form1 :Form
    {
        private void SelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEx cfbd = new FolderBrowserDialogEx();
            cfbd.Title = "Please choose an fMRI run directory";
            cfbd.SelectedPath = @"C:\";
            cfbd.ShowEditbox = true;
            cfbd.ShowNewFolderButton = false;
            cfbd.RootFolder = Environment.SpecialFolder.Desktop;
            cfbd.StartPosition = FormStartPosition.CenterScreen;

            DialogResult dr = cfbd.ShowDialog(this);
            string txtRtnCode = dr.ToString();
            string txtRtnFolder = "";
            if (dr == DialogResult.OK)
            {
                txtRtnFolder = cfbd.SelectedPath;
            }

            //for future inits else-where this is how this is done.
            //comboStartPos.DataSource = Enum.GetValues(typeof(FormStartPosition));
            //comboRootFolder.DataSource = Enum.GetValues(typeof(Environment.SpecialFolder));
            //(Environment.SpecialFolder)comboRootFolder.selectedValue
        }

        public bool SelectWorkingDirFiles()
        { 
            string extension = "*.err";
            //dataType inputType = dataType.None;
            openFileDialog1.FileName = "";
            if (GuiPreferences.Instance.FileType==DataType.SVMFile)
            {
                openFileDialog1.Filter = "(SVM Sparse File)|*.svm";
                extension = "*.svm";
                //inputType = dataType.SVMFile;
            }
            else if (GuiPreferences.Instance.FileType == DataType.betaValue)
            {
                openFileDialog1.Filter = "(Beta Values)|*.bdat";
                extension = "*.bdat";
                //inputType = dataType.betaValue;
            }
            else if (GuiPreferences.Instance.FileType == DataType.rawValue)
            {
                openFileDialog1.Filter = "(Voxel Raw Values)|*.vdat";
                extension = "*.vdat";
                //inputType = dataType.rawValue;
            }
            else if (GuiPreferences.Instance.FileType == DataType.tContrastValue)
            {
                openFileDialog1.Filter = "(T Contrast Values)|*.mdat";
                extension = "*.mdat";
                //inputType = dataType.tContrastValue;
            }

            openFileDialog1.InitialDirectory = @"D:\My_Dropbox\UProjects\OriBrainLearner\Data";

            DialogResult result = openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "")
            {
                TbSVMLog = "Import Failed: empty file name";
                return false;
            }
            if (result == DialogResult.OK)
            {
                getWorkingDir();
                updatingWorkingDirFileName(GuiPreferences.Instance.FileType);
                // no need to do a directory run if we are loading one svm file.
                if (GuiPreferences.Instance.FileType != DataType.SVMFile)
                {
                    GuiManager.getFilePaths(extension);
                    GuiManager.updateFilePaths();
                }
                else
                {
                    Preferences.Instance.filesInWorkingDir = new string[1];
                    GuiManager.updateFilePaths();
                }
            }
            else
            {
                TbSVMLog = "Import Failed: unknown reason";
                return false;
            }
            return true;
        } 

        private void getWorkingDir()
        {
            //get working directory and filename in strings.
            TbSVMLog = "Configuring Directory & Files";
            string target = @"\";
            char[] anyOf = target.ToCharArray();
            int lastSlash = openFileDialog1.FileName.LastIndexOfAny(anyOf);
            string WorkingDir = openFileDialog1.FileName.Substring(0, lastSlash + 1);
            string FileName = openFileDialog1.FileName.Substring(lastSlash + 1);
            if (GuiPreferences.Instance.FileType != DataType.SVMFile)
            {
                target = @"-";
                anyOf = target.ToCharArray();
                int lastDash = FileName.LastIndexOfAny(anyOf);
                FileName = FileName.Substring(0, lastDash + 1);

                //FileName = FileName.Substring(0, FileName.Length - 6);
            }
            else
                FileName = FileName.Substring(0, FileName.Length - 4);
            GuiPreferences.Instance.WorkDirectory = WorkingDir;
            GuiPreferences.Instance.FileName = FileName;
        }

        private void updatingWorkingDirFileName(DataType inputType)
        {
            string WorkingDir = GuiPreferences.Instance.WorkDirectory;
            string FileName = GuiPreferences.Instance.FileName;
            TbSVMLog = "Input Type: " + inputType.ToString();
            if (GuiPreferences.Instance.FileType != inputType)
                GuiPreferences.Instance.FileType = inputType;

            TbSVMLog = "Working Directory: " + WorkingDir;
            openFileDialog1.InitialDirectory = WorkingDir;
            //Preferences.Instance.workingDir = WorkingDir;
            GuiPreferences.Instance.WorkDirectory = WorkingDir;

            TbSVMLog = "File Name: " + FileName;
            GuiPreferences.Instance.FileName = FileName;
            //Preferences.Instance.inputFileName = FileName; 
        }


        private string svmLogLine;
        //add a line to the svmlog textbox
        public string TbSVMLog
        {
            set 
            {
                lock (this)
                {
                    svmLogLine = DateTime.Now.ToLongTimeString() + ": " + value + " \r\n";
                    updateTbSVMLog();
                }
            }
            private get { return null; }

        }

        private void updateTbSVMLog()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)updateTbSVMLog);
            }
            else
            {
                tbSVMLog.AppendText(svmLogLine);
            }

        }

        public string TbProtocolFileName
        {
            get { return tbProtocolFileName.Text; }
            set { tbProtocolFileName.Text = value; }

        }


        public static bool readProtocolCheck()
        {
            Preferences.Instance.prot = new ProtocolManager(true);
            // i assume that i dont want the protocol if it doesnt have enough Trs. an alternative would be to load it anyway but not import/process, 
            //which would lead to some back updating if you load a not bvalid protocol, then load data that fits the protocol. so that read protocol variable inside the 
            //class instance needs to be checked and verified from several places.
            if (!Preferences.Instance.prot.readProtocol)
            {
                Preferences.Instance.prot = null;
                return false;
            }
            return true;

        }

        public static bool readProtocolNoCheck()
        {
            Preferences.Instance.prot = new ProtocolManager();
            // i assume that i dont want the protocol if it doesnt have enough Trs. an alternative would be to load it anyway but not import/process, 
            //which would lead to some back updating if you load a not bvalid protocol, then load data that fits the protocol. so that read protocol variable inside the 
            //class instance needs to be checked and verified from several places.
            if (!Preferences.Instance.prot.readProtocol)
            {
                Preferences.Instance.prot = null;
                return false;
            }
            return true;

        }


        public bool selectProtocolFile()
        {
            openFileDialog2.Filter = "(Protocol File)|*.prt";
            openFileDialog2.FileName = "";
            openFileDialog2.InitialDirectory = @"D:\My_Dropbox\VERE\MRI_data\Tirosh\";
            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (openFileDialog2.FileName == "")
                {
                    TbSVMLog = "Import Protocol Failed: empty file name";
                    treeView1.Enabled = false;
                    return false; 
                }

                Program.mainForm.TbProtocolFileName = openFileDialog2.FileName;
                GuiPreferences.Instance.ProtocolFile = openFileDialog2.FileName;

                //for a single run use readProtocolCheck(), for multi treeview use NoCheck()
                if (readProtocolNoCheck())
                {
                    treeView1.Enabled = true;
                    Preferences.Instance.protocolLoaded = true; //loaded
                    Training_MultiRunProcessing.setClassesLabels();
                }
                else
                {
                    Preferences.Instance.protocolLoaded = false;
                        //not loaded if protocol amount of trs doesnt fit loaded data
                    return false;
                }

            }
            else
            {
                TbSVMLog = "Import Protocol Failed: unknown reason";
                treeView1.Enabled = false;
                return false;
            }
            //clear DataDridView and refresh GUI
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            UncheckAllNodes(treeView1.Nodes);
            return true;
        }
         
         
        private void clickUP()
        {
            if (this.lbFeatureProcessFlow.SelectedIndex == -1 || this.lbFeatureProcessFlow.SelectedIndex == 0)
                return;

            Object select, previous, temp;
            select = lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex];
            previous = lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex - 1];
            int selectedIndex = lbFeatureProcessFlow.SelectedIndex;
            temp = select;
            select = previous;
            previous = temp;
            lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex] = select;
            lbFeatureProcessFlow.Items[lbFeatureProcessFlow.SelectedIndex - 1] = previous;
            lbFeatureProcessFlow.SelectedIndices.Clear();
            lbFeatureProcessFlow.SelectedIndex = selectedIndex - 1;

            // selection manipulation for K
            Object selectK, previousK, tempK;
            selectK = lbK.Items[selectedIndex];
            previousK = lbK.Items[selectedIndex - 1];
            tempK = selectK;
            selectK = previousK;
            previousK = tempK;
            lbK.Items[selectedIndex] = selectK;
            lbK.Items[selectedIndex - 1] = previousK; ;
            lbK.SelectedIndices.Clear();
        }

        public void UDPServerLoop()
        {
            //ports are still hard coded! fix to use events and text boxes
            if (!Preferences.Instance.udpsim.isWorking())
            {
                Preferences.Instance.udpsim.SendLoop();
            }
            else
            {
                Preferences.Instance.udpsim.StopLoop();
            }
        }

    }
}
