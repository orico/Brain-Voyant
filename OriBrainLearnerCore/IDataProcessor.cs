using System;
using System.Linq; 
using System.Collections.Generic;
using System.Text;
using libSVMWrapper;  

namespace OriBrainLearnerCore
{
    public interface IDataProcessor
    { 
        void setProblem(libSVM_ExtendedProblem problem);
        libSVM_ExtendedProblem getProblem();
        void preProcess(); 
        void Process();
        void postProcess();
        void clearInput();
        void clearOutput();
        int K { get; set; } 
        int CurrentLocationInFlow { get; set;} 
        //int getK(); 
        //void setK(int K);


        /*example of how these should look inside derived classes,. they are called from Process.
        private void ProcessFULL()
        {
            
        }

        private void ProcessForSingleTest()
        {
            if (Preferences.Instance.currentUDPVector < Preferences.Instance.KInPreviousVectors[currentLocationInFlow])
                return;
        }
        */
    }
}
