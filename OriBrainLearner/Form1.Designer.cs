namespace OriBrainLearner
{
    public partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bImport = new System.Windows.Forms.Button();
            this.rbRawValues = new System.Windows.Forms.RadioButton();
            this.rbBetaValues = new System.Windows.Forms.RadioButton();
            this.rbTcontrastValues = new System.Windows.Forms.RadioButton();
            this.tbWorkingDir = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.nudEyeSliceFirstLines = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.nudFilterEyeSlices = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nudToTR = new System.Windows.Forms.NumericUpDown();
            this.nudThresholdSVM = new System.Windows.Forms.NumericUpDown();
            this.nudTConThreshold = new System.Windows.Forms.NumericUpDown();
            this.rbSVM = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.nudFromTR = new System.Windows.Forms.NumericUpDown();
            this.nudBetaThreshold = new System.Windows.Forms.NumericUpDown();
            this.nudThreshold = new System.Windows.Forms.NumericUpDown();
            this.gbTrainTestSplit = new System.Windows.Forms.GroupBox();
            this.cbAutoAll = new System.Windows.Forms.CheckBox();
            this.cbSVM = new System.Windows.Forms.CheckBox();
            this.cbSMO = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.nudTrainTestSplit = new System.Windows.Forms.NumericUpDown();
            this.rbRFE = new System.Windows.Forms.RadioButton();
            this.rbGridSearch = new System.Windows.Forms.RadioButton();
            this.rbCrossValidation = new System.Windows.Forms.RadioButton();
            this.rbTrainTestSplit = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.nudGridFolds = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.lFolds = new System.Windows.Forms.Label();
            this.nudCvFolds = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.rbWekaPipeline = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbIGVoxelCount = new System.Windows.Forms.RadioButton();
            this.rbIGThreshold = new System.Windows.Forms.RadioButton();
            this.nudIGThreshold = new System.Windows.Forms.NumericUpDown();
            this.nudIGVoxelAmount = new System.Windows.Forms.NumericUpDown();
            this.cbPeekHigherTRsIG = new System.Windows.Forms.CheckBox();
            this.cmbClass1 = new System.Windows.Forms.ComboBox();
            this.cmbClass2 = new System.Windows.Forms.ComboBox();
            this.bTrain = new System.Windows.Forms.Button();
            this.lClass1 = new System.Windows.Forms.Label();
            this.lClass2 = new System.Windows.Forms.Label();
            this.cbMultiClass = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.nudExtractFromTR = new System.Windows.Forms.NumericUpDown();
            this.nudExtractToTR = new System.Windows.Forms.NumericUpDown();
            this.nudClassifyUsingTR = new System.Windows.Forms.NumericUpDown();
            this.cbOverwriteProcessedFiles = new System.Windows.Forms.CheckBox();
            this.bProcess = new System.Windows.Forms.Button();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bLoadContrastFile = new System.Windows.Forms.Button();
            this.tbContrastFileName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbTRs = new System.Windows.Forms.TextBox();
            this.bSelectFiles = new System.Windows.Forms.Button();
            this.bLoadProtocol = new System.Windows.Forms.Button();
            this.tbProtocolFileName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Folder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Files = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Train = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Model = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TestDir = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbPerEvent = new System.Windows.Forms.CheckBox();
            this.bExport = new System.Windows.Forms.Button();
            this.rbCSV = new System.Windows.Forms.RadioButton();
            this.rbSparse = new System.Windows.Forms.RadioButton();
            this.tbSVMLog = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbFeatureProcessingUnprocessed = new System.Windows.Forms.CheckBox();
            this.lbFeatureProcessOptions = new System.Windows.Forms.ListBox();
            this.gbSVMOutput = new System.Windows.Forms.GroupBox();
            this.gbFeatureProcessFlow = new System.Windows.Forms.GroupBox();
            this.bPlus = new System.Windows.Forms.Button();
            this.bMinus = new System.Windows.Forms.Button();
            this.lbK = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lbFeatureProcessFlow = new System.Windows.Forms.ListBox();
            this.bUp = new System.Windows.Forms.Button();
            this.bDown = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.nudPreOnset = new System.Windows.Forms.NumericUpDown();
            this.nudPostOnset = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cbOnsetFromProtocol = new System.Windows.Forms.CheckBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.bLoadModel = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.UnityPort = new System.Windows.Forms.TextBox();
            this.LocalPort = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.UDP = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.UnityIP = new System.Windows.Forms.TextBox();
            this.LocalIP = new System.Windows.Forms.TextBox();
            this.bStats = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.bLeft = new System.Windows.Forms.Button();
            this.bRight = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.bSaveModel = new System.Windows.Forms.Button();
            this.bPlot = new System.Windows.Forms.Button();
            this.gbProcessing = new System.Windows.Forms.GroupBox();
            this.rdRawMinusMedWin = new System.Windows.Forms.RadioButton();
            this.rbNoNorm = new System.Windows.Forms.RadioButton();
            this.nudMovingWindow = new System.Windows.Forms.NumericUpDown();
            this.rdRawMinusMedWinIqrWin = new System.Windows.Forms.RadioButton();
            this.rbRawMinusMedDivMed = new System.Windows.Forms.RadioButton();
            this.rbRawDivMed = new System.Windows.Forms.RadioButton();
            this.rbRawMinusMed = new System.Windows.Forms.RadioButton();
            this.rbMinMax = new System.Windows.Forms.RadioButton();
            this.button3 = new System.Windows.Forms.Button();
            this.cbSoundON = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEyeSliceFirstLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFilterEyeSlices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToTR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThresholdSVM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTConThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFromTR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBetaThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).BeginInit();
            this.gbTrainTestSplit.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrainTestSplit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGridFolds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCvFolds)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIGThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIGVoxelAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtractFromTR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtractToTR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudClassifyUsingTR)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.gbSVMOutput.SuspendLayout();
            this.gbFeatureProcessFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreOnset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPostOnset)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.gbProcessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMovingWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // bImport
            // 
            this.bImport.Location = new System.Drawing.Point(5, 215);
            this.bImport.Name = "bImport";
            this.bImport.Size = new System.Drawing.Size(133, 30);
            this.bImport.TabIndex = 0;
            this.bImport.Text = "Import Data";
            this.toolTip1.SetToolTip(this.bImport, "push to import, will reselect & reload working directory if data type changed, wi" +
                    "ll reselect & reload protocol if not loaded previously. otherwise it will import" +
                    " quietly.");
            this.bImport.UseVisualStyleBackColor = true;
            this.bImport.Click += new System.EventHandler(this.bImport_Click);
            // 
            // rbRawValues
            // 
            this.rbRawValues.AutoSize = true;
            this.rbRawValues.Checked = true;
            this.rbRawValues.Location = new System.Drawing.Point(6, 20);
            this.rbRawValues.Name = "rbRawValues";
            this.rbRawValues.Size = new System.Drawing.Size(74, 17);
            this.rbRawValues.TabIndex = 6;
            this.rbRawValues.TabStop = true;
            this.rbRawValues.Text = "rawValues";
            this.toolTip1.SetToolTip(this.rbRawValues, "import raw values");
            this.rbRawValues.UseVisualStyleBackColor = true;
            this.rbRawValues.CheckedChanged += new System.EventHandler(this.rbRawValues_CheckedChanged);
            // 
            // rbBetaValues
            // 
            this.rbBetaValues.AutoSize = true;
            this.rbBetaValues.Enabled = false;
            this.rbBetaValues.Location = new System.Drawing.Point(6, 89);
            this.rbBetaValues.Name = "rbBetaValues";
            this.rbBetaValues.Size = new System.Drawing.Size(78, 17);
            this.rbBetaValues.TabIndex = 8;
            this.rbBetaValues.Text = "betaValues";
            this.toolTip1.SetToolTip(this.rbBetaValues, "import beta values");
            this.rbBetaValues.UseVisualStyleBackColor = true;
            this.rbBetaValues.Visible = false;
            this.rbBetaValues.CheckedChanged += new System.EventHandler(this.rbBetaValues_CheckedChanged);
            // 
            // rbTcontrastValues
            // 
            this.rbTcontrastValues.AutoSize = true;
            this.rbTcontrastValues.Enabled = false;
            this.rbTcontrastValues.Location = new System.Drawing.Point(6, 112);
            this.rbTcontrastValues.Name = "rbTcontrastValues";
            this.rbTcontrastValues.Size = new System.Drawing.Size(60, 17);
            this.rbTcontrastValues.TabIndex = 10;
            this.rbTcontrastValues.Text = "tValues";
            this.toolTip1.SetToolTip(this.rbTcontrastValues, "import t contrast values");
            this.rbTcontrastValues.UseVisualStyleBackColor = true;
            this.rbTcontrastValues.Visible = false;
            this.rbTcontrastValues.CheckedChanged += new System.EventHandler(this.rbTcontrastValues_CheckedChanged);
            // 
            // tbWorkingDir
            // 
            this.tbWorkingDir.Location = new System.Drawing.Point(66, 270);
            this.tbWorkingDir.Name = "tbWorkingDir";
            this.tbWorkingDir.Size = new System.Drawing.Size(400, 20);
            this.tbWorkingDir.TabIndex = 1;
            this.tbWorkingDir.Text = "\\My_Dropbox\\UProjects\\OriBrainLearner\\Data";
            this.toolTip1.SetToolTip(this.tbWorkingDir, "Sets your working directory");
            this.tbWorkingDir.Visible = false;
            this.tbWorkingDir.TextChanged += new System.EventHandler(this.tbWorkingDir_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.nudEyeSliceFirstLines);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.nudFilterEyeSlices);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.nudToTR);
            this.groupBox1.Controls.Add(this.nudThresholdSVM);
            this.groupBox1.Controls.Add(this.nudTConThreshold);
            this.groupBox1.Controls.Add(this.rbSVM);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.nudFromTR);
            this.groupBox1.Controls.Add(this.bImport);
            this.groupBox1.Controls.Add(this.nudBetaThreshold);
            this.groupBox1.Controls.Add(this.rbRawValues);
            this.groupBox1.Controls.Add(this.rbBetaValues);
            this.groupBox1.Controls.Add(this.rbTcontrastValues);
            this.groupBox1.Controls.Add(this.nudThreshold);
            this.groupBox1.Location = new System.Drawing.Point(7, 399);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 251);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "3 - Data Type Input";
            this.toolTip1.SetToolTip(this.groupBox1, "choose data type");
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(5, 68);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 13);
            this.label21.TabIndex = 47;
            this.label21.Text = "Eye Slice Lines";
            this.toolTip1.SetToolTip(this.label21, "Eye Slice Lines - how many lines from the top of a slice to remove");
            // 
            // nudEyeSliceFirstLines
            // 
            this.nudEyeSliceFirstLines.Location = new System.Drawing.Point(92, 66);
            this.nudEyeSliceFirstLines.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.nudEyeSliceFirstLines.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEyeSliceFirstLines.Name = "nudEyeSliceFirstLines";
            this.nudEyeSliceFirstLines.Size = new System.Drawing.Size(46, 20);
            this.nudEyeSliceFirstLines.TabIndex = 46;
            this.nudEyeSliceFirstLines.Tag = "";
            this.toolTip1.SetToolTip(this.nudEyeSliceFirstLines, "Eye Slice Lines - how many lines from the top of a slice to remove");
            this.nudEyeSliceFirstLines.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.nudEyeSliceFirstLines.ValueChanged += new System.EventHandler(this.nudEyeSliceFirstLines_ValueChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(5, 45);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(75, 13);
            this.label20.TabIndex = 45;
            this.label20.Text = "Eye Slices Filtr";
            this.toolTip1.SetToolTip(this.label20, "Eye Slices Filter - if 0 will not filter, if more than 0 will filter every slice " +
                    "from 1 to N");
            // 
            // nudFilterEyeSlices
            // 
            this.nudFilterEyeSlices.Location = new System.Drawing.Point(92, 43);
            this.nudFilterEyeSlices.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudFilterEyeSlices.Name = "nudFilterEyeSlices";
            this.nudFilterEyeSlices.Size = new System.Drawing.Size(46, 20);
            this.nudFilterEyeSlices.TabIndex = 14;
            this.nudFilterEyeSlices.Tag = "";
            this.toolTip1.SetToolTip(this.nudFilterEyeSlices, "Eye Slices Filter - if 0 will not filter, if more than 0 will filter every slice " +
                    "from 1 to N");
            this.nudFilterEyeSlices.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudFilterEyeSlices.ValueChanged += new System.EventHandler(this.nudFilterEyeSlices_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "To TR:";
            this.toolTip1.SetToolTip(this.label5, "uses up limited amount of TRs when importing");
            // 
            // nudToTR
            // 
            this.nudToTR.Enabled = false;
            this.nudToTR.Location = new System.Drawing.Point(83, 181);
            this.nudToTR.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudToTR.Name = "nudToTR";
            this.nudToTR.Size = new System.Drawing.Size(55, 20);
            this.nudToTR.TabIndex = 4;
            this.toolTip1.SetToolTip(this.nudToTR, "uses up limited amount of TRs when importing");
            this.nudToTR.ValueChanged += new System.EventHandler(this.nudToTR_ValueChanged);
            // 
            // nudThresholdSVM
            // 
            this.nudThresholdSVM.DecimalPlaces = 1;
            this.nudThresholdSVM.Location = new System.Drawing.Point(85, 135);
            this.nudThresholdSVM.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudThresholdSVM.Name = "nudThresholdSVM";
            this.nudThresholdSVM.Size = new System.Drawing.Size(53, 20);
            this.nudThresholdSVM.TabIndex = 0;
            this.nudThresholdSVM.Tag = "";
            this.toolTip1.SetToolTip(this.nudThresholdSVM, "minimum threshold for svm values (default is 100.0), integer for raw values, deci" +
                    "mal for betas/tcontrast.");
            this.nudThresholdSVM.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudThresholdSVM.Visible = false;
            this.nudThresholdSVM.ValueChanged += new System.EventHandler(this.nudThresholdSVM_ValueChanged);
            // 
            // nudTConThreshold
            // 
            this.nudTConThreshold.DecimalPlaces = 1;
            this.nudTConThreshold.Enabled = false;
            this.nudTConThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTConThreshold.Location = new System.Drawing.Point(92, 112);
            this.nudTConThreshold.Name = "nudTConThreshold";
            this.nudTConThreshold.Size = new System.Drawing.Size(46, 20);
            this.nudTConThreshold.TabIndex = 11;
            this.toolTip1.SetToolTip(this.nudTConThreshold, "minimum threshold for t contrast values (default is 0)");
            this.nudTConThreshold.Visible = false;
            this.nudTConThreshold.ValueChanged += new System.EventHandler(this.nudTConThreshold_ValueChanged);
            // 
            // rbSVM
            // 
            this.rbSVM.AutoSize = true;
            this.rbSVM.Location = new System.Drawing.Point(6, 135);
            this.rbSVM.Name = "rbSVM";
            this.rbSVM.Size = new System.Drawing.Size(67, 17);
            this.rbSVM.TabIndex = 12;
            this.rbSVM.Text = "SVM File";
            this.toolTip1.SetToolTip(this.rbSVM, "import svm file");
            this.rbSVM.UseVisualStyleBackColor = true;
            this.rbSVM.Visible = false;
            this.rbSVM.CheckedChanged += new System.EventHandler(this.rbSVM_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "From TR:";
            this.toolTip1.SetToolTip(this.label4, "uses up limited amount of TRs when importing");
            // 
            // nudFromTR
            // 
            this.nudFromTR.Enabled = false;
            this.nudFromTR.Location = new System.Drawing.Point(83, 158);
            this.nudFromTR.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFromTR.Name = "nudFromTR";
            this.nudFromTR.Size = new System.Drawing.Size(55, 20);
            this.nudFromTR.TabIndex = 2;
            this.toolTip1.SetToolTip(this.nudFromTR, "uses up limited amount of TRs when importing");
            this.nudFromTR.ValueChanged += new System.EventHandler(this.nudFromTR_ValueChanged);
            // 
            // nudBetaThreshold
            // 
            this.nudBetaThreshold.DecimalPlaces = 1;
            this.nudBetaThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudBetaThreshold.Location = new System.Drawing.Point(92, 89);
            this.nudBetaThreshold.Name = "nudBetaThreshold";
            this.nudBetaThreshold.Size = new System.Drawing.Size(46, 20);
            this.nudBetaThreshold.TabIndex = 9;
            this.toolTip1.SetToolTip(this.nudBetaThreshold, "minimum threshold for beta values (default is 0)");
            this.nudBetaThreshold.Visible = false;
            this.nudBetaThreshold.ValueChanged += new System.EventHandler(this.nudBetaThreshold_ValueChanged);
            // 
            // nudThreshold
            // 
            this.nudThreshold.Location = new System.Drawing.Point(92, 20);
            this.nudThreshold.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudThreshold.Name = "nudThreshold";
            this.nudThreshold.Size = new System.Drawing.Size(46, 20);
            this.nudThreshold.TabIndex = 7;
            this.nudThreshold.Tag = "";
            this.toolTip1.SetToolTip(this.nudThreshold, "threshold for raw values, anything above value will be kept (default is 200)");
            this.nudThreshold.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nudThreshold.ValueChanged += new System.EventHandler(this.nudThreshold_ValueChanged);
            // 
            // gbTrainTestSplit
            // 
            this.gbTrainTestSplit.Controls.Add(this.cbAutoAll);
            this.gbTrainTestSplit.Controls.Add(this.cbSVM);
            this.gbTrainTestSplit.Controls.Add(this.cbSMO);
            this.gbTrainTestSplit.Controls.Add(this.panel2);
            this.gbTrainTestSplit.Controls.Add(this.rbWekaPipeline);
            this.gbTrainTestSplit.Controls.Add(this.panel1);
            this.gbTrainTestSplit.Controls.Add(this.cmbClass1);
            this.gbTrainTestSplit.Controls.Add(this.cmbClass2);
            this.gbTrainTestSplit.Controls.Add(this.bTrain);
            this.gbTrainTestSplit.Controls.Add(this.lClass1);
            this.gbTrainTestSplit.Controls.Add(this.lClass2);
            this.gbTrainTestSplit.Controls.Add(this.cbMultiClass);
            this.gbTrainTestSplit.Location = new System.Drawing.Point(343, 399);
            this.gbTrainTestSplit.Name = "gbTrainTestSplit";
            this.gbTrainTestSplit.Size = new System.Drawing.Size(230, 316);
            this.gbTrainTestSplit.TabIndex = 7;
            this.gbTrainTestSplit.TabStop = false;
            this.gbTrainTestSplit.Text = "6B - Training";
            // 
            // cbAutoAll
            // 
            this.cbAutoAll.AutoSize = true;
            this.cbAutoAll.Checked = true;
            this.cbAutoAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoAll.Location = new System.Drawing.Point(8, 294);
            this.cbAutoAll.Name = "cbAutoAll";
            this.cbAutoAll.Size = new System.Drawing.Size(62, 17);
            this.cbAutoAll.TabIndex = 48;
            this.cbAutoAll.Text = "Auto All";
            this.cbAutoAll.UseVisualStyleBackColor = true;
            // 
            // cbSVM
            // 
            this.cbSVM.AutoSize = true;
            this.cbSVM.Location = new System.Drawing.Point(154, 64);
            this.cbSVM.Name = "cbSVM";
            this.cbSVM.Size = new System.Drawing.Size(49, 17);
            this.cbSVM.TabIndex = 35;
            this.cbSVM.Text = "SVM";
            this.cbSVM.UseVisualStyleBackColor = true;
            this.cbSVM.CheckedChanged += new System.EventHandler(this.cbSVM_CheckedChanged);
            // 
            // cbSMO
            // 
            this.cbSMO.AutoSize = true;
            this.cbSMO.Checked = true;
            this.cbSMO.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSMO.Location = new System.Drawing.Point(103, 64);
            this.cbSMO.Name = "cbSMO";
            this.cbSMO.Size = new System.Drawing.Size(50, 17);
            this.cbSMO.TabIndex = 34;
            this.cbSMO.Text = "SMO";
            this.cbSMO.UseVisualStyleBackColor = true;
            this.cbSMO.CheckedChanged += new System.EventHandler(this.cbSMO_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.nudTrainTestSplit);
            this.panel2.Controls.Add(this.rbRFE);
            this.panel2.Controls.Add(this.rbGridSearch);
            this.panel2.Controls.Add(this.rbCrossValidation);
            this.panel2.Controls.Add(this.rbTrainTestSplit);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.nudGridFolds);
            this.panel2.Controls.Add(this.numericUpDown2);
            this.panel2.Controls.Add(this.lFolds);
            this.panel2.Controls.Add(this.nudCvFolds);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(5, 89);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(221, 85);
            this.panel2.TabIndex = 47;
            // 
            // nudTrainTestSplit
            // 
            this.nudTrainTestSplit.DecimalPlaces = 1;
            this.nudTrainTestSplit.Enabled = false;
            this.nudTrainTestSplit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTrainTestSplit.Location = new System.Drawing.Point(178, 1);
            this.nudTrainTestSplit.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTrainTestSplit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTrainTestSplit.Name = "nudTrainTestSplit";
            this.nudTrainTestSplit.Size = new System.Drawing.Size(40, 20);
            this.nudTrainTestSplit.TabIndex = 32;
            this.nudTrainTestSplit.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudTrainTestSplit.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // rbRFE
            // 
            this.rbRFE.AutoSize = true;
            this.rbRFE.Enabled = false;
            this.rbRFE.Location = new System.Drawing.Point(1, 64);
            this.rbRFE.Name = "rbRFE";
            this.rbRFE.Size = new System.Drawing.Size(46, 17);
            this.rbRFE.TabIndex = 31;
            this.rbRFE.Text = "RFE";
            this.rbRFE.UseVisualStyleBackColor = true;
            this.rbRFE.CheckedChanged += new System.EventHandler(this.rbRFE_CheckedChanged);
            // 
            // rbGridSearch
            // 
            this.rbGridSearch.AutoSize = true;
            this.rbGridSearch.Enabled = false;
            this.rbGridSearch.Location = new System.Drawing.Point(1, 43);
            this.rbGridSearch.Name = "rbGridSearch";
            this.rbGridSearch.Size = new System.Drawing.Size(143, 17);
            this.rbGridSearch.TabIndex = 30;
            this.rbGridSearch.Text = "Grid Search (Best Model)";
            this.rbGridSearch.UseVisualStyleBackColor = true;
            this.rbGridSearch.CheckedChanged += new System.EventHandler(this.rbGridSearch_CheckedChanged);
            // 
            // rbCrossValidation
            // 
            this.rbCrossValidation.AutoSize = true;
            this.rbCrossValidation.Enabled = false;
            this.rbCrossValidation.Location = new System.Drawing.Point(1, 22);
            this.rbCrossValidation.Name = "rbCrossValidation";
            this.rbCrossValidation.Size = new System.Drawing.Size(147, 17);
            this.rbCrossValidation.TabIndex = 29;
            this.rbCrossValidation.Text = "Cross Validation no Model";
            this.rbCrossValidation.UseVisualStyleBackColor = true;
            this.rbCrossValidation.CheckedChanged += new System.EventHandler(this.rbCrossValidation_CheckedChanged);
            // 
            // rbTrainTestSplit
            // 
            this.rbTrainTestSplit.AutoSize = true;
            this.rbTrainTestSplit.Enabled = false;
            this.rbTrainTestSplit.Location = new System.Drawing.Point(1, 1);
            this.rbTrainTestSplit.Name = "rbTrainTestSplit";
            this.rbTrainTestSplit.Size = new System.Drawing.Size(171, 17);
            this.rbTrainTestSplit.TabIndex = 28;
            this.rbTrainTestSplit.Text = "Chronological Train / Test Split";
            this.rbTrainTestSplit.UseVisualStyleBackColor = true;
            this.rbTrainTestSplit.CheckedChanged += new System.EventHandler(this.rbTrainTestSplit_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(146, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 13);
            this.label13.TabIndex = 27;
            this.label13.Text = "Folds";
            // 
            // nudGridFolds
            // 
            this.nudGridFolds.Enabled = false;
            this.nudGridFolds.Location = new System.Drawing.Point(180, 43);
            this.nudGridFolds.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudGridFolds.Name = "nudGridFolds";
            this.nudGridFolds.Size = new System.Drawing.Size(38, 20);
            this.nudGridFolds.TabIndex = 26;
            this.nudGridFolds.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudGridFolds.ValueChanged += new System.EventHandler(this.nudGridFolds_ValueChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Enabled = false;
            this.numericUpDown2.Location = new System.Drawing.Point(180, 63);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(38, 20);
            this.numericUpDown2.TabIndex = 17;
            this.numericUpDown2.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // lFolds
            // 
            this.lFolds.AutoSize = true;
            this.lFolds.Location = new System.Drawing.Point(146, 26);
            this.lFolds.Name = "lFolds";
            this.lFolds.Size = new System.Drawing.Size(32, 13);
            this.lFolds.TabIndex = 10;
            this.lFolds.Text = "Folds";
            // 
            // nudCvFolds
            // 
            this.nudCvFolds.Enabled = false;
            this.nudCvFolds.Location = new System.Drawing.Point(180, 22);
            this.nudCvFolds.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCvFolds.Name = "nudCvFolds";
            this.nudCvFolds.Size = new System.Drawing.Size(38, 20);
            this.nudCvFolds.TabIndex = 2;
            this.nudCvFolds.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCvFolds.ValueChanged += new System.EventHandler(this.nudCvFolds_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(87, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "| Elimination Steps:";
            // 
            // rbWekaPipeline
            // 
            this.rbWekaPipeline.AutoSize = true;
            this.rbWekaPipeline.Checked = true;
            this.rbWekaPipeline.Location = new System.Drawing.Point(7, 64);
            this.rbWekaPipeline.Name = "rbWekaPipeline";
            this.rbWekaPipeline.Size = new System.Drawing.Size(94, 17);
            this.rbWekaPipeline.TabIndex = 33;
            this.rbWekaPipeline.TabStop = true;
            this.rbWekaPipeline.Text = "Weka Pipeline";
            this.rbWekaPipeline.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbIGVoxelCount);
            this.panel1.Controls.Add(this.rbIGThreshold);
            this.panel1.Controls.Add(this.nudIGThreshold);
            this.panel1.Controls.Add(this.nudIGVoxelAmount);
            this.panel1.Controls.Add(this.cbPeekHigherTRsIG);
            this.panel1.Location = new System.Drawing.Point(5, 180);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 73);
            this.panel1.TabIndex = 46;
            // 
            // rbIGVoxelCount
            // 
            this.rbIGVoxelCount.AutoSize = true;
            this.rbIGVoxelCount.Checked = true;
            this.rbIGVoxelCount.Location = new System.Drawing.Point(4, 29);
            this.rbIGVoxelCount.Name = "rbIGVoxelCount";
            this.rbIGVoxelCount.Size = new System.Drawing.Size(96, 17);
            this.rbIGVoxelCount.TabIndex = 46;
            this.rbIGVoxelCount.TabStop = true;
            this.rbIGVoxelCount.Text = "IG Voxel Count";
            this.rbIGVoxelCount.UseVisualStyleBackColor = true;
            this.rbIGVoxelCount.CheckedChanged += new System.EventHandler(this.rbVoxelCount_CheckedChanged);
            // 
            // rbIGThreshold
            // 
            this.rbIGThreshold.AutoSize = true;
            this.rbIGThreshold.Location = new System.Drawing.Point(4, 7);
            this.rbIGThreshold.Name = "rbIGThreshold";
            this.rbIGThreshold.Size = new System.Drawing.Size(86, 17);
            this.rbIGThreshold.TabIndex = 45;
            this.rbIGThreshold.Text = "IG Threshold";
            this.rbIGThreshold.UseVisualStyleBackColor = true;
            this.rbIGThreshold.CheckedChanged += new System.EventHandler(this.rbIGThreshold_CheckedChanged);
            // 
            // nudIGThreshold
            // 
            this.nudIGThreshold.DecimalPlaces = 3;
            this.nudIGThreshold.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudIGThreshold.Location = new System.Drawing.Point(163, 7);
            this.nudIGThreshold.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudIGThreshold.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147352576});
            this.nudIGThreshold.Name = "nudIGThreshold";
            this.nudIGThreshold.Size = new System.Drawing.Size(55, 20);
            this.nudIGThreshold.TabIndex = 41;
            this.nudIGThreshold.Tag = "";
            this.toolTip1.SetToolTip(this.nudIGThreshold, "real time usage, not implemented yet.");
            this.nudIGThreshold.Value = new decimal(new int[] {
            15,
            0,
            0,
            131072});
            this.nudIGThreshold.ValueChanged += new System.EventHandler(this.nudIGThreshold_ValueChanged);
            // 
            // nudIGVoxelAmount
            // 
            this.nudIGVoxelAmount.Increment = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudIGVoxelAmount.Location = new System.Drawing.Point(155, 29);
            this.nudIGVoxelAmount.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudIGVoxelAmount.Name = "nudIGVoxelAmount";
            this.nudIGVoxelAmount.Size = new System.Drawing.Size(63, 20);
            this.nudIGVoxelAmount.TabIndex = 44;
            this.nudIGVoxelAmount.Tag = "";
            this.toolTip1.SetToolTip(this.nudIGVoxelAmount, "real time usage, not implemented yet.");
            this.nudIGVoxelAmount.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudIGVoxelAmount.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged_1);
            // 
            // cbPeekHigherTRsIG
            // 
            this.cbPeekHigherTRsIG.AutoSize = true;
            this.cbPeekHigherTRsIG.Checked = true;
            this.cbPeekHigherTRsIG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPeekHigherTRsIG.Location = new System.Drawing.Point(4, 52);
            this.cbPeekHigherTRsIG.Name = "cbPeekHigherTRsIG";
            this.cbPeekHigherTRsIG.Size = new System.Drawing.Size(124, 17);
            this.cbPeekHigherTRsIG.TabIndex = 43;
            this.cbPeekHigherTRsIG.Text = "Peek Higher TR\'s IG";
            this.toolTip1.SetToolTip(this.cbPeekHigherTRsIG, resources.GetString("cbPeekHigherTRsIG.ToolTip"));
            this.cbPeekHigherTRsIG.UseVisualStyleBackColor = true;
            this.cbPeekHigherTRsIG.CheckedChanged += new System.EventHandler(this.cbPeekHigherTRsIG_CheckedChanged);
            // 
            // cmbClass1
            // 
            this.cmbClass1.Enabled = false;
            this.cmbClass1.FormattingEnabled = true;
            this.cmbClass1.Location = new System.Drawing.Point(39, 17);
            this.cmbClass1.Name = "cmbClass1";
            this.cmbClass1.Size = new System.Drawing.Size(73, 21);
            this.cmbClass1.TabIndex = 21;
            this.cmbClass1.SelectedIndexChanged += new System.EventHandler(this.cmbClass1_SelectedIndexChanged);
            // 
            // cmbClass2
            // 
            this.cmbClass2.Enabled = false;
            this.cmbClass2.FormattingEnabled = true;
            this.cmbClass2.Location = new System.Drawing.Point(154, 17);
            this.cmbClass2.Name = "cmbClass2";
            this.cmbClass2.Size = new System.Drawing.Size(69, 21);
            this.cmbClass2.TabIndex = 3;
            this.cmbClass2.SelectedIndexChanged += new System.EventHandler(this.cmbClass2_SelectedIndexChanged);
            // 
            // bTrain
            // 
            this.bTrain.Location = new System.Drawing.Point(4, 259);
            this.bTrain.Name = "bTrain";
            this.bTrain.Size = new System.Drawing.Size(220, 29);
            this.bTrain.TabIndex = 19;
            this.bTrain.Text = "Train";
            this.bTrain.UseVisualStyleBackColor = true;
            this.bTrain.Click += new System.EventHandler(this.bTrain_Click);
            // 
            // lClass1
            // 
            this.lClass1.AutoSize = true;
            this.lClass1.Enabled = false;
            this.lClass1.Location = new System.Drawing.Point(3, 21);
            this.lClass1.Name = "lClass1";
            this.lClass1.Size = new System.Drawing.Size(38, 13);
            this.lClass1.TabIndex = 16;
            this.lClass1.Text = "Class1";
            // 
            // lClass2
            // 
            this.lClass2.AutoSize = true;
            this.lClass2.Enabled = false;
            this.lClass2.Location = new System.Drawing.Point(116, 21);
            this.lClass2.Name = "lClass2";
            this.lClass2.Size = new System.Drawing.Size(38, 13);
            this.lClass2.TabIndex = 15;
            this.lClass2.Text = "Class2";
            // 
            // cbMultiClass
            // 
            this.cbMultiClass.AutoSize = true;
            this.cbMultiClass.Checked = true;
            this.cbMultiClass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMultiClass.Location = new System.Drawing.Point(6, 42);
            this.cbMultiClass.Name = "cbMultiClass";
            this.cbMultiClass.Size = new System.Drawing.Size(76, 17);
            this.cbMultiClass.TabIndex = 14;
            this.cbMultiClass.Text = "Multi Class";
            this.cbMultiClass.UseVisualStyleBackColor = true;
            this.cbMultiClass.CheckedChanged += new System.EventHandler(this.cbMultiClass_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(122, 42);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 13);
            this.label18.TabIndex = 40;
            this.label18.Text = "To:";
            this.toolTip1.SetToolTip(this.label18, "going over Max TR per event may result in unexpected behaviour");
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 69);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(93, 13);
            this.label17.TabIndex = 39;
            this.label17.Text = "Classify Using TR:";
            this.toolTip1.SetToolTip(this.label17, "real time usage, not implemented yet.");
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 45);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(84, 13);
            this.label16.TabIndex = 27;
            this.label16.Text = "Extract from TR:";
            this.toolTip1.SetToolTip(this.label16, "going over Max TR per event may result in unexpected behaviour");
            // 
            // nudExtractFromTR
            // 
            this.nudExtractFromTR.Enabled = false;
            this.nudExtractFromTR.Location = new System.Drawing.Point(86, 43);
            this.nudExtractFromTR.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudExtractFromTR.Name = "nudExtractFromTR";
            this.nudExtractFromTR.Size = new System.Drawing.Size(34, 20);
            this.nudExtractFromTR.TabIndex = 38;
            this.nudExtractFromTR.Tag = "";
            this.toolTip1.SetToolTip(this.nudExtractFromTR, "threshold for raw values, anything above value will be kept (default is 200)");
            this.nudExtractFromTR.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudExtractFromTR.ValueChanged += new System.EventHandler(this.nudExtractFromTR_ValueChanged);
            // 
            // nudExtractToTR
            // 
            this.nudExtractToTR.Location = new System.Drawing.Point(146, 43);
            this.nudExtractToTR.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudExtractToTR.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudExtractToTR.Name = "nudExtractToTR";
            this.nudExtractToTR.Size = new System.Drawing.Size(38, 20);
            this.nudExtractToTR.TabIndex = 37;
            this.nudExtractToTR.Tag = "";
            this.toolTip1.SetToolTip(this.nudExtractToTR, "threshold for raw values, anything above value will be kept (default is 200)");
            this.nudExtractToTR.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudExtractToTR.ValueChanged += new System.EventHandler(this.nudExtractToTR_ValueChanged);
            // 
            // nudClassifyUsingTR
            // 
            this.nudClassifyUsingTR.Location = new System.Drawing.Point(146, 66);
            this.nudClassifyUsingTR.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudClassifyUsingTR.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudClassifyUsingTR.Name = "nudClassifyUsingTR";
            this.nudClassifyUsingTR.Size = new System.Drawing.Size(38, 20);
            this.nudClassifyUsingTR.TabIndex = 13;
            this.nudClassifyUsingTR.Tag = "";
            this.toolTip1.SetToolTip(this.nudClassifyUsingTR, "real time usage, not implemented yet.");
            this.nudClassifyUsingTR.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudClassifyUsingTR.ValueChanged += new System.EventHandler(this.nudClassifyUsingTR_ValueChanged);
            // 
            // cbOverwriteProcessedFiles
            // 
            this.cbOverwriteProcessedFiles.AutoSize = true;
            this.cbOverwriteProcessedFiles.Checked = true;
            this.cbOverwriteProcessedFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverwriteProcessedFiles.Location = new System.Drawing.Point(6, 21);
            this.cbOverwriteProcessedFiles.Name = "cbOverwriteProcessedFiles";
            this.cbOverwriteProcessedFiles.Size = new System.Drawing.Size(154, 17);
            this.cbOverwriteProcessedFiles.TabIndex = 36;
            this.cbOverwriteProcessedFiles.Text = "Overwrite Processed Files?";
            this.toolTip1.SetToolTip(this.cbOverwriteProcessedFiles, resources.GetString("cbOverwriteProcessedFiles.ToolTip"));
            this.cbOverwriteProcessedFiles.UseVisualStyleBackColor = true;
            this.cbOverwriteProcessedFiles.CheckedChanged += new System.EventHandler(this.cbOverwriteProcessedFiles_CheckedChanged);
            // 
            // bProcess
            // 
            this.bProcess.Location = new System.Drawing.Point(6, 216);
            this.bProcess.Name = "bProcess";
            this.bProcess.Size = new System.Drawing.Size(178, 29);
            this.bProcess.TabIndex = 22;
            this.bProcess.Text = "Process";
            this.bProcess.UseVisualStyleBackColor = true;
            this.bProcess.Click += new System.EventHandler(this.bProcess_Click);
            // 
            // tbFileName
            // 
            this.tbFileName.Enabled = false;
            this.tbFileName.Location = new System.Drawing.Point(66, 294);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(213, 20);
            this.tbFileName.TabIndex = 2;
            this.tbFileName.Text = "tirosh-";
            this.toolTip1.SetToolTip(this.tbFileName, "displays chosen file name (even if the 100th file was chosen)");
            this.tbFileName.Visible = false;
            this.tbFileName.TextChanged += new System.EventHandler(this.tbFileName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bLoadContrastFile);
            this.groupBox3.Controls.Add(this.tbContrastFileName);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.tbTRs);
            this.groupBox3.Controls.Add(this.bSelectFiles);
            this.groupBox3.Controls.Add(this.bLoadProtocol);
            this.groupBox3.Controls.Add(this.tbProtocolFileName);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tbFileName);
            this.groupBox3.Controls.Add(this.tbWorkingDir);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.dataGridView1);
            this.groupBox3.Controls.Add(this.treeView1);
            this.groupBox3.Location = new System.Drawing.Point(7, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(565, 344);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "1 - Protocol / Data / Model";
            this.toolTip1.SetToolTip(this.groupBox3, "please choose your working dir, file name and protocol file name");
            // 
            // bLoadContrastFile
            // 
            this.bLoadContrastFile.Enabled = false;
            this.bLoadContrastFile.Location = new System.Drawing.Point(468, 318);
            this.bLoadContrastFile.Name = "bLoadContrastFile";
            this.bLoadContrastFile.Size = new System.Drawing.Size(43, 20);
            this.bLoadContrastFile.TabIndex = 33;
            this.bLoadContrastFile.Text = "Load";
            this.toolTip1.SetToolTip(this.bLoadContrastFile, "select and load protocol file");
            this.bLoadContrastFile.UseVisualStyleBackColor = true;
            this.bLoadContrastFile.Visible = false;
            // 
            // tbContrastFileName
            // 
            this.tbContrastFileName.Enabled = false;
            this.tbContrastFileName.Location = new System.Drawing.Point(66, 317);
            this.tbContrastFileName.Name = "tbContrastFileName";
            this.tbContrastFileName.Size = new System.Drawing.Size(400, 20);
            this.tbContrastFileName.TabIndex = 4;
            this.tbContrastFileName.Text = "\\My_Dropbox\\VERE\\MRI_data\\Tirosh\\tirosh_body_3dir.ctr ";
            this.toolTip1.SetToolTip(this.tbContrastFileName, "location and file name for the protocol");
            this.tbContrastFileName.Visible = false;
            this.tbContrastFileName.TextChanged += new System.EventHandler(this.tbContrastFileName_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 321);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 32;
            this.label12.Text = "Contast :";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            this.label12.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(285, 297);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Available TRs / Files";
            this.toolTip1.SetToolTip(this.label10, "maximum available TRs");
            this.label10.Visible = false;
            // 
            // tbTRs
            // 
            this.tbTRs.Enabled = false;
            this.tbTRs.Location = new System.Drawing.Point(396, 294);
            this.tbTRs.Name = "tbTRs";
            this.tbTRs.Size = new System.Drawing.Size(70, 20);
            this.tbTRs.TabIndex = 29;
            this.tbTRs.Text = "0";
            this.toolTip1.SetToolTip(this.tbTRs, "maximum available TRs");
            this.tbTRs.Visible = false;
            // 
            // bSelectFiles
            // 
            this.bSelectFiles.Location = new System.Drawing.Point(468, 271);
            this.bSelectFiles.Name = "bSelectFiles";
            this.bSelectFiles.Size = new System.Drawing.Size(43, 44);
            this.bSelectFiles.TabIndex = 13;
            this.bSelectFiles.Text = "Select Files";
            this.toolTip1.SetToolTip(this.bSelectFiles, "select working directory and file name");
            this.bSelectFiles.UseVisualStyleBackColor = true;
            this.bSelectFiles.Visible = false;
            this.bSelectFiles.Click += new System.EventHandler(this.bSelectFiles_Click);
            // 
            // bLoadProtocol
            // 
            this.bLoadProtocol.Location = new System.Drawing.Point(503, 11);
            this.bLoadProtocol.Name = "bLoadProtocol";
            this.bLoadProtocol.Size = new System.Drawing.Size(60, 21);
            this.bLoadProtocol.TabIndex = 12;
            this.bLoadProtocol.Text = "Load";
            this.toolTip1.SetToolTip(this.bLoadProtocol, "select and load protocol file");
            this.bLoadProtocol.UseVisualStyleBackColor = true;
            this.bLoadProtocol.Click += new System.EventHandler(this.bLoadProtocol_Click);
            // 
            // tbProtocolFileName
            // 
            this.tbProtocolFileName.Location = new System.Drawing.Point(66, 11);
            this.tbProtocolFileName.Name = "tbProtocolFileName";
            this.tbProtocolFileName.Size = new System.Drawing.Size(435, 20);
            this.tbProtocolFileName.TabIndex = 3;
            this.tbProtocolFileName.Text = "\\My_Dropbox\\VERE\\MRI_data\\Tirosh\\20112006.3directions_NamedDir.prt";
            this.toolTip1.SetToolTip(this.tbProtocolFileName, "location and file name for the protocol");
            this.tbProtocolFileName.TextChanged += new System.EventHandler(this.tbProtocolFileName_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Protocol :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Work Dir:";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 298);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "File Name:";
            this.toolTip1.SetToolTip(this.label2, "displays chosen file name (even if the 100th file was chosen)");
            this.label2.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Folder,
            this.Files,
            this.Train,
            this.Model,
            this.TestDir});
            this.dataGridView1.Location = new System.Drawing.Point(268, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(295, 306);
            this.dataGridView1.TabIndex = 46;
            // 
            // Folder
            // 
            this.Folder.HeaderText = "Folder";
            this.Folder.Name = "Folder";
            // 
            // Files
            // 
            this.Files.HeaderText = "Files";
            this.Files.Name = "Files";
            this.Files.Width = 40;
            // 
            // Train
            // 
            this.Train.HeaderText = "Train";
            this.Train.Name = "Train";
            this.Train.Width = 50;
            // 
            // Model
            // 
            this.Model.HeaderText = "Model";
            this.Model.Name = "Model";
            this.Model.Width = 50;
            // 
            // TestDir
            // 
            this.TestDir.HeaderText = "TestDir";
            this.TestDir.Name = "TestDir";
            this.TestDir.Width = 50;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Enabled = false;
            this.treeView1.Location = new System.Drawing.Point(11, 32);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(253, 306);
            this.treeView1.TabIndex = 44;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbPerEvent);
            this.groupBox4.Controls.Add(this.bExport);
            this.groupBox4.Controls.Add(this.rbCSV);
            this.groupBox4.Controls.Add(this.rbSparse);
            this.groupBox4.Location = new System.Drawing.Point(576, 702);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(140, 103);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "X - Data Type Output";
            this.toolTip1.SetToolTip(this.groupBox4, "optional usually a model is created from the SVM");
            this.groupBox4.Visible = false;
            // 
            // cbPerEvent
            // 
            this.cbPerEvent.AutoSize = true;
            this.cbPerEvent.Location = new System.Drawing.Point(5, 54);
            this.cbPerEvent.Name = "cbPerEvent";
            this.cbPerEvent.Size = new System.Drawing.Size(102, 17);
            this.cbPerEvent.TabIndex = 24;
            this.cbPerEvent.Text = "per event / map";
            this.toolTip1.SetToolTip(this.cbPerEvent, "export per event from betas");
            this.cbPerEvent.UseVisualStyleBackColor = true;
            this.cbPerEvent.Visible = false;
            this.cbPerEvent.CheckedChanged += new System.EventHandler(this.cbPerEvent_CheckedChanged);
            // 
            // bExport
            // 
            this.bExport.Location = new System.Drawing.Point(5, 71);
            this.bExport.Name = "bExport";
            this.bExport.Size = new System.Drawing.Size(129, 29);
            this.bExport.TabIndex = 0;
            this.bExport.Text = "Export Data";
            this.toolTip1.SetToolTip(this.bExport, "push to export");
            this.bExport.UseVisualStyleBackColor = true;
            this.bExport.Click += new System.EventHandler(this.bExport_Click);
            // 
            // rbCSV
            // 
            this.rbCSV.AutoSize = true;
            this.rbCSV.Location = new System.Drawing.Point(5, 36);
            this.rbCSV.Name = "rbCSV";
            this.rbCSV.Size = new System.Drawing.Size(46, 17);
            this.rbCSV.TabIndex = 1;
            this.rbCSV.Text = "CSV";
            this.toolTip1.SetToolTip(this.rbCSV, "export full vectors to CSV");
            this.rbCSV.UseVisualStyleBackColor = true;
            this.rbCSV.CheckedChanged += new System.EventHandler(this.rbCSV_CheckedChanged);
            // 
            // rbSparse
            // 
            this.rbSparse.AutoSize = true;
            this.rbSparse.Checked = true;
            this.rbSparse.Location = new System.Drawing.Point(6, 19);
            this.rbSparse.Name = "rbSparse";
            this.rbSparse.Size = new System.Drawing.Size(48, 17);
            this.rbSparse.TabIndex = 2;
            this.rbSparse.TabStop = true;
            this.rbSparse.Text = "SVM";
            this.toolTip1.SetToolTip(this.rbSparse, "export to svm files (natively sparse)");
            this.rbSparse.UseVisualStyleBackColor = true;
            this.rbSparse.CheckedChanged += new System.EventHandler(this.rbSparse_CheckedChanged);
            // 
            // tbSVMLog
            // 
            this.tbSVMLog.Location = new System.Drawing.Point(6, 19);
            this.tbSVMLog.Multiline = true;
            this.tbSVMLog.Name = "tbSVMLog";
            this.tbSVMLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSVMLog.Size = new System.Drawing.Size(627, 646);
            this.tbSVMLog.TabIndex = 10;
            this.tbSVMLog.TextChanged += new System.EventHandler(this.tbSVMLog_TextChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbFeatureProcessingUnprocessed);
            this.groupBox5.Controls.Add(this.lbFeatureProcessOptions);
            this.groupBox5.Location = new System.Drawing.Point(782, 698);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(159, 191);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "4 - Feature Process Options";
            this.toolTip1.SetToolTip(this.groupBox5, "chooses the types of feature processing ");
            this.groupBox5.Visible = false;
            // 
            // cbFeatureProcessingUnprocessed
            // 
            this.cbFeatureProcessingUnprocessed.AutoSize = true;
            this.cbFeatureProcessingUnprocessed.Checked = true;
            this.cbFeatureProcessingUnprocessed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFeatureProcessingUnprocessed.Location = new System.Drawing.Point(7, 169);
            this.cbFeatureProcessingUnprocessed.Name = "cbFeatureProcessingUnprocessed";
            this.cbFeatureProcessingUnprocessed.Size = new System.Drawing.Size(167, 17);
            this.cbFeatureProcessingUnprocessed.TabIndex = 0;
            this.cbFeatureProcessingUnprocessed.Text = "Weka Pipeline (Unprocessed)";
            this.toolTip1.SetToolTip(this.cbFeatureProcessingUnprocessed, "will disable feature processing (engineering or reduction)");
            this.cbFeatureProcessingUnprocessed.UseVisualStyleBackColor = true;
            this.cbFeatureProcessingUnprocessed.CheckedChanged += new System.EventHandler(this.cbFeatureReductionUnprocessed_CheckedChanged);
            // 
            // lbFeatureProcessOptions
            // 
            this.lbFeatureProcessOptions.Enabled = false;
            this.lbFeatureProcessOptions.FormattingEnabled = true;
            this.lbFeatureProcessOptions.Items.AddRange(new object[] {
            "Glm",
            "Voxelize",
            "Onset Averaging",
            "Onset Max",
            "Onset Absolute Max",
            "Zscore Normalize",
            "ROI",
            "PCA"});
            this.lbFeatureProcessOptions.Location = new System.Drawing.Point(7, 16);
            this.lbFeatureProcessOptions.Name = "lbFeatureProcessOptions";
            this.lbFeatureProcessOptions.ScrollAlwaysVisible = true;
            this.lbFeatureProcessOptions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFeatureProcessOptions.Size = new System.Drawing.Size(147, 147);
            this.lbFeatureProcessOptions.TabIndex = 1;
            // 
            // gbSVMOutput
            // 
            this.gbSVMOutput.Controls.Add(this.tbSVMLog);
            this.gbSVMOutput.Location = new System.Drawing.Point(576, 3);
            this.gbSVMOutput.Name = "gbSVMOutput";
            this.gbSVMOutput.Size = new System.Drawing.Size(635, 671);
            this.gbSVMOutput.TabIndex = 12;
            this.gbSVMOutput.TabStop = false;
            this.gbSVMOutput.Text = "8 - SVM output";
            // 
            // gbFeatureProcessFlow
            // 
            this.gbFeatureProcessFlow.Controls.Add(this.bPlus);
            this.gbFeatureProcessFlow.Controls.Add(this.bMinus);
            this.gbFeatureProcessFlow.Controls.Add(this.lbK);
            this.gbFeatureProcessFlow.Controls.Add(this.button1);
            this.gbFeatureProcessFlow.Controls.Add(this.lbFeatureProcessFlow);
            this.gbFeatureProcessFlow.Controls.Add(this.bUp);
            this.gbFeatureProcessFlow.Controls.Add(this.bDown);
            this.gbFeatureProcessFlow.Enabled = false;
            this.gbFeatureProcessFlow.Location = new System.Drawing.Point(969, 698);
            this.gbFeatureProcessFlow.Name = "gbFeatureProcessFlow";
            this.gbFeatureProcessFlow.Size = new System.Drawing.Size(184, 191);
            this.gbFeatureProcessFlow.TabIndex = 22;
            this.gbFeatureProcessFlow.TabStop = false;
            this.gbFeatureProcessFlow.Text = "5 - Feature Process Flow";
            this.toolTip1.SetToolTip(this.gbFeatureProcessFlow, "determine the order of feature processing ");
            this.gbFeatureProcessFlow.Visible = false;
            // 
            // bPlus
            // 
            this.bPlus.Location = new System.Drawing.Point(148, 165);
            this.bPlus.Name = "bPlus";
            this.bPlus.Size = new System.Drawing.Size(15, 23);
            this.bPlus.TabIndex = 30;
            this.bPlus.Text = "+";
            this.toolTip1.SetToolTip(this.bPlus, "push feature process up");
            this.bPlus.UseVisualStyleBackColor = true;
            this.bPlus.Click += new System.EventHandler(this.bPlus_Click);
            // 
            // bMinus
            // 
            this.bMinus.Location = new System.Drawing.Point(163, 165);
            this.bMinus.Name = "bMinus";
            this.bMinus.Size = new System.Drawing.Size(15, 23);
            this.bMinus.TabIndex = 31;
            this.bMinus.Text = "-";
            this.toolTip1.SetToolTip(this.bMinus, "push feature process down");
            this.bMinus.UseVisualStyleBackColor = true;
            this.bMinus.Click += new System.EventHandler(this.bMinus_Click);
            // 
            // lbK
            // 
            this.lbK.Enabled = false;
            this.lbK.FormattingEnabled = true;
            this.lbK.Location = new System.Drawing.Point(148, 16);
            this.lbK.Name = "lbK";
            this.lbK.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbK.Size = new System.Drawing.Size(30, 147);
            this.lbK.TabIndex = 29;
            this.lbK.SelectedIndexChanged += new System.EventHandler(this.lbK_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(92, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 23);
            this.button1.TabIndex = 28;
            this.button1.Text = "clear";
            this.toolTip1.SetToolTip(this.button1, "clear flow");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbFeatureProcessFlow
            // 
            this.lbFeatureProcessFlow.FormattingEnabled = true;
            this.lbFeatureProcessFlow.Location = new System.Drawing.Point(5, 16);
            this.lbFeatureProcessFlow.Name = "lbFeatureProcessFlow";
            this.lbFeatureProcessFlow.ScrollAlwaysVisible = true;
            this.lbFeatureProcessFlow.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFeatureProcessFlow.Size = new System.Drawing.Size(138, 147);
            this.lbFeatureProcessFlow.TabIndex = 2;
            this.lbFeatureProcessFlow.SelectedIndexChanged += new System.EventHandler(this.lbFeatureProcessFlow_SelectedIndexChanged);
            // 
            // bUp
            // 
            this.bUp.Location = new System.Drawing.Point(3, 165);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(39, 23);
            this.bUp.TabIndex = 26;
            this.bUp.Text = "up";
            this.toolTip1.SetToolTip(this.bUp, "push feature process up");
            this.bUp.UseVisualStyleBackColor = true;
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bDown
            // 
            this.bDown.Location = new System.Drawing.Point(43, 165);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(48, 23);
            this.bDown.TabIndex = 27;
            this.bDown.Text = "down";
            this.toolTip1.SetToolTip(this.bDown, "push feature process down");
            this.bDown.UseVisualStyleBackColor = true;
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.InitialDirectory = "\\My_Dropbox\\UProjects\\OriBrainLearner\\Data";
            // 
            // nudPreOnset
            // 
            this.nudPreOnset.Enabled = false;
            this.nudPreOnset.Location = new System.Drawing.Point(42, 18);
            this.nudPreOnset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudPreOnset.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPreOnset.Name = "nudPreOnset";
            this.nudPreOnset.Size = new System.Drawing.Size(42, 20);
            this.nudPreOnset.TabIndex = 0;
            this.toolTip1.SetToolTip(this.nudPreOnset, "all values for current TR are taken from this onset");
            this.nudPreOnset.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPreOnset.ValueChanged += new System.EventHandler(this.nudPreOnset_ValueChanged);
            // 
            // nudPostOnset
            // 
            this.nudPostOnset.Enabled = false;
            this.nudPostOnset.Location = new System.Drawing.Point(149, 18);
            this.nudPostOnset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudPostOnset.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPostOnset.Name = "nudPostOnset";
            this.nudPostOnset.Size = new System.Drawing.Size(38, 20);
            this.nudPostOnset.TabIndex = 1;
            this.toolTip1.SetToolTip(this.nudPostOnset, "all values for current TR are taken until this onset");
            this.nudPostOnset.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudPostOnset.ValueChanged += new System.EventHandler(this.nudPostOnset_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Onset";
            this.toolTip1.SetToolTip(this.label7, "all values for current TR are taken from this onset");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(89, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Post Onset";
            this.toolTip1.SetToolTip(this.label9, "all values for current TR are taken until this onset");
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cbOnsetFromProtocol);
            this.groupBox8.Controls.Add(this.label9);
            this.groupBox8.Controls.Add(this.label7);
            this.groupBox8.Controls.Add(this.nudPreOnset);
            this.groupBox8.Controls.Add(this.nudPostOnset);
            this.groupBox8.Location = new System.Drawing.Point(7, 351);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(308, 42);
            this.groupBox8.TabIndex = 27;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "2 - Event";
            this.toolTip1.SetToolTip(this.groupBox8, "chooses the range of onsets or automaticaly chooses for you, hover control for de" +
                    "tails");
            // 
            // cbOnsetFromProtocol
            // 
            this.cbOnsetFromProtocol.AutoSize = true;
            this.cbOnsetFromProtocol.Checked = true;
            this.cbOnsetFromProtocol.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOnsetFromProtocol.Enabled = false;
            this.cbOnsetFromProtocol.Location = new System.Drawing.Point(190, 19);
            this.cbOnsetFromProtocol.Name = "cbOnsetFromProtocol";
            this.cbOnsetFromProtocol.Size = new System.Drawing.Size(122, 17);
            this.cbOnsetFromProtocol.TabIndex = 22;
            this.cbOnsetFromProtocol.Tag = "";
            this.cbOnsetFromProtocol.Text = "Onset From Protocol";
            this.toolTip1.SetToolTip(this.cbOnsetFromProtocol, "Automaticaly calculate onsets (formula = length(first event) + length(second even" +
                    "t)");
            this.cbOnsetFromProtocol.UseVisualStyleBackColor = true;
            this.cbOnsetFromProtocol.CheckedChanged += new System.EventHandler(this.cbOnsetFromProtocol_CheckedChanged);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.InitialDirectory = "\\My_Dropbox\\UProjects\\OriBrainLearner\\Data";
            // 
            // bLoadModel
            // 
            this.bLoadModel.Location = new System.Drawing.Point(6, 95);
            this.bLoadModel.Name = "bLoadModel";
            this.bLoadModel.Size = new System.Drawing.Size(120, 24);
            this.bLoadModel.TabIndex = 13;
            this.bLoadModel.Text = "Load Model";
            this.toolTip1.SetToolTip(this.bLoadModel, "push to import, will reselect & reload working directory if data type changed, wi" +
                    "ll reselect & reload protocol if not loaded previously. otherwise it will import" +
                    " quietly.");
            this.bLoadModel.UseVisualStyleBackColor = true;
            this.bLoadModel.Visible = false;
            this.bLoadModel.Click += new System.EventHandler(this.bLoadModel_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 140);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 25);
            this.button2.TabIndex = 14;
            this.button2.Text = "load Test SVM";
            this.toolTip1.SetToolTip(this.button2, "load Test SVM");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(6, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(120, 41);
            this.button4.TabIndex = 15;
            this.button4.Text = "RT Test Server";
            this.toolTip1.SetToolTip(this.button4, "Test");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.RTServer_Click);
            // 
            // UnityPort
            // 
            this.UnityPort.Enabled = false;
            this.UnityPort.Location = new System.Drawing.Point(481, 353);
            this.UnityPort.Name = "UnityPort";
            this.UnityPort.Size = new System.Drawing.Size(41, 20);
            this.UnityPort.TabIndex = 34;
            this.UnityPort.Text = "171";
            this.toolTip1.SetToolTip(this.UnityPort, "maximum available TRs");
            // 
            // LocalPort
            // 
            this.LocalPort.Enabled = false;
            this.LocalPort.Location = new System.Drawing.Point(481, 373);
            this.LocalPort.Name = "LocalPort";
            this.LocalPort.Size = new System.Drawing.Size(41, 20);
            this.LocalPort.TabIndex = 39;
            this.LocalPort.Text = "170";
            this.toolTip1.SetToolTip(this.LocalPort, "maximum available TRs");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(323, 356);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 13);
            this.label14.TabIndex = 34;
            this.label14.Text = "Unity:";
            this.toolTip1.SetToolTip(this.label14, "displays chosen file name (even if the 100th file was chosen)");
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(321, 377);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(36, 13);
            this.label15.TabIndex = 40;
            this.label15.Text = "Local:";
            this.toolTip1.SetToolTip(this.label15, "displays chosen file name (even if the 100th file was chosen)");
            // 
            // UDP
            // 
            this.UDP.Location = new System.Drawing.Point(6, 190);
            this.UDP.Name = "UDP";
            this.UDP.Size = new System.Drawing.Size(120, 43);
            this.UDP.TabIndex = 37;
            this.UDP.Text = "UDP smo clsfr Listener";
            this.toolTip1.SetToolTip(this.UDP, resources.GetString("UDP.ToolTip"));
            this.UDP.UseVisualStyleBackColor = true;
            this.UDP.Click += new System.EventHandler(this.UDP_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(6, 166);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(120, 23);
            this.button5.TabIndex = 38;
            this.button5.Text = "UDP Unity Send sim";
            this.toolTip1.SetToolTip(this.button5, "simulates unity, will create a timer that sends file names every 2s. if the liste" +
                    "ner is activated (below) it will catch these file names and do realtime testing");
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // UnityIP
            // 
            this.UnityIP.Location = new System.Drawing.Point(361, 353);
            this.UnityIP.Name = "UnityIP";
            this.UnityIP.Size = new System.Drawing.Size(119, 20);
            this.UnityIP.TabIndex = 43;
            this.UnityIP.Text = "127.0.0.1";
            this.toolTip1.SetToolTip(this.UnityIP, "132.77.68.94");
            // 
            // LocalIP
            // 
            this.LocalIP.Location = new System.Drawing.Point(361, 372);
            this.LocalIP.Name = "LocalIP";
            this.LocalIP.Size = new System.Drawing.Size(119, 20);
            this.LocalIP.TabIndex = 44;
            this.LocalIP.Text = "127.0.0.1";
            this.toolTip1.SetToolTip(this.LocalIP, "132.77.68.90");
            // 
            // bStats
            // 
            this.bStats.Location = new System.Drawing.Point(6, 57);
            this.bStats.Name = "bStats";
            this.bStats.Size = new System.Drawing.Size(120, 38);
            this.bStats.TabIndex = 39;
            this.bStats.Text = "Generate Stats";
            this.toolTip1.SetToolTip(this.bStats, "push to import, will reselect & reload working directory if data type changed, wi" +
                    "ll reselect & reload protocol if not loaded previously. otherwise it will import" +
                    " quietly.");
            this.bStats.UseVisualStyleBackColor = true;
            this.bStats.Click += new System.EventHandler(this.bStats_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(578, 677);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(630, 19);
            this.progressBar1.TabIndex = 11;
            // 
            // bLeft
            // 
            this.bLeft.Enabled = false;
            this.bLeft.Location = new System.Drawing.Point(941, 788);
            this.bLeft.Name = "bLeft";
            this.bLeft.Size = new System.Drawing.Size(28, 23);
            this.bLeft.TabIndex = 28;
            this.bLeft.Text = "<-";
            this.bLeft.UseVisualStyleBackColor = true;
            this.bLeft.Visible = false;
            this.bLeft.Click += new System.EventHandler(this.bLeft_Click);
            // 
            // bRight
            // 
            this.bRight.Enabled = false;
            this.bRight.Location = new System.Drawing.Point(941, 764);
            this.bRight.Name = "bRight";
            this.bRight.Size = new System.Drawing.Size(28, 23);
            this.bRight.TabIndex = 33;
            this.bRight.Text = "->";
            this.bRight.UseVisualStyleBackColor = true;
            this.bRight.Visible = false;
            this.bRight.Click += new System.EventHandler(this.bRight_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.bStats);
            this.groupBox6.Controls.Add(this.bSaveModel);
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Controls.Add(this.button2);
            this.groupBox6.Controls.Add(this.bLoadModel);
            this.groupBox6.Controls.Add(this.button5);
            this.groupBox6.Controls.Add(this.UDP);
            this.groupBox6.Location = new System.Drawing.Point(207, 651);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(134, 238);
            this.groupBox6.TabIndex = 34;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "7 - Testing";
            // 
            // bSaveModel
            // 
            this.bSaveModel.Location = new System.Drawing.Point(6, 119);
            this.bSaveModel.Name = "bSaveModel";
            this.bSaveModel.Size = new System.Drawing.Size(120, 20);
            this.bSaveModel.TabIndex = 16;
            this.bSaveModel.Text = "Save Model";
            this.bSaveModel.UseVisualStyleBackColor = true;
            this.bSaveModel.Visible = false;
            this.bSaveModel.Click += new System.EventHandler(this.bSaveModel_Click);
            // 
            // bPlot
            // 
            this.bPlot.Location = new System.Drawing.Point(1157, 835);
            this.bPlot.Name = "bPlot";
            this.bPlot.Size = new System.Drawing.Size(54, 49);
            this.bPlot.TabIndex = 36;
            this.bPlot.Text = "Plot";
            this.bPlot.UseVisualStyleBackColor = true;
            this.bPlot.Visible = false;
            this.bPlot.Click += new System.EventHandler(this.bPlot_Click);
            // 
            // gbProcessing
            // 
            this.gbProcessing.Controls.Add(this.rdRawMinusMedWin);
            this.gbProcessing.Controls.Add(this.rbNoNorm);
            this.gbProcessing.Controls.Add(this.nudMovingWindow);
            this.gbProcessing.Controls.Add(this.rdRawMinusMedWinIqrWin);
            this.gbProcessing.Controls.Add(this.rbRawMinusMedDivMed);
            this.gbProcessing.Controls.Add(this.rbRawDivMed);
            this.gbProcessing.Controls.Add(this.rbRawMinusMed);
            this.gbProcessing.Controls.Add(this.rbMinMax);
            this.gbProcessing.Controls.Add(this.cbOverwriteProcessedFiles);
            this.gbProcessing.Controls.Add(this.nudClassifyUsingTR);
            this.gbProcessing.Controls.Add(this.nudExtractToTR);
            this.gbProcessing.Controls.Add(this.label18);
            this.gbProcessing.Controls.Add(this.nudExtractFromTR);
            this.gbProcessing.Controls.Add(this.label17);
            this.gbProcessing.Controls.Add(this.label16);
            this.gbProcessing.Controls.Add(this.bProcess);
            this.gbProcessing.Location = new System.Drawing.Point(152, 399);
            this.gbProcessing.Name = "gbProcessing";
            this.gbProcessing.Size = new System.Drawing.Size(189, 251);
            this.gbProcessing.TabIndex = 45;
            this.gbProcessing.TabStop = false;
            this.gbProcessing.Text = "6A - Processing";
            // 
            // rdRawMinusMedWin
            // 
            this.rdRawMinusMedWin.AutoSize = true;
            this.rdRawMinusMedWin.Checked = true;
            this.rdRawMinusMedWin.Location = new System.Drawing.Point(6, 182);
            this.rdRawMinusMedWin.Name = "rdRawMinusMedWin";
            this.rdRawMinusMedWin.Size = new System.Drawing.Size(116, 17);
            this.rdRawMinusMedWin.TabIndex = 49;
            this.rdRawMinusMedWin.TabStop = true;
            this.rdRawMinusMedWin.Text = "Raw - MedWindow";
            this.rdRawMinusMedWin.UseVisualStyleBackColor = true;
            this.rdRawMinusMedWin.CheckedChanged += new System.EventHandler(this.rdRawMinusMedWin_CheckedChanged);
            // 
            // rbNoNorm
            // 
            this.rbNoNorm.AutoSize = true;
            this.rbNoNorm.Location = new System.Drawing.Point(6, 97);
            this.rbNoNorm.Name = "rbNoNorm";
            this.rbNoNorm.Size = new System.Drawing.Size(105, 17);
            this.rbNoNorm.TabIndex = 48;
            this.rbNoNorm.Text = "No Normalization";
            this.rbNoNorm.UseVisualStyleBackColor = true;
            this.rbNoNorm.CheckedChanged += new System.EventHandler(this.rbNoNorm_CheckedChanged);
            // 
            // nudMovingWindow
            // 
            this.nudMovingWindow.Location = new System.Drawing.Point(146, 190);
            this.nudMovingWindow.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.nudMovingWindow.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudMovingWindow.Name = "nudMovingWindow";
            this.nudMovingWindow.Size = new System.Drawing.Size(38, 20);
            this.nudMovingWindow.TabIndex = 47;
            this.nudMovingWindow.Tag = "";
            this.nudMovingWindow.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.nudMovingWindow.ValueChanged += new System.EventHandler(this.nudMovingWindow_ValueChanged);
            // 
            // rdRawMinusMedWinIqrWin
            // 
            this.rdRawMinusMedWinIqrWin.AutoSize = true;
            this.rdRawMinusMedWinIqrWin.Location = new System.Drawing.Point(6, 199);
            this.rdRawMinusMedWinIqrWin.Name = "rdRawMinusMedWinIqrWin";
            this.rdRawMinusMedWinIqrWin.Size = new System.Drawing.Size(128, 17);
            this.rdRawMinusMedWinIqrWin.TabIndex = 46;
            this.rdRawMinusMedWinIqrWin.Text = "(Raw - MedW) / IqrW";
            this.rdRawMinusMedWinIqrWin.UseVisualStyleBackColor = true;
            this.rdRawMinusMedWinIqrWin.CheckedChanged += new System.EventHandler(this.rdRawMinusMedWinIqrWin_CheckedChanged);
            // 
            // rbRawMinusMedDivMed
            // 
            this.rbRawMinusMedDivMed.AutoSize = true;
            this.rbRawMinusMedDivMed.Location = new System.Drawing.Point(6, 165);
            this.rbRawMinusMedDivMed.Name = "rbRawMinusMedDivMed";
            this.rbRawMinusMedDivMed.Size = new System.Drawing.Size(115, 17);
            this.rbRawMinusMedDivMed.TabIndex = 45;
            this.rbRawMinusMedDivMed.Text = "(Raw - Med) / Med";
            this.rbRawMinusMedDivMed.UseVisualStyleBackColor = true;
            this.rbRawMinusMedDivMed.CheckedChanged += new System.EventHandler(this.rbRawMinusMedDivMed_CheckedChanged);
            // 
            // rbRawDivMed
            // 
            this.rbRawDivMed.AutoSize = true;
            this.rbRawDivMed.Location = new System.Drawing.Point(6, 148);
            this.rbRawDivMed.Name = "rbRawDivMed";
            this.rbRawDivMed.Size = new System.Drawing.Size(110, 17);
            this.rbRawDivMed.TabIndex = 44;
            this.rbRawDivMed.Text = "Raw / Med BSLN";
            this.rbRawDivMed.UseVisualStyleBackColor = true;
            this.rbRawDivMed.CheckedChanged += new System.EventHandler(this.rbRawDivMed_CheckedChanged);
            // 
            // rbRawMinusMed
            // 
            this.rbRawMinusMed.AutoSize = true;
            this.rbRawMinusMed.Location = new System.Drawing.Point(6, 131);
            this.rbRawMinusMed.Name = "rbRawMinusMed";
            this.rbRawMinusMed.Size = new System.Drawing.Size(120, 17);
            this.rbRawMinusMed.TabIndex = 43;
            this.rbRawMinusMed.Text = "Raw - Med Baseline";
            this.rbRawMinusMed.UseVisualStyleBackColor = true;
            this.rbRawMinusMed.CheckedChanged += new System.EventHandler(this.rbRawMinusMed_CheckedChanged);
            // 
            // rbMinMax
            // 
            this.rbMinMax.AutoSize = true;
            this.rbMinMax.Location = new System.Drawing.Point(6, 114);
            this.rbMinMax.Name = "rbMinMax";
            this.rbMinMax.Size = new System.Drawing.Size(65, 17);
            this.rbMinMax.TabIndex = 42;
            this.rbMinMax.Text = "Min Max";
            this.rbMinMax.UseVisualStyleBackColor = true;
            this.rbMinMax.CheckedChanged += new System.EventHandler(this.rbMinMax_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(586, 816);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 46;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbSoundON
            // 
            this.cbSoundON.AutoSize = true;
            this.cbSoundON.Checked = true;
            this.cbSoundON.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSoundON.Location = new System.Drawing.Point(351, 725);
            this.cbSoundON.Name = "cbSoundON";
            this.cbSoundON.Size = new System.Drawing.Size(82, 17);
            this.cbSoundON.TabIndex = 47;
            this.cbSoundON.Text = "Sound ON?";
            this.cbSoundON.UseVisualStyleBackColor = true;
            this.cbSoundON.CheckedChanged += new System.EventHandler(this.cbSoundON_CheckedChanged);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(594, 854);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(103, 30);
            this.button6.TabIndex = 48;
            this.button6.Text = "Display IG voxels";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1211, 887);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.cbSoundON);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.gbProcessing);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.LocalIP);
            this.Controls.Add(this.UnityIP);
            this.Controls.Add(this.UnityPort);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.LocalPort);
            this.Controls.Add(this.bPlot);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.bRight);
            this.Controls.Add(this.bLeft);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.gbFeatureProcessFlow);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.gbSVMOutput);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbTrainTestSplit);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ori\'s Brain Learner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEyeSliceFirstLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFilterEyeSlices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToTR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThresholdSVM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTConThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFromTR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBetaThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).EndInit();
            this.gbTrainTestSplit.ResumeLayout(false);
            this.gbTrainTestSplit.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrainTestSplit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGridFolds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCvFolds)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIGThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIGVoxelAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtractFromTR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtractToTR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudClassifyUsingTR)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.gbSVMOutput.ResumeLayout(false);
            this.gbSVMOutput.PerformLayout();
            this.gbFeatureProcessFlow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudPreOnset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPostOnset)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.gbProcessing.ResumeLayout(false);
            this.gbProcessing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMovingWindow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bImport;
        private System.Windows.Forms.RadioButton rbRawValues;
        private System.Windows.Forms.RadioButton rbBetaValues;
        private System.Windows.Forms.RadioButton rbTcontrastValues;
        private System.Windows.Forms.TextBox tbWorkingDir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbTrainTestSplit;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lFolds;
        private System.Windows.Forms.NumericUpDown nudCvFolds;
        private System.Windows.Forms.Label lClass1;
        private System.Windows.Forms.Label lClass2;
        public System.Windows.Forms.CheckBox cbMultiClass;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bExport;
        private System.Windows.Forms.RadioButton rbCSV;
        private System.Windows.Forms.RadioButton rbSparse;
        public System.Windows.Forms.NumericUpDown nudThreshold;
        private System.Windows.Forms.TextBox tbProtocolFileName;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox tbSVMLog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox gbSVMOutput;
        private System.Windows.Forms.GroupBox gbFeatureProcessFlow;
        private System.Windows.Forms.Button bTrain;
        private System.Windows.Forms.Button bLoadProtocol;
        private System.Windows.Forms.Button bSelectFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.NumericUpDown nudPreOnset;
        public System.Windows.Forms.NumericUpDown nudPostOnset;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox8;
        public System.Windows.Forms.CheckBox cbOnsetFromProtocol;
        private System.Windows.Forms.RadioButton rbSVM;
        private System.Windows.Forms.CheckBox cbFeatureProcessingUnprocessed;
        public System.Windows.Forms.NumericUpDown nudBetaThreshold;
        public System.Windows.Forms.NumericUpDown nudTConThreshold;
        public System.Windows.Forms.ComboBox cmbClass2;
        public System.Windows.Forms.ComboBox cmbClass1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown nudFromTR;
        private System.Windows.Forms.TextBox tbTRs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.NumericUpDown nudThresholdSVM;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown nudToTR;
        private System.Windows.Forms.Button bLoadContrastFile;
        private System.Windows.Forms.TextBox tbContrastFileName;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cbPerEvent;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button bDown;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.ListBox lbFeatureProcessOptions;
        private System.Windows.Forms.ListBox lbFeatureProcessFlow;
        private System.Windows.Forms.Button bLeft;
        private System.Windows.Forms.Button bRight;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bLoadModel;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox lbK;
        private System.Windows.Forms.Button bPlus;
        private System.Windows.Forms.Button bMinus;
        private System.Windows.Forms.Button bSaveModel;
        private System.Windows.Forms.Button bProcess;
        private System.Windows.Forms.Button bPlot;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudGridFolds;
        private System.Windows.Forms.RadioButton rbGridSearch;
        private System.Windows.Forms.RadioButton rbCrossValidation;
        private System.Windows.Forms.RadioButton rbTrainTestSplit;
        private System.Windows.Forms.RadioButton rbRFE;
        private System.Windows.Forms.Button UDP;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox UnityPort;
        private System.Windows.Forms.TextBox LocalPort;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.NumericUpDown nudTrainTestSplit;
        private System.Windows.Forms.RadioButton rbWekaPipeline;
        private System.Windows.Forms.CheckBox cbSVM;
        private System.Windows.Forms.CheckBox cbSMO;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox UnityIP;
        private System.Windows.Forms.TextBox LocalIP;
        public System.Windows.Forms.CheckBox cbOverwriteProcessedFiles;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.NumericUpDown nudExtractFromTR;
        public System.Windows.Forms.NumericUpDown nudExtractToTR;
        public System.Windows.Forms.NumericUpDown nudClassifyUsingTR;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.NumericUpDown nudIGThreshold;
        public System.Windows.Forms.CheckBox cbPeekHigherTRsIG;
        public System.Windows.Forms.NumericUpDown nudFilterEyeSlices;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        public System.Windows.Forms.NumericUpDown nudEyeSliceFirstLines;
        private System.Windows.Forms.Button bStats;
        private System.Windows.Forms.GroupBox gbProcessing;
        private System.Windows.Forms.DataGridViewTextBoxColumn Folder;
        private System.Windows.Forms.DataGridViewTextBoxColumn Files;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Train;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Model;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TestDir;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RadioButton rbRawMinusMedDivMed;
        private System.Windows.Forms.RadioButton rbRawDivMed;
        private System.Windows.Forms.RadioButton rbRawMinusMed;
        private System.Windows.Forms.RadioButton rbMinMax;
        public System.Windows.Forms.NumericUpDown nudMovingWindow;
        private System.Windows.Forms.RadioButton rdRawMinusMedWinIqrWin;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbNoNorm;
        private System.Windows.Forms.RadioButton rdRawMinusMedWin;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbIGVoxelCount;
        private System.Windows.Forms.RadioButton rbIGThreshold;
        public System.Windows.Forms.NumericUpDown nudIGVoxelAmount;
        private System.Windows.Forms.CheckBox cbAutoAll;
        private System.Windows.Forms.CheckBox cbSoundON;
        private System.Windows.Forms.Button button6;
    }
}

