using System;
using System.Collections.Generic;
using LibSVMScale;

namespace OriBrainLearnerCore
{
    public class NormalizationManager
    {
        //NOTE: at the moment koppel said to keep using normal min/max normalization in the TRAINING stage.
        public static NormalizationInterface svmscale;
        
        //used in the TRAINING STAGE
        public static void ScaleTrFiles(string CsharpDirectory)
        {
            for (decimal i = GuiPreferences.Instance.NudExtractFromTR; i <= GuiPreferences.Instance.NudExtractToTR; i++)
            {
                string commandLine = "-T " + i.ToString() + " " + //custom option only for the moving window which needs to know in which TR we are at in order to access the eventPerTR mapping.
                                     "-l 0 " +
                                     "-s " + CsharpDirectory + "TrainSet_" + i.ToString() + "th_vectors_scale_paramCS.libsvm " +
                                     "-o " + CsharpDirectory + "TrainSet_" + i.ToString() + "th_vectors_scaledCS.libsvm " +
                                     CsharpDirectory + "TrainSet_" + i.ToString() + "th_vectors.libsvm";

                string[] commandArray = commandLine.Split(' ');
                
                //TODO need to know which line in the 40 sample libsvm file belongs to which line in the original libsvm, so that we can know which sample is #1 and calculate 50 samples back. put it in the config.
                svmscale.runModularFunctions(commandArray,
                    Preferences.Instance.MinMax[Preferences.Instance.currentProcessedRun][Convert.ToInt32(i)].feature_max,
                    Preferences.Instance.MinMax[Preferences.Instance.currentProcessedRun][Convert.ToInt32(i)].feature_min);
                GuiPreferences.Instance.setLog("Normalized TR" + i.ToString());
            }

            //---------------------------------------------------------------------------------------------------//
            //java used to run like this
            //------------------------------------------------------------------------------------------//
            /*commandLine = "-l 0 " +
                          "-s " + CsharpDirectory + "TrainSet_4th_vectors_scale_paramCS.libsvm " +
                          "-o " + CsharpDirectory + "TrainSet_4th_vectors_scaledCS.libsvm " +
                          CsharpDirectory + "TrainSet_4th_vectors.libsvm";

            commandArray = commandLine.Split(' ');
            svmscale = new svm_scale_java();
            svmscale.runModularFunctions(commandArray);*/
        }
    }

    /*public static void calculate2ndLowestPareanthesis(double[] feature_min, double[] feature_max)
        {
            int totalRuns = Preferences.Instance.dirList.Count;
            int totalFeatures = Preferences.Instance.dim_x*Preferences.Instance.dim_y*Preferences.Instance.dim_z;
            double[] finalFeature_medianMax = new double[feature_max[0].Length];
            double[] finalFeature_medianMin = new double[feature_max[0].Length];
            var tempValues_medianMax = new List<double>(totalRuns);
            var tempValues_medianMin = new List<double>(totalRuns);
            for (int k = 0; k < totalRuns; k++)
            {
                tempValues_medianMax.Add(0);
                tempValues_medianMin.Add(0);
            }
            int lowRangeMinus = 0;
            int highRangeMinus = 0;
            //calculate median ranges. from baseline.
            for (int j = 1; j < feature_max[0].Length - 1; j++)
            {
                for (int k = 0; k < totalRuns; k++)
                {
                    //min = test baseline median - 2nd smallest (training baseline median - training min)
                    //max = test baseline median + 2nd smallest (training baseline median - training max)
                    tempValues_medianMax[k] = feature_max[k][j] - Preferences.Instance.Medians[k].median[j - 1];
                    tempValues_medianMin[k] = Preferences.Instance.Medians[k].median[j - 1] - feature_min[k][j];
                }

                //from the feature-based median group get the 2nd lowest.
                tempValues_medianMax.Sort();
                tempValues_medianMin.Sort();
                if (tempValues_medianMax[1] <= 0)
                {
                    highRangeMinus++;
                }
                if (tempValues_medianMin[1] <= 0)
                {
                    lowRangeMinus++;
                }

                finalFeature_medianMax[j] = tempValues_medianMax[1];
                finalFeature_medianMin[j] = tempValues_medianMin[1];
            }
            //////////////////////////////////////////////////////////
            //for verification save ranges (this file it not to be used!)

            Normalize_MinMax.saveConfigMinMax_CSharp(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scale_MedianRangeFromBaseline.params.txt", finalFeature_medianMin, finalFeature_medianMax, 204801, 0.0f, 1.0f);
            Preferences.Instance.medianRange = new MinMax(finalFeature_medianMin, finalFeature_medianMax);
            XMLSerializer.serializeArrayToFile<MinMax>(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_MedianRangeFromBaseline.xml", Preferences.Instance.medianRange);
            GuiPreferences.Instance.setLog("low range <= 0: " + lowRangeMinus.ToString() + "&& high range <= 0: " + highRangeMinus.ToString());
        }*/
}
