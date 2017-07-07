using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OriBrainLearnerCore
{
    public class Training_MultiRunPlotting
    {
        public static void hardCodedIGPlot()
        {
            string dicomDir = @"H:\EXPERIMENT3_AVATAR_ML\Rest voxels Analysis Via IG\Keren\MM\";
            string firstFile = @"001_000001_000001.dcm";
            bool thresholdOrVoxelAmount = false;  

            double[][]  externalRankedAttributes = new double[1024][]; 
            string text = File.ReadAllText(dicomDir+"rankedAttributes.csv");
            string[] farr = text.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int v = 0;
            foreach (string line in farr)
            {   
                string[] spl = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                
                externalRankedAttributes[v] = new double[2];
                externalRankedAttributes[v][0] = Convert.ToDouble(spl[1]);
                externalRankedAttributes[v][1] = Convert.ToDouble(spl[0]);
                v++;
            }

            Form plotForm = new DicomImageViewer.MainForm(dicomDir + firstFile, firstFile,
                                                            externalRankedAttributes,
                                                            Convert.ToDouble(GuiPreferences.Instance.NudIGThreshold),
                                                            Convert.ToInt32(1024),
                                                            thresholdOrVoxelAmount,
                                                            dicomDir + "brain.png");

            GuiPreferences.Instance.setLog("Automatically Saved brain.png image to final dir");
            plotForm.StartPosition = FormStartPosition.CenterParent;
            plotForm.ShowDialog();
            plotForm.Close(); 
        }

        public static void WekaPlotPipelineForMultiRuns(List<string> directoryList)
        {
            //display top IG on dicom view 
            if (Preferences.Instance.attsel == null)
            {
                GuiPreferences.Instance.setLog("there are no ranked IG attributes or selected attr, continuing but please fix this possible bug.");
            }

            string dicomDir = directoryList[0];
            if (dicomDir.Contains("rtp") == true)
                dicomDir = dicomDir.Substring(0, dicomDir.Length - 4) + @"master\";
            else
                dicomDir += @"master\"; 
            string[] files = System.IO.Directory.GetFiles(dicomDir, "*.dcm");
            if (files.Length > 0)
            {
                string firstFile = files[0].Substring(files[0].LastIndexOf(@"\") + 1);


                bool thresholdOrVoxelAmount;
                if (GuiPreferences.Instance.IgSelectionType == IGType.Threshold)
                {
                    thresholdOrVoxelAmount = true;
                }
                else
                {
                    thresholdOrVoxelAmount = false;
                } 

                Form plotForm = new DicomImageViewer.MainForm(dicomDir + firstFile, firstFile,
                                                              Preferences.Instance.attsel.rankedAttributes(),
                                                              Convert.ToDouble(GuiPreferences.Instance.NudIGThreshold),
                                                              Convert.ToInt32(GuiPreferences.Instance.NudIGVoxelAmount),
                                                              thresholdOrVoxelAmount,
                                                              GuiPreferences.Instance.WorkDirectory + "brain.png");

                GuiPreferences.Instance.setLog("Automatically Saved brain.png image to final dir");
                plotForm.StartPosition = FormStartPosition.CenterParent;
                plotForm.ShowDialog();
                plotForm.Close();
            }
            else
            {
                GuiPreferences.Instance.setLog("Dicom Plot + IG overlay cant be displayed, .dcm file doesnt exist");
            }
        }
    }
}
