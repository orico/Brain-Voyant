using System;
using System.Collections.Generic;
using System.Linq; 
using System.IO; 
using System.Text;
using System.Threading;

namespace OriBrainLearnerCore 
{ 
    public class IntIntStr : IComparable
    {
        public int var1;
        public int var2;
        public string var3;

        public int CompareTo(object obj)
        {
            IntIntStr c = (IntIntStr)obj; 
            if (this.var1>c.var1) 
                   return 1;
            else 
                if(this.var1==c.var1) return 0;        
            //will return small as a default.. just to make sure there is a global return
            //if (this.var1<c.var1) 
            return -1; 
        }

        public IntIntStr(int v1, int v2, string v3)
        {
            var1 = v1;
            var2 = v2;
            var3 = v3;
        }

        public IntIntStr getValues()
        {
            return new IntIntStr(var1, var2, var3);
        }

    }

    public class ProtocolManager
    {
        private string currentEvent;
        public bool readProtocol = false;

        /// <summary>
        /// default load without checking if tr loaded are equal to protocol trs
        /// </summary>
        public ProtocolManager()
        {
            InitNoCheck();              
        }

        /// <summary>
        /// load with checking if tr loaded are equal to protocol trs
        /// </summary>
        public ProtocolManager( bool check)
        {
            if (check)
            {
                InitWithAmountCheck();
            }
            else
            {
                InitNoCheck();
            }
        }

        /// <summary>
        /// load protocol and dont check to see if files amount match the loaded files. this is used in a single run load and not in the treeview method.
        /// </summary>
        public void InitNoCheck()
        {
            GuiPreferences.Instance.setLog("Attempting to read protocol file");
            GuiPreferences.Instance.setLog("TODO: the proper way of getting the info from the protocol would be to implement a temporary list in ProtocolManager Class and if successful with the read, then clone it to the preferences. at the moment we are working directly on the preferences' list");
            readProtocol = ReadProtocol();
            if (readProtocol)
            {
                GuiPreferences.Instance.setLog("Success");
                GuiPreferences.Instance.setLog("-------------------------------------------------");
            }
            else
            {
                GuiPreferences.Instance.setLog("Failed");
                GuiPreferences.Instance.setLog("-------------------------------------------------");
            }
        }

        /// <summary>
        /// load protocol and check if amount of loaded trs match the protocol trs
        /// </summary>
        public void InitWithAmountCheck()
        {
            GuiPreferences.Instance.setLog("Attempting to read protocol file");
            GuiPreferences.Instance.setLog("TODO: the proper way of getting the info from the protocol would be to implement a temporary list in ProtocolManager Class and if successful with the read, then clone it to the preferences. at the moment we are working directly on the preferences' list");
            readProtocol = ReadProtocol();
            if (readProtocol)
            {
                if (GuiPreferences.Instance.TbTRs >= 0 && GuiPreferences.Instance.TbTRs == Preferences.Instance.events.EventListLastTr)
                {
                    GuiPreferences.Instance.setLog("Success");
                    GuiPreferences.Instance.setLog("-------------------------------------------------");
                }
                else
                {
                    GuiPreferences.Instance.setLog("Failed: Loaded protocol doesnt have the same amount of TRs as already-read-files");
                    GuiPreferences.Instance.setLog("Or maybe you didnt load the files first?!?");
                    GuiPreferences.Instance.setLog("read files  : " + GuiPreferences.Instance.TbTRs.ToString());
                    GuiPreferences.Instance.setLog("protocol Trs: " + Preferences.Instance.events.EventListLastTr.ToString());
                    GuiPreferences.Instance.setLog("-------------------------------------------------");
                    readProtocol = false;
                }

            }
            else
            {
                GuiPreferences.Instance.setLog("Failed");
                GuiPreferences.Instance.setLog("-------------------------------------------------");
            }
        }

    public bool ReadProtocol()
        {
            GuiPreferences.Instance.setLog("Protocol File : " + GuiPreferences.Instance.ProtocolFile);
            string text = File.ReadAllText(GuiPreferences.Instance.ProtocolFile);
            string[] farr = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int step = 1;
            int subStep = 1;
            int IMaxEvents = 0;
            int retTrueWhenReachedMaxSteps = 0;
            //clear labels when reloading protocol
            GuiPreferences.Instance.ClearLabels();

            Preferences.Instance.events.eventList.Clear();
            Preferences.Instance.events.EventListLastTr = 0;

            foreach (string line in farr)
            {
                if (step == 1)
                {
                    string[] spl = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (spl.Contains("NrOfConditions:"))
                    {
                        GuiPreferences.Instance.setLog("NrOfConditions Found : " + spl[1]);
                        Preferences.Instance.numberOfConditions = int.Parse(spl[1]);
                        retTrueWhenReachedMaxSteps = int.Parse(spl[1]) + 2; 
                        step += 1;
                    }
                }
                else if (step >= 2)
                {
                    if (subStep == 1)
                    {
                        string[] spl = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        currentEvent = spl[0];
                        //GuiPreferences.Instance.setLog("Event: " + currentEvent;
                        subStep += 1;                        
                    }
                    else if (subStep == 2)
                    {
                        string[] SMaxEvents = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        IMaxEvents = int.Parse(SMaxEvents[0]);
                        GuiPreferences.Instance.setLog( "Event: " + currentEvent + " ==> " + IMaxEvents + " Found");
                        //update the found labels inside pref
                        if (!GuiPreferences.Instance.containsLabel(currentEvent))
                            GuiPreferences.Instance.addLabel(currentEvent);
                        subStep += 1;
                    }
                    else if (subStep == 3)
                    {
                        if (line.Contains("\t"))
                        {
                            string[] param = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            Preferences.Instance.events.eventList.Add(new IntIntStr(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()), currentEvent));
                        }
                        else if (line.Contains("Color"))
                        {
                            subStep = 1;
                            step += 1;
                        }
                    }

                }
            }

            Preferences.Instance.events.eventList.Sort();

            //get the last TR
            Preferences.Instance.events.EventListLastTr = Preferences.Instance.events.eventList[Preferences.Instance.events.eventList.Count - 1].var2;
            GuiPreferences.Instance.setLog("Total TRs: " + Preferences.Instance.events.EventListLastTr.ToString());

            if (step == retTrueWhenReachedMaxSteps)
            {
                if (GuiPreferences.Instance.CbOnsetFromProtocolChecked)
                {
                    //Program.mainForm.nudPostOnset.Value = (Preferences.Instance.eventList[0].var2 - Preferences.Instance.eventList[0].var1 + 1) + (Preferences.Instance.eventList[1].var2 - Preferences.Instance.eventList[1].var1 + 1);
                    GuiPreferences.Instance.PostOnset = (Preferences.Instance.events.eventList[0].var2 - Preferences.Instance.events.eventList[0].var1 + 1) + (Preferences.Instance.events.eventList[1].var2 - Preferences.Instance.events.eventList[1].var1 + 1);
                    //Program.mainForm.nudPreOnset.Value = 1; 
                    GuiPreferences.Instance.PreOnset = 1; 
                } 
                return true;
            }
            else return false;
        }

        /// <summary>
        /// get new event list with full Events, baseline range is assigned to previous event.
        /// </summary>
        /// <returns></returns>
        public List<IntIntStr> getHDREvents()
        {
            List<IntIntStr> noRestList  = new List<IntIntStr>();
            int k = 0;
            for (int i = 0; i < Preferences.Instance.events.eventList.Count; i++)
            {
                if (Preferences.Instance.events.eventList[i].var3 == "Baseline")
                {
                    int j = i - 1;

                    while ((j > 0) && (Preferences.Instance.events.eventList[j].var3 == "Baseline") && (Preferences.Instance.events.eventList[j].var3 != null))
                    {
                        j--;
                    }
                    if (j > 0)
                        noRestList[k - 1].var2 = Preferences.Instance.events.eventList[i].var2;
                        //noRestList.Add(new IntIntStr(Preferences.Instance.eventList[i].var1, Preferences.Instance.eventList[i].var2, Preferences.Instance.eventList[j].var3));
                }
                else
                {
                    noRestList.Add(Preferences.Instance.events.eventList[i]);
                    k++;
                } 
            } 
            return noRestList;
        }


       
    }    
} 