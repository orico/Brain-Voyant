using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libSVMWrapper;
using weka.core;
using weka.core.converters;
using weka.filters;

namespace OriBrainLearnerCore
{
    class Training_OLDProcessing
    {
        /// <summary>
        /// Process is the most updated code toward all the classification options, but should be used when doing offline classification as it expects the param file to be located in the test dir
        /// in the online version it should look for the Param file 
        /// </summary>
        public static void OfflineProcess()
        {
            TrainingTesting_SharedVariables._trialProblem = null;
            if (Preferences.Instance.ProblemOriginal != null)
                if (Preferences.Instance.ProblemOriginal.samples != null)
                {
                    //we only clear all memory in offline analysis, and once in the first realtime processing vector - which is done externally in testdll
                    if (!Preferences.Instance.RealTimeTestingSingleVector)
                    {
                        GuiPreferences.Instance.setLog("Starting Processing.");
                        Training_MultiRunProcessing.clearProblem();
                        Training_MultiRunProcessing.clearSVM();
                        JobManager.clearJob();
                        GC.Collect();
                    }

                    //if unprocessed is UNchecked then process all jobs.
                    if (!GuiPreferences.Instance.UnprocessedChecked)
                    {
                        //we only need to add jobs at the beginning and do it once. 
                        if ((!Preferences.Instance.RealTimeTestingSingleVector) || (Preferences.Instance.RealTimeTestingSingleVector && Preferences.Instance.ProblemOriginal.samples[1] == null))
                        {
                            JobManager.addFeatureJobs();
                            JobManager.calculatePreviousKValues();
                        }

                        //pre for all jobs, future support only
                        JobManager.preProcessFeatureJobs();
                        //here the pre/proc/post is done for every job.
                        JobManager.processFeatureJobs();
                        //post for all jobs, future support only
                        JobManager.postProcessFeatureJobs();


                        //TODO: do this each time we get out of processing, or do this just when "Testing" is activated". seems harmless to do it all the time.
                        TrainingTesting_SharedVariables._trialProblem = Preferences.Instance.ProblemFinal;

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
                                    RealTimeProcessing.transmitPrediction(pred);
                                }
                            }
                        }
                    }
                    else //process without PROCESSING JOBS, the original problem
                    {
                        //real-time testing
                        if (Preferences.Instance.RealTimeTestingSingleVector)
                        {

                            // we load the configuration file for min/max for all features - ONCE.
                            if (!TrainingTesting_SharedVariables._svmscaleTraining.ConfigFileLoaded)
                            {
                                //load normalization configuration from training stage.
                                string commandLine = "-l 0 " +
                                                     "-r " + GuiPreferences.Instance.WorkDirectory +
                                                     "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scale_paramCS.libsvm " +
                                                     GuiPreferences.Instance.WorkDirectory + "nofile";

                                TrainingTesting_SharedVariables._svmscaleTraining.initSingleVectorConfig(commandLine.Split(' '), 80 * 80 * 32);
                            }

                            if (GuiPreferences.Instance.CbSMOChecked)
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
                                //if (Preferences.Instance.currentUDPVector >= Preferences.Instance.maxKvectorsToWaitBeforeClassification)
                                {
                                    Preferences.Instance.currentClassifiedVector += 1;
                                    //allow to predict only if sample is not labeled as baseline - this happens in the beginning of a experiment session
                                    //if (Preferences.Instance.ProblemFinal.labels[Preferences.Instance.currentClassifiedVector - 1] != 1.0)
                                    {
                                        //allow to predict
                                        double pred = 1.0;
                                        pred = Preferences.Instance.svmWrapper.GetAccuracyFromTestSingleSample(Preferences.Instance.ProblemFinal, Preferences.Instance.svmModel);
                                        RealTimeProcessing.transmitPrediction(pred);
                                        GuiPreferences.Instance.setLog("Single Vector Processing Finished..");
                                    }
                                }
                            }
                        }
                        else
                        {
                            TrainingTesting_SharedVariables._trialProblem = Preferences.Instance.ProblemOriginal;

                            //process the data, save files, normalize, filter, pick upto 1000, save etc..
                            TrainingTesting_SharedVariables._trialWekaData = WekaPipeline_Unprocessed(TrainingTesting_SharedVariables._trialProblem);
                            if (TrainingTesting_SharedVariables._trialWekaData != null)
                            {
                                GuiPreferences.Instance.setLog(
                                    "Weka ProcessingFinished! (in UN-Processing code condition)");
                            }
                            else
                            {
                                GuiPreferences.Instance.setLog(
                                    "Processing Failed: Weka Processing (in UN-Processing code condition)");
                            }

                            if (GuiPreferences.Instance.CbSVMChecked)
                            {
                                Preferences.Instance.ProblemOriginal = new libSVM_ExtendedProblem();
                                //here we need to load the filtered DS
                                //Preferences.Instance.svmWrapper.

                                //GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scaledCS_filteredIG.libsvm";
                            }
                        }
                    }
                }
                else GuiPreferences.Instance.setLog("Processing Failed: samples empty!");
            else GuiPreferences.Instance.setLog("Processing Failed: Problem null!");
        }


        public static Instances WekaPipeline_Unprocessed(libSVM_ExtendedProblem _trialProblem)
        {

            //export to libsvm file
            if (_trialProblem.samples == null)
            {
                GuiPreferences.Instance.setLog("Export Failed: Problem has no samples!");
                return null;
            }

            string trainFileName = GuiPreferences.Instance.WorkDirectory /*+ GuiPreferences.Instance.FileName*/ + "TrainSet";


            //todo add proper named to saved files, check if null is logical at all.
            if ((_trialProblem.samples != null))
            {
                _trialProblem.Save(trainFileName + ".libsvm");
                GuiPreferences.Instance.setLog("saved Original Problem LibSVM file: " + trainFileName + ".libsvm");
            }

            //separate DS to 3rd and 4th TR
            ////example: ExecuteSelectKthVectorScript(@"TrainSet", @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\4\rtp\");
            KthExtractionManager.ExecuteSelectKthVectorScript(/*GuiPreferences.Instance.FileName +*/ "TrainSet", GuiPreferences.Instance.WorkDirectory);
            GuiPreferences.Instance.setLog("Created TR3 & TR4 files");

            //normalize 3rd and 4th TR files.
            NormalizationManager.ScaleTrFiles(GuiPreferences.Instance.WorkDirectory);
            GuiPreferences.Instance.setLog("Normalized TR3 & TR4 files");

            //convert tr4 and tr3 to arff + REMOVE 204801 FAKE FEATURE, THAT WAS PLACES TO MAKE SURE WE GET 204800 FEATURES IN THE ARFF FILE.
            if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scaledCS.libsvm", 204800))
                GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_3th_vectors_scaledCS.libsvm");
            if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(GuiPreferences.Instance.WorkDirectory + "TrainSet_4th_vectors_scaledCS.libsvm", 204800))
                GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_4th_vectors_scaledCS.libsvm");

            //---------------------------------- filter tr3 based on top 1000 from tr4 (the trick) -----------------------------
            //load TR4
            ConverterUtils.DataSource source = new ConverterUtils.DataSource(GuiPreferences.Instance.WorkDirectory + "TrainSet_4th_vectors_scaledCS.libsvm.arff");
            Instances data = source.getDataSet();

            //assign last as index.
            if (data.classIndex() == -1)
                data.setClassIndex(data.numAttributes() - 1);

            //if class not nominal, convert to
            if (!data.classAttribute().isNominal())
            {
                var filter = new weka.filters.unsupervised.attribute.NumericToNominal();

                filter.setOptions(weka.core.Utils.splitOptions("-R last"));
                //filter.setAttributeIndices("last");
                filter.setInputFormat(data);
                data = Filter.useFilter(data, filter);
            }

            //run ig and get top 1000 or up to 1000 bigger than zero, from tr4
            WekaTrainingMethods.useLowLevelInformationGainFeatureSelection(data);

            TrainingTesting_SharedVariables._trainTopIGFeatures = Preferences.Instance.attsel.selectedAttributes();

            //this should be done ONCE
            Preferences.Instance.fastvector = RealTimeProcessing.CreateFastVector(TrainingTesting_SharedVariables._trainTopIGFeatures.Length);
            GuiPreferences.Instance.setLog("created fast vector of length " + TrainingTesting_SharedVariables._trainTopIGFeatures.Length.ToString());

            //serialize (save) topIG indices to file.
            XMLSerializer.serializeArrayToFile(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG_indices.xml", TrainingTesting_SharedVariables._trainTopIGFeatures);
            GuiPreferences.Instance.setLog("saved IG indices to a file (in the same order as IG gave it)");
            //int [] _trainTopIGFeatures_loaded = DeserializeArrayToFile(GuiPreferences.Instance.WorkDirectory + "TrainSet_3th_vectors_scaledCS_filteredIG_indices.xml");

            GuiPreferences.Instance.setLog(TrainingTesting_SharedVariables._trainTopIGFeatures.Length.ToString() + " features above zero value selected (including the Class feature)");

            //load tr3
            source = new ConverterUtils.DataSource(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff");
            data = source.getDataSet();

            //filter top IG
            data = WekaTrainingMethods.useRemoveFilter(data, TrainingTesting_SharedVariables._trainTopIGFeatures, true);

            //after filtering last feature needs to be the class
            if (data.classIndex() == -1)
                data.setClassIndex(data.numAttributes() - 1);

            //save filtered to a file
            WekaCommonFileOperation.SaveLIBSVM(data, GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG");

            return data;
        }
    }
}
