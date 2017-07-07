using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using OriBrainLearnerCore;

namespace libSVMWrapper
{
    public class libSVM_ExtendedProblem : libSVM_Problem
    {
        /// <summary>
        /// current sample when reading from memory
        /// </summary>
        public int currentSample = 0;

        /// <summary>
        /// the last index in a sample vector (last feature's index) - is calculated when loading from memory in AddSample
        /// </summary>
        public int lastIndex = 0;


        /// <summary>
        /// Save problem to a file in CSV format
        /// </summary>
        /// <param name="_filename"></param>
        public void SaveCSV(string _filename)
        {
            if (labels == null || labels.Length == 0) throw new Exception("no labels");
            if (samples == null || samples.Length == 0) throw new Exception("no samples");
            if (labels.Length != samples.Length) throw new Exception("labels.length != samples.length");

            StreamWriter sw = new StreamWriter(File.Create(_filename));

            for (int i = 0; i < labels.Length; i++)
            {
                if (samples[i] == null) throw new Exception("samples[ " + i + " ] = null");

                for (int j = 1; j <= lastIndex; j++)
                {
                    if (samples[i].ContainsKey(j))
                    {
                        sw.Write(samples[i][j].ToString(CultureInfo.InvariantCulture) + ",");
                    }
                    else
                    {
                        sw.Write("0,");
                    }
                }
                sw.Write(labels[i].ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(" ");
            }
            sw.Close();
        }


        /// <summary>
        /// PrepareSamplesLabelsSize prepares before hand the size of the dictionary and labels
        /// </summary>
        /// <param name="nr_samples">how many samples & labels to be entered into dictionary</param>
        public void PrepareSamplesLabelsSize(int nr_samples)
        {
            samples = null;
            labels = null;
            currentSample = 0;
            labels = new double[nr_samples];
            samples = new SortedDictionary<int, double>[nr_samples];
            // if we want to resize use this
            //Array.Resize(ref samples, nr_samples);
            //Array.Resize(ref labels, nr_samples);
        }

        public int GetLastIndex()
        {
            //UpdateLastIndex();
            return lastIndex;
        }
        //only meant to be used when loading from svm file and we dont know what is the last index possible...
        public void UpdateMaximumIndex()
        {
            lastIndex = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                if (samples[i] != null)
                {
                    foreach (KeyValuePair<int, double> s in samples[i])
                    {
                        if (s.Key > lastIndex)
                            lastIndex = s.Key;
                    }
                }
            }

        }
        /// <summary>
        /// AddSample to Problem from memory into libSVM samples data structure
        /// </summary>
        /// <param name="_filename">problem file</param>
        /// <param name="cachedLength">size of buffer (cached)</param> 
        /// <param name="_label">label for the new sample</param> 
        public int AddSample<T>(T[] buffer, int _cachedLength, int _label, T threshold)
        {
            //make sure we always have the last index possible, for when we want to write a full matrix in saveCSV
            if (_cachedLength > lastIndex) lastIndex = _cachedLength;

            if (_cachedLength == 0) throw new Exception("no values in sample");

            SortedDictionary<int, double> sample = new SortedDictionary<int, double>();

            int _threshold = Convert.ToInt32(threshold);
            int nonfiltered = 0;
            double averageBeforeFilter = 0;
            double averageAfterFilter = 0;

            int relativeTR = Preferences.Instance.events.findConditionsRelativeTrBasedOnTr(currentSample + 1);
            if (relativeTR != -1) //&& !Preferences.Instance.RealTimeTestingSingleVector))
            {
                
                //add bogus class range (ignored later on) //training and testing
                Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_max[
                    Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_max.Length - 1] = 999;
                Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min[
                    Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min.Length - 1] = 111;

                //training
                if (currentSample + 1 == Preferences.Instance.events.eventList[1].var1 && !Preferences.Instance.RealTimeTestingSingleVector)
                { 
                    //remove two noisy cells, calculate baseline median, clean memory.
                    Preferences.Instance.TrainingBaselineMedians[Preferences.Instance.currentProcessedRun].removeFirst2BaselineCells();
                    Preferences.Instance.TrainingBaselineMedians[Preferences.Instance.currentProcessedRun].createMedian();
                    Preferences.Instance.TrainingBaselineMedians[Preferences.Instance.currentProcessedRun].createMin();
                    
                    //fake add median figures for the class, this is needed because libsvm holds class as a feature and we need to save this to the normalized libsvm file.
                    Preferences.Instance.TrainingBaselineMedians[Preferences.Instance.currentProcessedRun].addBogusMedianFigureForClass();    
                }

            }

            for (int i = 1; i <= _cachedLength; i++)
            {
                int index = i;
                int value = Convert.ToInt32(buffer[i - 1]);
                if (index < 0) throw new Exception("index:value -> index<0");
                //keren 64000 (10 slices.
                //dona 83200 (13 slices.
                //if (Math.Abs(value) > _threshold && i > Convert.ToInt32(GuiPreferences.Instance.NudFilterEyeSlices * 80 *80))
                averageBeforeFilter += value;

                //save baseline median IN Training
                if (!Preferences.Instance.RealTimeTestingSingleVector)
                {
                    if (currentSample + 1 <= Preferences.Instance.events.eventList[0].var2)
                    {
                        //could be optimized to push a full array array, i.e., convert buffer to double array.
                        Preferences.Instance.TrainingBaselineMedians[Preferences.Instance.currentProcessedRun].updateMatrix(currentSample + 1, i - 1, value);
                    }
                    
                    //add every vector/feature to the raw window calculation, at the end of the run we calculate the median per voxel and delete the RAM. 
                    Preferences.Instance.TrainingBaselineMediansRunningWindow[Preferences.Instance.TrainingBaselineMediansRunningWindow.Count - 1].updateMatrix(currentSample + 1, i - 1, value);
                }

                ///////////////////////////////////////////////////////////MIN MAX//////////////////////////////////////////////////////////////////////////
                //saving min/max values per tr is done in training and testing
                if (relativeTR != -1)// && !Preferences.Instance.RealTimeTestingSingleVector)
                {
                    if (currentSample + 1 >= Preferences.Instance.events.eventList[1].var1 && currentSample + 1 <= Preferences.Instance.events.eventList[2].var2)
                    {
                        //if training (can play with init min baseline or just values)
                        if (!Preferences.Instance.RealTimeTestingSingleVector)
                        {
                            //[run][relativeTR][voxel]
                            Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min[i] =
                                //assign minimum baseline values as the initial values of the minimum, in order to fit the minimum values more closely,
                                //and to get rid of all the negative values - results with dona are worse up to 4 classificaitons less
                                //Preferences.Instance.Medians[Preferences.Instance.Medians.Count - 1].min[i - 1];

                                //or init the actual value like the max below, results in many negatives
                                value;
                        }
                        //if testing you cant play..
                        else if (Preferences.Instance.RealTimeTestingSingleVector)
                        {
                            Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min[i] = value;
                        }

                        Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_max[i] = value;
                    }

                    //init values at the first event for all trs after the baseline
                    /*if (currentSample + 1 >= Preferences.Instance.events.eventList[1].var1 && currentSample + 1 <= Preferences.Instance.events.eventList[2].var2)
                    {
                        Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_max[i] = value;
                        Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min[i] = value;
                        
                    }*/
                    else if (currentSample + 1 >= Preferences.Instance.events.eventList[3].var1)// update min/max from 2nd event etc..
                    {
                        if (value > Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_max[i])
                        {
                            Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_max[i] = value;
                        }
                        else if (value < Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min[i])
                        {
                            Preferences.Instance.MinMax[Preferences.Instance.MinMax.Count - 1][relativeTR].feature_min[i] = value;
                        }
                    }
                }

                //in training we filter based on eyes and threshold
                //in testing we dont filter, the filter used is only IG!
                if  (!Preferences.Instance.RealTimeTestingSingleVector && Math.Abs(value) > _threshold && !Preferences.Instance.EyeVoxels.ContainsKey(i) ) // in training add based on threshold and eyes                    
                {
                    sample.Add(index, (double)value);
                    averageAfterFilter += value;
                    nonfiltered++;
                }
                else if (Preferences.Instance.RealTimeTestingSingleVector) //in test we only need IG filtering!
                {
                    //samples is 1-based while _trainTopIGFeatures is 0-based, we need to convert the 0-based index to a 1-based index by adding 1.
                    if (TrainingTesting_SharedVariables._trainTopIGFeatures.Contains(index-1))
                    { 
                        //add every vector/IG feature to the raw window calculation, at the end of the run we calculate the median per voxel and delete the RAM. 
                        Preferences.Instance.TestingBaselineMediansRunningWindow.updateMatrix(currentSample + 1, i - 1, value);
 
                        sample.Add(index, (double)value);
                        averageAfterFilter += value;
                        nonfiltered++;
                    }
                }
            }

            averageBeforeFilter = averageBeforeFilter / _cachedLength;
            averageAfterFilter  = averageAfterFilter  / nonfiltered;
            /*
            //if sample is empty
            if (sample.Count == 0)
            {
                nr_samples--; //it will be ++ at the begining of the loop
                continue;
            }*/

            //corrupted vector
            //this test will prevent corrupted vectors from being used, we send a hard coded "Rest" instead of this classification.
            //output brain's average raw value
            if (Preferences.Instance.RealTimeTestingSingleVector)
            {
                GuiPreferences.Instance.setLog("Averaged Voxels raw value (before threshold + eyes filtering): " +
                                               Convert.ToString(averageBeforeFilter));
                GuiPreferences.Instance.setLog("Averaged Voxels raw value (after threshold + eyes filtering): " +
                                               Convert.ToString(averageAfterFilter));
                Preferences.Instance.BrainMean.Add( averageBeforeFilter.ToString() + "," +  averageAfterFilter.ToString() );
            }

            if (averageBeforeFilter > Preferences.Instance.validMaxBrainAverage)
            {
                samples[currentSample] = new SortedDictionary<int, double>(); //empty
                labels[currentSample] = _label; 
                currentSample++;
                Preferences.Instance.corruptedVector = true;
                GuiPreferences.Instance.setLog("A (corrupted) vector with an average of " + averageBeforeFilter.ToString() + " which bigger than " + Preferences.Instance.validMaxBrainAverage.ToString() + " was detected. ignored. classification sent as hard coded 'Rest'.");                
                return 0; 
            }

            labels[currentSample] = _label; 
            samples[currentSample] = sample;
            currentSample++;
            Preferences.Instance.corruptedVector = false;
            return nonfiltered;
        }

        public double getBrainAverage(SortedDictionary<int, double> sample)
        {
            ///////////////////////////////////// average ///////////////////////////////////////// 
            double Average =0 ;
            foreach (var pair in sample)
            { 
                
                Average += pair.Value;
            }
            Average = Average / sample.Count;
            GuiPreferences.Instance.setLog("Averaged Voxels raw value (after threshold + eyes filtering): " + Convert.ToString(Average));

            ///////////////////////////////////// histogram ///////////////////////////////////////
            SortedDictionary<double, int> histogram = new SortedDictionary<double, int>();  
            foreach (var pair in sample)
            {
                if (histogram.ContainsKey(pair.Value))
                {
                    histogram[pair.Value]++;
                }
                else
                {
                    histogram[pair.Value] = 1;
                }
            }

            return Average;
            /*foreach (KeyValuePair<short, int> pair in histogram)
            {
                Console.WriteLine("{0} occurred {1} times", pair.Key, pair.Value);
            }
            MemoryStream ms = new MemoryStream(buffer);
            Image i = Image.FromStream(ms);
            i.Save("theimage" + current.ToString());
            */
        }
        ///// <summary>
        ///// AddSample to Problem from memory into libSVM samples data structure
        ///// </summary>
        ///// <param name="_filename">problem file</param>
        ///// <param name="cachedLength">size of buffer (cached)</param> 
        ///// <param name="_label">label for the new sample</param> 
        //public void AddSample(ushort[] buffer, int _cachedLength, int _label, short _threshold)
        //{
        //    //make sure we always have the last index possible, for when we want to write a full matrix in saveCSV
        //    if (_cachedLength > lastIndex) lastIndex = _cachedLength;

        //    if (_cachedLength == 0) throw new Exception("no values in sample");

        //    SortedDictionary<int, double> sample = new SortedDictionary<int, double>();

        //    for (int i = 1; i <= _cachedLength; i++)
        //    {
        //        int index = i;
        //        int value = buffer[i - 1];

        //        if (index < 0) throw new Exception("index:value -> index<0");
        //        if (Math.Abs(value) > _threshold)
        //            sample.Add(index, value);
        //    }

        //    /*
        //    //if sample is empty
        //    if (sample.Count == 0)
        //    {
        //        nr_samples--; //it will be ++ at the begining of the loop
        //        continue;
        //    }*/

        //    labels[currentSample] = _label;
        //    samples[currentSample] = sample;
        //    currentSample++;
        //}

        //public void AddSample(double[] buffer, int _cachedLength, int _label, double _threshold)
        //{
        //    //make sure we always have the last index possible, for when we want to write a full matrix in saveCSV
        //    if (_cachedLength > lastIndex) lastIndex = _cachedLength;

        //    if (_cachedLength == 0) throw new Exception("no values in sample");

        //    SortedDictionary<int, double> sample = new SortedDictionary<int, double>();

        //    for (int i = 1; i <= _cachedLength; i++)
        //    {
        //        int index = i;
        //        double value = buffer[i - 1];

        //        if (index < 0) throw new Exception("index:value -> index<0");
        //        if (Math.Abs(value) > _threshold)
        //            sample.Add(index, value);
        //    }

        //    /*
        //    //if sample is empty
        //    if (sample.Count == 0)
        //    {
        //        nr_samples--; //it will be ++ at the begining of the loop
        //        continue;
        //    }*/

        //    labels[currentSample] = _label;
        //    samples[currentSample] = sample;
        //    currentSample++;
        //}

        //public void AddSample(float[] buffer, int _cachedLength, int _label, float _threshold)
        //{
        //    //make sure we always have the last index possible, for when we want to write a full matrix in saveCSV
        //    if (_cachedLength > lastIndex) lastIndex = _cachedLength;

        //    if (_cachedLength == 0) throw new Exception("no values in sample");

        //    SortedDictionary<int, double> sample = new SortedDictionary<int, double>();

        //    for (int i = 1; i <= _cachedLength; i++)
        //    {
        //        int index = i;
        //        double value = buffer[i - 1];

        //        if (index < 0) throw new Exception("index:value -> index<0");
        //        if (Math.Abs(value) > _threshold)
        //            sample.Add(index, value);
        //    }

        //    /*
        //    //if sample is empty
        //    if (sample.Count == 0)
        //    {
        //        nr_samples--; //it will be ++ at the begining of the loop
        //        continue;
        //    }*/

        //    labels[currentSample] = _label;
        //    samples[currentSample] = sample;
        //    currentSample++;
        //}
    }
}
