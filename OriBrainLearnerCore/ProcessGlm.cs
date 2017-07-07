using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libSVMWrapper;
using MathWorks.MATLAB.NET.Arrays; 

namespace OriBrainLearnerCore
{ 
    //done for each TR
    public class ProcessGlm : IDataProcessor
    {
        libSVM_ExtendedProblem _iproblem = null;
        libSVM_ExtendedProblem _oproblem = new libSVM_ExtendedProblem();
        int _lastIndex;
        int currentLocationInFlow;
        public int k = 7;


        public ProcessGlm()
        {
            if (Preferences.Instance.RealTimeTestingSingleVector)
            {
                _oproblem.labels = new double[0];
                _oproblem.samples = new SortedDictionary<int, double>[0];
            }
        }

        public int CurrentLocationInFlow
        {
            get { return currentLocationInFlow; }
            set { currentLocationInFlow = value; }
        }
         
        public int K
        {
            get { return k; }
            set { k = value; }
        } 

        public void setProblem(libSVM_ExtendedProblem problem)
        {
            _iproblem = problem;
            _lastIndex = _iproblem.GetLastIndex();
        }

        public libSVM_ExtendedProblem getProblem()
        {
            return _oproblem;
        }

        /// <summary>
        /// first set of indices in array 0 will determine the set indices for the entire matrix. it is not equal in all arrays.
        /// can be the union or cut of first 5 arrays.
        /// </summary>
        /// <returns></returns>
        public int[] getIndices()
        {
            //return _iproblem.samples[K-1].Keys.ToArray();
            return _iproblem.samples[0].Keys.ToArray();
        }

        ///isamples doesnt get added the vectors, at i=7 it has 1...

        public void preProcess() { }

        public void Process()
        {
            if (!Preferences.Instance.RealTimeTestingSingleVector)
            {
                ProcessFULL();
                //ProcessForTrainingBlocksOptimized();
            }
            else
            {
                ProcessForSingleTest();
            }

        }
        //this old version will process the entire buffer
        //this version needs a 64bit version of the exe or else there will be out of memory errors.
        //32bit, dont use more than [250x190000]
        public void ProcessFULL()
        {
            _oproblem.labels = new double[_iproblem.labels.Length - K + 1];
            _oproblem.samples = new SortedDictionary<int, double>[_iproblem.samples.Length - K + 1];

            int[] _indices = getIndices();

            //get inverse model once
            MWArray[] invModel = new MWArray[1];
            invModel = Preferences.Instance.glm.getInverseModel(1, (MWArray)K);
            MWNumericArray invmodel = (MWNumericArray)invModel[0];
            double[,] invModelDoubleArray = (double[,])invmodel.ToArray();

            //get maximum size of a dictionary inside the array
            int maxIndexDictionary = 0;
            int _size = 0;
            foreach (SortedDictionary<int, double> d in _iproblem.samples)
            {
                _size = d.Keys.Max();
                if (_size > maxIndexDictionary)
                { maxIndexDictionary = _size; }
            }

            //copy each dictionary to the matrix that is going inside the beta calculation function
            float[,] matrix = new float[_iproblem.samples.Length, maxIndexDictionary];

            int mx = 0;
            foreach (SortedDictionary<int, double> d in _iproblem.samples)
            {
                foreach (int key in d.Keys)
                {
                    matrix[mx, key - 1] = (float)d[key];
                }
                mx++;
            }

            //beta function is calculating for all the samples matrix
            MWArray[] Betas = new MWArray[1];

            //calculate betas
            Betas = Preferences.Instance.glm.Beta_calculation_indices(1, (MWNumericArray)_indices, (MWNumericArray)matrix, (MWNumericArray)K, (MWNumericArray)Preferences.Instance.glm.getInverseModel(1, (MWArray)K)[0]);

            matrix = null;

            MWNumericArray betas = (MWNumericArray)Betas[0];

            Betas = null;

            double[,] betasArray = (double[,])betas.ToArray();

            betas = null;
            //Double[] betasArray = (Double[])betas.ToVector(MWArrayComponent.Real);
            //double[] betasArray = (double[])betas.ToArray();

            //stitch back betas and labels
            for (int i = 0; i < _iproblem.samples.Length - K + 1; i++)
            {
                double[] vec = new double[_indices.Length];
                int key = 0;
                for (int j = 0; j < _indices.Length; j++)
                {
                    vec[key] = betasArray[i, j];
                    key++;
                }
                //_oproblem.samples[i].Keys = (SortedDictionary<int,double>.KeyCollection)_indices;

                //convert to dictionary: zip array '0' indices and new beta values. and create a sorted dictionary
                //SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));
                SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));

                //#endif

                _oproblem.samples[i] = dic;
                double _label = _iproblem.labels[i + K - 1];
                int iBackward = i;
                while (_label == 1)
                {
                    iBackward--;
                    if (iBackward + K - 1 >= 0)
                        _label = _iproblem.labels[iBackward + K - 1];
                    else
                    {
                        GuiPreferences.Instance.setLog("samples at beginning of files are from the baseline, cant assign a valid label to them because they are before an action, not after!");
                    }
                }

 
                 _oproblem.labels[i] = _label; //!!! if baseline take the event that happened before
            }

            _indices = null;
            betasArray = null;

            _oproblem.UpdateMaximumIndex();
        }
        
        //this will process chunks froom the samples, using a blocks of data using a divider
        //this version works better than chunksFULLMem . it scales up to 300 using 32bit.
        //32bit, dont use more than [300x190000]
        public void ProcessForTrainingBlocksOptimized()
        {
            try
            {
                _oproblem.labels = new double[_iproblem.labels.Length - K + 1];
                //_oproblem.samples = new SortedDictionary<int, double>[_iproblem.samples.Length - K + 1];
                _oproblem.samples = new SortedDictionary<int, double>[1];

                int[] _indices = getIndices();

                //get inverse model once
                MWArray[] invModel = new MWArray[1];
                invModel = Preferences.Instance.glm.getInverseModel(1, (MWArray)K);
                MWNumericArray invmodel = (MWNumericArray)invModel[0];
                double[,] invModelDoubleArray = (double[,])invmodel.ToArray();

                //get maximum size of a dictionary inside the array
                int maxIndexDictionary = 0;
                int _size = 0;
                foreach (SortedDictionary<int, double> d in _iproblem.samples)
                {
                    _size = d.Keys.Max();
                    if (_size > maxIndexDictionary)
                    { maxIndexDictionary = _size; }
                }

                int MatrixBufSize = Preferences.Instance.glmBufferDivider; //matrix[100,196000]
                int fromSample = 0;
                int toSample = MatrixBufSize - 1;
                // if less than buffer size then we just need to process what there is. set lower limit on samples count
                if (toSample > _iproblem.samples.Length)
                    toSample = _iproblem.samples.Length - 1;

                while (fromSample < _iproblem.samples.Length - 1 - K)
                {
                    //copy each dictionary to the matrix that is going inside the beta calculation function
                    float[,] matrix = new float[toSample - fromSample + 1, maxIndexDictionary];
                    
                    for (int i = fromSample; i <= toSample; i++)
                    {
                        SortedDictionary<int, double> d = _iproblem.samples[i];
                        foreach (int key in d.Keys)
                        {
                            matrix[i - fromSample, key - 1] = (float)d[key];
                        }
                    }

                    //beta function is calculating for all the samples matrix
                    MWArray[] Betas = new MWArray[1];

                    //calculate betas
                    Betas = Preferences.Instance.glm.Beta_calculation_indices(1, (MWNumericArray)_indices, (MWNumericArray)matrix, (MWNumericArray)K, (MWNumericArray)Preferences.Instance.glm.getInverseModel(1, (MWArray)K)[0]);

                    matrix = null;

                    MWNumericArray betas = (MWNumericArray)Betas[0];

                    //Betas = null;

                    double[,] betasArray = (double[,])betas.ToArray();

                    MWArray.DisposeArray(Betas);
                    Betas = null;
                    MWNumericArray.DisposeArray(betas);
                    betas = null;
                    GC.Collect();


                    //stitch back betas and labels
                    for (int i = fromSample + K - 1; i <= toSample; i++)
                    //for (int i = 0; i < _iproblem.samples.Length - K + 1; i++)
                    {
                        double[] vec = new double[_indices.Length];
                        int key = 0;
                        for (int j = 0; j < _indices.Length; j++)
                        {
                            vec[key] = betasArray[(betasArray.GetLength(0) -1) /*- (i - (fromSample + K - 1))*/, j];                            
                            key++;
                        }

                        //TODO: resize array was moved to extensionfunctions and was made static, still need to make sure it works again!
                        betasArray = ExtensionFunctions.ResizeArray(betasArray, betasArray.GetLength(0) - 1, betasArray.GetLength(1));
                        //_oproblem.samples[i].Keys = (SortedDictionary<int,double>.KeyCollection)_indices;

                        //convert to dictionary: zip array '0' indices and new beta values. and create a sorted dictionary
                        //SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));
                        SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));

                        vec = null;
                        //#endif

                        _oproblem.samples[i - K + 1] = dic;
                        GuiPreferences.Instance.setLog("resized from: " + _oproblem.samples.Length.ToString() + " to: " + (_oproblem.samples.Length + 1).ToString() + " " +RamMethods.getRam().ToString());
                        Array.Resize(ref _oproblem.samples, _oproblem.samples.Length + 1);

                        double _label = _iproblem.labels[i];
                        int iBackward = i;
                        while (_label == 1)
                        {
                            iBackward--;
                            _label = _iproblem.labels[iBackward];
                        }
                        _oproblem.labels[i - K + 1] = _label; //!!! if baseline take the event that happened before
                    }
                    betasArray = null;

                    fromSample = toSample + 1 - K + 1;
                    toSample += MatrixBufSize;
                    GuiPreferences.Instance.setLog("resized finished");
                    // if less than buffer size then we just need to process what there is. set lower limit on samples count
                    if (toSample > _iproblem.samples.Length)
                        toSample = _iproblem.samples.Length - 1;

                    GC.Collect();
                }

                _indices = null;
                Array.Resize(ref _oproblem.samples, _oproblem.samples.Length - 1);
                _oproblem.UpdateMaximumIndex();
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog(e.ToString());
            }
        }

        //this will process chunks froom the samples, using a blocks of data using a divider
        //this version works better than FULL but still needs a 64bit version of the exe or else there will be out of memory errors.
        // will throw out of memory in several locations. use optimized.
        //32bit, dont use more than [100x190000]
        public void ProcessChunksFullMem()
        { 
            try
                {
                _oproblem.labels = new double[_iproblem.labels.Length - K + 1];
                _oproblem.samples = new SortedDictionary<int, double>[_iproblem.samples.Length - K + 1];
                //_oproblem.samples = new SortedDictionary<int, double>[1];

                int[] _indices = getIndices();

                //get inverse model once
                MWArray[] invModel = new MWArray[1];
                invModel = Preferences.Instance.glm.getInverseModel(1, (MWArray)K);
                MWNumericArray invmodel = (MWNumericArray)invModel[0];
                double[,] invModelDoubleArray = (double[,])invmodel.ToArray();

                //get maximum size of a dictionary inside the array
                int maxIndexDictionary = 0;
                int _size = 0;
                foreach (SortedDictionary<int, double> d in _iproblem.samples)
                {
                    _size = d.Keys.Max();
                    if (_size > maxIndexDictionary)
                    { maxIndexDictionary = _size; }
                }

                int MatrixBufSize = Preferences.Instance.glmBufferDivider; //matrix[100,196000]
                int fromSample = 0;
                int toSample = MatrixBufSize-1; 
                // if less than buffer size then we just need to process what there is. set lower limit on samples count
                if (toSample > _iproblem.samples.Length) 
                    toSample = _iproblem.samples.Length -1;

                while (fromSample < _iproblem.samples.Length - 1 - K)
                { 
                    //copy each dictionary to the matrix that is going inside the beta calculation function
                    float[,] matrix = new float[toSample-fromSample+1, maxIndexDictionary];
                     
                    /*int mx = 0;
                    foreach (SortedDictionary<int, double> d in _iproblem.samples)
                    {
                        foreach (int key in d.Keys)
                        {
                            matrix[mx, key - 1] = (float)d[key];
                        }
                        mx++;
                    }*/

                    for (int i = fromSample; i <= toSample; i++)
                    {
                        SortedDictionary<int, double> d = _iproblem.samples[i];
                        foreach (int key in d.Keys)
                        {
                            matrix[i - fromSample, key - 1] = (float)d[key];
                        }
                    }
                     
                    //beta function is calculating for all the samples matrix
                    MWArray[] Betas = new MWArray[1];

                    //calculate betas
                    //Preferences.Instance.glm = new glmDll.MatlabGLM();
                    Betas = Preferences.Instance.glm.Beta_calculation_indices(1, (MWNumericArray)_indices, (MWNumericArray)matrix, (MWNumericArray)K, (MWNumericArray)Preferences.Instance.glm.getInverseModel(1, (MWArray)K)[0]);
                     
                    matrix = null;

                    MWNumericArray betas = (MWNumericArray)Betas[0];

                    //Betas = null;

                    double[,] betasArray = (double[,])betas.ToArray();

                    MWArray.DisposeArray(Betas);
                    Betas = null;
                    MWNumericArray.DisposeArray(betas);
                    betas = null;
                    GC.Collect(); 

                    //Double[] betasArray = (Double[])betas.ToVector(MWArrayComponent.Real);
                    //double[] betasArray = (double[])betas.ToArray();

                    //stitch back betas and labels
                    for (int i = fromSample + K - 1; i <= toSample; i++)
                    //for (int i = 0; i < _iproblem.samples.Length - K + 1; i++)
                    {
                        double[] vec = new double[_indices.Length];
                        int key = 0;
                        for (int j = 0; j < _indices.Length; j++)
                        {
                            vec[key] = betasArray[i - (fromSample + K - 1), j];
                            key++;
                        }
                        //_oproblem.samples[i].Keys = (SortedDictionary<int,double>.KeyCollection)_indices;

                        //convert to dictionary: zip array '0' indices and new beta values. and create a sorted dictionary
                        //SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));
                        SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));

                        //#endif

                        _oproblem.samples[i - K + 1] = dic;
                        //GuiPreferences.Instance.setLog("resized from: " + _oproblem.samples.Length.ToString() + " to: " + (_oproblem.samples.Length + 1).ToString() + " " +PublicMethods.getRam().ToString());
                        //Array.Resize(ref _oproblem.samples, _oproblem.samples.Length + 1);

                        double _label = _iproblem.labels[i];
                        int iBackward = i;
                        while (_label == 1)
                        {
                            iBackward--;
                            _label = _iproblem.labels[iBackward];
                        }
                        _oproblem.labels[i - K + 1] = _label; //!!! if baseline take the event that happened before
                    }
                    betasArray = null;

                    fromSample = toSample +1 - K + 1;
                    toSample += MatrixBufSize;
                    GuiPreferences.Instance.setLog("resized finished");        
                // if less than buffer size then we just need to process what there is. set lower limit on samples count
                if (toSample > _iproblem.samples.Length) 
                    toSample = _iproblem.samples.Length -1;

                GC.Collect();
                }

                _indices = null;
                _oproblem.UpdateMaximumIndex();
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog(e.ToString());
            }
            GuiPreferences.Instance.setLog("finished");
        }

        public void ProcessForSingleTest()
        {
            if (Preferences.Instance.currentUDPVector < Preferences.Instance.KInPreviousVectors[currentLocationInFlow]) 
                return;
            try
            {
                //TODO: init only once, later processing add + 1 array resize
                Array.Resize(ref _oproblem.labels, _oproblem.labels.Count() + 1);
                Array.Resize(ref _oproblem.samples, _oproblem.samples.Count() + 1);

                //temp that holds the last K.
                //TOOD: its possible that we dont need to reassign a new K every processing but its safe to clean it.
                libSVM_ExtendedProblem _iproblemKvec = new libSVM_ExtendedProblem();
                _iproblemKvec.labels = new double[K];
                _iproblemKvec.samples = new SortedDictionary<int, double>[K];

                int[] _indices = getIndices();

                //get inverse model once - NOT USED IN THIS CODE
                /*MWArray[] invModel = new MWArray[1];
                invModel = Preferences.Instance.glm.getInverseModel(1, (MWArray)K);
                MWNumericArray invmodel = (MWNumericArray)invModel[0];
                double[,] invModelDoubleArray = (double[,])invmodel.ToArray();*/


                //copy from iproblem to iproblemKvec


                int l = 0;
                for (int i = Preferences.Instance.currentUDPVector - K; i < Preferences.Instance.currentUDPVector; i++)
                {
                    _iproblemKvec.samples[l] = _iproblem.samples[i];
                    _iproblemKvec.labels[l] = _iproblem.labels[i];
                    l++;
                }


                //copy each dictionary to the matrix that is going inside the beta calculation function
                float[,] matrix = new float[K, Preferences.Instance.dim_x * Preferences.Instance.dim_y * Preferences.Instance.dim_z];

                int mx = 0;
                foreach (SortedDictionary<int, double> d in _iproblemKvec.samples)
                {
                    foreach (int key in d.Keys)
                    {
                        matrix[mx, key - 1] = (float)d[key];
                    }
                    mx++;
                }

                //beta function is calculating for all the samples matrix
                MWArray[] Betas = new MWArray[1];

                if (Preferences.Instance.currentUDPVector > 165)
                {
                    int a;
                    /// indices in beta error will occure now.
                }

                //calculate betas
                try
                {
                    Betas = Preferences.Instance.glm.Beta_calculation_indices(1, (MWNumericArray)_indices, (MWNumericArray)matrix, (MWNumericArray)K, (MWNumericArray)Preferences.Instance.glm.getInverseModel(1, (MWArray)K)[0]);
                }
                catch (Exception e)
                {
                    GuiPreferences.Instance.setLog(e.ToString());
                }

                matrix = null;

                MWNumericArray betas = (MWNumericArray)Betas[0];

                Betas = null;

                double[,] betasArray = (double[,])betas.ToArray();

                betas = null;

                //stitch back betas and labels
                for (int i = 0; i < _iproblemKvec.samples.Length - K + 1; i++)
                {
                    double[] vec = new double[_indices.Length];
                    int key = 0;
                    for (int j = 0; j < _indices.Length; j++)
                    {
                        vec[key] = betasArray[i, j];
                        key++;
                    }
                    //_oproblem.samples[i].Keys = (SortedDictionary<int,double>.KeyCollection)_indices;

                    //convert to dictionary: zip array '0' indices and new beta values. and create a sorted dictionary
                    //SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));
                    SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));

                    //#endif

                    _oproblem.samples[Preferences.Instance.currentUDPVector - K] = dic;
                    double _label = _iproblem.labels[Preferences.Instance.currentUDPVector - 1];
                    int iBackward = Preferences.Instance.currentUDPVector - 2;
                    while (_label == 1 && iBackward >= 0)
                    {
                        iBackward--;
                        if (iBackward >= 0)
                            _label = _iproblem.labels[iBackward];
                    }
                    if (_label == 1)
                    {
                        ///TODO: what do we do when the label is 1 and there is no more to go back [1,1,1,1,1,1,1,1,1,1,1,2]
                        ///should we remove this beta from the problem ?
                        ///anyway prediction results should not be sent to unity if label is 1. this is done outside of this function
                    }
                    _oproblem.labels[Preferences.Instance.currentUDPVector - K] = _label; //!!! if baseline take the event that happened before
                }
                _indices = null;
                betasArray = null;
                _oproblem.UpdateMaximumIndex();
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog(e.ToString());
            }
        }

        public void clearInput()
        {
            _iproblem = null;
        }

        public void clearOutput()
        {
            _oproblem = null;
        }

        public void postProcess() { }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using libSVMWrapper;
//using MathWorks.MATLAB.NET.Arrays;
//using glmDll;
//using System.Windows.Forms;

//namespace OriBrainLearner
//{
//    //done for each TR
//    public class ProcessGlm : IDataProcessor
//    {
//        libSVM_ProblemCustom _iproblem = null;
//        libSVM_ProblemCustom _oproblem = new libSVM_ProblemCustom();
//        //int _lastIndex;
//        int K = 7;
//        int numberOfSamplesRead = 0;

//        private static MatlabGLM glm = new MatlabGLM();

//        public void setProblem(libSVM_ProblemCustom problem)
//        {
//            _iproblem = problem;
//            //_lastIndex = _iproblem.GetLastIndex();
//            numberOfSamplesRead = GuiPreferences.Instance.ToTR - GuiPreferences.Instance.FromTR + 1;
//            int manualNOSR = 0;
//            foreach (SortedDictionary<int, double> d in _iproblem.samples)
//                if (d != null)
//                    manualNOSR++;
//            //outputing here might not be good for unity or any other usage of this class in other apps.
//            if (manualNOSR != numberOfSamplesRead)
//            {
//                var form = Form.ActiveForm as Form1;
//                form.TbSVMLog = "adding to glm Failed: from-to-TR doesnt match manual calculation";
//            }

//        }

//        public libSVM_ProblemCustom getProblem()
//        {
//            return _oproblem;
//        }

//        /// <summary>
//        /// first set of indices in array 0 will determine the set indices for the entire matrix. it is not equal in all arrays.
//        /// can be the union or cut of first 5 arrays.
//        /// </summary>
//        /// <returns></returns>
//        public int[] getIndices()
//        {
//            //optional is to take only voxels appear  are in 99% of all samples, for example voxel V336=200 but is noise based and will appear sometimes and sometimes not..

//            return _iproblem.samples[K - 1].Keys.ToArray(); //deals with from to TR
//        }

//        public void preProcess() { }

//        public void Process()
//        {
//            _oproblem.labels = new double[numberOfSamplesRead - K + 1];
//            _oproblem.samples = new SortedDictionary<int, double>[numberOfSamplesRead - K + 1];

//            int[] _indices = getIndices();

//            //get inverse model once - now inside beta calc function

//            MWArray[] invModel = new MWArray[1];
//            invModel = glm.getInverseModel(1, (MWArray)K);
//            MWNumericArray invmodel = (MWNumericArray)invModel[0];
//            double[,] invModelDoubleArray = (double[,])invmodel.ToArray();

//            //get maximum size of a dictionary inside the array
//            int maxIndexDictionary = 0;
//            int _size = 0;
//            foreach (SortedDictionary<int, double> d in _iproblem.samples)
//            {
//                if (d != null)//deals with from to TR
//                {
//                    _size = d.Keys.Max();
//                    if (_size > maxIndexDictionary)
//                    { maxIndexDictionary = _size; }
//                }
//            }

//            //fake new
//            double[,] matrix = new double[1, 1];

//            //copy each dictionary to the matrix that is going inside the beta calculation function
//            //but keep assigning half the size of the matrix in order to fit the memory limits.
//            //this will be done only one in real time as we will surely have enough memory for K vectors.
//            bool success = false;
//            int division = 0;
//            int dimX = 0;
//            while (!success)
//            {
//                success = true;
//                division++;
//                dimX = (int)Math.Ceiling((float)_iproblem.samples.Length / division);
//                try
//                {
//                    matrix = new double[dimX + K, maxIndexDictionary];
//                }
//                catch
//                {
//                    success = false;
//                }

//            }

//            int mx = 0;
//            int pos = 0;
//            int start = dimX * pos;
//            int finish = dimX * (pos + 1);
//            while (finish <= numberOfSamplesRead)
//            {
//                for (int i = start; i < finish; i++)
//                {
//                    if (_iproblem.samples != null)//deals with from to TR
//                    {
//                        foreach (int key in _iproblem.samples[i].Keys)
//                        {
//                            matrix[mx, key - 1] = _iproblem.samples[i][key];
//                        }
//                        mx++;
//                    }
//                }

//                //beta function is calculating for all the samples matrix
//                MWArray[] Betas = new MWArray[1];

//                //calculate betas
//                Betas = glm.Beta_calculation_indices(1, (MWNumericArray)_indices, (MWNumericArray)matrix, (MWNumericArray)K, (MWNumericArray)invmodel);//glm.getInverseModel(1, (MWArray)K)[0]);

//                matrix = null;

//                MWNumericArray betas = (MWNumericArray)Betas[0];

//                Betas = null;

//                double[,] betasArray = (double[,])betas.ToArray();

//                betas = null;
//                //Double[] betasArray = (Double[])betas.ToVector(MWArrayComponent.Real);
//                //double[] betasArray = (double[])betas.ToArray();

//                //stitch back betas and labels
//                //another option is to do from tr to tr here inside the loop instead of 0 to length. then you dont need the second IF..
//                for (int i = 0; i < _iproblem.samples.Length - K + 1; i++)
//                {
//                    if (_iproblem.samples[i] != null) //deals with from to TR
//                    {
//                        double[] vec = new double[_indices.Length];
//                        int key = 0;
//                        for (int j = 0; j < _indices.Length; j++)
//                        {
//                            vec[key] = betasArray[i, j];
//                            key++;
//                        }
//                        //_oproblem.samples[i].Keys = (SortedDictionary<int,double>.KeyCollection)_indices;

//                        //convert to dictionary: zip array '0' indices and new beta values. and create a sorted dictionary
//                        SortedDictionary<int, double> dic = new SortedDictionary<int, double>(_indices.Zip(vec, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v));
//                        _oproblem.samples[i] = dic;
//                        double _label = _iproblem.labels[i + K - 1];
//                        int iBackward = i;
//                        while (_label == 1)
//                        {
//                            iBackward--;
//                            _label = _iproblem.labels[iBackward + K - 1];
//                        }
//                        _oproblem.labels[i] = _label; //!!! if baseline take the event that happened before
//                    }
//                }
//                _indices = null;
//                betasArray = null;

//                //start k-1 backward, this is the second buffer we process, unlike the first buffer we need to calculate the first k vectors 
//                pos++;
//                start = (dimX * pos) - (K - 1);
//                finish = dimX * (pos + 1);
//                if (finish > numberOfSamplesRead)
//                {
//                    finish = numberOfSamplesRead;
//                }
//                matrix = new double[dimX + K, maxIndexDictionary];
//            }
//            _oproblem.UpdateMaximumIndex();
//        }

//        public void clearInput()
//        {
//            _iproblem = null;
//        }

//        public void clearOutput()
//        {
//            _oproblem = null;
//        }

//        public void postProcess() { }
//    }
//}

