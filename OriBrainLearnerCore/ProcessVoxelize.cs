using System;
using System.Collections.Generic;
using System.Text;
using libSVMWrapper; 

namespace OriBrainLearnerCore
{ 
    public class ProcessVoxelize : IDataProcessor
    {
        libSVM_ExtendedProblem _iproblem = null;
        libSVM_ExtendedProblem _oproblem = new libSVM_ExtendedProblem();
        int _lastIndex;        
        //to access a known voxel => z_coord*dim_x*dim_y + y_coord*dim_x + x_coord

        int currentLocationInFlow;
        public int k = 3;

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


        private void ProcessFULL()
        {
            K = K - 1 + 1;
            _oproblem.labels = new double[_iproblem.labels.Length];
            _oproblem.samples = new SortedDictionary<int, double>[_iproblem.samples.Length];

            _oproblem = _iproblem;
            return;
        }

        private void ProcessForSingleTest()
        {

            /// THIS IS NOT NEEDED IN VOXELIZE, HOWEVER IT IS NEEDED IN GLM BECAUSE K = LAST K VECTORS. IN VOXELIZE K = NUMBER OF DIMENTIONAL REDUCTION
            /*if (Preferences.Instance.currentUDPVector < Preferences.Instance.KInPreviousVectors[currentLocationInFlow])
                return;*/

            //always process the last one.

            //1 Array resize _oproblem
            //2 voxelize the last vector from _iproblem
            //3 add To oproblem 


            /// VOXELIZE DOESNT WORK YET, HOW EVER WE MUST COPY THE INPUT TO OUTPUT SO THAT THE CHAIN OF BLOCKS ARE ALWAYS FEEDING ONE ANOTHER.
            /// THIS CODE IS IMPORTANT AND CAN BE INJECTED INTO OTHER PROCESSING BLOCKS AT THE SAME LOCATION.
            /// REUSE IT LATER WHEN DOING NEW BLOCKS WITHOUT ANY NEW CODE IN THEM.
            if (_iproblem.labels != null)
            { 
                _oproblem.labels = _iproblem.labels;
                _oproblem.samples = _iproblem.samples; 
            }
            return;
        }


        public void clearInput()
        {
            _iproblem = null; 
        }

        public void clearOutput()
        { 
            _oproblem = null;
        }

        public void postProcess() {}
    }
}
