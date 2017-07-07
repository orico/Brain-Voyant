using System;
using System.Collections.Generic;
using System.Text;
using libSVMWrapper;

namespace OriBrainLearnerCore
{
    public class ProcessOnsetAveraging : IDataProcessor
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
            _oproblem.labels = new double[_iproblem.labels.Length];
            _oproblem.samples = new SortedDictionary<int,double>[_iproblem.samples.Length];
            for (int i = 0; i < _iproblem.samples.Length; i++)
            {
                SortedDictionary<int,double> sd = new SortedDictionary<int,double>();
                foreach (KeyValuePair<int,double> pair in _iproblem.samples[i])
                {
                    //1st is the current TR,
                    double voxelSum = pair.Value;
                    double voxelCount = 1.0;
                    // 2-8 are the onset group
                    //if we want -1 to 8 we need to add -1 to the int k preonset and voxelsum and count == 0
                    for (int k = GuiPreferences.Instance.PreOnset; k <= GuiPreferences.Instance.PostOnset - 1; k++)
                    {
                        if ((i + k) < _iproblem.samples.Length) //overflow protection
                        {
                            if (_iproblem.samples[i + k].ContainsKey(pair.Key))
                            {
                                voxelSum += _iproblem.samples[i + k][pair.Key];
                                voxelCount += 1;
                            }
                        }
                    }
                    //if (voxelCount > 0)
                    sd.Add(pair.Key, voxelSum / voxelCount);
                    //else //NaN
                    //    sd.Add(pair.Key, 0.0);
                }
                _oproblem.samples[i] = new SortedDictionary<int, double>(sd);
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

        public void postProcess() {}
    }
}
