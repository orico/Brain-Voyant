using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using WekaApi;
using weka.attributeSelection;
using weka.filters;
using weka.core;
using weka.core.converters;
using weka.filters.unsupervised.instance;
using Attribute = weka.core.Attribute;

namespace OriBrainLearnerCore
{ 

    public class WekaCommonFileOperation
    { 

        //converts TR-based files from a directory to Arff
        public static void ConvertToArff(string CsharpDirectory)
        {
            for (decimal i = GuiPreferences.Instance.NudExtractFromTR; i <= GuiPreferences.Instance.NudExtractToTR; i++)
            {
                //convert libsvm tr file to arff
                if (WekaCommonFileOperation.ConvertLIBSVM2ARFF(CsharpDirectory + "TrainSet_" + i.ToString() + "th_vectors_scaledCS.libsvm", 204800))
                    GuiPreferences.Instance.setLog("Converted to ARFF: TrainSet_" + i.ToString() + "th_vectors_scaledCS.libsvm");
            }
        }

        //converts a libsvm file to arff
        public static bool ConvertLIBSVM2ARFF(string filename,int featureToRemove)
        {
            try
            {
                var source = new ConverterUtils.DataSource(filename);
                Instances insts = source.getDataSet();

                if (featureToRemove>-1)
                {
                    insts.deleteAttributeAt(featureToRemove);
                }

                if (insts.classIndex() == -1)
                    insts.setClassIndex(insts.numAttributes());

                if (!insts.classAttribute().isNominal())
                {
                    var filter = new weka.filters.unsupervised.attribute.NumericToNominal();
                    filter.setOptions(weka.core.Utils.splitOptions("-R last"));
                    filter.setInputFormat(insts);
                    insts = Filter.useFilter(insts, filter);
                }

                SaveArff(insts, filename);
                GuiPreferences.Instance.setLog("Converted to arff.");
            }
            catch (Exception e)
            {

                //optional save method
                /*var writer = new java.io.BufferedWriter(new java.io.FileWriter(filename+"1.arff"));
                writer.write(insts.toString());
                writer.flush();
                writer.close();*/
                GuiPreferences.Instance.setLog(e.ToString() + " ");
                GuiPreferences.Instance.setLog(filename);
                return false;
            }

            return true;
        }
        

        //saves instances to arff format
        public static void SaveArff(Instances insts, string filename)
        {
            if (insts == null)
            {
                Console.WriteLine("instances empty null");
                return;
            }
            var saver = new ArffSaver();
            saver.setInstances(insts);
            saver.setFile(new java.io.File(filename + ".arff"));
            saver.writeBatch();
        }


        //saves instances to libsvm format
        public static void SaveLIBSVM(Instances insts, string filename)
        {
            if (insts == null)
            {
                Console.WriteLine("instances empty null");
                return;
            }


            var saver = new LibSVMSaver();
            saver.setInstances(insts);
            saver.setFile(new java.io.File(filename + ".libsvm"));
            saver.writeBatch();
        }


        //saves instances to csv format
        public static void SaveCSV(Instances insts, string filename)
        {
            if (insts == null)
            {
                Console.WriteLine("instances empty null");
                return;
            }
            var saver = new CSVSaver();
            saver.setInstances(insts);
            saver.setFile(new java.io.File(filename + ".csv"));
            saver.writeBatch();
        } 

    } //class
} //namespace
