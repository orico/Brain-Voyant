using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibSVMScale;
using libSVMWrapper;
using weka.core; 

namespace OriBrainLearnerCore
{
    /// <summary>
    /// In the training Stage we have 1.Import 2.process and 3.Train, this is stage 2.
    /// </summary>
    public class Training_MultiRunProcessing
    {

        public static void ProcessMultiRuns(List<string> dirList)
        {
            TrainingTesting_SharedVariables._trialProblem = null;

            if (Preferences.Instance.ProblemOriginalMulti == null)
            {
                GuiPreferences.Instance.setLog("Processing Failed: Problem null!");
                return;
            }

            if (Preferences.Instance.ProblemOriginalMulti.Count <= 0)
            {
                GuiPreferences.Instance.setLog("Processing Failed: Problem List is Empty");
                return;
            }

            for (int i = 0; i < Preferences.Instance.ProblemOriginalMulti.Count; i++)
            {
                if (Preferences.Instance.ProblemOriginalMulti[i].samples == null)
                {
                    GuiPreferences.Instance.setLog("Processing problem " + i.ToString() + " Failed: samples empty!");
                    return;
                }
            }

            Preferences.Instance.dirList = dirList;

            //we only clear all memory in offline analysis, and once in the first realtime processing vector - which is done externally in testdll
            if (!Preferences.Instance.RealTimeTestingSingleVector)
            {
                GuiPreferences.Instance.setLog("Starting Processing.");
                clearProblem();
                clearSVM();
                JobManager.clearJob();
                GC.Collect();
            }

            //if unprocessed is UNchecked then process all jobs.
            if (!GuiPreferences.Instance.UnprocessedChecked)
            {
                //NOTE: this is commented out because this is not recoded for multi dirs yet. and not needed in the current pipeline.
                /*
                //we only need to add jobs at the beginning and do it once. 
                if ((!Preferences.Instance.RealTimeTestingSingleVector) || (Preferences.Instance.RealTimeTestingSingleVector && Preferences.Instance.ProblemOriginal.samples[1] == null))
                {
                    addFeatureJobs();
                    calculatePreviousKValues();
                }

                //pre for all jobs, future support only
                preProcessFeatureJobs();
                //here the pre/proc/post is done for every job.
                processFeatureJobs();
                //post for all jobs, future support only
                postProcessFeatureJobs();


                //TODO: do this each time we get out of processing, or do this just when "Testing" is activated". seems harmless to do it all the time.
                _trialProblem = Preferences.Instance.ProblemFinal;

                //we only need to assign to the final problem when we are in offline mode
                //in real time we only need to send the classification result and sample weight.
                if (!Preferences.Instance.RealTimeTestingSingleVector)
                {
                    GuiPreferences.Instance.setLog("Processing Finished!");
                }
                else
                {
                    if (Preferences.Instance.currentUDPVector >= Preferences.Instance.maxKvectorsToWaitBeforeClassification)
                    {
                        Preferences.Instance.currentClassifiedVector += 1;
                        //allow to predict only if sample is not labeled as baseline - this happens in the beginning of a experiment session
                        if (Preferences.Instance.ProblemFinal.labels[Preferences.Instance.currentClassifiedVector - 1] != 1.0)
                        {
                            //allow to predict
                            double pred = 1.0;
                            GuiPreferences.Instance.setLog("Single Vector Processing Finished..");
                            pred = Preferences.Instance.svmWrapper.GetAccuracyFromTestSingleSample(Preferences.Instance.ProblemFinal, Preferences.Instance.svmModel);
                            transmitPrediction(pred);
                        }
                    }
                }*/
            }
            else //process without PROCESSING JOBS, the original problem
            {
                //real-time testing
                if (Preferences.Instance.RealTimeTestingSingleVector)
                {

                    //NOTE: minmax for testing loading moved to form1.cs before pipe async creation.
                    //NOTE: minmax for testing loading moved to form1.cs before pipe async creation.
                    //NOTE: minmax for testing loading moved to form1.cs before pipe async creation.
                    //NOTE: minmax for testing loading moved to form1.cs before pipe async creation.
                    //NOTE: minmax for testing loading moved to form1.cs before pipe async creation.
                    //NOTE: minmax for testing loading moved to form1.cs before pipe async creation.
                    //if you need it here please put it back.

                    if (GuiPreferences.Instance.CbSMOChecked) //real time testing using WEKA AND SMO
                    {
                        RealTimeProcessing.ExecuteRealtimeTest(TrainingTesting_SharedVariables.smo);

                        Preferences.Instance.currentClassifiedVector += 1;

                        //test on self should get 100%
                        //testingEval = new weka.classifiers.Evaluation(data);
                        //testingEval.evaluateModel(smo, data);
                        //printWekaResults(testingEval.toSummaryString("\nResults\n======\n", false));
                        //GuiPreferences.Instance.setLog("RealTime SMO Model Testing on current vector");
                        //pred = (double)testingEval.predictions().elementAt(0);
                        //transmitPrediction(pred);

                        //GuiPreferences.Instance.setLog("Single Vector Processing Finished..");
                    }
                    else if (GuiPreferences.Instance.CbSVMChecked) //weka + SVM, pipeline unfinished.
                    {
                        //NOTE: commented out because this code is not yet finished to support weka and SVM (not SMO)
                        /*
                        //if (Preferences.Instance.currentUDPVector >= Preferences.Instance.maxKvectorsToWaitBeforeClassification)
                        {
                            Preferences.Instance.currentClassifiedVector += 1;
                            //allow to predict only if sample is not labeled as baseline - this happens in the beginning of a experiment session
                            //if (Preferences.Instance.ProblemFinal.labels[Preferences.Instance.currentClassifiedVector - 1] != 1.0)
                            {
                                //allow to predict
                                double pred = 1.0;
                                pred = Preferences.Instance.svmWrapper.GetAccuracyFromTestSingleSample(Preferences.Instance.ProblemFinal, Preferences.Instance.svmModel);
                                transmitPrediction(pred);
                                GuiPreferences.Instance.setLog("Single Vector Processing Finished..");
                            }
                        }*/
                    }
                }
                else // TRAINING W/ WEKA + SMO
                {
                    //go over all available and loaded RUNS and run the weka pipeline on them
                    for (int i = 0; i < Preferences.Instance.ProblemOriginalMulti.Count; i++)
                    {
                        TrainingTesting_SharedVariables._trialProblem = Preferences.Instance.ProblemOriginalMulti[i];
                        Preferences.Instance.currentProcessedRun = i; //0-based index (run 0 is the first run)
                        GuiPreferences.Instance.WorkDirectory = dirList[i];
                        TrainingTesting_SharedVariables.topIGFeatures = new double[][] { };
                        Instances data;
                        bool createFiles = false;

                        //if overwrite then delete all non relevant files in the directory (arff/libsvm)
                        if (GuiPreferences.Instance.CbOverwriteProcessedFilesChecked)
                        {
                            foreach (string fileName in Preferences.Instance.deleteFiles)
                            {
                                FileDirectoryOperations.DeleteFile(GuiPreferences.Instance.WorkDirectory + fileName);
                            }
                        }


                        //if overwrite we create new files (after the above deletion)
                        if (GuiPreferences.Instance.CbOverwriteProcessedFilesChecked)
                        {
                            createFiles = true;
                        }
                        else // we check to see if the files are there and only if no we create.
                        {
                            // if files are not here its safe to create them from scratch, if not then we pass.
                            if (!(File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scale_paramCS.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scaledCS.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scaledCS.libsvm.arff") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_4th_vectors.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_4th_vectors_scale_paramCS.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_4th_vectors_scaledCS.libsvm") &&
                                File.Exists(GuiPreferences.Instance.WorkDirectory + "TrainSet_4th_vectors_scaledCS.libsvm.arff")
                                ))
                            {
                                createFiles = true;
                            }

                        }

                        if (createFiles)
                        {
                            //files are loaded,thresholded,vectorized (arff),normalized. false means that IG and training are not done here.
                            if (!ProcessSingleRunOffline(ref TrainingTesting_SharedVariables.topIGFeatures, TrainingTesting_SharedVariables._trialProblem))
                            {
                                GuiPreferences.Instance.setLog("Samples are empty");
                            }
                            Preferences.Instance.TrainingBaselineMediansRunningWindow[i].clearMatrixMemory();
                        }
                        else
                        {
                            GuiPreferences.Instance.setLog("Skipped Processing for directory: " +
                                                           GuiPreferences.Instance.WorkDirectory);
                        }
                    }

                    //Concatenate the data
                    WekaProcessingPipelineForMultiRuns(dirList);


                    if (GuiPreferences.Instance.CbSVMChecked)
                    {
                        Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
                        //here we need to load the filtered DS
                        //Preferences.Instance.svmWrapper.

                        //GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scaledCS_filteredIG.libsvm";
                    }
                    TrainingTesting_SharedVariables._trialProblemMulti = Preferences.Instance.ProblemOriginalMulti;
                    GuiPreferences.Instance.setLog("Finished Processing.");

                    Sound.beep(Sound.beepType.Asterisk);
                    //System.Media.SystemSounds.Beep.Play();
                }
            }

        }


        /// <summary>
        /// determine if we have samples and can proceed with the processing of the samples 
        /// </summary>
        public static bool SamplesExist(libSVM_ExtendedProblem problem)
        {
            if (problem.samples == null)
            {
                GuiPreferences.Instance.setLog("Export Failed: Problem has no samples!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// concatenates 3TR 4TR etc according to what is assigned in the gui.
        /// </summary>
        /// <param name="directoryList"></param>
        public static void ConcatenateLibsvmVectorizedPerTR(List<string> directoryList)
        {
            for (decimal i = GuiPreferences.Instance.NudExtractFromTR; i <= GuiPreferences.Instance.NudExtractToTR; i++)
            {
                FileStream fileStream;
                FileStream outputFileStream =
                    new FileStream(
                        GuiPreferences.Instance.WorkDirectory + "TrainSet_" + i.ToString() + "th_vectors_scaledCS.libsvm", FileMode.CreateNew, FileAccess.Write);
                foreach (string directory in directoryList)
                {
                    fileStream = new FileStream(directory + "TrainSet_" + i.ToString() + "th_vectors_scaledCS.libsvm", FileMode.Open, FileAccess.Read);
                    CopyStream(outputFileStream, fileStream);
                    fileStream.Close();
                }
                outputFileStream.Close();

                //save concatenated tr to a file
                if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + i.ToString() + "th_vectors_scaledCS.libsvm", 204800))
                    GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_" + i.ToString() + "th_vectors_scaledCS.arff");
            }
        }

        /// <summary>
        /// copying files to finalData, processing, creating config files/minmax/etc and converting to arff.
        /// </summary>
        /// <param name="_trialProblem"></param>
        /// <returns></returns>
        public static void WekaProcessingPipelineForMultiRuns(List<string> directoryList)
        {
            //create a dir that holds the final DS in C:\
            //GuiPreferences.Instance.WorkDirectory = @"C:\FinalData_" + DateTime.Now.ToLongTimeString().Replace(':', '-');
            GuiPreferences.Instance.WorkDirectory = @"C:\FinalData_" +
                                                    "TR" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "_" +
                                                    GuiPreferences.Instance.NormalizedType.ToString() +
                                                    GuiPreferences.Instance.NudMovingWindow.ToString() + "_";

            if (GuiPreferences.Instance.IgSelectionType == IGType.Threshold)
                GuiPreferences.Instance.WorkDirectory = GuiPreferences.Instance.WorkDirectory + "IG_Thr" + GuiPreferences.Instance.NudIGThreshold.ToString() + "_";
            else if (GuiPreferences.Instance.IgSelectionType == IGType.Voxels)
                GuiPreferences.Instance.WorkDirectory = GuiPreferences.Instance.WorkDirectory + "IG_Vox" + GuiPreferences.Instance.NudIGVoxelAmount.ToString() + "_";

                                                    //GuiPreferences.Instance.ProtocolFile + "_" +
            GuiPreferences.Instance.WorkDirectory = GuiPreferences.Instance.WorkDirectory + Preferences.Instance.events.EventListLastTr.ToString();

            if (GuiPreferences.Instance.CbPeekHigherTRsIGChecked == true)
                GuiPreferences.Instance.WorkDirectory = GuiPreferences.Instance.WorkDirectory + "_Peeking";
            GuiPreferences.Instance.setLog(@"Creating Final Directory in: " + GuiPreferences.Instance.WorkDirectory);
            FileDirectoryOperations.CreateDirectory(GuiPreferences.Instance.WorkDirectory);
            GuiPreferences.Instance.WorkDirectory += @"\";

            ConcatenateLibsvmVectorizedPerTR(directoryList);

            //NOTE: min/max values are taken from the param files of each run. which means that you get N max values and N min values.
            //if a median is needed, we have to go over all columns for each feature and concat all values to a huge list that contains all N files and only then do the median.
            //the median code here is not conceptually not right. as we take median out of 4 max values or min values. its a bad way to calculate a median.
            //median code should be done in the normalization class and the code here should reflect the concept behind it.

            //NOTE2: this code goes over N max and min values from each param file and get the MAX(maxes) and MIN(mins).
            //these max and mins are saved to be used as the initial min/max values for the testing stage.
            double[][] feature_max = new double[directoryList.Count][];
            double[][] feature_min = new double[directoryList.Count][];
            int i = 0;
            int max_index = -1;
            foreach (string directory in directoryList)
            {
                TrainingTesting_SharedVariables._svmscaleTraining.getConfigFileMinMaxValues(
                    //use previous tr min/maxes for median consideration
                    //directory + "TrainSet_" + (GuiPreferences.Instance.NudClassifyUsingTR - 1).ToString() + "th_vectors_scale_paramCS.libsvm",

                    //use current tr for min/max median
                    directory + "TrainSet_" + (GuiPreferences.Instance.NudClassifyUsingTR).ToString() + "th_vectors_scale_paramCS.libsvm", 

                    ref feature_max[i], ref feature_min[i], ref max_index);
                i++;
            }


            //calculate Mean + save new min/max param to C:\
            double[] finalFeature_max = new double[feature_max[0].Length];
            double[] finalFeature_min = new double[feature_max[0].Length];
            double[] finalFeature_medianMax = new double[feature_max[0].Length];
            double[] finalFeature_medianMin = new double[feature_max[0].Length];

            //create a TEMP list with enough values for the runs, in order to calculate the MIN/MAX median
            var values_max = new List<double>(feature_max.Length);
            var values_min = new List<double>(feature_max.Length);
            var values_medianMax = new List<double>(feature_max.Length);
            var values_medianMin = new List<double>(feature_max.Length);
            for (int k = 0; k < feature_max.Length; k++)
            {
                //init zeros
                values_max.Add(0);
                values_min.Add(0);
                values_medianMax.Add(0);
                values_medianMin.Add(0);
            }

            for (int j = 0; j < feature_max[0].Length; j++)
            {
                for (int k = 0; k < feature_max.Length; k++)
                {
                    // for each feature group all run-based values together
                    values_max[k] = feature_max[k][j];
                    values_min[k] = feature_min[k][j];
                }

                //get median of maxes/mins optional here
                /*finalFeature_max[j] = GetMedian(values_max);
                finalFeature_min[j] = GetMedian(values_min); */

                //get Max and Min here.
                finalFeature_max[j] = values_max.Max();
                finalFeature_min[j] = values_min.Min();
            }
            //save max/median param file
            TrainingTesting_SharedVariables._svmscaleTraining.saveConfigMinMax_CSharp(
                GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scale_paramCS.libsvm", 
                finalFeature_min, finalFeature_max, 204801, 0.0f, 1.0f);

            int lowRangeMinus = 0;
            int highRangeMinus = 0;
            //calculate median ranges. from baseline.
            for (int j = 1; j < feature_max[0].Length - 1; j++)
            {
                for (int k = 0; k < feature_max.Length; k++)
                {
                    //min = test baseline median - 2nd smallest (training baseline median - training min)
                    //max = test baseline median + 2nd smallest ( training max - training baseline median)
                    values_medianMax[k] = feature_max[k][j] - Preferences.Instance.TrainingBaselineMedians[k].median[j - 1];
                    values_medianMin[k] = Preferences.Instance.TrainingBaselineMedians[k].median[j - 1] - feature_min[k][j];
                }

                double chosenLowRange;
                double chosenHighRange;

                chosenLowRange  = getSecondLowest(values_medianMin);
                chosenHighRange = getSecondLowest(values_medianMax);

                //chosenLowRange = getSecondHighest(values_medianMin);
                //chosenHighRange = getSecondHighest(values_medianMax);
                
                //chosenLowRange  = StatisticsFeatures.GetMedian(values_medianMin);
                //chosenHighRange = StatisticsFeatures.GetMedian(values_medianMax);
                

                finalFeature_medianMin[j] = chosenLowRange;
                finalFeature_medianMax[j] = chosenHighRange;
                 
                if (chosenLowRange <= 0)
                {
                    lowRangeMinus++;
                }

                if (chosenHighRange <= 0)
                {
                    highRangeMinus++;
                }
            }
            //////////////////////////////////////////////////////////
            //for verification save ranges (this file it not to be used!)
            
            TrainingTesting_SharedVariables._svmscaleTraining.saveConfigMinMax_CSharp(
                GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + 
                "th_vectors_scale_MedianRangeFromBaseline.params.txt", finalFeature_medianMin, finalFeature_medianMax, 204801, 0.0f, 1.0f);

            Preferences.Instance.medianRange = new MinMax(finalFeature_medianMin,finalFeature_medianMax);

            XMLSerializer.serializeArrayToFile<MinMax>(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + 
                GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_MedianRangeFromBaseline.xml", Preferences.Instance.medianRange);

            GuiPreferences.Instance.setLog("out of 204K features, low range <= 0: " + lowRangeMinus.ToString() + " && high range <= 0: " + highRangeMinus.ToString());
        }

        public static double getSecondHighest(List<double> array)
        {
            if (array.Count > 1)
            {
                array.Sort();
                array.Reverse();
                return array[1];
            }
            return array[0];
        }

        public static double getSecondLowest(List<double> array)
        {   
            if (array.Count > 1)
            {
                array.Sort();
                return array[1];
            }
            return array[0];
        }


        //the final normalized and concatenated file doesnt need to be vectorized + normalized again.
        // if first line "instances data = " is 4 and the rest of the lines are 3, then the trick is used.
        // if all lines are 3 then only tr3 is used
        // if all lines except the saveArff is tr4 then it will use tr4 data and save to a filename with "3" in it, at the moment this is done for compatibility when loading this file in real-time 
        public static Instances ConcatenationPipeLine(string filenameTR3, string filenameTR4)
        {
            //filter tr3 based on top 1000 from tr4 (the trick) 
            //load TR4 !!! NOTE: trick changed from tr4 to 3 because i didnt see any increase in % in real time.. + this line can be removed to speed up things.
            Instances data;
            if (GuiPreferences.Instance.CbPeekHigherTRsIGChecked)
            {
                GuiPreferences.Instance.setLog("using final dataset: " + filenameTR4);
                data = WekaTrainingMethods.loadDataSetFile(filenameTR4); // peeking at a higher TR's IG values (trick)
            }
            else
            {
                GuiPreferences.Instance.setLog("using final dataset: " + filenameTR3);
                data = WekaTrainingMethods.loadDataSetFile(filenameTR3); // no peeking (no trick)
            }


            //select 1000 IG values, serialize to file
            WekaTrainingMethods.selectIGSerialize(ref data);

            //load tr3
            data = WekaTrainingMethods.loadDataSetFile(filenameTR3);



            //filter top IG
            data = WekaTrainingMethods.useRemoveFilter(data, Preferences.Instance.attsel.selectedAttributes(), true);

            //save filtered tr3 to a file
            WekaCommonFileOperation.SaveArff(data, GuiPreferences.Instance.WorkDirectory + filenameTR3 + "_filteredIG.arff");
            WekaCommonFileOperation.SaveCSV(data, GuiPreferences.Instance.WorkDirectory + filenameTR3 + "_filteredIG_CSV.arff");

            return data;
        }

        /// <summary>
        /// saves ProblemOriginal to a file, separates into tr3 and tr4, normalizes, and converts the libsvm files into arff.
        /// </summary>
        public static void VectorizeAndNormalize(libSVM_ExtendedProblem problem)
        {
            string trainFileName = GuiPreferences.Instance.WorkDirectory /*+ GuiPreferences.Instance.FileName*/ + "TrainSet";

            //todo add proper named to saved files, check if null is logical at all.
            //if ((Preferences.Instance.ProblemOriginal.samples != null))
            //{
            problem.Save(trainFileName + ".libsvm", 80 * 80 * 32 + 1);
            GuiPreferences.Instance.setLog("saved Original Problem LibSVM file: " + trainFileName + ".libsvm");
            //}

            //separate DS to 3rd and 4th TR
            ////example: ExecuteSelectKthVectorScript(@"TrainSet", @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\4\rtp\");
            KthExtractionManager.ExecuteSelectKthVectorScript(/*GuiPreferences.Instance.FileName +*/ "TrainSet", GuiPreferences.Instance.WorkDirectory);
            //GuiPreferences.Instance.setLog("Created TR3, TR4, TR5, TR6 files (5+6 depends if added to the python script)");

            //NORMALIZING all TR Files
            NormalizationManager.ScaleTrFiles(GuiPreferences.Instance.WorkDirectory);

            //CONVERTING all TR files
            WekaCommonFileOperation.ConvertToArff(GuiPreferences.Instance.WorkDirectory);
        }

        //although SingleRun uses all the functions here, but this class is still for MULTICLASS
        public static bool ProcessSingleRunOffline(ref double[][] topIGFeatures, libSVM_ExtendedProblem problem)
        {

            // --- from this point the preprocessing of a single run begins --- //

            //export to libsvm file
            if (!SamplesExist(problem)) return false;

            VectorizeAndNormalize(problem);

            return true;

            /*
            //filter tr3 based on top 1000 from tr4 (the trick) 
            //load TR4 !!! NOTE: trick changed from tr4 to 3 because i didnt see any increase in % in real time.. + this line can be removed to speed up things.
            Instances data = ThePipeline.loadDataSetFile("TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff");

            //select 1000 IG values, serialize to file
            ThePipeline.selectIGSerialize(ref topIGFeatures, ref data);

            //load tr3
            data = ThePipeline.loadDataSetFile("TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff");


            //filter top IG
            data = WekaCommonMethods.useRemoveFilter(data, topIGFeatures, true);

            // NOTE:!! use this as a replacement for the two lines above, in case its not the same files as the previous
            // ConverterUtils.DataSource source = new ConverterUtils.DataSource(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff");
            //data = source.getDataSet();

            //filter top IG
            //data = WekaCommonMethods.useRemoveFilter(data, topIGFeatures, true);

            //after filtering last feature needs to be the class
            //if (data.classIndex() == -1)
            //    data.setClassIndex(data.numAttributes() - 1);

            //save filtered tr3 to a file
            WekaCommonFileOperation.SaveArff(data, GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG");

            // --- from this point the training phaze begins --- //
            ThePipeline.trainSMO(data);

            return true;*/
        }


        public static libSVM_ExtendedProblem getBinaryClases(libSVM_ExtendedProblem trainingProblem)
        {
            libSVM_ExtendedProblem _trainingProblem = new libSVM_ExtendedProblem();
            _trainingProblem.labels = new double[1];
            _trainingProblem.samples = new SortedDictionary<int, double>[1];
            int j = 0;
            for (int i = 0; i < trainingProblem.labels.Length; i++)
            {
                if ((trainingProblem.labels[i] == GuiPreferences.Instance.CmbClass1Selected + 1) || (trainingProblem.labels[i] == GuiPreferences.Instance.CmbClass2Selected + 1))
                {
                    Array.Resize(ref _trainingProblem.labels, j + 1);
                    Array.Resize(ref _trainingProblem.samples, j + 1);

                    _trainingProblem.labels[j] = trainingProblem.labels[i];
                    _trainingProblem.samples[j] = trainingProblem.samples[i];

                    j++;
                }

            }
            return _trainingProblem;

        }


        public static void labelStats(libSVM_ExtendedProblem trainingProblem)
        {
            int[] stats = new int[Preferences.Instance.numberOfConditions];
            for (int i = 0; i < trainingProblem.labels.Length; i++)
            {
                stats[(int)trainingProblem.labels[i] - 1]++;
            }

            string line = "";
            for (int i = 0; i < stats.Length; i++)
            {
                line += GuiPreferences.Instance.getLabel(i) + " : " + stats[i].ToString() + ", ";
            }

            GuiPreferences.Instance.setLog(line);

        }


        public static void setClassesLabels()
        {
            /*GuiPreferences.cmbClass1.Items.Clear();
            //cmbClass2.Items.Clear();
            //for (int i = 0; i < GuiPreferences.Instance.Labels.Count; i++)
            {
             //   GuiPreferences.Instance.addLabel(label[i]);
                //GuiPreferences.Instance.labels[i]);
            }            
            GuiPreferences.Instance.CmbClass1Selected = 1;
            GuiPreferences.Instance.CmbClass2Selected = 2;
             * */
        }


        public static void CopyStream(Stream destination, Stream source)
        {
            int count;
            byte[] buffer = new byte[1000000];
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, count);
        }


        public static void clearProblem()
        {
            //Preferences.Instance.ProblemOriginal = new libSVM_ProblemCustom();
            //clear the final problem output when starting a new training set
            Preferences.Instance.ProblemFinal = null;
            Preferences.Instance.ProblemFinalMulti = new List<libSVM_ExtendedProblem>();
        }

        public static void clearSVM()
        {
            //Preferences.Instance.svm.Dispose();
            Preferences.Instance.svmWrapper = null;
        }
    }
}
