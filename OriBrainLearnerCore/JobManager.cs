using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace OriBrainLearnerCore
{
    public class JobManager
    {
        //was implemented in a button, not used anymore.
        private void ClearProblemSVMJob(object sender, EventArgs e)
        {
            Training_MultiRunProcessing.clearProblem();
            Training_MultiRunProcessing.clearSVM();
            JobManager.clearJob();
            GC.Collect();
        }

        public static void clearJob()
        {
            //kill all jobs, being newed when adding jobs to the queue
            if (Preferences.Instance.jobs != null)
            {
                Preferences.Instance.jobs = null;
            }
        }

        public static IDataProcessor getProcessBlockInstance(string processBlockString)
        {
            //IDataProcessor j = (IDataProcessor)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("OriBrainLearner.Process" + processBlockString, true);
            // return j;
            Type objType = typeof(OriBrainLearnerCore.ProcessGlm);
            Assembly ass = System.Reflection.Assembly.GetAssembly(objType);
            return (IDataProcessor)ass.CreateInstance("OriBrainLearnerCore.Process" + processBlockString);
        }

        public static void addFeatureJobs()
        {
            Preferences.Instance.jobs = new JobsQueue(Preferences.Instance.ProblemOriginal);
            if (GuiPreferences.Instance.UnprocessedChecked)
            {
            }
            else
            {
                List<int> k = GuiPreferences.Instance.getKvalues();
                List<string> f = GuiPreferences.Instance.getFeatureProcessJobs();
                for (int i = 0; i < f.Count(); i++)
                {
                    Preferences.Instance.jobs.addJob(getProcessBlockInstance(f[i].Replace(" ", string.Empty)), i);
                    Preferences.Instance.jobs.setJobK(i, k[i]);
                    //idea: jobs are per voxel. the sample and feature loop is in jobs and runs a job on each voxel. 
                    // maybe one after the other if its logical i.e onset, voxelize, etc.
                    //this will save looping times.
                    //also add a preprocess loop. to get last index, to get avg for zscore etc.
                    //pre and post looks good also a double loop that goes over Problem and does pre.
                }
            }
        }

        public static void calculatePreviousKValues()
        {
            int maxK = 0;
            int k;
            Preferences.Instance.KInPreviousVectors = new int[Preferences.Instance.jobs.getLength()];
            for (int i = 0; i < Preferences.Instance.jobs.getLength(); i++)
            {
                k = Preferences.Instance.jobs.getJobK(i);
                if (k > maxK)
                {
                    maxK = k;
                }
                Preferences.Instance.KInPreviousVectors[i] = maxK;
            }
            Preferences.Instance.maxKvectorsToWaitBeforeClassification = maxK;
        }

        public static void processFeatureJobs()
        {
            int _length = Preferences.Instance.jobs.getLength();
            if (_length > 0)
                Preferences.Instance.jobs.processJobs();
            else GuiPreferences.Instance.setLog("Training Failed: no jobs to process");
        }

        public static void preProcessFeatureJobs()
        {
            int _length = Preferences.Instance.jobs.getLength();
            if (_length > 0)
                Preferences.Instance.jobs.preProcess();
            else GuiPreferences.Instance.setLog("Training Failed: no jobs to process");
        }

        public static void postProcessFeatureJobs()
        {
            int _length = Preferences.Instance.jobs.getLength();
            if (_length > 0)
                Preferences.Instance.jobs.postProcess();
            else GuiPreferences.Instance.setLog("Training Failed: no jobs to process");
        }



        public static void addFeatureProcess(string item)
        {
            try
            {
                IDataProcessor cls = JobManager.getProcessBlockInstance(item);//(IDataProcessor)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("OriBrainLearner.Process" + item.ToString().Replace(" ", string.Empty), true);
                //lbK.Items.Add(cls.getK()); 
                //lbFeatureProcessFlow.Items.Add(item);
                GuiPreferences.Instance.addKvalues(cls.K);
                GuiPreferences.Instance.setFeatureProcessJobs(item);
            }
            catch
            {
                GuiPreferences.Instance.setLog("Adding Failed: K + Class doesnt exist yet");
            }
        }

        public static void removeFeatureProcess(string item)
        {

        }

        public static void clearFeatureProcesses()
        {
            GuiPreferences.Instance.ClearFeatureProcessJobs();
            GuiPreferences.Instance.ClearKvalues();
        }
    }
}
