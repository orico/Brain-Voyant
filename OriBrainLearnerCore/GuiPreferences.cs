using System;
using System.Linq; 
using System.Collections.Generic; 

namespace OriBrainLearnerCore
{
    /// <summary>
    /// this class deals with all the gui related variables that needs to be updated via events.
    /// </summary>
    public class GuiPreferences
    {


        /// <summary>
        /// configuration - 1 -  location variables
        /// <summary>

        private string workDirectory;
        public string WorkDirectory
        {
            get { return workDirectory; }
            set
            {
                if (workDirectory != value)
                    workDirectory = value;
                onWorkDirectoryUpdate();
            }
        }
        public event EventHandler workDirectoryUpdateEvent;
        private void onWorkDirectoryUpdate()
        {
            EventHandler handler = workDirectoryUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private string fileName = "tirosh-";
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                fileName = value;
                onFileNameUpdate();
            }
        }
        public event EventHandler FileNameUpdateEvent;
        private void onFileNameUpdate()
        {
            EventHandler handler = FileNameUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private string protocolFile;
        public string ProtocolFile
        {
            get { return protocolFile; }
            set
            { 
                if (protocolFile != value)
                protocolFile = value;
                onProtocolFileUpdate();
            }
        }
        public event EventHandler ProtocolFileUpdateEvent;
        private void onProtocolFileUpdate()
        {
            EventHandler handler = ProtocolFileUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private string contrastFile;
        public string ContrastFile
        {
            get { return contrastFile; }
            set
            {
                if (contrastFile != value)
                contrastFile = value;
                onContrastFileUpdate();
            }
        }
        public event EventHandler contrastFileUpdateEvent;
        private void onContrastFileUpdate()
        {
            EventHandler handler = contrastFileUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private int tbTRs;
        public int TbTRs
        {
            get { return tbTRs; }
            set 
            { 
                if (tbTRs != value)
                tbTRs = value;
                ontbTRUpdate();
            }
        }

        public event EventHandler tbTRUpdateEvent;
        private void ontbTRUpdate()
        { 
            EventHandler handler = tbTRUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }
         
        
        /// <summary>
        /// configuration - 2 - onsets private int preOnset;
        /// <summary>

        private int preOnset;
        public int PreOnset
        {
            get
            {
                return preOnset;
            }
            set
            { 
                if (preOnset != value)
                    preOnset = value;
                //activate shooting function
                onPreOnsetUpdate();
            }
        }

        public event EventHandler PreOnsetUpdateEvent;
        private void onPreOnsetUpdate()
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler handler = PreOnsetUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private int postOnset;
        public int PostOnset
        {
            get
            {
                return postOnset;
            }
            set
            {
                if (postOnset != value)
                {
                    postOnset = value; 
                    onPostOnsetUpdate();
                }
            }
        }


        public event EventHandler PostOnsetUpdateEvent;
        private void onPostOnsetUpdate()
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler handler = PostOnsetUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        public class OnsetChangeEventArg : EventArgs
        {
            public int _pre
            {
                get;
                private set;
            }
            public int _post
            {
                get;
                private set;
            }

            public OnsetChangeEventArg(int pre, int post)
            {
                _pre = pre;
                _post = post;
            }

        }
        private bool cbOnsetFromProtocolChecked;
        public bool CbOnsetFromProtocolChecked
        {
            get
            {
                return cbOnsetFromProtocolChecked;
            }
            set
            {
                if (cbOnsetFromProtocolChecked != value)
                {
                    cbOnsetFromProtocolChecked = value;
                    onOnsetFromProtocolUpdate();
                }
            }
        }


        public event EventHandler onsetFromProtocolUpdateEvent;
        private void onOnsetFromProtocolUpdate()
        {
            EventHandler handler = onsetFromProtocolUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// configuration - 3 - import, radio buttons binary file types
        /// <summary>  

        public bool changedDataType = false;
        private DataType fileType = DataType.rawValue;
        public DataType FileType
        {
            get
            {
                return fileType;
            }
            set
            { 
                fileType = value;
                //unchekcing can be removed if in the future we need to use GLM again, right now there is no need for all the fancy processing options.
                if (fileType == DataType.rawValue)
                {
                    UnprocessedChecked = true;
                }
                changedDataType = true; 
                onFileTypeUpdate();
            }
        }
        public event EventHandler FileTypeUpdateEvent;
        private void onFileTypeUpdate()
        {
            EventHandler handler = FileTypeUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        public int getMaxAvailableTrs()
        {
            if (fileType == DataType.SVMFile)
            {
                if (Preferences.Instance.svmLoaded)
                    return Preferences.Instance.ProblemOriginal.samples.Count();
                else
                    return 1;
            }
            return TbTRs;
        }

        //event for numeric up down 
        public event EventHandler<TrChangeEventArg> TrUpdateEvent;
        private int fromTR;
        public int FromTR
        {
            set
            {
                if (value > toTR){}
                else
                { 
                    fromTR = value;
                    onTrUpdate(fromTR, toTR, minTR, maxTR);
                }
            }
            get
            {
                return fromTR;
            }
        }
        private int toTR;
        public int ToTR
        {
            set
            {
                if (value < fromTR) { }
                else
                {
                    toTR = value;
                    onTrUpdate(fromTR, toTR, minTR, maxTR);
                }
            }
            get
            {
                return toTR;
            }
        }
        private int minTR;
        public int MinTR
        {
            set
            {
                minTR = value;
                onTrUpdate(fromTR, toTR, minTR, maxTR); 
            }
            get
            {
                return minTR;
            }
        }
        private int maxTR;
        public int MaxTR
        {
            set
            {
                maxTR = value;
                onTrUpdate(fromTR, toTR, minTR, maxTR);
            }
            get
            {
                return maxTR;
            }
        }


        private void onTrUpdate(int fromTR, int toTR, int minTR, int maxTR)
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler<TrChangeEventArg> handler = TrUpdateEvent;
            if (handler != null)
            {
                handler(this, new TrChangeEventArg(fromTR, toTR, minTR, maxTR));
            }
        }

        public class TrChangeEventArg : EventArgs
        {
            //private int _fromTR, _toTR, _minTR, _maxTr;

            public int _fromTR
            {
                get;
                private set;
            }
            public int _toTR
            {
                get;
                private set;
            }

            public int _minTR
            {
                get;
                private set;
            }
            public int _maxTr
            {
                get;
                private set;
            }

            public TrChangeEventArg(int fromTR, int toTR, int minTR, int maxTR)
            {
                _fromTR = fromTR;
                _toTR = toTR;
                _minTR = minTR;
                _maxTr = maxTR;
            }

        }

        private Decimal nudThreshold;
        public Decimal NudThreshold 
        {
            get { return nudThreshold; }
            set
            { 
                if (nudThreshold != value)
                    nudThreshold = value;
                onNudThresholdUpdate();
            }
        }
        public event EventHandler nudThresholdEvent;
        private void onNudThresholdUpdate()
        {
            EventHandler handler = nudThresholdEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }


        private Decimal nudTConThreshold;
        public Decimal NudTConThreshold
        {
            get { return nudTConThreshold; }
            set
            {
                if (nudTConThreshold != value)
                    nudTConThreshold = value;
                onNudTConThresholdUpdate();
            }
        }
        public event EventHandler nudTConThresholdEvent;
        private void onNudTConThresholdUpdate()
        {
            EventHandler handler = nudTConThresholdEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudBetaThreshold;
        public Decimal NudBetaThreshold
        {
            get { return nudBetaThreshold; }
            set
            {
                if (nudBetaThreshold != value)
                    nudBetaThreshold = value;
                onNudBetaThresholdUpdate();
            }
        }
        public event EventHandler nudBetaThresholdEvent;
        private void onNudBetaThresholdUpdate()
        {
            EventHandler handler = nudBetaThresholdEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudSVMThreshold;
        public Decimal NudSVMThreshold
        {
            get { return nudSVMThreshold; }
            set
            {
                if (nudSVMThreshold != value)
                    nudSVMThreshold = value;
                onNudSVMThresholdUpdate();
            }
        }
        public event EventHandler nudSVMThresholdEvent;
        private void onNudSVMThresholdUpdate()
        {
            EventHandler handler = nudSVMThresholdEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        //eye slices - if 0 will not filter, if more than 0 will filter every slice from 1 to N
        private Decimal nudFilterEyeSlices;
        public Decimal NudFilterEyeSlices
        {
            get { return nudFilterEyeSlices; }
            set
            {
                if (nudFilterEyeSlices != value)
                    nudFilterEyeSlices = value;
                onFilterEyeSlicesUpdate();
            }
        }
        public event EventHandler nudFilterEyeSlicesEvent;
        private void onFilterEyeSlicesUpdate()
        {
            EventHandler handler = nudFilterEyeSlicesEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        //eye slice line filter, if there are slices to be removed, we remove the first N lines
        private Decimal nudEyeSliceFirstLines;
        public Decimal NudEyeSliceFirstLines
        {
            get { return nudEyeSliceFirstLines; }
            set
            {
                if (nudEyeSliceFirstLines != value)
                    nudEyeSliceFirstLines = value;
                onEyeSliceFirstLinesUpdate();
            }
        }
        public event EventHandler nudEyeSliceFirstLinesEvent;
        private void onEyeSliceFirstLinesUpdate()
        {
            EventHandler handler = nudEyeSliceFirstLinesEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Feature Process Options - 4 -------------------------------------------------------
        /// <summary>

        public bool changedNormalizationType = false;
        private NormalizationType normalizedType = NormalizationType.RawMinusMedWin;
        public NormalizationType NormalizedType
        {
            get
            {
                return normalizedType;
            }
            set
            {
                normalizedType = value;
                changedNormalizationType = true;
                onNormalizationTypeUpdate();
            }
        }

        public event EventHandler NormalizationTypeUpdateEvent;
        private void onNormalizationTypeUpdate()
        {
            EventHandler handler = NormalizationTypeUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        //------------------------------------------------------------------------------------

        private bool unprocessedChecked = false;
        public bool UnprocessedChecked
        {
            get
            {
                return unprocessedChecked;
            }
            set
            {
                if (unprocessedChecked != value)
                    unprocessedChecked = value;
                unprocessedUpdate();
            }
        }

        public event EventHandler unprocessed;
        private void unprocessedUpdate()
        {
            EventHandler handler = unprocessed;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Data Type Output - X -------------------------------------------------------
        /// <summary>        
        private bool rbSvmOutputChecked;
        public bool RbSvmOutputChecked
        {
            get
            {
                return rbSvmOutputChecked;
            }
            set
            {
                if (rbSvmOutputChecked != value)
                    rbSvmOutputChecked = value;
                rbSvmOutputUpdate();
            }
        }

        public event EventHandler rbSvmOutput;
        private void rbSvmOutputUpdate()
        {
            EventHandler handler = rbSvmOutput;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private bool rbCSVOutputChecked;
        public bool RbCSVOutputChecked
        {
            get
            {
                return rbCSVOutputChecked;
            }
            set
            {
                if (rbCSVOutputChecked != value)
                    rbCSVOutputChecked = value;
                rbCSVOutputUpdate();
            }
        }

        public event EventHandler rbCSVOutput;
        private void rbCSVOutputUpdate()
        {
            EventHandler handler = rbCSVOutput;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private bool perEventMapChecked;
        public bool PerEventMapChecked
        {
            get
            {
                return perEventMapChecked;
            }
            set
            {
                if (perEventMapChecked != value)
                    perEventMapChecked = value;
                perEventMapCheckedUpdate();
            }
        }


        public event EventHandler perEventMap;
        private void perEventMapCheckedUpdate()
        {
            EventHandler handler = perEventMap;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        } 

        //from tr 3 to 4-5-6 we can extract, overflowing may result in unexpected behavior, no checks are implemented.
        private Decimal nudExtractFromTR;
        public Decimal NudExtractFromTR
        {
            get { return nudExtractFromTR; }
            set
            {
                if (nudExtractFromTR != value)
                    nudExtractFromTR = value;
                onNudExtractFromTRUpdate();
            }
        }
        public event EventHandler nudExtractFromTREvent;
        private void onNudExtractFromTRUpdate()
        {
            EventHandler handler = nudExtractFromTREvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudExtractToTR;
        public Decimal NudExtractToTR
        {
            get { return nudExtractToTR; }
            set
            {
                if (nudExtractToTR != value)
                    nudExtractToTR = value;
                onNudExtractToTRUpdate();
            }
        }
        public event EventHandler nudExtractToTREvent;
        private void onNudExtractToTRUpdate()
        {
            EventHandler handler = nudExtractToTREvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudClassifyUsingTR;
        public Decimal NudClassifyUsingTR
        {
            get { return nudClassifyUsingTR; }
            set
            {
                if (nudClassifyUsingTR != value)
                    nudClassifyUsingTR = value;
                onNudClassifyUsingTRUpdate();
            }
        }
        public event EventHandler nudClassifyUsingTREvent;
        private void onNudClassifyUsingTRUpdate()
        {
            EventHandler handler = nudClassifyUsingTREvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudMovingWindow;
        public Decimal NudMovingWindow
        {
            get { return nudMovingWindow; }
            set
            {
                if (nudMovingWindow != value)
                    nudMovingWindow = value;
                onNudMovingWindowUpdate();
            }
        }
        public event EventHandler nudMovingWindowEvent;
        private void onNudMovingWindowUpdate()
        {
            EventHandler handler = nudMovingWindowEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudIGThreshold;
        public Decimal NudIGThreshold
        {
            get { return nudIGThreshold; }
            set
            {
                if (nudIGThreshold != value)
                    nudIGThreshold = value;
                onNudIGThresholdUpdate();
            }
        }
        public event EventHandler nudIGThresholdEvent;
        private void onNudIGThresholdUpdate()
        {
            EventHandler handler = nudIGThresholdEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudIGVoxelAmount;
        public Decimal NudIGVoxelAmount
        {
            get { return nudIGVoxelAmount; }
            set
            {
                if (nudIGVoxelAmount != value)
                    nudIGVoxelAmount = value;
                onNudIGVoxelAmountUpdate();
            }
        }
        public event EventHandler nudIGVoxelAmountEvent;
        private void onNudIGVoxelAmountUpdate()
        {
            EventHandler handler = nudIGVoxelAmountEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }
        //----
         
        private IGType igSelectionType = IGType.Voxels;
        public IGType IgSelectionType
        {
            get
            {
                return igSelectionType;
            }
            set
            {
                igSelectionType = value; 
                onIGTypeUpdate();
            }
        }

        public event EventHandler IGTypeUpdateEvent;
        private void onIGTypeUpdate()
        {
            EventHandler handler = IGTypeUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }
        /// <summary>
        ///  5 -----------------------------------------------
        /// </summary>

        private List<string> FeatureProcessJobs = new List<string>();
        private int indexFeatureProcessJobs = 0;
        public void setFeatureProcessJobs(string line)
        {
            FeatureProcessJobs.Add(line);
            onFeatureProcessJobsUpdate();
        }

        public void RemoveAtFeatureProcessJobs(int _pos)
        {
            FeatureProcessJobs.RemoveAt(_pos);
            onFeatureProcessJobsUpdate();
        }


        public void ClearFeatureProcessJobs()
        {
            FeatureProcessJobs.Clear();
            onFeatureProcessJobsUpdate();
        }
        public List<string> getFeatureProcessJobs()
        {
            List<string> temp = new List<string>();
            for (int t = 0; t < FeatureProcessJobs.Count; t++)
            {
                temp.Add(FeatureProcessJobs[t]);
            }

            indexFeatureProcessJobs += FeatureProcessJobs.Count - indexFeatureProcessJobs;
            return temp;
        }

        public event EventHandler featureProcessJobsUpdateEvent;
        private void onFeatureProcessJobsUpdate()
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler handler = featureProcessJobsUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }


        private List<int> Kvalues = new List<int>();
        private int indexKvalues = 0;


        public void incKvalues(int index)
        {
            Kvalues[index] = Kvalues[index] + 1;
            onKvaluesUpdate();
        }

        public void decKvalues(int index)
        {
            if (Kvalues[index] - 1 >= 1)
            {
                Kvalues[index] = Kvalues[index] - 1;
                onKvaluesUpdate();
            }
        }

        public void setKvalues(int index, int k)
        {
            Kvalues[index] = (k);
            onKvaluesUpdate();
        }

        public void addKvalues(int k)
        {
            Kvalues.Add(k);
            onKvaluesUpdate();
        }

        public List<int> getKvalues()
        {
            List<int> temp = new List<int>();
            for (int t = 0; t < Kvalues.Count; t++)
            {
                temp.Add(Kvalues[t]);
            }

            indexKvalues += Kvalues.Count - indexKvalues;
            return temp;
        }

        public void RemoveAtKvalues(int _pos)
        {
            Kvalues.RemoveAt(_pos);
            onKvaluesUpdate();

        }
        public void ClearKvalues()
        {
            Kvalues.Clear();
            onKvaluesUpdate();
        }
        public event EventHandler KvaluesUpdateEvent;
        private void onKvaluesUpdate()
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler handler = KvaluesUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        } 
        /// <summary>
        /// SVM - 6 -------------------------------------------------------
        /// <summary>
        
        private bool cbMultiClassChecked = true;
        public bool CbMultiClassChecked
        {
            get
            {
                return cbMultiClassChecked;
            }
            set
            {
                if (cbMultiClassChecked != value)
                {
                    cbMultiClassChecked = value;
                    cbMultiClassCheckedUpdate();
                }
            }
        }

        public event EventHandler cbMultiClassCheckedEvent;
        private void cbMultiClassCheckedUpdate()
        {
            EventHandler handler = cbMultiClassCheckedEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }
         
        private bool cbOverwriteProcessedFilesChecked = true;
        public bool CbOverwriteProcessedFilesChecked
        {
            get
            {
                return cbOverwriteProcessedFilesChecked;
            }
            set
            {
                if (cbOverwriteProcessedFilesChecked != value)
                {
                    cbOverwriteProcessedFilesChecked = value;
                    cbOverwriteProcessedFilesCheckedUpdate();
                }
            }
        }

        public event EventHandler cbOverwriteProcessedFilesCheckedEvent;
        private void cbOverwriteProcessedFilesCheckedUpdate()
        {
            EventHandler handler = cbOverwriteProcessedFilesCheckedEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        } 

        // should we peek in a higher TR's IG ?
        private bool cbPeekHigherTRsIGChecked = true;
        public bool CbPeekHigherTRsIGChecked
        {
            get
            {
                return cbPeekHigherTRsIGChecked;
            }
            set
            {
                if (cbPeekHigherTRsIGChecked != value)
                {
                    cbPeekHigherTRsIGChecked = value;
                    cbPeekHigherTRsIGCheckedUpdate();
                }
            }
        }

        public event EventHandler cbPeekHigherTRsIGCheckedEvent;
        private void cbPeekHigherTRsIGCheckedUpdate()
        {
            EventHandler handler = cbPeekHigherTRsIGCheckedEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }  

        //svm algorithm check box for using the c# wrapper.
        private bool cbSVMChecked = false;
        public bool CbSVMChecked
        {
            get
            {
                return cbSVMChecked;
            }
            set
            {
                if (cbSVMChecked != value)
                {
                    cbSVMChecked = value;
                    cbSVMCheckedUpdate();
                }
            }
        }

        public event EventHandler cbSVMCheckedUpdateEvent;
        private void cbSVMCheckedUpdate()
        {
            EventHandler handler = cbSVMCheckedUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }


        //sound check box.
        private bool cbSoundONChecked = true;
        public bool CbSoundONChecked
        {
            get
            {
                return cbSoundONChecked;
            }
            set
            {
                if (cbSoundONChecked != value)
                {
                    cbSoundONChecked = value;
                    cbSoundONCheckeddUpdate();
                }
            }
        }

        public event EventHandler cbSoundONCheckeddUpdateEvent;
        private void cbSoundONCheckeddUpdate()
        {
            EventHandler handler = cbSoundONCheckeddUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        //WEKA SMO algorithm check box.
        private bool cbSMOChecked = true;
        public bool CbSMOChecked
        {
            get
            {
                return cbSMOChecked;
            }
            set
            {
                if (cbSMOChecked != value)
                {
                    cbSMOChecked = value;
                    cbSMOCheckedUpdate();
                }
            }
        }

        public event EventHandler cbSMOCheckedUpdateEvent;
        private void cbSMOCheckedUpdate()
        {
            EventHandler handler = cbSMOCheckedUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }


        /// <summary>
        ///  class 1 and class two combo boxes
        /// </summary>
        /// 
        private int cmbClass1Selected = 1; // updated after population and selection
        public int CmbClass1Selected
        {
            get
            {
                return cmbClass1Selected;
            }
            set
            {
                if (cmbClass1Selected != value)
                {
                    cmbClass1Selected = value;
                    cmbClass1SelectedUpdate();
                }
            }
        }


        public event EventHandler cmbClass1Event;
        private void cmbClass1SelectedUpdate()
        {
            EventHandler handler = cmbClass1Event;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private int cmbClass2Selected = 2; // updated after population and selection
        public int CmbClass2Selected
        {
            get
            {
                return cmbClass2Selected;
            }
            set
            {
                if (cmbClass2Selected != value)
                {
                    cmbClass2Selected = value;
                    cmbClass2SelectedUpdate();
                }
            }
        }


        public event EventHandler cmbClass2Event;
        private void cmbClass2SelectedUpdate()
        {
            EventHandler handler = cmbClass2Event;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

         
        //=============================================
        private List<string> labels = new List<string>();
        public int indexOfLabel(string label)
        {
            return labels.IndexOf(label);
        }

        public int indexOfSlectedLabel1()
        {
            return labels.IndexOf(labels[cmbClass1Selected]);
        }

        public int indexOfSlectedLabel2()
        {
            return labels.IndexOf(labels[cmbClass2Selected]);
        }

        public bool containsLabel(string label)
        {
            return labels.Contains(label);
            
        }

        public void addLabel(string label)
        {
            labels.Add(label);
            onLabelsUpdate();
        }

        public string getLabel(int pos)
        {
            if (labels.Count < pos)
                return "Doesnt Exist";
            else
                return labels[pos];
        }

        public List<string> getLabels()
        {
            List<string> temp = new List<string>();
            for (int t = 0; t < labels.Count; t++)
            {
                temp.Add(labels[t]);
            } 
            return temp;
        }

        public void RemoveLabels(int _pos)
        {
            labels.RemoveAt(_pos);
            onLabelsUpdate();

        }

        public void ClearLabels()
        {
            labels.Clear();
            onKvaluesUpdate();
        }

        public event EventHandler labelsUpdateEvent;
        private void onLabelsUpdate()
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler handler = labelsUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        } 
 
        /// <summary>
        /// radio buttons learning method + folds
        /// </summary> 
        ///  
        private TrainingType trainType = TrainingType.Weka; //init training type here according to radio button
        public TrainingType TrainType
        {
            get
            {
                return trainType;
            }
            set
            {
                trainType = value; 
                onTrainTypeUpdate();
            }
        }
        public event EventHandler TrainTypeUpdateEvent;
        private void onTrainTypeUpdate()
        {
            EventHandler handler = TrainTypeUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudCVFolds;
        public Decimal NudCVFolds
        {
            get { return nudCVFolds; }
            set
            {
                if (nudCVFolds != value)
                    nudCVFolds = value;
                onNudCVFoldsUpdate();
            }
        }
        public event EventHandler nudCVFoldsEvent;
        private void onNudCVFoldsUpdate()
        {
            EventHandler handler = nudCVFoldsEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudTrainTestSplit;
        public Decimal NudTrainTestSplit
        {
            get { return nudTrainTestSplit; }
            set
            {
                if (nudTrainTestSplit != value)
                    nudTrainTestSplit = value;
                onNudTrainTestSplit();
            }
        }
        public event EventHandler nudTrainTestSplitEvent;
        private void onNudTrainTestSplit()
        {
            EventHandler handler = nudTrainTestSplitEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudGridFolds; 
        public Decimal NudGridFolds
        {
            get { return nudGridFolds; }
            set
            {
                if (nudGridFolds != value)
                    nudGridFolds = value;
                onNudGridFoldsUpdate();
            }
        }
        public event EventHandler nudGridFoldsEvent;
        private void onNudGridFoldsUpdate()
        {
            EventHandler handler = nudGridFoldsEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        private Decimal nudRFEFolds;
        public Decimal NudRFEFolds
        {
            get { return nudRFEFolds; }
            set
            {
                if (nudRFEFolds != value)
                    nudRFEFolds = value;
                onNudRFEFoldsUpdate();
            }
        }
        public event EventHandler nudRFEFoldsEvent;
        private void onNudRFEFoldsUpdate()
        {
            EventHandler handler = nudRFEFoldsEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// SVM output - 8 -------------------------------------------------------
        /// <summary>
        private List<string> logger = new List<string>();
        private int indexLogger = 0;


        public event DelegateMessage LogMessage;

        public void setLog(string line)
        { 
            logger.Add(line);
            //either logmessage or onloggerupdate can be used, the onlogger is more complicated and uses two variables, the first one just sends a string and invokes when in the main form thread.
            LogMessage.Invoke(line);

            //onLoggerUpdate(); 
        }

        public List<string> getLog()
        {
            List<string> temp = new List<string>();
            for (int t = indexLogger; t < logger.Count; t++)
            {
                temp.Add(logger[t]);
            }

            indexLogger += logger.Count - indexLogger;
            return temp;
        } 

        public event EventHandler LoggerUpdateEvent;
        private void onLoggerUpdate()
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler handler = LoggerUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new EventArgs());
            }
        }

         
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////  Progress Bar Stuff
        private int progressBarMax;
        public int ProgressBarMax
        {
            get
            {
                return progressBarMax;
            }
            set
            {
                progressBarMax = value;
                //activate shooting function
                onProgressBarUpdate(progressBarValue, value);
            }
        }
        private int progressBarValue;
        public int ProgressBarValue
        {
            get
            {
                return progressBarValue;
            }
            set
            {
                progressBarValue = value;
                //activate shooting function
                onProgressBarUpdate(value, progressBarMax);
            }
        }
        public event EventHandler<ProgressBarChangeEventArg> ProgressBarUpdateEvent;
        private void onProgressBarUpdate(int theValue, int theMax)
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler<ProgressBarChangeEventArg> handler = ProgressBarUpdateEvent;
            if (handler != null)
            {
                //the real shoot, operates all the registered functions in this event.
                handler(this, new ProgressBarChangeEventArg(theValue, theMax));
            }
        }

        public class ProgressBarChangeEventArg : EventArgs
        {
            public int _value
            {
                get;
                private set;
            }
            public int _max
            {
                get;
                private set;
            }

            public ProgressBarChangeEventArg(int theValue, int theMax)
            {
                _max = theMax;
                _value = theValue;
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////////
        //singleton
        private static readonly GuiPreferences instance = new GuiPreferences();        
        private GuiPreferences() { }
        static GuiPreferences() { }
        public static GuiPreferences Instance
        {
            get
            {
               return instance;
               
            }
        }
    }
}
