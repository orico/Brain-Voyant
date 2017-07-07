using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libSVMWrapper;
using weka.core;

namespace OriBrainLearnerCore
{
    public class Training_MultiRunTraining
    {

        /// <summary>
        /// when train button pushed, we add jobs, pre, process, post them.
        /// </summary>
        public static void TrainMultiRun(List<string> dirList)
        {
            GC.Collect();

            if (TrainingTesting_SharedVariables._trialProblemMulti == null)
            {
                GuiPreferences.Instance.setLog(
                   "Training Failed: Original(unprocessed) or Final Problem(processed) null!");
                return;
            }

            if (TrainingTesting_SharedVariables._trialProblemMulti.Count <= 0)
            {
                GuiPreferences.Instance.setLog("Training Failed: Original(unprocessed) or Final Problem(processed)list empty!");
                return;
            }

            for (int i = 0; i < TrainingTesting_SharedVariables._trialProblemMulti.Count; i++)
            {
                if (TrainingTesting_SharedVariables._trialProblemMulti[i].samples == null)
                {
                    GuiPreferences.Instance.setLog("Training problem " + i.ToString() + " Failed: samples empty!");
                    return;
                }
            }

            //WEKA HERE: temporarily the weka pipeline is the first thing we want to do, as it doesnt need all the C# wrapper functions and configuration
            if (GuiPreferences.Instance.TrainType == TrainingType.Weka)
            {
                if (GuiPreferences.Instance.CbSVMChecked)
                {
                    GuiPreferences.Instance.setLog("SVM C# Wrapper Training..");
                    //SVMWrapperTrainingPipeline();
                    return;
                }
                else if (GuiPreferences.Instance.CbSMOChecked)
                {
                    GuiPreferences.Instance.setLog("SMO Training..");
                    WekaTrainingPipelineForMultiRuns();

                    GuiPreferences.Instance.setLog("Saving Training Preferences.");
                    ConfigManager.saveConfigFile();

                    Sound.PlayTrainingFinished();

                    Training_MultiRunPlotting.WekaPlotPipelineForMultiRuns(dirList);
                }

                Logging.saveLog("Training");
                GuiPreferences.Instance.setLog("Finished Training.");
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
                int[] test_indices = Training_SingleRunTraining.ChronologicalSplitTrainTest(TrainingTesting_SharedVariables._trainingProblem, ref train_problem, ref test_problem);
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
        
        public static void WekaTrainingPipelineForMultiRuns()
        {
            //todo check max index in file, 
            //todo check if needs to remove 204801 from it so it doesnt effect the class.

            Instances finalData = Training_MultiRunProcessing.ConcatenationPipeLine("TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff",
                                                                    "TrainSet_" + (GuiPreferences.Instance.NudClassifyUsingTR + 1).ToString() + "th_vectors_scaledCS.libsvm.arff");
            WekaTrainingMethods.TrainSMO(finalData);
        }
    }
}
