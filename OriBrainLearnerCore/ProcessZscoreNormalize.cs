using System;
using System.Collections.Generic;
using System.Text;
using libSVMWrapper; 

namespace OriBrainLearnerCore
{
    public class ProcessZscoreNormalize: IDataProcessor
    {

        libSVM_ExtendedProblem _iproblem = null;
        libSVM_ExtendedProblem _oproblem = new libSVM_ExtendedProblem(); 
        int _lastIndex;

        int currentLocationInFlow;
        public int k = -1;

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

        public ProcessZscoreNormalize() { } 

        public void setProblem(libSVM_ExtendedProblem problem)
        {
            _iproblem = problem;
            _lastIndex = _iproblem.GetLastIndex();
        }

        public libSVM_ExtendedProblem getProblem()
        {
            return _oproblem;
        }

        //public double avg(KeyValuePair<int, double>);
        public void preProcess()
        {
            //dont know how to use this averate thing. some enumerator blah
            //averages[i] = _problem.samples[i].Average<double>();
        }
        
        public void Process()
        {
            //_oproblem.labels = new double[_iproblem.labels.Length];
            //_oproblem.samples = new SortedDictionary<int, double>[_iproblem.samples.Length];
            //for (int i = 0; i < _iproblem.samples.Length; i++)
            //{
            //    SortedDictionary<int, double> sd = new SortedDictionary<int, double>();
            //    //for (int j = 1; j < _lastIndex; j++)
            //    foreach (KeyValuePair<int, double> pair in _iproblem.samples[i])
            //    {
            //        if (j == 22564)
            //        {
            //            int pp = 1;
            //        }
            //        //if (_iproblem.samples[i].ContainsKey(j))
            //        {
            //            double voxelSum = 0.0;
            //            double voxelCount = 0.0;
            //            //1st is the current TR, 2-8 are the onset group
            //            for (int k = Preferences.Instance.preOnset - 1; k <= Preferences.Instance.postOnset - 1; k++)
            //            {
            //                if ((i + k) < _iproblem.samples.Length) //overflow protection
            //                {
            //                    if (_iproblem.samples[i + k].ContainsKey(j))
            //                    {
            //                        voxelSum += _iproblem.samples[i + k][j];
            //                        voxelCount += 1;
            //                    }
            //                }
            //            }
            //            if (voxelCount > 0)
            //                sd.Add(j, voxelSum / voxelCount);
            //            else //NaN
            //                sd.Add(j, 0.0);
            //        }
            //        //sd.CopyTo(_oproblem.samples[j], 0);
            //    }
            //    // for (int j = 0; j < _lastIndex; j++)
            //    //{
            //    //    if (_iproblem.samples[i].ContainsKey(j))
            //    //    { 
            //}
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