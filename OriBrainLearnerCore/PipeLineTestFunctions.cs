using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using IronPython.Hosting;
//using IronPythonDLL; //to use this remember to add reference to the Iron Python DLL solution.
using Microsoft.Scripting.Hosting;
using LibSVMScale;
using weka.classifiers.functions;
using weka.core;
using weka.core.converters;
using weka.filters;
using WekaWrapper;

namespace OriBrainLearnerCore
{
    //NOTE: Deprecated! for reference only
    //NOTE: Deprecated! for reference only
    //NOTE: Deprecated! for reference only

    //this file has some of the functiont that i used initially to test how the training and classification should be
    //how weka works, how python works. etc. 
    //part 2 of this class holds singleRun related functions.
    //public partial class TrainingSingleRun
    public class PipeLineTestFunctions
    {

        private bool UDPListenActive = false;
        /// <summary>
        /// tests if iron python works.
        /// </summary>
        public void TestIronPython()
        {
            /*IronPythonCLS ir = new IronPythonCLS();
            var res = ir.ExecuteBusinessRules();
            GuiPreferences.Instance.setLog(res.ToString());*/
            //ExecuteSelectKthVectorScript();



            string CsharpFileName = @"TrainSet";
            string CsharpDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\4\rtp\";
            /*ExecuteSelectKthVectorScript(CsharpFileName, CsharpDirectory);
            svm_scale_java svmscale = new svm_scale_java();

            string commandLine = "-l 0 " +
                                 "-s " + CsharpDirectory + "TrainSet_3th_vectors_scale_paramcs.libsvm " +
                                 "-o " + CsharpDirectory + "TrainSet_3th_vectors_scaledcs.libsvm " +
                                         CsharpDirectory + "TrainSet_3th_vectors.libsvm";

            string[] commandArray = commandLine.Split(' ');
            svmscale.run(commandArray);

            commandLine = "-l 0 " +
                                 "-s " + CsharpDirectory + "TrainSet_4th_vectors_scale_paramcs.libsvm " +
                                 "-o " + CsharpDirectory + "TrainSet_4th_vectors_scaledcs.libsvm " +
                                         CsharpDirectory + "TrainSet_4th_vectors.libsvm";
            commandArray = commandLine.Split(' ');
            svmscale.run(commandArray);*/

            ////////////////////////WekaCommon.Main(null);
            ////////////////////////var source = new ConverterUtils.DataSource(CsharpDirectory + "TrainSet_3th_vectors_scaledCS.libsvm");

            //convert tr4 and tr3 to arff
            /*if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(CsharpDirectory + "TrainSet_3th_vectors_scaledCS.libsvm"))
                GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_3th_vectors_scaledCS.libsvm");
            if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(CsharpDirectory + "TrainSet_4th_vectors_scaledCS.libsvm"))
                GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_4th_vectors_scaledCS.libsvm");*/

            //infogain on tr4 and get 1000 top features.

            ConverterUtils.DataSource source = new ConverterUtils.DataSource(CsharpDirectory + "TrainSet_4th_vectors_scaledCS.libsvm.arff");
            Instances data = source.getDataSet();
            if (data.classIndex() == -1)
                data.setClassIndex(data.numAttributes() - 1);

            if (!data.classAttribute().isNominal())
            {
                var filter = new weka.filters.unsupervised.attribute.NumericToNominal();

                filter.setOptions(weka.core.Utils.splitOptions("-R last"));
                //filter.setAttributeIndices("last");
                filter.setInputFormat(data);
                data = Filter.useFilter(data, filter);
            }

            int[] topIGFeatures = Preferences.Instance.attsel.selectedAttributes();

            //load tr3
            source = new ConverterUtils.DataSource(CsharpDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff");
            data = source.getDataSet();

            int[] invertedTopIGFeatures= new int[data.numAttributes()-topIGFeatures.Length];

            //alternative use of the filter, 
            var dict = topIGFeatures.ToDictionary(key => key, value => value);
            int position = 0;
            for (int feat=0; feat<data.numAttributes();feat++)
            {
                if  (!dict.ContainsKey(feat))
                {
                    invertedTopIGFeatures[position] = feat;
                    position++;
                }
            }
            
            //filter top IG
            //data = WekaCommonMethods.useRemoveFilter(data, topIGFeatures, true);
            data = WekaTrainingMethods.useRemoveFilter(data, invertedTopIGFeatures,false);
            WekaCommonFileOperation.SaveArff(data, CsharpDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG2.libsvm1.arff");

            //train
            /*weka.classifiers.functions.SMO smo = new SMO();
            smo.setOptions(weka.core.Utils.splitOptions(" -C 1.0 -L 0.001 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.PolyKernel -C 250007 -E 1.0\""));
            if (data.classIndex() == -1)
                data.setClassIndex(data.numAttributes() - 1);

            

            smo.buildClassifier(data);
           
            
            //test on self should get 100%
            weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(data);
            eval.evaluateModel(smo, data);
            GuiPreferences.Instance.setLog(eval.toSummaryString("\nResults\n======\n", false));
           
            //save model serialize model
            weka.core.SerializationHelper.write(CsharpDirectory + "TrainSet_3th_vectors_scaledCS_filteredIG.libsvm.arff.model", smo);

            //load model deserialize model
            smo = (weka.classifiers.functions.SMO)weka.core.SerializationHelper.read(CsharpDirectory + "TrainSet_3th_vectors_scaledCS_filteredIG.libsvm.arff.model");
           
            //test loaded model 
            eval = new weka.classifiers.Evaluation(data);
            eval.evaluateModel(smo, data);
            GuiPreferences.Instance.setLog(eval.toSummaryString("\nResults\n======\n", false));*/
           
            //display top IG.
            //PublicMethods.plotBrainDicomViewer();

            if (Preferences.Instance.attsel == null)
            {
                GuiPreferences.Instance.setLog("there are no ranked IG attributes or selected attr, continuing but please fix this possible bug.");
            }

            string dicomDir = CsharpDirectory;
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

            Form plotForm = new DicomImageViewer.MainForm(dicomDir + firstFile, firstFile, Preferences.Instance.attsel.rankedAttributes(),
                                                          Convert.ToDouble(GuiPreferences.Instance.NudIGThreshold),
                                                          Convert.ToInt32(GuiPreferences.Instance.NudIGVoxelAmount),
                                                          thresholdOrVoxelAmount, GuiPreferences.Instance.WorkDirectory + "brain");
            plotForm.StartPosition = FormStartPosition.CenterParent;
            plotForm.ShowDialog();
            plotForm.Close();
        }

        /// <summary>
        /// WAS A BIG BUTTON: quickloads a range of commands to test the weka pipeline
        /// loads protocol and data, processes for SMO, trick, IG, etc.. has java/python intergration
        /// 1. Trick: QuickLoad, Export to Libsvm, separate to TRs files in libsvm,  convert TR-3 and TR-4 to arff, use TR4 + IG to get 1000 features, filter TR-3 based on features from TR-4, save result to libsvm format, train using LibSvm (grid?), save model, test on training data - must get 100%, display 1000 on viewport
        /// 2. No Trick: QuickLoad, Export to Libsvm, separate to TRs files in libsvm,  convert TR-3 to arff, filter TR-3 based on 1000 top IG, save result to libsvm format, train using LibSvm (grid?), save model, test on training data - must get 100%, display 1000 on viewport
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool QuickProcessWekaPipeline(int from)
        {
            
            // --- from this point the loading data phaze begins --- //

            // tirosh null movement
            /*
             * GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\4\rtp\";                        
             * GuiPreferences.Instance.FileName = "tirosh-";            
             * GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;            
             * GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120705.NullClass1_zbaseline.prt";
            */
 
            // magali classification
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\Experiment1\Kozin_Magali\20121231.movement.3.imagery.1\18-classification.movement\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";
             * */

            /// moshe sherf classification, 4 aggregated to test on 1.
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\1234-5\";
            //GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\1234-5\20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";

            string[] directoryList =
                {
                    @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\05_classification\rtp\",
                    @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\07_classification\rtp\",
                    @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\09_classification\rtp\",
                    @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\11_classification\rtp\"
                };
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\15_classification\rtp\";

            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.DataType.rawValue;            
            
            
            

            //read prot file 
            Preferences.Instance.prot = new ProtocolManager();

            double[][] topIGFeatures = {};
            foreach (string directory in directoryList)
            {
                GuiPreferences.Instance.WorkDirectory = directory;
                //delete all files that are going to be created, in order to prevent anomaly vectors.
                string[] deleteFiles =
                    {
                        "TrainSet.libsvm",
                        "TrainSet_3th_vectors.libsvm",
                        "TrainSet_3th_vectors_scale_paramCS.libsvm",
                        "TrainSet_3th_vectors_scaledCS.libsvm",
                        "TrainSet_3th_vectors_scaledCS.libsvm.arff",
                        "TrainSet_3th_vectors_scaledCS_filteredIG.arff",
                        "TrainSet_3th_vectors_scaledCS_filteredIG.model",
                        "TrainSet_3th_vectors_scaledCS_filteredIG_indices.xml",
                        "TrainSet_4th_vectors.libsvm",
                        "TrainSet_4th_vectors_scale_paramCS.libsvm",
                        "TrainSet_4th_vectors_scaledCS.libsvm",
                        "TrainSet_4th_vectors_scaledCS.libsvm.arff"
                    };

                foreach (string fileName in deleteFiles)
                {
                    FileDirectoryOperations.DeleteFile(GuiPreferences.Instance.WorkDirectory + fileName);
                }

                //get all files in the path with this extention
                GuiManager.getFilePaths("*.vdat");

                //update certain info
                GuiManager.updateFilePaths();

                //assigned after we know what to assign from the protocol
                //PublicMethods.setClassesLabels();
                GuiPreferences.Instance.CmbClass1Selected = 1; //left
                GuiPreferences.Instance.CmbClass2Selected = 2; //right                                

                //NEED TO ADD A VARIABLE FOR EVERY OPTION IN THE GUI. RAW VALUES. UNPROCESSED. MULTI CLASS. CROSS VALD, GRID, FOLDS, ETC...
                //and for every button a function! 


                //for the training set
                GuiPreferences.Instance.FromTR = from; // 264;

                //for the test set
                //GuiPreferences.Instance.FromTR = 46;

                //GuiPreferences.Instance.ToTR = 100;// 264;
                 

                //finally load
                TrainingTesting_SharedVariables.binary.loadRawData();

                topIGFeatures = new double[][]{};
                Instances data;
                //files are loaded,thresholded,vectorized,normalized. false means that IG and training are not done here.
                if (!Training_MultiRunProcessing.ProcessSingleRunOffline(ref topIGFeatures, Preferences.Instance.ProblemOriginal))
                {
                    GuiPreferences.Instance.setLog("Samples are empty");
                } 
                //++grab findl vectors and concat them
                // grab min max values for saving the median.
            }

            //create a dir that holds the final DS in C:\
            GuiPreferences.Instance.WorkDirectory = @"C:\FinalData_" + DateTime.Now.ToLongTimeString().Replace(':', '-') ;
            GuiPreferences.Instance.setLog(@"Creating Final Directory in: " + GuiPreferences.Instance.WorkDirectory);
            FileDirectoryOperations.CreateDirectory(GuiPreferences.Instance.WorkDirectory);
            GuiPreferences.Instance.WorkDirectory += @"\";



            //concatenate libsvm normalized and vectorized files
            FileStream fileStream;
            FileStream outputFileStream = new FileStream(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm", FileMode.CreateNew, FileAccess.Write);
            foreach (string directory in directoryList)
            {
                fileStream = new FileStream(directory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm", FileMode.Open, FileAccess.Read);
                Training_MultiRunProcessing.CopyStream(outputFileStream, fileStream);
                fileStream.Close();
            }
            outputFileStream.Close();

            //save concatenated tr3 to a file
            if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm", 204800))
                GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.arff");
             



            double[][] feature_max = new double[directoryList.Length][];
            double[][] feature_min = new double[directoryList.Length][];
            int i = 0;
            int max_index =-1;
            foreach (string directory in directoryList)
            {
                TrainingTesting_SharedVariables._svmscaleTraining.getConfigFileMinMaxValues(
                    directory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scale_paramCS.libsvm", 
                    ref feature_max[i], ref feature_min[i], ref max_index);
                i++;
            }
            

            //calculate Mean + save new min/max param to C:\
            double[] finalFeature_max = new double[feature_max[0].Length];
            double[] finalFeature_min = new double[feature_max[0].Length];
            
            //create a list with enough values for the runs, in order to calculate the median
            var values_max = new List<double>(feature_max.Length);
            var values_min = new List<double>(feature_max.Length); 
            for (int k = 0; k < feature_max.Length; k++)
            {
                //init zeros
                values_max.Add(0);
                values_min.Add(0);
            }

            for (int j = 0; j < feature_max[0].Length; j++)
            {
                for (int k = 0; k < feature_max.Length; k++)
                {
                    values_max[k] = feature_max[k][j];
                    values_min[k] = feature_min[k][j];
                }
                //finalFeature_max[j] = GetMedian(values_max);
                //finalFeature_min[j] = GetMedian(values_min); 
                finalFeature_max[j] = values_max.Max();
                finalFeature_min[j] = values_min.Min();
            }

            TrainingTesting_SharedVariables._svmscaleTraining.saveConfigMinMax_CSharp(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scale_paramCS.libsvm", finalFeature_min, finalFeature_max, 204801, 0.0f, 1.0f);

            //todo check max index in file, 
            //todo check if needs to remove 204801 from it so it doesnt effect the class.
             
            double[][] FinaltopIGFeatures = { };
            Instances finalData = Training_MultiRunProcessing.ConcatenationPipeLine("TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS.libsvm.arff", "TrainSet_4th_vectors_scaledCS.libsvm.arff");
            WekaTrainingMethods.TrainSMO(finalData);
            //save median param file

            //display top IG on dicom view
            string dicomDir = directoryList[0];
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
             
            //NOTE final top may be empty, please remember that the IG are not at preferences.instance.attsel.selectedattributes or rankedattributes.
            Form plotForm = new DicomImageViewer.MainForm(dicomDir + firstFile, firstFile, FinaltopIGFeatures,
                                                          Convert.ToDouble(GuiPreferences.Instance.NudIGThreshold),
                                                          Convert.ToInt32(GuiPreferences.Instance.NudIGVoxelAmount),
                                                          thresholdOrVoxelAmount, 
                                                          GuiPreferences.Instance.WorkDirectory + "brain");

            plotForm.StartPosition = FormStartPosition.CenterParent;
            plotForm.ShowDialog();
            plotForm.Close();

            return true;
        }

        

        /// <summary>
        /// quick loads protocol and data, for testing puposes
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool QuickLoadOfProtocolAndDataPreConfigured(int from)
        {

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// tirosh leg motor and leg thought

            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20110217.legs_thought_random\1\rtp\";
            GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20110217.legs_motor_random\1\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.DataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20110217.legs_motor_random\20110217.legs_motor_random.prt";

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Dona: null class, run2 and run3 may have problems.

            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Dona\20121113.null.class\03\rtp\";
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Dona\20121113.null.class\07\rtp\";
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Dona\20121113.null.class\13_268-270_corrupted\rtp\"; 
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120705.NullClass1_zbaseline.prt";

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // tirosh: null class
            
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\2\rtp\";
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\3\rtp\";
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\4\rtp\";
            //GuiPreferences.Instance.FileName = "tirosh-";
            //GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            //GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120705.NullClass1_zbaseline.prt";


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Tirosh : rapidness / speed 

            //protocol has no baseline, only 5 has tbv file. dont use unless needed.
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120507.Rapid1\2\rtp\";
            GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120507.Rapid1\3\rtp\";
            GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120507.Rapid1\4\rtp\";
            GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120507.Rapid1\5\rtp\";
            GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120507.Rapid1\6\rtp\";
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20121903.Rapidness.Protocol.prt";*/

            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\7\rtp\"; //left movement
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\8\rtp\"; //left imagery
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120509.Rapid.day3\2\rtp\"; // right movement
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120509.Rapid.day3\4\rtp\"; // leg movement
            

            //GuiPreferences.Instance.FileName = "tirosh-";
            //GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            //GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20121903.Rapidness_zbaseline.Protocol.prt";

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //general classification run (moshe sherf)
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Moshe\20121010.classification.fc.1\03_localizer\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20112810.localizer.4th.exp.hands.legs.zscore.thought.prt";*/

            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Moshe\20121010.classification.fc.1\15_classification\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:/My_Dropbox/VERE/MRI_data/Tirosh/20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";

            // 4th experiment, localizer run + localizer protocol
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20112810.short.4th.exp.hands.legs.zscore.thought\0-localizer\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20112810.localizer.4th.exp.hands.legs.zscore.thought.prt";*/

            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20112810.short.4th.exp.hands.legs.zscore.thought\3\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:/My_Dropbox/VERE/MRI_data/Tirosh/20112810.short.4th.exp.hands.legs.zscore.thought_LRF.prt";


            // 5th experiment, localizer run + localizer protocol
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought\0-localizer\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20112810.localizer.4th.exp.hands.legs.zscore.thought.prt";                  */

            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought\6\rtp\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:/My_Dropbox/VERE/MRI_data/Tirosh/20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";


            //null class thought
            //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\svmtestlocalizer\null\null_thought_3\raw\";
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\svmtestlocalizer\null\null_movement_4\raw\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120705.NullClass1_zbaseline.prt";*/

            //rapid
            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\svmtestlocalizer\rapidRight\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:/My_Dropbox/VERE/MRI_data/Tirosh/20121903.Rapidness_zbaseline.Protocol.prt";*/


            /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\UProjects\OriBrainLearner\Data\";
            GuiPreferences.Instance.FileName = "tirosh-";
            GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
            GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20112508.3dir.3rd.exp.hands.legs.zscore.thought.prt";*/
            
            //read prot file 
            Preferences.Instance.prot = new ProtocolManager();

            //get all files in the path with this extention
            GuiManager.getFilePaths("*.vdat");

            //update certain info
            GuiManager.updateFilePaths();

            //assigned after we know what to assign from the protocol
            //PublicMethods.setClassesLabels();
            GuiPreferences.Instance.CmbClass1Selected = 1; //left
            GuiPreferences.Instance.CmbClass2Selected = 2; //right                                

            //NEED TO ADD A VARIABLE FOR EVERY OPTION IN THE GUI. RAW VALUES. UNPROCESSED. MULTI CLASS. CROSS VALD, GRID, FOLDS, ETC...
            //and for every button a function! 


            //for the training set
            GuiPreferences.Instance.FromTR = from;// 264;

            //for the test set
            //GuiPreferences.Instance.FromTR = 46;

            //GuiPreferences.Instance.ToTR = 100;// 264;

            //finally load
            TrainingTesting_SharedVariables.binary.loadRawData();


            /*
             * pipes only
             * 
                //register this function that deals with loading of the filenames
                //BUT ONLY ONCE!    
                Preferences.Instance.pipeServer.registerEvent(PublicMethods.binary.loadRawDataUsingPipes_ReceiveData);

                //PublicMethods.binary.loadRawDataUsingPipes_SendData();

                //* to automatically send data ever 2s as a simulation and to test the classes.
                PublicMethods.binary.loadRawDataUsingPipes_SendDataTimer();
             */

             return true; 
        }

        /// <summary>
        /// quick  the udp classification, model loading etc.
        /// </summary>
        /// <returns></returns>
        public bool testUDPSVMWRAPPER()
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
                GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought\6\rtp\";
                GuiPreferences.Instance.FileName = "tirosh-";
                GuiPreferences.Instance.FileType = OriBrainLearnerCore.DataType.rawValue;
                GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought_LRF_fastreact.prt";

                //read prot file 
                Preferences.Instance.prot = new ProtocolManager();

                //get all files in the path with this extention
                GuiManager.getFilePaths("*.vdat");

                //update certain info
                GuiManager.updateFilePaths();

                //assigned after we know what to assign from the protocol
                //PublicMethods.setClassesLabels();
                GuiPreferences.Instance.CmbClass1Selected = 1; //left
                GuiPreferences.Instance.CmbClass2Selected = 2; //right                                

                //NEED TO ADD A VARIABLE FOR EVERY OPTION IN THE GUI. RAW VALUES. UNPROCESSED. MULTI CLASS. CROSS VALD, GRID, FOLDS, ETC...
                //and for every button a function! 

                /*PublicMethods.clearProblem() ;
                PublicMethods.clearSVM();
                PublicMethods.clearJob();
                GC.Collect();*/

                //load a model for testing
                GuiPreferences.Instance.TrainType = TrainingType.TrainTestSplit;
                GuiPreferences.Instance.setLog("Loading model ori.svm - 62mb this will take a while....");
                WekaTrainingMethods.loadModel();

                //GuiPreferences.Instance.FromTR = from;// 264;
                //GuiPreferences.Instance.ToTR = 100;// 264;
                Preferences.Instance.currentUDPVector = 0; 
                Preferences.Instance.udp.RegisterCallBack(TrainingTesting_SharedVariables.binary.loadRawDataUsing_UDP);
                
                //finally load
                //PublicMethods.binary.loadRawData();
                /*
                    //register this function that deals with loading of the filenames
                    //BUT ONLY ONCE!    
                    Preferences.Instance.pipeServer.registerEvent(PublicMethods.binary.loadRawDataUsingPipes_ReceiveData);

                    //PublicMethods.binary.loadRawDataUsingPipes_SendData();

                    //* to automatically send data ever 2s as a simulation and to test the classes.
                    PublicMethods.binary.loadRawDataUsingPipes_SendDataTimer();
                 */
            }

            UDPListenActive = !UDPListenActive;
            return true;
        }

        /// <summary>
        /// quick tests udp classification via weka's SMO
        /// </summary>
        /// <returns></returns>
        public bool testUdpWekaSMO()
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


                //GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought\6\rtp\";
                //GuiPreferences.Instance.FileName = "tirosh-";
                //GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
                //GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";

                // tirosh null movement processed for 204800 features + 1 class = 204801.
                /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120508.Rapid+NullClass.day2\1\rtp\";
                GuiPreferences.Instance.FileName = "tirosh-";
                GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
                GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20120705.NullClass1_zbaseline.prt";
                */

                // magali classification
                /*GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\Experiment1\Kozin_Magali\20121231.movement.3.imagery.1\18-classification.movement\rtp\";
                GuiPreferences.Instance.FileName = "tirosh-";
                GuiPreferences.Instance.FileType = OriBrainLearnerCore.dataType.rawValue;
                GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";*/


                /// moshe sherf classification, 4 aggregated to test on 1.
                GuiPreferences.Instance.WorkDirectory = @"H:\My_Dropbox\VERE\Experiment1\Sherf_Moshe\20121010.movement.1\15_classification\rtp\";
                GuiPreferences.Instance.FileName = "tirosh-";
                GuiPreferences.Instance.FileType = OriBrainLearnerCore.DataType.rawValue;
                GuiPreferences.Instance.ProtocolFile = @"H:\My_Dropbox\VERE\MRI_data\Tirosh\20113110.short.5th.exp.hands.legs.zscore.thought_LRF.prt";                

                //read prot file 
                Preferences.Instance.prot = new ProtocolManager();

                //get all files in the path with this extention
                GuiManager.getFilePaths("*.vdat");

                //update certain info
                GuiManager.updateFilePaths();

                //assigned after we know what to assign from the protocol
                //PublicMethods.setClassesLabels();
                GuiPreferences.Instance.CmbClass1Selected = 1; //left
                GuiPreferences.Instance.CmbClass2Selected = 2; //right                                

                //NEED TO ADD A VARIABLE FOR EVERY OPTION IN THE GUI. RAW VALUES. UNPROCESSED. MULTI CLASS. CROSS VALD, GRID, FOLDS, ETC...
                //and for every button a function! 

                /*PublicMethods.clearProblem() ;
                PublicMethods.clearSVM();
                PublicMethods.clearJob();
                GC.Collect();*/

                //load a model for testing
                GuiPreferences.Instance.TrainType = TrainingType.Weka;
                GuiPreferences.Instance.setLog("Deserializing Model");
                WekaTrainingMethods.loadModel();

                double[][] rankedArray = XMLSerializer.DeserializeFile<double[][]>(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG_indices.xml");
                for (int a = 0 ; a<rankedArray.Length;a++)
                {
                    TrainingTesting_SharedVariables._trainTopIGFeatures[a] = Convert.ToInt32(rankedArray[a][0]);
                }
                
                //this should be done ONCE - move elsewhere.
                Preferences.Instance.fastvector = RealTimeProcessing.CreateFastVector(TrainingTesting_SharedVariables._trainTopIGFeatures.Length);

                //GuiPreferences.Instance.FromTR = from;// 264;
                //GuiPreferences.Instance.ToTR = 100;// 264;
                Preferences.Instance.currentUDPVector = 0;
                Preferences.Instance.udp.RegisterCallBack(TrainingTesting_SharedVariables.binary.loadRawDataUsing_UDP);

                //finally load
                //PublicMethods.binary.loadRawData();
                /*
                    //register this function that deals with loading of the filenames
                    //BUT ONLY ONCE!    
                    Preferences.Instance.pipeServer.registerEvent(PublicMethods.binary.loadRawDataUsingPipes_ReceiveData);

                    //PublicMethods.binary.loadRawDataUsingPipes_SendData();

                    //* to automatically send data ever 2s as a simulation and to test the classes.
                    PublicMethods.binary.loadRawDataUsingPipes_SendDataTimer();
                 */
            }

            UDPListenActive = !UDPListenActive;
            return true;
        }


        /// <summary>
        /// adds jobs to the job manager and does the old training offline processing.
        /// </summary>
        /// <returns></returns>
        public bool testGLM()
        { 
            JobManager.addFeatureProcess("Glm");
            JobManager.addFeatureProcess("Voxelize");
            Training_OLDProcessing.OfflineProcess();
            return true;
        }

        
        
    }
     
}
