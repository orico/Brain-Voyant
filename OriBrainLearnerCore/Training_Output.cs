using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OriBrainLearnerCore
{
    public class Training_Output
    {

        public static void printWekaResults(string results)
        {
            string[] lines = results.Split('\n');
            foreach (string line in lines)
            {
                GuiPreferences.Instance.setLog(line);
            }
        } 

    }
}
