using System;
using System.Collections.Generic;
//using WekaApi;
using libSVMWrapper;
using weka.attributeSelection;
using weka.filters;
using weka.core;
using weka.classifiers.functions; 
using weka.core.converters; 

namespace OriBrainLearnerCore
{
    public class WekaTrainingMethods
    {

        /// <summary>
        /// loads arff file using weka, assigns index as last feature and converts to nominal if needed
        /// </summary>
        /// <returns></returns>
        public static Instances loadDataSetFile(string filename)
        {
            ConverterUtils.DataSource source = new ConverterUtils.DataSource(GuiPreferences.Instance.WorkDirectory + filename);
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
            return data;
        }

        public static void saveModel()
        {
            try
            {
                if (GuiPreferences.Instance.TrainType == TrainingType.Weka)
                {
                    //save model serialize model
                    weka.core.SerializationHelper.write(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.libsvm.arff.model", TrainingTesting_SharedVariables.smo);
                    GuiPreferences.Instance.setLog("SMO Model Serialized and saved.");
                }
                else
                {
                    TrainingTesting_SharedVariables.training.saveModel("ori.svm");
                }
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog("Model Saving Failed: no model! " + e.ToString());
            }
        }

        public static bool loadModel()
        {
            try
            {
                if (GuiPreferences.Instance.TrainType == TrainingType.Weka)
                {
                    //load model deserialize model
                    TrainingTesting_SharedVariables.smo = (weka.classifiers.functions.SMO)weka.core.SerializationHelper.read(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.model");
                    GuiPreferences.Instance.setLog("SMO Model DeSerialized and loaded.");
                    return true;
                }
                else
                {
                    return LibSVM_TrainTest.loadModelStatic("ori.svm");
                }
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog("Model Loading Failed: no model! " + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// selects top IG features above 0 weight, then saves serialized data to a file.
        /// </summary>
        /// <param name="topIGFeatures"></param>
        /// <param name="data"></param>
        public static void selectIGSerialize(ref Instances data)
        {
            //run ig and get top 1000 or up to 1000 bigger than zero, from tr4
            WekaTrainingMethods.useLowLevelInformationGainFeatureSelection(data);
            GuiPreferences.Instance.setLog(Preferences.Instance.attsel.selectedAttributes().Length.ToString() + " features above zero value selected (including the Class feature)");

            //serialize (save) ALL 204k indices to file.
            
            //serialize (save) TOP ig indices to file.
            XMLSerializer.serializeArrayToFile<int[]>(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG_indices.xml", Preferences.Instance.attsel.selectedAttributes());
            GuiPreferences.Instance.setLog("saved top + All IG indices to TWO XML files (in the same order as IG gave it)");
            //int [] _trainTopIGFeatures_loaded = DeserializeArrayToFile(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG_indices.xml");
        }

        public static void TrainSMO(Instances data)
        {
            //grab SMO, config
            weka.classifiers.functions.SMO smo = new SMO();
            smo.setOptions(weka.core.Utils.splitOptions(" -C 1.0 -L 0.001 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.PolyKernel -C 250007 -E 1.0\""));

            //train
            smo.buildClassifier(data);

            //test on self should get 100%
            weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(data);
            eval.evaluateModel(smo, data);
            Training_Output.printWekaResults(eval.toSummaryString("\nResults\n======\n", false));

            //save model serialize model
            weka.core.SerializationHelper.write(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.model", smo);

            //load model deserialize model
            smo = (weka.classifiers.functions.SMO)weka.core.SerializationHelper.read(GuiPreferences.Instance.WorkDirectory + "TrainSet_" + GuiPreferences.Instance.NudClassifyUsingTR.ToString() + "th_vectors_scaledCS_filteredIG.model");

            //test loaded model 
            eval = new weka.classifiers.Evaluation(data);
            eval.evaluateModel(smo, data);
            Training_Output.printWekaResults(eval.toSummaryString("\nResults\n======\n", false));
        }

        /**
        * uses a filter
        */
        public static void useFilter(Instances data)
        {
            weka.filters.supervised.attribute.AttributeSelection filter = new weka.filters.supervised.attribute.AttributeSelection();
            InfoGainAttributeEval eval = new InfoGainAttributeEval();
            Ranker search = new Ranker();
            search.setNumToSelect(1000);
            search.setThreshold(-1.7976931348623157E308d);
            filter.setEvaluator(eval);
            filter.setSearch(search);
            filter.setInputFormat(data);
            Instances newData = Filter.useFilter(data, filter);
            Console.WriteLine(newData);
        }

        //uses a 'Remove' filter
        public static Instances useRemoveFilter(Instances data, int[] topIGFeatures, bool invert)
        {
            weka.filters.unsupervised.attribute.Remove filter = new weka.filters.unsupervised.attribute.Remove();
            filter.setAttributeIndicesArray(topIGFeatures);
            filter.setInvertSelection(invert);
            filter.setInputFormat(data);
            Instances newData = Filter.useFilter(data, filter);
            return newData;
        }

        //public static T[] SubArray<T>(this T[] data, int index, int length)
        //{
        //    T[] result = new T[length];
        //    Array.Copy(data, index, result, 0, length);
        //    return result;
        //}

        public static void useLowLevelInformationGainFeatureSelection(Instances data)
        {
            AttributeSelection attsel = new AttributeSelection();
            InfoGainAttributeEval eval = new InfoGainAttributeEval();
            Ranker search = new Ranker();

            //1000 features >0, should be equal to -1 features and threshold 0.0. anyway i use 0.01.

            //int numtoselect = 1000;// 1520;
            //search.setThreshold(-1.7976931348623157E308d);  

            
            int numtoselect = 0;
            float threshold = 0;

            if (GuiPreferences.Instance.IgSelectionType == IGType.Threshold)
            {   
                numtoselect = -1;
                threshold = Convert.ToSingle(GuiPreferences.Instance.NudIGThreshold);
                GuiPreferences.Instance.setLog("Filtering using IG threshold: " + GuiPreferences.Instance.NudIGThreshold.ToString());
            }
            else if (GuiPreferences.Instance.IgSelectionType == IGType.Voxels)
            {
                //num of vox plus above 0 cut off.
                numtoselect = Convert.ToInt32(GuiPreferences.Instance.NudIGVoxelAmount);
                //search.setThreshold(-1.7976931348623157E308d);
                GuiPreferences.Instance.setLog("Filtering using IG Voxel Amount of: " + GuiPreferences.Instance.NudIGVoxelAmount.ToString());
            }
            else
            {
                GuiPreferences.Instance.setLog("error wrong IG type");
            }

            
            search.setNumToSelect(numtoselect);
            search.setThreshold(threshold);
            attsel.setEvaluator(eval);
            attsel.setSearch(search);
            attsel.SelectAttributes(data);

            //reeturned back to the global instance
            Preferences.Instance.attsel = attsel;

            //hIstogram saving indices and ranked to preferences for easy access
            SortedDictionary<double, int> Histogram = new SortedDictionary<double, int>();
            double[][] blah = attsel.rankedAttributes();
            
            for (double i = -0.05; i < 1.2; i+=0.05 )
            {
                if (!Histogram.ContainsKey(i))
                    Histogram.Add(i,0);
                for (int j=0;j<blah.Length;j++)
                {
                    if (blah[j][1] > i - 0.05 && blah[j][1] <= i)
                    {
                        Histogram[i] += 1;
                    }
                }
            }

            GuiPreferences.Instance.setLog("Histogram:");
            for (double i = -0.05; i < 1.2; i += 0.05)
            {
                GuiPreferences.Instance.setLog("Threshold: " +i.ToString()+": "+Histogram[i].ToString());
            }

            //--------------

            if (GuiPreferences.Instance.IgSelectionType == IGType.Voxels)
            {
                //SELECT K BIGGER THAN ZERO.
                int IgVoxAboveZero = 0;
                for (int j = 0; j < GuiPreferences.Instance.NudIGVoxelAmount; j++)
                {
                    if (blah[j][1] > 0)
                    {
                        IgVoxAboveZero++;
                    }
                }

                //this is a bit redundant, to replace this redundancy, create two vectors, selected() & ranked()
                search.setNumToSelect(IgVoxAboveZero);
                search.setThreshold(threshold);
                attsel.setEvaluator(eval);
                attsel.setSearch(search);
                attsel.SelectAttributes(data);
                Preferences.Instance.attsel = attsel;

                GuiPreferences.Instance.NudIGVoxelAmount = IgVoxAboveZero;
                GuiPreferences.Instance.setLog("Filtering using IG Voxel Amount of (above zero): " + GuiPreferences.Instance.NudIGVoxelAmount.ToString());
                GuiPreferences.Instance.setLog("Changing NudIGVoxelAmount to the above figure!");
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            /// 
            //NOTE: uncommenting this proves that ranked attributes are 0-based. this sorts them out from 0 to 204799 (Also threshold above should be 0
            /*double[][] blah = attsel.rankedAttributes();
            double[] blat = new double[blah.Length];
            for (int i = 0; i < blah.Length;i++ )
            {
                blat[i] = blah[i][0];
            }
                Array.Sort(blat);*/
            //////////////////////////////////////////////////////////////////////////////////////////////////////


            //if this code is used it will need to be ammended for numtoselect which is -1 (All) in this case
            /*double[][] ranked = attsel.rankedAttributes();
            int indexBiggerThanZero = 0;
            int classColumn = indices[indices.Length - 1];
            while ((ranked[indexBiggerThanZero][1] > threshold) && (indexBiggerThanZero < numtoselect))
                indexBiggerThanZero++;
            
            //return K features whose IG value is bigger than zero
            int[] indicesFinal = new int[] {};
            //less than K features, we dynamically resize an array and return it + class
            if (indexBiggerThanZero < numtoselect)
            {
                Array.Resize(ref indicesFinal, indexBiggerThanZero+1);
                Array.Copy(indices, 0, indicesFinal, 0, indexBiggerThanZero);

                //Array.Copy(indices, 0, indices, 0, indices.Length+1);
                indicesFinal[indicesFinal.Length - 1] = classColumn;

                return indicesFinal;
            }
            else
            {
                //if indexBiggerThanZero is the same, all features ig values are aboce zero, we return the original int array 
                return indices;
            }*/

        }

        public void insertattributetodatasetexample()
        {
            //insert new attributes to data.
            /*weka.core.Attribute attr = new weka.core.Attribute("att_");           
            for (int att = data.numAttributes(); att < 80 * 80 * 32; att ++ )
            {                
                data.insertAttributeAt(attr, data.numAttributes());
                data.renameAttribute(att,"att_"+att.ToString());
                or 
                data.insertAttributeAt(new attributes("namehere"), data.numAttributes());
            } */
        }
    }
}
