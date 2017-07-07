using System.Collections.Generic;
using System.IO;
using System.Linq; 

namespace OriBrainLearnerCore
{
    public class KthVectors
    {
        private string svm = ".libsvm";
        private string _filename;
        private string _directory;
        private string _outputfilename;
        private int _k;
        private string[] _lines;
        private List<int> _eventStartLocations =new List<int>();
        private List<string> _datasetVectors = new List<string>();
        
        public KthVectors(string filename, string directory/*, int k*/)
        {
            _filename = filename;
            _directory = directory;
            //_k = k;
        } 

        //""" this functions gets all the start indices events. we decide based on a rest condition followed by an event
        //this will work for protocols where rest was before the first event, it will fail to ge the first event if there is no rest before!""" 
        public void getEventStartLocations()
        {
            for (int i=0; i< _lines.Length - 1;i++)
            {
                if ((_lines[i][0] == '1') && (_lines[i + 1][0] != '1'))
                {
                    _eventStartLocations.Add(i + 1);
                }
            }
        }

        //""" "step 1 - vector selection" """
        //""" accumulate the Kth vectors from all events """
        public void  selectKthVectors()
        {
            for (int i=0; i< _eventStartLocations.Count; i++)
            {
                //if '1' is the class, replace the '1' class with the real event's class 
                if ((_eventStartLocations[i] + (_k - 1) < _lines.Length) && (_eventStartLocations[i] + (_k - 1) <= _eventStartLocations.Max() + (_k - 1)))
                {
                    _datasetVectors.Add(_lines[_eventStartLocations[i]][0] + _lines[_eventStartLocations[i] + (_k - 1)].Substring(1));
                }
            }
        }

        public void batchProcessPerK(List<int> listK)
        {
            //GuiPreferences.Instance.setLog(_filename);
            loadSourceFile();
            getEventStartLocations();
            //GuiPreferences.Instance.setLog(_eventStartLocations.Count.ToString() + " " +_eventStartLocations);
            for (int i=0; i< listK.Count; i++)
            {
                _datasetVectors = new List<string>();
                _k = listK[i];
                _outputfilename = _filename + "_" + _k.ToString() + "th_vectors";
                storePerTRLocations();
                selectKthVectors();
                writeFile();
            }
        }

        public void storePerTRLocations()
        {
            //save to preferences
            if (!Preferences.Instance.TrainingEventStartLocationsPerTR[Preferences.Instance.currentProcessedRun].ContainsKey(_k.ToString()))
            {
                List<int> perTRLocation = new List<int>();
                for (int i=0;i<_eventStartLocations.Count;i++)
                {
                    perTRLocation.Add(_eventStartLocations[i] + (_k - 1));
                }
                Preferences.Instance.TrainingEventStartLocationsPerTR[Preferences.Instance.currentProcessedRun].Add(_k.ToString(), new List<int>(perTRLocation));
            }
        }

        public void  loadSourceFile()
        {
            _lines = File.ReadAllLines(_directory + _filename + svm);
            //GuiPreferences.Instance.setLog(@"protocol should be this many lines: " + _lines.Length.ToString());
        }

        public void writeFile()
        {
            File.WriteAllLines(_directory + _outputfilename + svm, _datasetVectors); 
            GuiPreferences.Instance.setLog(@"file saved: " + _directory + _outputfilename + svm);
        }
    }

}
