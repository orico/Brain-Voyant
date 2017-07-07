using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OriBrainLearnerCore
{
    public class RamMethods
    { 
        public static long getRam()
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            return proc.PrivateMemorySize64;

            /*System.Diagnostics.PerformanceCounter cpuCounter; 
            System.Diagnostics.PerformanceCounter ramCounter; 

            cpuCounter = new System.Diagnostics.PerformanceCounter(); 

            cpuCounter.CategoryName = "Processor"; 
            cpuCounter.CounterName = "% Processor Time"; 
            cpuCounter.InstanceName = "_Total";

            ramCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            GuiPreferences.Instance.setLog(cpuCounter.NextValue().ToString() + "%");
            GuiPreferences.Instance.setLog(ramCounter.NextValue().ToString() + "Mb"); */
        }


        public static void setMemoryLimit()
        {
            //demonstrated at 
            //PublicDomain.Win32.Job.CreateJobWithMemoryLimits(uint minWorkingSetSize, uint maxWorkingSetSize, params Process[] processesToLimit)
            /*using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo("OriBrainLearner.exe");
                p.Start();

                using (PublicDomain.Win32.Job j = PublicDomain.Win32.Job.CreateJobWithMemoryLimits(
                    (uint)PublicDomain.GlobalConstants.BytesInAMegabyte * 12,
                    (uint)PublicDomain.GlobalConstants.BytesInAMegabyte * 30,
                    p))
                {
                    // As long as the Job object j is alive, there
                    // will be a memory limit of 30 Mb on the Process p
                }
            }*/
        }
    }
}
