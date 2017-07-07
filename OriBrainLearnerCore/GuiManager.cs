using System;
using System.IO;

namespace OriBrainLearnerCore
{
    public class GuiManager
    {


        public static void getFilePaths(string extension)
        {
            string WorkingDir = GuiPreferences.Instance.WorkDirectory;
            string FileName = GuiPreferences.Instance.FileName;
            //find how many files are there
            string[] filePaths = Directory.GetFiles(WorkingDir, extension);
            Preferences.Instance.filesInWorkingDir = new string[filePaths.Length];
            Array.Copy(filePaths, Preferences.Instance.filesInWorkingDir, filePaths.Length);
        }


        public static void updateFilePaths()
        {
            GuiPreferences.Instance.setLog("-------------------------------------------------");
            GuiPreferences.Instance.setLog("Processing Folder: " + GuiPreferences.Instance.WorkDirectory);

            GuiPreferences.Instance.setLog("Total Files in Run: " + Preferences.Instance.filesInWorkingDir.Length);
            GuiPreferences.Instance.TbTRs = Preferences.Instance.filesInWorkingDir.Length;

            updateFromTo();
            GuiPreferences.Instance.TbTRs = GuiPreferences.Instance.getMaxAvailableTrs();
            GuiPreferences.Instance.setLog("Starting..");
        }


        public static void updateFromTo()
        {
            //on load update all the values including min and max.

            // then we change to the new values                
            if (GuiPreferences.Instance.FromTR == 0)
            {
                GuiPreferences.Instance.MaxTR = GuiPreferences.Instance.getMaxAvailableTrs();
                GuiPreferences.Instance.FromTR = 1;
                GuiPreferences.Instance.ToTR = GuiPreferences.Instance.getMaxAvailableTrs();
                GuiPreferences.Instance.MinTR = 1;
            }
            else
            {
                GuiPreferences.Instance.FromTR = 1;
                if (GuiPreferences.Instance.ToTR >= GuiPreferences.Instance.getMaxAvailableTrs())
                {
                    GuiPreferences.Instance.ToTR = GuiPreferences.Instance.getMaxAvailableTrs();
                    GuiPreferences.Instance.MaxTR = GuiPreferences.Instance.getMaxAvailableTrs();
                }
                else if (GuiPreferences.Instance.ToTR < GuiPreferences.Instance.getMaxAvailableTrs())
                {
                    GuiPreferences.Instance.MaxTR = GuiPreferences.Instance.getMaxAvailableTrs();
                    GuiPreferences.Instance.ToTR = GuiPreferences.Instance.getMaxAvailableTrs();
                }
                //Preferences.Instance.MinTR = 1;
            }

        }
    }
}
