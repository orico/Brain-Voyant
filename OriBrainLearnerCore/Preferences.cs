using System;
using System.Collections.Generic;
using System.Windows.Forms;
using libSVMWrapper;
using glmDll;
//using glmDllMWARRAY; 
using UDPWrapper;
using weka.attributeSelection;
using weka.core;

namespace OriBrainLearnerCore
{
    
    public enum DataType
    {
        None,
        rawValue,
        betaValue,
        tContrastValue,
        SVMFile,
    }

    public enum NormalizationType
    {
        None,
        MinMax,
        RawMinusMed,                // (raw - med)
        RawDivMed,                  // (raw / med)
        RawMinusMedDivMed,          // (raw - med) / med
        RawMinusMedWin   ,          // (raw - med (window))         --koppel
        RawMinusMedWinDivIqrWin,    // (raw - med (window)) / iqr
    }

    public enum IGType
    {
        None,
        Threshold,
        Voxels,
    }

    public enum TrainingType
    {
        None,
        Weka,
        TrainTestSplit,
        CrossValidation,
        GridSearch,
        RFE,
    }

    public enum ProcessBlockType
    {
        OnsetAveraging,
        OnsetMax,
        OnsetAbsoluteMax,
        Voxelize,
        ZscoreNormalize,
        ROI,
        PCA,
    }

    // enum string pattern http://stackoverflow.com/questions/424366/c-sharp-string-enums
    public enum AvatarCommand
    {
        Nothing,
        Left,
        Right,
        Forward
    }
    //http://forum.unity3d.com/threads/22628-Arrays-in-Structs-in-C
    
    /// <summary>
    /// min/max values saved for all features in all trs.
    /// </summary>
    public struct MinMax
    { 
        public MinMax( double[] feature_min_temp, double[] feature_max_temp)
        { 
            feature_min = feature_min_temp;
            feature_max = feature_max_temp;
        }

        public double[] feature_min;
        public double[] feature_max;
    }


    public class Preferences
    {
        // ALL class variables are initialized in initvariables();
        public double validMaxBrainAverage;
        public bool corruptedVector;
        public StatisticsFeatures MatrixStatistics;
        public StatisticsFeatures MatrixStatisticsNormalized;
        public StatisticsFeatures TestingBaselineStatistics; // keeps the N first TRs.
        
        //min max values are saved here for all runs in Training. this prevents the minimum value from being zero.
        //list is the Runs.
        //second int is TR to classify
        public List<Dictionary<int,MinMax>> MinMax;
        //used to keep run-based baseline trs median in training
        public List<StatisticsFeatures> TrainingBaselineMedians;

        public List<StatisticsFeatures> TrainingBaselineMediansRunningWindow;
        public StatisticsFeatures TestingBaselineMediansRunningWindow; 

        public MinMax medianRange;
        public List<string> BrainMean; // [global mean, after ig filter mean] - for checking the drift issue globally per tr
 
        public int cumulativeTR;
        public int nextEvent;
        public int nextTRinEvent;
        public string currentRunningEvent;
        public NamedPipeClient pipeClient;
        public NamedPipeServer pipeServer;
        public string PipeServerName;
        public string PipeClientName;
        public UDP udp;
        public UDPSim udpsim;

        // DirMonitor variables
        public string masterPath;
        public string pathName;
        public string dicomMasterPath;
        public string dicomTbvWatchPath;

        //configuration - 1 - working dirs 
        public string[] filesInWorkingDir; 

        //configuration - 1b - protocol 
        public int numberOfConditions;
        public ProtocolManager prot;
        public bool protocolLoaded;
        public Events events;

        //configuration - 3 - import, radio buttons binary file types
        public bool svmLoaded;
        public libSVM_ExtendedProblem ProblemOriginal;
        public libSVM_ExtendedProblem ProblemFinal;
        public List<libSVM_ExtendedProblem> ProblemOriginalMulti = new List<libSVM_ExtendedProblem>();
        public List<libSVM_ExtendedProblem> ProblemFinalMulti = new List<libSVM_ExtendedProblem>();
        public short[] BrainRawValueForPlot;
        public int dim_x, dim_y, dim_z;
        public libSVM_Extension svmWrapper; 
        public libSVM svmModel; //only used to store a model and Predict(testset problem)
        public bool RealTimeTestingSingleVector = false;
        public int maxKvectorsToWaitBeforeClassification; //kargest K vector from the process jobs to wait before classifying and sending results
        public int[] KInPreviousVectors; //for each block find the largest K in the previous vectors including itself, to wait before starting to process itself.
        public int currentUDPVector = 0;
        public int currentClassifiedVector = 0;
        public bool modelLoaded = false;
        public IntPtr model;
        public Dictionary<int, int> EyeVoxels;
        public int currentProcessedRun;

        //keeps a mapping of 
        public Dictionary<int, Dictionary<string, List<int>>> TrainingEventStartLocationsPerTR;  //runs 1..N, tr, list of indices        
        public string RealTimeModelDirectory; // variable used to store the location of the Model file, param file and xml file

        //configuration - 6 - train
        public JobsQueue jobs;
        public string[] deleteFiles;

        //holds indices and ranked list after it was used for IG
        public AttributeSelection attsel;

        public List<string[]> classification;
        public List<string> dirList;
        public Dictionary<string,string> configFile; 
        //7-plots 
        public MatlabGLM glm = null;
        public int glmBufferDivider = 100; // this buffer divider is for the line of code in processGLM, on my computer it is possible to assign up to 300 for matrix [,]. on other computers this must change
        public FastVector fastvector;
        /////////////////////////////////////////////////////////////////////////////////////////
        private void initVariables()
        {
            udp = new UDP(); 

            udpsim = new UDPSim();
            
            //holds min max values which prevents the min from being zeroed out after the threhold filter.
            MinMax = new List<Dictionary<int, MinMax>>();
            BrainMean = new List<string>(); //checking for global drift before and after IG filter
            TrainingEventStartLocationsPerTR = new Dictionary<int, Dictionary<string, List<int>>>(); //run, tr, list of indices
            validMaxBrainAverage = 1000;
            corruptedVector = false;
            PipeServerName = "OBL"; //in here the client uses OBL pipe to communicate to the server, if a communication is needed from server to client, i believe we should use another pipe - check this.
            PipeClientName = "Unity";

            pipeServer = new NamedPipeServer(PipeServerName);
            pipeClient = new NamedPipeClient(PipeServerName);    

            cumulativeTR = 0;
            nextEvent = 0;
            nextTRinEvent = 0;
            currentRunningEvent = "";

            // DirMonitor variables
            //===========================  
            masterPath = "/My_Dropbox/VERE/MRI_data/Nir/";
            pathName = masterPath + "NIR_run1_rtp/";
            dicomMasterPath = masterPath + "NIR_run1_dcm_master/";
            dicomTbvWatchPath = masterPath + "NIR_run1_dcm_watch/";

            //configuration - 1b - protocol 
            events = new Events(new List<IntIntStr>());
            protocolLoaded = false;

            //configuration - 3 - import, radio buttons binary file types
            svmLoaded = false;            
            ProblemOriginal = new libSVM_ExtendedProblem();
            svmWrapper = new libSVM_Extension();

            deleteFiles =  new string[]
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
                        "TrainSet_4th_vectors_scaledCS.libsvm.arff",
                        "TrainSet_5th_vectors.libsvm",
                        "TrainSet_5th_vectors_scale_paramCS.libsvm",
                        "TrainSet_5th_vectors_scaledCS.libsvm",
                        "TrainSet_5th_vectors_scaledCS.libsvm.arff",
                        "TrainSet_6th_vectors.libsvm",
                        "TrainSet_6th_vectors_scale_paramCS.libsvm",
                        "TrainSet_6th_vectors_scaledCS.libsvm",
                        "TrainSet_6th_vectors_scaledCS.libsvm.arff"
                    };

            //memory capcity test, each array up to 2gb!!
            /*try
            {
                int[] ar = new int[1];
                int[] ar2 = new int[1];
                for (int i = 1; i < 100000; i++)
                {
                    Array.Resize(ref ar, ar.Length + 100000000);
                    Array.Resize(ref ar2, ar.Length + 100000000);
                    long l = PublicMethods.getRam();
                }
            }
            catch (Exception e)
            {
                long l = PublicMethods.getRam();
                string s = e.ToString();
            }*/


            classification = new List<string[]>();

            //7-plots 
            try
            {
                //caused problems in mri's pc
                //glm = new MatlabGLM();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Warning: Matlab Initialization failed, please install matlab 2011a 64bit! (it is SAFE to CONTINUE!)" + e.ToString(),
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button2);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        //singleton
        private static readonly Preferences instance = new Preferences();
        private Preferences() { 
            initVariables(); 
        }
        static Preferences() { }

        public static Preferences Instance
        {
            get
            { 
                return instance;
            }
        }

    }
}

