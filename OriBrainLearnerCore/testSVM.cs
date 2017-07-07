using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using JLib.SVM;
using libSVMWrapper; 

// NOTE: THIS LIBSVM version ONLY PREDICTS. INSTEAD USE SVMWRAPPER.
namespace OriBrainLearnerCore
{
    class testSVM
    {
        public static void testSVMWrapperPackage() //rafal parsal
        {
            libSVM_Problem Problem = libSVM_Problem.Load("../../../LibSVMFull/testSvmWrapper/data/train.dat");
            GuiPreferences.Instance.setLog("trainnig data loaded"); 
            //GuiPreferences.Instance.setLog("trainnig data loaded"; 
            Problem.Save("../../../LibSVMFull/testSvmWrapper/data/train_saved");
            GuiPreferences.Instance.setLog("training data saved"); 
            libSVM_Parameter Parameter = new libSVM_Parameter();
            libSVM_Extension svm = new libSVM_Extension();

            Parameter.svm_type = SVM_TYPE.C_SVC;
            Parameter.kernel_type = KERNEL_TYPE.LINEAR;

            svm = new libSVM_Extension();
            svm.Train(Problem, Parameter);
            libSVM_Problem Test = libSVM_Problem.Load("../../../LibSVMFull/testSvmWrapper/data/test.dat");
            svm.GetAccuracyFromTest(Test);
            double accuracy = svm.output.accuracy;
            GuiPreferences.Instance.setLog("Predicted accuracy from testing set: " + accuracy.ToString());
            svm.Dispose();


            svm = new libSVM_Extension();            
            //svm.TrainAuto(10, Problem, Parameter, libSVM_Grid.C(), libSVM_Grid.gamma(), libSVM_Grid.p(), libSVM_Grid.nu(), libSVM_Grid.coef0(), libSVM_Grid.degree());
            libSVM_Grid grid = new libSVM_Grid();
            accuracy = svm.TrainAuto(10, Problem, Parameter, grid, null, null, null, null, null);
            GuiPreferences.Instance.setLog("Predicted accuracy from 10 cross fold validation: " + accuracy.ToString());
            svm.Save("../../../LibSVMFull/testSvmWrapper/data/model_file");
            GuiPreferences.Instance.setLog("10 cfv best model saved");            
            svm.Dispose();    
        }  

        public static void testSVMPredictOnlyPackage()
        {
            string data_folder = @"D:\My_Dropbox\UProjects\OriBrainLearner\TestData\";
            string data_file = data_folder + "svmguide1.t";
            string range_file = data_folder + "svmguide1.range";
            string svm_file = data_folder + "svmguide1.scale.model";
            GuiPreferences.Instance.setLog("THIS PACKAGE ONLY PREDICTS!");

            //Read in SVM model and feature range data
            LibSVM svm = new LibSVM(); svm.LoadModel(svm_file);
            double target_min, target_max; double[] features_min, features_max;
            LibSVM.ReadRange(range_file, out target_min, out target_max, out features_min, out features_max);

            //Read in Ground Truth data
            List<int> labels_g = new List<int>();
            List<LibSVMNode[]> samples = new List<LibSVMNode[]>();
            using (StreamReader sr = new StreamReader(data_file))
            {
                string line; LibSVMNode[] sample; double label;
                while ((line = sr.ReadLine()) != null)
                {
                    LibSVM.ToLibSVMFormat(line, out sample, out label);
                    samples.Add(sample);
                    labels_g.Add((int)label);
                }
            }

            //Convert to Array and List format
            List<double[]> samples_array = new List<double[]>();
            List<List<double>> samples_list = new List<List<double>>();
            for (int i = 0; i < samples.Count; i++)
            {
                LibSVMNode[] sample = samples[i];
                List<double> sample_list = new List<double>();
                for (int k = 0, l = 0; k < sample.Length; k++)
                {
                    int index = sample[k].index; if (index == -1) break;
                    double value = sample[k].value;
                    for (int m = l; m < index - 1; m++) sample_list.Add(double.NaN);
                    sample_list.Add(value); l = index;
                }
                samples_list.Add(sample_list);
                samples_array.Add(sample_list.ToArray());
            }

            int count; double[] probs, dec_values;

            //Scale the original data
            LibSVMNode[][] samples_scaled = new LibSVMNode[samples.Count][];
            GuiPreferences.Instance.setLog("Test scale data from SVMNode[] to SVMNode[]");
            for (int i = 0; i < samples.Count; i++)
                samples_scaled[i] = LibSVM.ScaleData(samples[i], target_min, target_max, features_min, features_max);
            GuiPreferences.Instance.setLog("Test Predict functions. "); 
            count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.Predict(samples_scaled[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = "+ ((double)count / labels_g.Count).ToString());

            GuiPreferences.Instance.setLog("Test scale data from double[] to SVMNode[]");
            for (int i = 0; i < samples_array.Count; i++)
                samples_scaled[i] = LibSVM.ScaleData(samples_array[i], target_min, target_max, features_min, features_max);
            GuiPreferences.Instance.setLog("Test Predict functions. "); 
            count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.Predict(samples_scaled[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());

            GuiPreferences.Instance.setLog("Test scale data from list<double> to SVMNode[]");
            for (int i = 0; i < samples_list.Count; i++)
                samples_scaled[i] = LibSVM.ScaleData(samples_list[i], target_min, target_max, features_min, features_max);
            GuiPreferences.Instance.setLog("Test Predict functions. ");
            count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.Predict(samples_scaled[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());

            GuiPreferences.Instance.setLog("Test scale data from double[][] to SVMNode[][]");
            samples_scaled = LibSVM.ScaleData(samples_array.ToArray(), target_min, target_max, features_min, features_max);
            GuiPreferences.Instance.setLog("Test Predict functions. "); count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.Predict(samples_scaled[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = "+ ((double)count / labels_g.Count).ToString());

            GuiPreferences.Instance.setLog("Test scale data from List<List<double>> to SVMNode[][]");
            samples_scaled = LibSVM.ScaleData(samples_list, target_min, target_max, features_min, features_max);
            GuiPreferences.Instance.setLog("Test Predict functions. "); count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.Predict(samples_scaled[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());

            //Convert scaled data to double[] and list<double> format
            List<double[]> samples_scaled_array = new List<double[]>();
            List<List<double>> samples_scaled_list = new List<List<double>>();
            for (int i = 0; i < samples_scaled.Length; i++)
            {
                LibSVMNode[] sample = samples_scaled[i];
                List<double> sample_list = new List<double>();
                for (int k = 0, l = 0; k < sample.Length; k++)
                {
                    int index = sample[k].index; if (index == -1) break;
                    double value = sample[k].value;
                    for (int m = l; m < index - 1; m++) sample_list.Add(double.NaN);
                    sample_list.Add(value); l = index;
                }
                samples_scaled_list.Add(sample_list);
                samples_scaled_array.Add(sample_list.ToArray());
            }

            GuiPreferences.Instance.setLog("Test Predict(SVMNode[]) functions. "); count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.Predict(samples_scaled[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

            GuiPreferences.Instance.setLog("Test Predict(List<double>) functions. "); count = 0;
            for (int i = 0; i < samples_scaled_list.Count; i++)
            {
                double label = svm.Predict(samples_scaled_list[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());

            GuiPreferences.Instance.setLog("Test Predict(double[]) functions. "); count = 0;
            for (int i = 0; i < samples_scaled_array.Count; i++)
            {
                double label = svm.Predict(samples_scaled_array[i]);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());

            GuiPreferences.Instance.setLog("Test PredictProb(SVMNode[]) functions. "); count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.PredictProb(samples_scaled[i], out probs);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

            GuiPreferences.Instance.setLog("Test PredictProb(List<double>) functions. "); count = 0;
            for (int i = 0; i < samples_scaled_list.Count; i++)
            {
                double label = svm.PredictProb(samples_scaled_list[i], out probs);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

            GuiPreferences.Instance.setLog("Test PredictProb(double[]) functions. "); count = 0;
            for (int i = 0; i < samples_scaled_array.Count; i++)
            {
                double label = svm.PredictProb(samples_scaled_array[i], out probs);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

            GuiPreferences.Instance.setLog("Test PredictValues(SVMNode[]) functions. "); count = 0;
            for (int i = 0; i < samples_scaled.GetLength(0); i++)
            {
                double label = svm.PredictValues(samples_scaled[i], out dec_values);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

            GuiPreferences.Instance.setLog("Test PredictValues(List<double>) functions. "); count = 0;
            for (int i = 0; i < samples_scaled_list.Count; i++)
            {
                double label = svm.PredictValues(samples_scaled_list[i], out dec_values);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

            GuiPreferences.Instance.setLog("Test PredictValues(double[]) functions. "); count = 0;
            for (int i = 0; i < samples_scaled_array.Count; i++)
            {
                double label = svm.PredictValues(samples_scaled_array[i], out dec_values);
                if ((int)label == labels_g[i]) count++;
            }
            GuiPreferences.Instance.setLog("Accuracy = " + ((double)count / labels_g.Count).ToString());
            

        }
    }
}
