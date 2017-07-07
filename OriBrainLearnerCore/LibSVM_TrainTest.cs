using System;
using System.Linq; 
using System.Collections.Generic;
using OriBrainLearnerCore;

namespace libSVMWrapper
{
    public class LibSVM_TrainTest
    {
        private libSVM_Parameter Parameter = new libSVM_Parameter();
        private int Folds;
        private int folds
        {
            get { return Folds; }
            set { Folds = value; }
        }
        public void setSvmType()
        { 
            Parameter.svm_type = SVM_TYPE.C_SVC;
            Parameter.kernel_type = KERNEL_TYPE.LINEAR;
        }

        public enum trainingType{
            best,
            bestTotal,
            cfv,
        }

        public void TrainFolds(int N,libSVM_ExtendedProblem problem, trainingType type)
        {
            folds = N;
            Preferences.Instance.svmWrapper = new libSVM_Extension();
            //svm.TrainAuto(10, Problem, Parameter, libSVM_Grid.C(), libSVM_Grid.gamma(), libSVM_Grid.p(), libSVM_Grid.nu(), libSVM_Grid.coef0(), libSVM_Grid.degree());


            //used without grids. only folds
            libSVM_Grid grid = new libSVM_Grid(); 
            GuiPreferences.Instance.setLog("Training + Cross Validation Started");    
            double accuracy=-1;
            if (type==trainingType.best)
                accuracy = Preferences.Instance.svmWrapper.TrainAuto(N, problem, Parameter, grid, null, null, null, null, null);
            else if (type == trainingType.bestTotal) 
            {
                Preferences.Instance.svmWrapper.TrainAutoBestTotal(N, problem, Parameter, grid, null, null, null, null, null); 
                accuracy = Preferences.Instance.svmWrapper.output.accuracy;
            }
            else if (type == trainingType.cfv) 
            {
                Preferences.Instance.svmWrapper.CrossValidate(problem, Parameter, N);
                accuracy = Preferences.Instance.svmWrapper.output.accuracy;
            }

            

            //used with default grids + only folds
            //double accuracy = Preferences.Instance.svm.TrainAuto(N, problem, Parameter);//, grid, null, null, null, null, null);            

            /*
            //used to compare against libsvm with grids and folds. when grids have the default minimum and max = min +1 and step = 2
            libSVM_Grid gridc = libSVM_Grid.C();
            gridc.max = gridc.min + 1;
            gridc.step = 2;
            libSVM_Grid gridgamma= libSVM_Grid.gamma();
            gridgamma.max = gridgamma.min + 1;
            gridgamma.step = 2;
            libSVM_Grid gridp = libSVM_Grid.p();
            gridp.max = gridp.min + 1;
            gridp.step = 2;
            libSVM_Grid gridnu = libSVM_Grid.nu();
            gridnu.max = gridnu.min + 1;
            gridnu.step = 2;
            libSVM_Grid gridcoef = libSVM_Grid.coef0();
            gridcoef.max = gridcoef.min + 1;
            gridcoef.step = 2;
            libSVM_Grid griddegree = libSVM_Grid.degree();
            griddegree.max = griddegree.min + 1;
            griddegree.step = 2;
            double accuracy = Preferences.Instance.svm.TrainAuto(N, problem, Parameter, gridc, gridgamma, gridp, gridnu, gridcoef, griddegree);
            */

            GuiPreferences.Instance.setLog("Training + Cross Validation Finished");
            //calculate tr statistics
            if (type == trainingType.bestTotal)
            {
                int i = 1;
                foreach (int key in Preferences.Instance.svmWrapper.output.getKeys())
                {
                   GuiPreferences.Instance.setLog(i.ToString() + ". " + key.ToString() +
                    ": " + problem.labels[key].ToString() +
                    " => " + Preferences.Instance.svmWrapper.output.getValue(key).ToString());
                   i++;
                }
                GuiPreferences.Instance.setLog("Best Model Predicted accuracy from " + folds.ToString() + " cross fold validation: " + accuracy.ToString());

                printTRStatistics(problem);
            }
            else
                GuiPreferences.Instance.setLog("Predicted accuracy from " + folds.ToString() + " cross fold validation: " + accuracy.ToString());           
        }

        public void printTRStatistics(libSVM_ExtendedProblem problem)
        {
            GuiPreferences.Instance.setLog("Prediction only at TR:");
            string line = "";

            List<IntIntStr> protocolWORests = Preferences.Instance.prot.getHDREvents();
            //possibly using Preferences.Instance.maxKvectorsToWaitBeforeClassification is not the right thing and we still need to use GLM.K some how. (its not static anymore so an instance need to be passed)
            int[,] stats = new int[Preferences.Instance.maxKvectorsToWaitBeforeClassification, 2];
            //for each full event
            for (int i = 0; i < protocolWORests.Count() - 1; i++)
            {
                int j = 0;
                //if the predicted label if its the same as the original label
                foreach (int key in Preferences.Instance.svmWrapper.output.getKeys())
                {
                    if ((key <= protocolWORests[i].var2) && (key >= protocolWORests[i].var1))
                    {
                        //get TR position
                        int TR = key - protocolWORests[i].var1;
                        if (Preferences.Instance.svmWrapper.output.getValue(key) == problem.labels[key])
                        {   
                            //possibly using Preferences.Instance.maxKvectorsToWaitBeforeClassification is not the right thing and we still need to use GLM.K some how. (its not static anymore so an instance need to be passed)
                            //actual trs that have correct predictions
                            if (TR < Preferences.Instance.maxKvectorsToWaitBeforeClassification)
                                stats[TR, 1]++;
                        }
                        //total count of trs (correct and wrong)
                        stats[TR, 0]++;
                    }
                    j++;
                }
            }

            //possibly using Preferences.Instance.maxKvectorsToWaitBeforeClassification is not the right thing and we still need to use GLM.K some how. (its not static anymore so an instance need to be passed)
            //print tr statistics
            for (int i = 0; i <  Preferences.Instance.maxKvectorsToWaitBeforeClassification - 1; i++)
            {
                line += (i+1).ToString() + ": " + ((int)((double)stats[i, 1] / (double)stats[i, 0] * 100)).ToString() + " ";
            }
            GuiPreferences.Instance.setLog(line);
        }

        public void TrainTestSplit(libSVM_ExtendedProblem trainingSet, libSVM_ExtendedProblem testingSet, int[] test_indices)
        { 
            Preferences.Instance.svmWrapper = new libSVM_Extension();
            GuiPreferences.Instance.setLog("Training 90% started");          
            Preferences.Instance.svmWrapper.Train(trainingSet, Parameter);
            GuiPreferences.Instance.setLog("Training 90% finished");
            GuiPreferences.Instance.setLog("Testing 10% started");
            Preferences.Instance.svmWrapper.GetAccuracyFromTest(testingSet);
            double accuracy = Preferences.Instance.svmWrapper.output.accuracy;
            GuiPreferences.Instance.setLog("Testing 10% finished");   
            GuiPreferences.Instance.setLog("Train90/Test10 Split, Predicted accuracy from testing set: " + accuracy.ToString());
            for (int i = 0; i < testingSet.samples.Length; i++)
            {
                GuiPreferences.Instance.setLog((GuiPreferences.Instance.FromTR + test_indices[i]).ToString() + ": " + testingSet.labels[i].ToString() + " => " + Preferences.Instance.svmWrapper.output.getValue(i).ToString());
            }
            GuiPreferences.Instance.setLog("TODO: NEED PROGRESSIVE ANALYSIS OF TRS");
        }
        /// <summary>
        /// Training on 100% of the input data.
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="testingSet"></param>
        /// <param name="test_indices"></param>
        public void TrainOnAllData(libSVM_ExtendedProblem trainingSet)
        { 
            Preferences.Instance.svmWrapper = new libSVM_Extension();
            GuiPreferences.Instance.setLog("Training 100% started");          
            Preferences.Instance.svmWrapper.Train(trainingSet, Parameter);
            GuiPreferences.Instance.setLog("Training 100% finished");
        }

        public void TestOnAllData(libSVM_ExtendedProblem testingSet)
        {
            GuiPreferences.Instance.setLog("Testing started");
            Preferences.Instance.svmWrapper.GetAccuracyFromTest(testingSet);
            double accuracy = Preferences.Instance.svmWrapper.output.accuracy;
            GuiPreferences.Instance.setLog("Testing finished");
            GuiPreferences.Instance.setLog("Predicted accuracy from testing set: " + accuracy.ToString());
            for (int i = 0; i < testingSet.samples.Length; i++)
            {
                GuiPreferences.Instance.setLog(i.ToString() + ": " + testingSet.labels[i].ToString() + " => " + Preferences.Instance.svmWrapper.output.getValue(i).ToString());
            }
        }

        public void saveModel(string filename)
        {
            //Preferences.Instance.svm.Save("../../../LibSVMFull/testSvmWrapper/data/model_file");
            Preferences.Instance.svmWrapper.Save(filename);

            if (GuiPreferences.Instance.TrainType == TrainingType.GridSearch)
                GuiPreferences.Instance.setLog(folds.ToString() + " best model saved");
            else if (GuiPreferences.Instance.TrainType == TrainingType.TrainTestSplit)
                GuiPreferences.Instance.setLog("90/10 model saved");
            //disposeSVM();
        }

        public bool loadModel(string filename)
        {
            try
            {
                Preferences.Instance.modelLoaded = false;
                if (GuiPreferences.Instance.TrainType != TrainingType.CrossValidation)
                {
                    libSVM _svm = libSVM.Load("ori.svm");
                    if (_svm != null)
                    {
                        Preferences.Instance.modelLoaded = true;
                        GuiPreferences.Instance.setLog("new loaded model overwritten the old model if there was any");
                        Preferences.Instance.svmModel =_svm;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    GuiPreferences.Instance.setLog("cant load a model for cross validation (it has K models)");
                    return false;
                }
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog("loadModel: " +e.ToString()); 
                return false;
            }
        }

        /// <summary>
        /// this is a static duplicate of LoadModel. at some point i need to remove this duplicate..
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool loadModelStatic(string filename)
        {
            try
            {
                Preferences.Instance.modelLoaded = false;
                if (GuiPreferences.Instance.TrainType != TrainingType.CrossValidation)
                {
                    libSVM _svm = libSVM.Load("ori.svm");
                    if (_svm != null)
                    {
                        Preferences.Instance.modelLoaded = true;
                        GuiPreferences.Instance.setLog("new loaded model overwritten the old model if there was any");
                        Preferences.Instance.svmModel = _svm;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    GuiPreferences.Instance.setLog("cant load a model for cross validation (it has K models)");
                    return false;
                }
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog("loadModel: " + e.ToString());
                return false;
            }
        }

        public void disposeSVM()
        {
            //Preferences.Instance.svm.Dispose();
        }

        public void TrainSplitExample()
        {
            Preferences.Instance.svmWrapper.Train(Preferences.Instance.ProblemFinal, Parameter);
            libSVM_Problem Test = libSVM_Problem.Load("../../../LibSVMFull/testSvmWrapper/data/test.dat");
            Preferences.Instance.svmWrapper.GetAccuracyFromTest(Test);
            double accuracy = Preferences.Instance.svmWrapper.output.accuracy;
            GuiPreferences.Instance.setLog("Predicted accuracy from testing set: " + accuracy.ToString());
            //disposeSVM();
        }

        public static void testSVMWrapperPackage() //rafal parsal
        {
            libSVM_Problem Problem = libSVM_Problem.Load("../../../LibSVMFull/testSvmWrapper/data/train.dat");
            GuiPreferences.Instance.setLog("trainnig data loaded");
            Problem.Save("../../../LibSVMFull/testSvmWrapper/data/train_saved");
            GuiPreferences.Instance.setLog("training data saved");
            libSVM_Extension svm = new libSVM_Extension();            
            libSVM_Parameter Parameter = new libSVM_Parameter(); 

            Parameter.svm_type = SVM_TYPE.C_SVC;
            Parameter.kernel_type = KERNEL_TYPE.LINEAR;

            svm = new libSVM_Extension();
            svm.Train(Problem, Parameter);
            libSVM_Problem Test = libSVM_Problem.Load("../../../LibSVMFull/testSvmWrapper/data/test.dat");
            svm.GetAccuracyFromTest(Test);
            double accuracy = Preferences.Instance.svmWrapper.output.accuracy;
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
    }
}
