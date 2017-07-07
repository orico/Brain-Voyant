using System; 
using System.IO; 

namespace OriBrainLearnerCore
{
    public class Logging
    {
        public static void saveLog(string type)
        {
            string filename = GuiPreferences.Instance.WorkDirectory + type + "_Log_" +
                              DateTime.Now.ToString().Replace(":", "_").Replace("/", "").Replace(" ", "_") + ".txt";
            GuiPreferences.Instance.setLog("Saving Log: " + filename);
            File.WriteAllLines(filename, GuiPreferences.Instance.getLog());
        }
    }
}
