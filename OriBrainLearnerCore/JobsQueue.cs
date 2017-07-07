using System;
using System.Linq; 
using System.Collections.Generic;
using System.Text;
using libSVMWrapper;  

namespace OriBrainLearnerCore
{
    public class JobsQueue
    {
        public libSVM_ExtendedProblem _problem = new libSVM_ExtendedProblem(); 
        private List <IDataProcessor> jobs = new List <IDataProcessor>();
        int lastIndex; 

        public JobsQueue(libSVM_ExtendedProblem problem)
        {
            _problem = problem;
            _problem.UpdateMaximumIndex();
            lastIndex = _problem.lastIndex;
        }

        public void addJob(IDataProcessor job, int flowIndex)
        {
            //this job uses the data from the previous job, its problem's information
            //this is how i chain the entire job process. one job uses the previous one's data as soon as its processed
            if (jobs.Count == 0)
            {
                //first job has the original problem
                job.setProblem(_problem);
            }
            else
            {
                //other jobs has the previous problem
                job.setProblem(jobs[jobs.Count - 1].getProblem());
            }
            job.CurrentLocationInFlow = flowIndex;
            jobs.Add(job);
        }

        public IDataProcessor getJob(int index)
        {
            if (index < 0) return null;
            return jobs[index];
        }

        public void setJobK(int index, int k)
        {
            if (jobs.Count >= index + 1)
            {
                jobs[index].K = k;
            }
        }

        public int getJobK(int index)
        {
            if (index < 0) return -1;
            return jobs[index].K;
        }

        public int getLength()
        {
            return jobs.Count;
        }

        public void clearLastJob(int currentJob)
        {
            //remove local reference in previous job for the output 
            //will not remove for -1 and will not remove for the last block (the one that is returned as the final output.
            if ((currentJob > 0) && (currentJob!=jobs.Count-1))
            { jobs[currentJob - 1].clearOutput(); }
            //remove local ref for the current input (which is previous output)
            jobs[currentJob].clearInput(); 
        }

        public void clearJobQueue()
        {
            //this may cause some issues, better double check that clearing this wont clear the original problem in preferences
            jobs.Clear();
            lastIndex = 0;
            _problem = null;
        }

        /// <summary>
        /// // this function is needed only if preprocessing is needed only for the whole jobs queue
        /// </summary>
        public void preProcess() 
        {
            
        }

        /// <summary>
        /// for every job we do a pre/process/post individually.
        /// </summary>
        public void processJobs()
        { 
            for (int i = 0; i < jobs.Count; i++)
            { 
                jobs[i].preProcess();
                jobs[i].Process();                
                jobs[i].postProcess();
                if (!Preferences.Instance.RealTimeTestingSingleVector)
                {
                    clearLastJob(i);
                    GC.Collect();
                    GuiPreferences.Instance.setLog("Processed job " + (i + 1).ToString() + " & GC Collected");
                }
            }

            //assign the last output so that we can find out the result outside of this function.
            //TODO: this line can be done only once unless it is overwritten in the last job block. if it is then it shouldnt happen every time we get a udp filename.
            //i believe it is harmless here every single time. total times it will be executed = number of trs.
            Preferences.Instance.ProblemFinal = jobs[jobs.Count() - 1].getProblem();

            //clean output and assign to final problem only needed in offline classification. 
            //in real time we only need to send the classification result and sample weight.
            if (!Preferences.Instance.RealTimeTestingSingleVector)
            {
                //only now clean the last output
                jobs[jobs.Count() - 1].clearOutput();
            }
        }

        /// <summary>
        /// // this function is needed only if postprocessing is needed only for the whole jobs queue
        /// </summary>
        public void postProcess() 
        { 
        }
    }
}
