using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OriBrainLearnerCore
{
    public class StatisticsAccuracy
    {

        //this function should be called after the whole classification is finished, but can also be called mid way.
        public static void generateStats()
        {
            ////////////////////// total accuracy ///////////////
            int TrsInEvent = 0;
            for (int i = 1; i < Preferences.Instance.events.eventList.Count; i += 2)
            {
                int trsInEvent = (Preferences.Instance.events.eventList[i].var2     - Preferences.Instance.events.eventList[i].var1     + 1) 
                                 +
                                 (Preferences.Instance.events.eventList[i + 1].var2 - Preferences.Instance.events.eventList[i + 1].var1 + 1); 
                if (trsInEvent>TrsInEvent)
                {
                    TrsInEvent = trsInEvent;
                }
            }
            TrsInEvent = TrsInEvent + 1;
            GuiPreferences.Instance.setLog("TrsInEvent: " + TrsInEvent.ToString());

            int[] totalAccuracy = new int[TrsInEvent]; //zero not used, "one-based tr indexing"
            //go over all event counts
            for (int i = 1; i < Preferences.Instance.events.eventList.Count; i += 2)
            {
                int idx = 1;
                for (int j = Preferences.Instance.events.eventList[i].var1; j <= Preferences.Instance.events.eventList[i].var2; j++)
                {
                    if (j - 1 < Preferences.Instance.classification.Count)
                        if (Preferences.Instance.classification[j - 1][0] == Preferences.Instance.classification[j - 1][1])
                            totalAccuracy[idx]++;
                    idx++;
                }

                for (int j = Preferences.Instance.events.eventList[i + 1].var1; j <= Preferences.Instance.events.eventList[i + 1].var2; j++)
                {
                    if (j - 1 < Preferences.Instance.classification.Count)
                        if (Preferences.Instance.classification[j - 1][0] == Preferences.Instance.classification[j - 1][1])
                        {
                            //GuiPreferences.Instance.setLog(idx.ToString());
                            totalAccuracy[idx]++;
                        }
                    idx++;
                }
            }

            for (int i = 1; i < totalAccuracy.Length; i++)
            { 
                if (i <= 5)
                {
                    GuiPreferences.Instance.setLog("TR: " + i.ToString() + " : " + totalAccuracy[i]); 
                }
                else if (totalAccuracy[i] > 0)
                {
                    GuiPreferences.Instance.setLog("TR: " + i.ToString() + " : " + totalAccuracy[i]);
                }
            }

            ////////////////////// per condition accuracy ///////////////
            Dictionary<string, int[]> perConditionAccuracy = new Dictionary<string, int[]>();  //int array: zero not used, "one-based tr indexing"

            //go over all event counts
            for (int i = 1; i < Preferences.Instance.events.eventList.Count; i += 2)
            {
                //add condition if doesnt exist (should not add baseline because we are going over 2 at a time)
                if (!perConditionAccuracy.ContainsKey(Preferences.Instance.events.eventList[i].var3))
                    perConditionAccuracy.Add(Preferences.Instance.events.eventList[i].var3, new int[TrsInEvent]);

                int idx = 1;
                for (int j = Preferences.Instance.events.eventList[i].var1; j <= Preferences.Instance.events.eventList[i].var2; j++)
                {
                    if (j - 1 < Preferences.Instance.classification.Count)
                        if (Preferences.Instance.classification[j - 1][0] == Preferences.Instance.classification[j - 1][1])
                            perConditionAccuracy[Preferences.Instance.events.eventList[i].var3][idx]++;
                    idx++;
                }

                for (int j = Preferences.Instance.events.eventList[i + 1].var1; j <= Preferences.Instance.events.eventList[i + 1].var2; j++)
                {
                    if (j - 1 < Preferences.Instance.classification.Count)
                        if (Preferences.Instance.classification[j - 1][0] == Preferences.Instance.classification[j - 1][1])
                            perConditionAccuracy[Preferences.Instance.events.eventList[i].var3][idx]++;
                    idx++;
                }
            }

            foreach (var condition in perConditionAccuracy)
            {
                for (int i = 1; i < condition.Value.Length; i++)
                {
                    if (i<=5)
                    {
                        GuiPreferences.Instance.setLog(condition.Key + ": TR: " + i.ToString() + " : " +
                                                          condition.Value[i]);
                    }
                    else if (condition.Value[i] > 0)
                    {
                        GuiPreferences.Instance.setLog(condition.Key + ": TR: " + i.ToString() + " : " +
                                                       condition.Value[i]);
                    }
                }
            }
        }
    }
}
