using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using libSVMWrapper;
using weka.classifiers.functions;
using weka.core;

namespace OriBrainLearnerCore
{
    public class Training_SingleRunTraining
    { 

        /// <summary>
        /// when train button pushed, we add jobs, pre, process, post them.
        /// </summary>
        public static void Train()
        {
            GC.Collect();
            if (TrainingTesting_SharedVariables._trialProblem != null)
                if (TrainingTesting_SharedVariables._trialProblem.samples != null)
                {
                    //temprarily the weka pipeline is the first thing we want to do, as it doesnt need all the C# wrapper functions and configuration
                    if (GuiPreferences.Instance.TrainType == TrainingType.Weka)
                    {
                        if (GuiPreferences.Instance.CbSVMChecked)
                        {
                            GuiPreferences.Instance.setLog("SVM C# Wrapper Training..");
                            SVMWrapperTrainingPipeline();
                        }
                        else if (GuiPreferences.Instance.CbSMOChecked)
                        {
                            GuiPreferences.Instance.setLog("SMO Training..");
                            WekaTrainingPipeline(TrainingTesting_SharedVariables._trialWekaData);
                        }

                        return;
                    }

                    GuiPreferences.Instance.setLog("TODO: if any models were loaded we ignore then as they would be replaced by training again.\n" +
                    "actually this should only be for rfe/grid/90-10, as cfv doesnt provide a model.");
                    Preferences.Instance.modelLoaded = false;

                    TrainingTesting_SharedVariables.training = new LibSVM_TrainTest();
                    TrainingTesting_SharedVariables.training.setSvmType();

                    GuiPreferences.Instance.setLog("NOTE: we shouldnt balance labels count. it should be balanced in the protocol");

                    //if not multiclass, keep only the two binary classes
                    if (!GuiPreferences.Instance.CbMultiClassChecked)
                    {
                        TrainingTesting_SharedVariables._trainingProblem = Training_MultiRunProcessing.getBinaryClases(TrainingTesting_SharedVariables._trialProblem);
                        GuiPreferences.Instance.setLog("Using only Binary Classes");
                    }
                    else
                        TrainingTesting_SharedVariables._trainingProblem = TrainingTesting_SharedVariables._trialProblem;

                    // show some statistics regarding the labels
                    Training_MultiRunProcessing.labelStats(TrainingTesting_SharedVariables._trainingProblem);

                    if (GuiPreferences.Instance.TrainType == TrainingType.TrainTestSplit)
                    {
                        GuiPreferences.Instance.setLog("Starting Training, splitting folds 90%/10%.");
                        libSVM_ExtendedProblem train_problem = new libSVM_ExtendedProblem();
                        libSVM_ExtendedProblem test_problem = new libSVM_ExtendedProblem();
                        int[] test_indices = ChronologicalSplitTrainTest(TrainingTesting_SharedVariables._trainingProblem, ref train_problem, ref test_problem);
                        TrainingTesting_SharedVariables.training.TrainTestSplit(train_problem, test_problem, test_indices);
                        GuiPreferences.Instance.setLog("Finished Training.");
                    }
                    else if (GuiPreferences.Instance.TrainType == TrainingType.CrossValidation)
                    {
                        GuiPreferences.Instance.setLog("Starting Cross Validation, using " + GuiPreferences.Instance.NudCVFolds.ToString() + " folds.");
                        TrainingTesting_SharedVariables.training.TrainFolds((int)GuiPreferences.Instance.NudCVFolds, TrainingTesting_SharedVariables._trainingProblem, LibSVM_TrainTest.trainingType.cfv);
                        GuiPreferences.Instance.setLog("Finished Cross Validation.");
                    }
                    else if (GuiPreferences.Instance.TrainType == TrainingType.GridSearch)
                    {
                        GuiPreferences.Instance.setLog("Starting Grid Search, using " + GuiPreferences.Instance.NudGridFolds.ToString() + " folds.");
                        TrainingTesting_SharedVariables.training.TrainFolds((int)GuiPreferences.Instance.NudGridFolds, TrainingTesting_SharedVariables._trainingProblem, LibSVM_TrainTest.trainingType.bestTotal);
                        GuiPreferences.Instance.setLog("Finished Grid Search.");
                    }
                    else if (GuiPreferences.Instance.TrainType == TrainingType.RFE)
                    {
                        GuiPreferences.Instance.setLog("Not Implemented yet.");
                    }

                    ///copy model into main model variable
                    if (GuiPreferences.Instance.TrainType != TrainingType.CrossValidation)
                    {
                        //training.
                        Preferences.Instance.svmModel = (libSVM)Preferences.Instance.svmWrapper;
                        GuiPreferences.Instance.setLog("Model saved into memory");
                    }

                }
                else GuiPreferences.Instance.setLog("Training Failed: Original(unprocessed) or Final Problem(processed) Samples empty!");
            else GuiPreferences.Instance.setLog("Training Failed: Original(unprocessed) or Final Problem(processed) null!");
        }


        private static void WekaTrainingPipeline(Instances data)
        {
            //grab SMO, config
            TrainingTesting_SharedVariables.smo = new SMO();
            TrainingTesting_SharedVariables.smo.setOptions(weka.core.Utils.splitOptions(" -C 1.0 -L 0.001 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.PolyKernel -C 250007 -E 1.0\""));
            GuiPreferences.Instance.setLog("SMO Assigned.");

            //train
            TrainingTesting_SharedVariables.smo.buildClassifier(data);
            GuiPreferences.Instance.setLog("Training on Data");

            //test on self should get 100%
            weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(data);
            eval.evaluateModel(TrainingTesting_SharedVariables.smo, data);
            Training_Output.printWekaResults(eval.toSummaryString("\nResults\n======\n", false));
            GuiPreferences.Instance.setLog("SMO Model Tested on Training Data, check that you get 100%.");

            //save model serialize model
            weka.core.SerializationHelper.write(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.libsvm.arff.model", TrainingTesting_SharedVariables.smo);
            GuiPreferences.Instance.setLog("SMO Model Serialized and saved.");

            //load model deserialize model
            TrainingTesting_SharedVariables.smo = (weka.classifiers.functions.SMO)weka.core.SerializationHelper.read(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.libsvm.arff.model");
            GuiPreferences.Instance.setLog("SMO Model DeSerialized and loaded.");

            //test loaded model 
            eval = new weka.classifiers.Evaluation(data);
            eval.evaluateModel(TrainingTesting_SharedVariables.smo, data);
            Training_Output.printWekaResults(eval.toSummaryString("\nResults\n======\n", false));
            GuiPreferences.Instance.setLog("SMO Model Tested on data (sanity check for loaded model).");

            //display top IG on dicom view            
            if (Preferences.Instance.attsel == null)
            {
                GuiPreferences.Instance.setLog("there are no ranked IG attributes or selected attr, continuing but please fix this possible bug.");
            }

            GuiPreferences.Instance.setLog("Dicom Viewer Displaying..");
            string dicomDir = GuiPreferences.Instance.WorkDirectory;
            dicomDir = dicomDir.Substring(0, dicomDir.Length - 4) + @"master\";
            string[] files = System.IO.Directory.GetFiles(dicomDir, "*.dcm");
            string firstFile = files[0].Substring(files[0].LastIndexOf(@"\") + 1);


            bool thresholdOrVoxelAmount;
            if (GuiPreferences.Instance.IgSelectionType == IGType.Threshold)
            {
                thresholdOrVoxelAmount = true;
            }
            else
            {
                thresholdOrVoxelAmount = false;
            }  
            Form plotForm = new DicomImageViewer.MainForm(dicomDir + firstFile, firstFile,
                                                          Preferences.Instance.attsel.rankedAttributes(), 
                                                          Convert.ToDouble(GuiPreferences.Instance.NudIGThreshold), 
                                                          Convert.ToInt32(GuiPreferences.Instance.NudIGVoxelAmount),
                                                          thresholdOrVoxelAmount);// _trainTopIGFeatures);
            plotForm.StartPosition = FormStartPosition.CenterParent;
            plotForm.ShowDialog();
            plotForm.Close();
            GuiPreferences.Instance.setLog("Dicom Viewer Closed.");
        }


        private static void SVMWrapperTrainingPipeline()
        {
            Preferences.Instance.modelLoaded = false;

            TrainingTesting_SharedVariables.training = new LibSVM_TrainTest();
            TrainingTesting_SharedVariables.training.setSvmType();

            GuiPreferences.Instance.setLog("NOTE: we shouldnt balance labels count. it should be balanced in the protocol");
            GuiPreferences.Instance.setLog("Default Training MultiClass on all data");

            TrainingTesting_SharedVariables._trainingProblem = TrainingTesting_SharedVariables._trialProblem;

            // show some statistics regarding the labels
            Training_MultiRunProcessing.labelStats(TrainingTesting_SharedVariables._trainingProblem);

            //train
            GuiPreferences.Instance.setLog("Training on Data");
            TrainingTesting_SharedVariables.training.TrainOnAllData(TrainingTesting_SharedVariables._trainingProblem);
            GuiPreferences.Instance.setLog("Finished Training.");


            //test on self should get 100%
            TrainingTesting_SharedVariables.training.TestOnAllData(TrainingTesting_SharedVariables._trainingProblem);
            GuiPreferences.Instance.setLog("SVM Model Tested on Training Data, check that you get 100%.");

            TrainingTesting_SharedVariables.training.saveModel(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.libsvm.model");
            GuiPreferences.Instance.setLog("SVM Model saved.");

            TrainingTesting_SharedVariables.training.loadModel(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.libsvm.model");
            GuiPreferences.Instance.setLog("SVM Model loaded.");

            //test loaded model - DOES IT REALY USE THE LOADED MODEL? THIS IS UNVERIFIED, DONT RELY UNTIL TESTED.
            TrainingTesting_SharedVariables.training.TestOnAllData(TrainingTesting_SharedVariables._trainingProblem);
            GuiPreferences.Instance.setLog("SVM Model Tested on data (sanity check for loaded model).");

            //display top IG on dicom view                  
            if (Preferences.Instance.attsel == null)
            {
                GuiPreferences.Instance.setLog("there are no ranked IG attributes or selected attr, continuing but please fix this possible bug.");
            }
            GuiPreferences.Instance.setLog("Dicom Viewer Displaying..");
            string dicomDir = GuiPreferences.Instance.WorkDirectory;
            dicomDir = dicomDir.Substring(0, dicomDir.Length - 4) + @"master\";
            string[] files = System.IO.Directory.GetFiles(dicomDir, "*.dcm");
            string firstFile = files[0].Substring(files[0].LastIndexOf(@"\") + 1);


            bool thresholdOrVoxelAmount;
            if (GuiPreferences.Instance.IgSelectionType == IGType.Threshold)
            {
                thresholdOrVoxelAmount = true;
            }
            else
            {
                thresholdOrVoxelAmount = false;
            } 
            Form plotForm = new DicomImageViewer.MainForm(dicomDir + firstFile, firstFile,
                                                          Preferences.Instance.attsel.rankedAttributes(), 
                                                          Convert.ToDouble(GuiPreferences.Instance.NudIGThreshold),
                                                          Convert.ToInt32(GuiPreferences.Instance.NudIGVoxelAmount),
                                                          thresholdOrVoxelAmount);// _trainTopIGFeatures);
            plotForm.StartPosition = FormStartPosition.CenterParent;
            plotForm.ShowDialog();
            plotForm.Close();
            GuiPreferences.Instance.setLog("Dicom Viewer Closed.");
        }


        /// <summary>
        /// divides teh training and testing set according: every 10th is assigned to the testing set, the rest are training.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="train_problem"></param>
        /// <param name="test_problem"></param>
        /// <returns></returns>
        private static int[] PrecentileSplitTrainTest(libSVM_ExtendedProblem problem, ref libSVM_ExtendedProblem train_problem, ref libSVM_ExtendedProblem test_problem)
        {
            GuiPreferences.Instance.setLog("Training/Testing Split is conceptually wrong, it will not evenly take 90% of the data");
            GuiPreferences.Instance.setLog("out of the training test 75% can be oen class and 25% of the rest. we need to balance this somehow.");

            train_problem.labels = new double[1];
            train_problem.samples = new SortedDictionary<int, double>[1];

            test_problem.labels = new double[1];
            test_problem.samples = new SortedDictionary<int, double>[1];
            int[] test_indices = new int[1];
            int precentage = 10;
            int j = 0;
            int k = 0;
            for (int i = 0; i < problem.samples.Length; i++)
                if (i % precentage == 0)
                {
                    Array.Resize(ref test_problem.labels, j + 1);
                    Array.Resize(ref test_problem.samples, j + 1);
                    Array.Resize(ref test_indices, j + 1);

                    test_problem.labels[j] = problem.labels[i];
                    test_problem.samples[j] = problem.samples[i];
                    test_indices[j] = i;
                    j++;
                }
                else
                {
                    Array.Resize(ref train_problem.labels, k + 1);
                    Array.Resize(ref train_problem.samples, k + 1);

                    train_problem.labels[k] = problem.labels[i];
                    train_problem.samples[k] = problem.samples[i];
                    k++;
                }
            return test_indices;
        }


        public static int[] ChronologicalSplitTrainTest(libSVM_ExtendedProblem problem, ref libSVM_ExtendedProblem train_problem, ref libSVM_ExtendedProblem test_problem)
        {
            GuiPreferences.Instance.setLog("Chronological Training/Testing Split");

            train_problem.labels = new double[1];
            train_problem.samples = new SortedDictionary<int, double>[1];

            test_problem.labels = new double[1];
            test_problem.samples = new SortedDictionary<int, double>[1];
            int[] test_indices = new int[1];

            int k = 0;
            int trainSize = (int)Math.Floor(problem.samples.Length * GuiPreferences.Instance.NudTrainTestSplit);

            for (int i = 0; i < trainSize; i++)
            {
                Array.Resize(ref train_problem.labels, k + 1);
                Array.Resize(ref train_problem.samples, k + 1);

                train_problem.labels[k] = problem.labels[i];
                train_problem.samples[k] = problem.samples[i];
                k++;
            }

            int j = 0;
            for (int i = trainSize + 10; i < problem.samples.Length; i++)
            {
                Array.Resize(ref test_problem.labels, j + 1);
                Array.Resize(ref test_problem.samples, j + 1);
                Array.Resize(ref test_indices, j + 1);

                test_problem.labels[j] = problem.labels[i];
                test_problem.samples[j] = problem.samples[i];
                test_indices[j] = i;
                j++;
            }

            return test_indices;
        }

    }
}
