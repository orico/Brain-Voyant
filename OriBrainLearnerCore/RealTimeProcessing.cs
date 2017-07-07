using System;
using System.Collections.Generic;
using System.IO;
using libSVMWrapper;
using weka.classifiers;
using weka.core;

namespace OriBrainLearnerCore
{
    public class RealTimeProcessing
    {
        private static double time;
        private static bool UDPListenActive = false;

        public static bool UdpWekaSMOMultiRun(string vdatDataFolder, string ModelDataFolder)
        {
            if (UDPListenActive)
            {
                TrainingTesting_SharedVariables.binary.shouldStop = true;
                GuiPreferences.Instance.setLog("Stopping UDP");
            }
            else
            {
                GuiPreferences.Instance.setLog("Starting UDP");
                TrainingTesting_SharedVariables.binary.shouldStop = false;

                /// NOTE: many initializations were moved to form1.cs to the rt server code behind the button.

                Preferences.Instance.currentUDPVector = 0;

                //UDP will process only if a valid filename,totalTRs is sent.
                Preferences.Instance.udp.RegisterCallBack(TrainingTesting_SharedVariables.binary.loadRawDataMultiRun_UDP);
                //PIPE is loaded before this function and it will process a memory buffer.

            }

            UDPListenActive = !UDPListenActive;
            return true;
        } 

        /// <summary>
        /// Process is the most updated code toward all the classification options, currently only used for a single vector online classificaiton, the difference from the offline is the ability to load the param file from the model directory
        /// </summary>
        public static void RealTimeProcess()
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
                                    transmitPrediction(pred);
                                }
                            }
                        }
                    }
                    else //process without PROCESSING JOBS, the original problem
                    {
                        //real-time testing
                        if (Preferences.Instance.RealTimeTestingSingleVector)
                        {
                            //NOTE: minmax loading moved to form1.cs before pipe async creation.

                            if (GuiPreferences.Instance.CbSMOChecked)
                            {
                                if (!Preferences.Instance.corruptedVector)
                                {
                                    ExecuteRealtimeTest(TrainingTesting_SharedVariables.smo);
                                }
                                else
                                {
                                    ExecuteRealtimeTestForCorrupted(TrainingTesting_SharedVariables.smo);
                                }

                                Preferences.Instance.currentClassifiedVector += 1;


                                //create Median (and more) after the baseline event has finished.
                                if (Preferences.Instance.currentClassifiedVector == Preferences.Instance.events.eventList[0].var2) //==10 for the first baseline
                                {
                                    Preferences.Instance.TestingBaselineStatistics.createAllStatistics();
                                    Preferences.Instance.TestingBaselineStatistics.saveCSV();
                                }

                                //create and save all the statistics the entire run has finished. must 
                                //NOTE it must be a complete run, no errors!
                                if (Preferences.Instance.currentClassifiedVector >= Preferences.Instance.events.EventListLastTr) //==10 for the first baseline
                                {
                                    Sound.PlayTestingFinished();

                                    //save full csv must happen before createallstatistics as we remove 2 baseline cells in there.
                                    //saved per feature RAW value at the 4th tr
                                    Preferences.Instance.MatrixStatistics.saveFullMatrixCSV(4,"RAW_");
                                 
                                    Preferences.Instance.MatrixStatistics.createAllStatistics();
                                    Preferences.Instance.MatrixStatistics.saveCSV();

                                    //saved per feature normalized value at the 4th tr
                                    Preferences.Instance.MatrixStatisticsNormalized.saveFullMatrixCSV(4,"NORM_");

                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    /// MOVE ME out of here: save true min and true max vs simulated min and simulated max (true minmax can only be seen at the end of the test run)
                                    string filename = GuiPreferences.Instance.WorkDirectory + "MinMaxSimulatedMinSimulatedMax_"+
                                        DateTime.Now.ToString("g").Replace(":", "_").Replace("/", "").Replace(" ", "_") + ".csv";
                                    List<string> l = new List<string>();

                                    l.Add("min,max,simulatedMin,simulatedMax");
                                    string line = "";

                                    

                                    for (int k = 0; k < TrainingTesting_SharedVariables._trainTopIGFeatures.Length; k++)
                                    {

                                        int infoGainFeature = TrainingTesting_SharedVariables._trainTopIGFeatures[k] + 1;
                                        double simulatedMin =
                                            Preferences.Instance.TestingBaselineStatistics.getMedianWhenIGIndicesSaved(infoGainFeature) - Preferences.Instance.medianRange.feature_min[infoGainFeature];
                                        double simulatedMax =
                                            Preferences.Instance.TestingBaselineStatistics.getMedianWhenIGIndicesSaved(infoGainFeature) + Preferences.Instance.medianRange.feature_max[infoGainFeature];
                                        //global minmax
                                        //line = Preferences.Instance.MatrixStatistics.min[k].ToString() + "," +
                                        //       Preferences.Instance.MatrixStatistics.max[k].ToString() + ","

                                        //tr4 or the one used minmax.
                                        line = Preferences.Instance.MinMax[0][Convert.ToInt32(GuiPreferences.Instance.NudClassifyUsingTR)].feature_min[infoGainFeature] + "," +
                                               Preferences.Instance.MinMax[0][Convert.ToInt32(GuiPreferences.Instance.NudClassifyUsingTR)].feature_max[infoGainFeature] + "," +
                                               simulatedMin.ToString() + "," + simulatedMax.ToString();
                                        l.Add(line);
                                    }
                                    GuiPreferences.Instance.setLog("Saving " + filename);
                                    File.WriteAllLines(filename, l);

                                    //global mean and after ig mean
                                    filename = GuiPreferences.Instance.WorkDirectory + "MeanBeforeIGMeanAfterIG" +
                                        DateTime.Now.ToString("g").Replace(":", "_").Replace("/", "").Replace(" ", "_") + ".csv";
                                    GuiPreferences.Instance.setLog("Saving " + filename);
                                    File.WriteAllLines(filename, Preferences.Instance.BrainMean);
                                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    try
                                    {
                                        StatisticsAccuracy.generateStats(); 
                                    }
                                    catch(Exception e)
                                    {
                                        GuiPreferences.Instance.setLog(e.Message);
                                    } 
                                    Logging.saveLog("Testing");
                                }

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
                                        transmitPrediction(pred);
                                        GuiPreferences.Instance.setLog("Single Vector Processing Finished..");
                                    }
                                }
                            }
                        }
                        else
                        {
                            TrainingTesting_SharedVariables._trialProblem = Preferences.Instance.ProblemOriginal;

                            //process the data, save files, normalize, filter, pick upto 1000, save etc..
                            TrainingTesting_SharedVariables._trialWekaData = Training_OLDProcessing.WekaPipeline_Unprocessed(TrainingTesting_SharedVariables._trialProblem);
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

                                //GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.libsvm";
                            }
                        }
                    }
                }
                else GuiPreferences.Instance.setLog("Processing Failed: samples empty!");
            else GuiPreferences.Instance.setLog("Processing Failed: Problem null!");
        }


        //code taken from here http://stackoverflow.com/questions/9616872/classification-of-instances-in-weka/14876081#14876081
        // This creates the data set's attributes vector
        public static FastVector CreateFastVector(int size)
        {
            var fv = new FastVector();
            weka.core.Attribute att;

            foreach (int key in TrainingTesting_SharedVariables._trainTopIGFeatures)
            {
                if (key != TrainingTesting_SharedVariables._trainTopIGFeatures[TrainingTesting_SharedVariables._trainTopIGFeatures.Length - 1])
                {
                    att = new weka.core.Attribute("att_" + (key + 1).ToString());
                    fv.addElement(att);
                }
            }

            {

                var classValues = new FastVector(1); //it doesnt matter if its 3 or 1, when addElement is used the fastvector grows.
                List<string> labels = GuiPreferences.Instance.getLabels();

                GuiPreferences.Instance.setLog("automatically! adding " + (labels.Count - 1).ToString() + " classes 2 -> " + (labels.Count + 1).ToString() + " to fast vector, based on protocol labels");

                //baseline is ignored, we start from the second event in the protocol
                for (int l = 1; l < labels.Count; l++)
                {
                    classValues.addElement((l + 1).ToString());
                }

                //classValues.addElement("2");
                //classValues.addElement("3");
                //classValues.addElement("4");
                //classValues.addElement("5");
                var classAttribute = new weka.core.Attribute("class", classValues);
                fv.addElement(classAttribute);
            }

            return fv;
        }


        // This is the "entry" method for the classification method
        public static void ExecuteRealtimeTest(Classifier model)
        {
            //double time = DateTime.Now.TimeOfDay.TotalMilliseconds;
            int time2 = DateTime.Now.Millisecond;
            //GuiPreferences.Instance.setLog( "1 " + DateTime.Now.Millisecond.ToString());
            Instances testVector = new Instances("real-time-test", Preferences.Instance.fastvector, 1);
            CreateInstances(testVector);

            // save instances to a file. useful for making sure why testing doesnt work, but using weka.
            //WekaCommonFileOperation.SaveArff(testVector, GuiPreferences.Instance.WorkDirectory + (Preferences.Instance.currentClassifiedVector+1).ToString());

            double classification = -1.0f;
            Instance instance = testVector.instance(0);
            try
            {
                classification = model.classifyInstance(instance);
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog(e.ToString());
            }

            //add classification to a list, after which we output some statistics.
            Preferences.Instance.classification.Add(new string[2] { TrToEvent(), getPrediction(classification + 1) });
            GuiPreferences.Instance.setLog("ADDED " + Convert.ToInt32(classification).ToString() + " " + (Preferences.Instance.currentClassifiedVector + 1).ToString() + " " + Preferences.Instance.currentUDPVector.ToString() + " " + TrToEvent() + "/" + getPrediction(classification + 1));
            //GuiPreferences.Instance.setLog("2 " + DateTime.Now.Millisecond.ToString());
            GuiPreferences.Instance.setLog("classified as:   " + (classification + 2).ToString() + "    time: " + (DateTime.Now.TimeOfDay.TotalMilliseconds - time).ToString());
            transmitPrediction(classification + 1);
            ////- evaluation is for full testing sets, while model.classifyInstance is (fast) for a single realtime vector.
            //try
            //{
            //    testingEval = new weka.classifiers.Evaluation(testVector);
            //    testingEval.evaluateModel(model, testVector);
            //    printWekaResults(testingEval.toSummaryString("\nResults\n======\n", false));
            //}
            //catch (Exception e)
            //{
            //    GuiPreferences.Instance.setLog(e.ToString());
            //} 
            //GuiPreferences.Instance.setLog("evaluateModel" + DateTime.Now.Millisecond);
        }

        // Used in Realtime: This is a helper method to create instances from the internal model files
        public static void CreateInstances(Instances instances)
        {
            int[] realKeyIndices = new int[TrainingTesting_SharedVariables._trainTopIGFeatures.Length];
            int[] indices = new int[TrainingTesting_SharedVariables._trainTopIGFeatures.Length];
            double[] values = new double[TrainingTesting_SharedVariables._trainTopIGFeatures.Length];
            int i = 0;
            int key;
            foreach (int k in TrainingTesting_SharedVariables._trainTopIGFeatures)
            {
                //samples is 1-based while key is 0-based, we need to convert the 0-based index to a 1-based index by adding 1.
                key = k + 1;
                if (Preferences.Instance.ProblemOriginal.samples[Preferences.Instance.currentClassifiedVector].ContainsKey(key))
                {
                    realKeyIndices[i] = key;

                    indices[i] = i;
                    values[i] = Preferences.Instance.ProblemOriginal.samples[Preferences.Instance.currentClassifiedVector][key];
                }
                else
                {
                    realKeyIndices[i] = key;

                    indices[i] = i;
                    if (i == TrainingTesting_SharedVariables._trainTopIGFeatures.Length - 1)
                        values[i] = 2; //  assign a class, '2' is temporarily, should be based on something else or completely ignored, does it matter which class it issince this is testing. 
                    //else 
                    //    values[i] = 1;  // is it proper to put 0 as a missing a value in a sparse array for a value which is not in the original problem? 
                }
                i += 1;
            }

            //int[] indices = Preferences.Instance.ProblemOriginal.samples[Preferences.Instance.currentClassifiedVector].Keys.ToArray();
            //double[] values = Preferences.Instance.ProblemOriginal.samples[Preferences.Instance.currentClassifiedVector].Values.ToArray();    

            //init statistical matrices. (done once)
            if (Preferences.Instance.MatrixStatistics == null)
            { Preferences.Instance.MatrixStatistics = new StatisticsFeatures(TrainingTesting_SharedVariables._trainTopIGFeatures.Length, Preferences.Instance.events.EventListLastTr, realKeyIndices, false); }
            if (Preferences.Instance.MatrixStatisticsNormalized == null)
            { Preferences.Instance.MatrixStatisticsNormalized = new StatisticsFeatures(TrainingTesting_SharedVariables._trainTopIGFeatures.Length, Preferences.Instance.events.EventListLastTr, realKeyIndices, false); }
            if (Preferences.Instance.TestingBaselineStatistics == null)
            { Preferences.Instance.TestingBaselineStatistics = new StatisticsFeatures(TrainingTesting_SharedVariables._trainTopIGFeatures.Length, Preferences.Instance.events.eventList[0].var2, realKeyIndices, true); }

            //update matrix
            Preferences.Instance.MatrixStatistics.updateMatrix(Preferences.Instance.currentClassifiedVector, values);
            Preferences.Instance.TestingBaselineStatistics.updateMatrix(Preferences.Instance.currentClassifiedVector, values);

            //normalize real time raw values.
            if (!TrainingTesting_SharedVariables._svmscaleTesting.NormalizeSingleVector(realKeyIndices, ref values))
            {
                GuiPreferences.Instance.setLog("normalization failed");
                return;
            }

            Preferences.Instance.MatrixStatisticsNormalized.updateMatrix(Preferences.Instance.currentClassifiedVector, values);

            //value is destroyed in the normalization stage, we reassign the fake class (not used in the testing).
            values[TrainingTesting_SharedVariables._trainTopIGFeatures.Length - 1] = 2;

            var instance = new SparseInstance(1.0f, values, indices, indices.Length);

            /*instances.setClassIndex(instances.numAttributes() - 1); 
             
            var instance = new SparseInstance(instances.numAttributes());
            instance.setDataset(instances);

            foreach (int key in Preferences.Instance.ProblemOriginal.samples[Preferences.Instance.currentClassifiedVector].Keys)
            {
                //var attribute = instances.attribute(pair.Key);
                //double value = pair.Value;
                //if (attribute.isNumeric())
                instance.setValue(instances.attribute(key), Preferences.Instance.ProblemOriginal.samples[Preferences.Instance.currentClassifiedVector][key]);
                //else 
                    //instance.setValue(instances.attribute(pair.Key), value.ToString());  
            }*/
            instances.add(instance);
            instances.setClassIndex(TrainingTesting_SharedVariables._trainTopIGFeatures.Length - 1);
        }

        //hardcoded REST class is sent when a corrupted vector appears.
        public static void ExecuteRealtimeTestForCorrupted(Classifier model)
        {
            GuiPreferences.Instance.setLog("Possibly Corrupted!");
            //double time = DateTime.Now.TimeOfDay.TotalMilliseconds;
            int time2 = DateTime.Now.Millisecond;
            double classification = GuiPreferences.Instance.getLabels().IndexOf("Rest");

            //add classification to a list, after which we output some statistics.
            Preferences.Instance.classification.Add(new string[2] { TrToEvent(), getPrediction(classification) });
            GuiPreferences.Instance.setLog("ADDED " + Convert.ToInt32(classification).ToString() + " " + (Preferences.Instance.currentClassifiedVector + 1).ToString() + " " + Preferences.Instance.currentUDPVector.ToString() + " " + TrToEvent() + "/" + getPrediction(classification));
            //GuiPreferences.Instance.setLog("2 " + DateTime.Now.Millisecond.ToString());
            GuiPreferences.Instance.setLog("classified as hard coded REST:   " + (classification).ToString() + "    time: " + (DateTime.Now.TimeOfDay.TotalMilliseconds - time).ToString());
            transmitPrediction(classification);
        }

        /// <summary>
        /// will send predictions via utp. ignores any prediction that is tagged as 1.0 = baseline
        /// TODO: doesnt support switching hands yet
        /// </summary>
        /// <param name="pred"></param>
        public static void transmitPrediction(double pred)
        {
            int p = (int)pred;
            string classificationCommand = GuiPreferences.Instance.getLabel(p);
            string UdpMsg = classificationCommand + "," + Preferences.Instance.cumulativeTR;
            Preferences.Instance.udp.SendUdpData(UdpMsg);
            GuiPreferences.Instance.setLog("Single Vector Testing Finished, result sent via UDP => " + classificationCommand + " " + pred.ToString() + " Label");
            GuiPreferences.Instance.setLog("UDP MESSAGE : " + UdpMsg);
        }


        //returns the event's name that belongs to the tr index. if baseline will return the previous event's name
        private static string TrToEvent()
        {
            for (int i = 0; i < Preferences.Instance.events.eventList.Count; i++)
            {
                if ((Preferences.Instance.currentClassifiedVector + 1 >= Preferences.Instance.events.eventList[i].var1) &&
                    (Preferences.Instance.currentClassifiedVector + 1 <= Preferences.Instance.events.eventList[i].var2))
                {
                    if (Preferences.Instance.events.eventList[i].var3.Equals("Baseline") && i > 0)
                        return Preferences.Instance.events.eventList[i - 1].var3;
                    else
                        return Preferences.Instance.events.eventList[i].var3;
                }
            }
            return "ERROR";
        }



        /// <summary>
        /// will return the classification 
        /// </summary>
        /// <param name="pred"></param>
        public static string getPrediction(double pred)
        {
            int p = (int)pred;
            return GuiPreferences.Instance.getLabel(p);
        }
    }
}
