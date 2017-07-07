using System;
using System.Linq; 
using System.Collections.Generic;
using System.IO; 
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using libSVMWrapper;  
using MathWorks.MATLAB.NET.Arrays;
using LibSVMScale;
using weka.classifiers;
using weka.core; 
using weka.classifiers.functions;
using weka.core.converters;
using weka.filters;
using System.Xml.Serialization;

namespace OriBrainLearnerCore
{
    public class TrainingTesting_SharedVariables
    {
        //NOTE: before separating to different classes, these variables were temporarilty shared through the entire process. to remove them i need to check if they need to exist in Realtimeprocessing, offlineprocessing, offlineimport, offlinetrain.
        // in if they still need to be shared, or made independent?
        public static RawReader binary = new RawReader();

        //the training problem variable shared between process() and train()
        public static libSVM_ExtendedProblem _trialProblem;
        public static List<libSVM_ExtendedProblem> _trialProblemMulti = new List<libSVM_ExtendedProblem>();
        public static Instances _trialWekaData;
        public static int[] _trainTopIGFeatures;
        public static weka.classifiers.functions.SMO smo;
        public static libSVM_ExtendedProblem _trainingProblem;
        public static LibSVM_TrainTest training;
        public static double[][] topIGFeatures = { };
        public static double time;

        private static weka.classifiers.Evaluation testingEval;
        //public static Normalize_MinMax _svmscaleTraining = new Normalize_MinMax();
        public static NormalizationInterface _svmscaleTraining = new Normalize_MinMax();
        public static NormalizationInterface _svmscaleTesting;
    }
}
