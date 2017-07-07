using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 

namespace OriBrainLearnerCore
{
    /// <summary>
    /// stores all feature values throughout the testing run in a matrix, updates in real time, creates min/max/med/avg/std, saves to csv .
    /// if isBaseline used, then only the first N Trs are accessed.
    /// usually used with selected IG features.
    /// </summary>
    public class StatisticalFeaturesDecemberIQR
    {
        public double[] mean;
        public double[] std;
        public double[] median;
        public double[] max;
        public double[] min;
        private bool isBaseline;
        private int features;
        private int vectors;
        private List<List<double>> matrix;
        private int[] trainTopIGFeatures;

        //OFFLINE running window calculation. 
        // until we have K samples, we use the First k baseline samples according to the protocol, reducing the first two).
        //this should be performed at the end of a run.
        public double[] CalcMedIQRRunningWindow(int feature, string currentTR, int currentLine)
        {
            if (feature >= matrix.Count) return new double[2]{-0.123456,-0.123456789}; //fake num for the class (204800 which is feature 204801 outside of this function)
            double[] baselineArray;

            int originalLineIndex = (Preferences.Instance.TrainingEventStartLocationsPerTR[Preferences.Instance.currentProcessedRun][currentTR][currentLine]); //in which 0-based line in the original libsvm we are at?

            if (originalLineIndex < (int)GuiPreferences.Instance.NudMovingWindow) //we calculate using first k-2 baseline until we accumulate K samples
            {
                baselineArray = new double[Preferences.Instance.events.eventList[0].var2 - 2];
                for (int i=2 ; i< Preferences.Instance.events.eventList[0].var2  ; i++) 
                {
                    baselineArray[i - 2] = matrix[feature][i]; //matrix is also 0-based
                }
            }
            else
            { 
                baselineArray = new double[(int)GuiPreferences.Instance.NudMovingWindow];
                int index = 0;
                for (int i = originalLineIndex - (int)GuiPreferences.Instance.NudMovingWindow; i < originalLineIndex; i++)
                {
                    baselineArray[index] = matrix[feature][i];  //matrix is also 0-based
                    index++;
                }
            }
            return GetMedianAndIQR(baselineArray);
        }

        public void addBogusMedianFigureForClass()
        {
            Array.Resize(ref median,median.Length + 1);
            median[median.Length - 1] = 999;
            features = features + 1;
        }
        /// <summary>
        /// returns the IG index 0-1000. used in the TEST stage.
        /// class may be used to save 1-204K (TRAINING) or 0-1000 IG values (TEST)
        /// when storing IG, use getMedian. when storing all the features, use median[]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double getMedianWhenIGIndicesSaved(int index)
        {
               for(int i=0;i<trainTopIGFeatures.Length;i++)
               {
                   if (trainTopIGFeatures[i]==index)
                   {
                       return median[i];
                   }
               }
            return -1;
        }

        /// <summary>
        /// returns the IG index 0-1000. used in the TEST stage.
        /// class may be used to save 1-204K (TRAINING) or 0-1000 IG values (TEST)
        /// when storing IG, use getMean. when storing all the features, use mean[]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double getMeanWhenIGIndicesSaved(int index)
        {
            for (int i = 0; i < trainTopIGFeatures.Length; i++)
            {
                if (trainTopIGFeatures[i] == index)
                {
                    return mean[i];
                }
            }
            return -1;
        }

        public StatisticalFeaturesDecemberIQR(int _features, int _vectors, int[] _trainTopIGFeatures, bool _isBaseline)
        {
            isBaseline = _isBaseline;
            features = _features;
            vectors = _vectors; //first N vectors to keep track of
            trainTopIGFeatures = _trainTopIGFeatures;

            matrix = new List<List<double>>();

            //save top IG per tr in statistical matrix - figuring out wether there is a pattern using std/median etc. (koppel)
            //1. init matrix            
            for (int i = 0; i < features; i++)
            {
                //add lines to the matrix
                matrix.Add(new List<double>());
            }
        }

        // init a matrix of this type, used to init and prepare statisticalMatrix and statisticalMatrixBaseline
        /// <summary>
        /// updates the matrix using the currentVector values, where currentVector should be Preferences.Instance.currentClassifiedVector 
        /// </summary>
        /// <param name="currentVector"></param>
        /// <param name="values"></param>
        public void updateMatrix(int currentVector, double[] values)
        {
            //2. convert row based vector and insert to a column based matrix (all values from every feature are inserted into a single vector)
            if (currentVector <= vectors)
            { 
                for (int i = 0; i < features; i++)
                {
                    matrix[i].Add(values[i]);
                }
            }
        }
        
        /// <summary>
        /// same as above, add a single value.
        /// </summary>
        /// <param name="currentVector"></param>
        /// <param name="featureIndex"></param>
        /// <param name="featureValue"></param>
        public void updateMatrix(int currentVector, int featureIndex, double featureValue)
        {
            //2. convert row based vector and insert to a column based matrix (all values from every feature are inserted into a single vector)
            if (currentVector <= vectors)
            {
                matrix[featureIndex].Add(featureValue);
            }
        }

        public void createAllStatistics()
        {
            //GuiPreferences.Instance.setLog("Total Vectors added to the statistical matrix: " + Preferences.Instance.statisticalMatrix[0].Count.ToString());
            GuiPreferences.Instance.setLog("Total Vectors added to the statistical matrix: " + vectors.ToString());
            GuiPreferences.Instance.setLog("Total features in each vector: " + features.ToString());
            removeFirst2BaselineCells();

            createMax();
            createMin();
            createMedian();
            createMean();
            createStd();
            /*
            for (int i = 0; i < features; i++)
            {
                if (isBaseline)
                {
                    //we need to remove the first two trs in baseline (noise)
                    matrix[i].RemoveAt(0);
                    matrix[i].RemoveAt(0);
                }
                max[i] = matrix[i].Max();
                min[i] = matrix[i].Min();
                median[i] = GetMedian(matrix[i]);
                mean[i] = matrix[i].Average();
                std[i] = GetStandartDev(matrix[i]);
            }*/
        }
        
        /// <summary>
        /// removes first two cells from the matrix for each feature.
        /// </summary>
        public void removeFirst2BaselineCells()
        {
            for (int i = 0; i < features; i++)
            {
                if (isBaseline)
                {
                    //we need to remove the first two trs in baseline (noise)
                    matrix[i].RemoveAt(0);
                    matrix[i].RemoveAt(0);
                }
            }
        }

        public void createMax()
        {
            max = new double[features];
         
            for (int i = 0; i < features; i++)
            {
                max[i] = matrix[i].Max();
            }
        }

        public void createMin()
        {
            min = new double[features];

            for (int i = 0; i < features; i++)
            {
                min[i] = matrix[i].Min();
            }
        }

        public void createMedian()
        {
            median = new double[features];
            for (int i = 0; i < features; i++)
            {
                median[i] = GetMedianSort(matrix[i]);
            }
        }

        public void createMean()
        {
            mean = new double[features];
            for (int i = 0; i < features; i++)
            {
                mean[i] = matrix[i].Average();
            }
        }

        public void createStd()
        {
            std = new double[features];
            for (int i = 0; i < features; i++)
            {
                std[i] = GetStandartDev(matrix[i]);
            }
        }


        public void clearMatrixMemory()
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                matrix[i] = null;
            }
            matrix = null;
        }

        public void saveCSV()
        {
            string description;
            //create text for csv file.
            if (isBaseline)
            {
                description = "Baseline";
            }
            else
            {
                description = "AllTrs";
            }

            string filename = GuiPreferences.Instance.WorkDirectory + "MinMaxMedMeanStd_" + description + "_" +
                              DateTime.Now.ToString("g").Replace(":", "_").Replace("/", "").Replace(" ", "_") + ".csv";
            List<string> l = new List<string>();
           
            l.Add("Feature Index,Min,Max,Median,|Median-Min|,|Median-Max|,Mean,|Mean-Min|,|Mean-Max|,Std");
            string line = "";
            for (int i = 0; i < features - 1; i++)
            {
                line = ",";
                if (trainTopIGFeatures != null)
                {
                    line = trainTopIGFeatures[i].ToString() + ",";
                }
                l.Add(line +  min[i].ToString() + "," + max[i].ToString() + "," +
                median[i].ToString() + "," + (median[i] - min[i]).ToString() + "," + (max[i] - median[i]).ToString() + "," +
                mean[i].ToString() + "," + (mean[i] - min[i]).ToString() + "," + (max[i] - mean[i]).ToString() + "," +
                std[i].ToString());
            }

            GuiPreferences.Instance.setLog("Saving " + filename);
            File.WriteAllLines(filename, l);
        }

        public void saveFullMatrixCSV(int relativeTRinEvent,string additionalDesc)
        {
            string description;
            //create text for csv file.
            if (isBaseline)
            {
                description = "Baseline";
            }
            else
            {
                description = "AllTrs";
            }

            string filename = GuiPreferences.Instance.WorkDirectory + "FullMatrix_" + additionalDesc + description + "_" +
                              DateTime.Now.ToString("g").Replace(":", "_").Replace("/", "").Replace(" ", "_") + ".csv";
            
            List<string> l = new List<string>();

            //prepare the header.
            string line = "";
            for (int i = 0; i < features - 1; i++)
            {
                //xml and topIG HERE is 1-based
                line = line + "att_" + (trainTopIGFeatures[i]).ToString();
                if (i != features - 2)
                    line = line + ",";
            }
            l.Add(line);
            for (int i = 0; i < vectors - 1; i++)
            {
                //if (Preferences.Instance.events.findConditionsRelativeTrBasedOnTr(i + 1)==relativeTRinEvent)
                {
                    line = "";
                    for (int j = 0; j < features - 1; j++)
                    {
                        line = line + matrix[j][i].ToString();
                        if (j != features - 2)
                            line = line + ",";
                    }
                    l.Add(line);
                }
            }

            GuiPreferences.Instance.setLog("Saving " + filename);
            File.WriteAllLines(filename, l);
            
        }

        public static double GetMedianSort(IEnumerable<double> source)
        {
            // Create a copy of the input, and sort the copy
            double[] temp = source.ToArray();
            Array.Sort(temp);
            return GetMedianFromArrayNoSort(temp);
        }

        private static double GetMedianFromArrayNoSort(double[] temp)
        { 
            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = temp[count / 2 - 1];
                double b = temp[count / 2];
                return (a + b) / 2f;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }

        public static double[] GetMedianAndIQR(IEnumerable<double> source)
        {
            // Create a copy of the input, and sort the copy
            double[] temp = source.ToArray();
            double[] s3, s1;
            Array.Sort(temp); 
            int count = temp.Length;
            
            double median,iqr;
            int sizeS;
            median = GetMedianFromArrayNoSort(temp);
            if (count % 2 == 0)
            {
                sizeS = (count/2) - 1;
            }
            else
            {
                sizeS = (count / 2);
            }

            s1 = new double[sizeS];
            s3 = new double[sizeS];

            Array.Copy(temp, 0, s1, 0, sizeS);
            Array.Copy(temp, (count) - sizeS, s3, 0, sizeS);
            iqr = GetMedianFromArrayNoSort(s3) - GetMedianFromArrayNoSort(s1);

            return new double[2] {median,iqr};
        }

        public static double GetStandartDev(IEnumerable<double> source)
        {
            double[] vector = source.ToArray();
            double average = vector.Average();
            double sumOfSquaresOfDifferences = vector.Select(val => (val - average) * (val - average)).Sum();
            return Math.Sqrt(sumOfSquaresOfDifferences / vector.Length);
        }
    }
}
