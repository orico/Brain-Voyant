using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OriBrainLearnerCore
{ 
    /// <summary>
    /// the class holds each event's tr range & the maximal TR in the protocol
    /// </summary>
    public class Events
    {
        //eventList can be converted into private at some point, but for the moment its public because it takes time to change all the eventList.adds eventList.sorts etc
        public List<IntIntStr> eventList;
        private int eventListLastTr;

        public Events(List<IntIntStr> _eventlist)
        {
            eventList = _eventlist;
        }

        /// <summary>
        /// returns the last TR in the list.
        /// </summary>
        public int EventListLastTr
        {
            get
            {
                return eventListLastTr;
            }
            set { eventListLastTr = value; }
        }

        /// <summary>
        /// This function returns the relative tr in an event, i.e., tr1..tr8
        /// input: tr in the timeline
        /// IGNORES First BASELINE EVENT.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        public int findConditionsRelativeTrBasedOnTr(int tr)
        {
            for (int i = 1; i < Preferences.Instance.events.eventList.Count; i += 2)
            {
                if (tr>=eventList[i].var1 && tr<=eventList[i].var2)
                {
                    return tr - eventList[i].var1 + 1;
                }
                
                if (tr >= eventList[i + 1].var1 && tr <= eventList[i + 1].var2)
                {
                    return tr - eventList[i].var1 + 1;
                }
            } 
            return -1;
        }

    }
}
