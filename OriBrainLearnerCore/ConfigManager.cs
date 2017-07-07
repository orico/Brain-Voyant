using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OriBrainLearnerCore
{
    public class ConfigManager
    { 
        public static void saveConfigFile()
        {
            Preferences.Instance.configFile = new Dictionary<string, string>();

            Preferences.Instance.configFile.Add("WorkDirectory", GuiPreferences.Instance.WorkDirectory);
            Preferences.Instance.configFile.Add("ProtocolFile", GuiPreferences.Instance.ProtocolFile);
            Preferences.Instance.configFile.Add("dirList", string.Join(",", Preferences.Instance.dirList.Select(u => u)));
            Preferences.Instance.configFile.Add("dataType.rawValue", GuiPreferences.Instance.FileType.ToString());
            Preferences.Instance.configFile.Add("NudThreshold", GuiPreferences.Instance.NudThreshold.ToString());//400
            Preferences.Instance.configFile.Add("NudExtractFromTR", GuiPreferences.Instance.NudExtractFromTR.ToString());// = 3;
            Preferences.Instance.configFile.Add("NudExtractToTR", GuiPreferences.Instance.NudExtractToTR.ToString());// = 4;
            Preferences.Instance.configFile.Add("NudClassifyUsingTR", GuiPreferences.Instance.NudClassifyUsingTR.ToString());// = 4;
            Preferences.Instance.configFile.Add("NudIGThreshold", GuiPreferences.Instance.NudIGThreshold.ToString());// = 0.15M; //0.15 dona, keren 0.10
            Preferences.Instance.configFile.Add("NudIGVoxelAmount", GuiPreferences.Instance.NudIGVoxelAmount.ToString());// = 0.15M; //0.15 dona, keren 0.10
            Preferences.Instance.configFile.Add("NudFilterEyeSlices", GuiPreferences.Instance.NudFilterEyeSlices.ToString());// = 13; // 13 dona, keren 10
            Preferences.Instance.configFile.Add("NudEyeSliceFirstLines", GuiPreferences.Instance.NudEyeSliceFirstLines.ToString());// = 80; 
            Preferences.Instance.configFile.Add("CbPeekHigherTRsIGChecked", GuiPreferences.Instance.CbPeekHigherTRsIGChecked.ToString());// = false;
            Preferences.Instance.configFile.Add("NormalizationType", GuiPreferences.Instance.NormalizedType.ToString());//formula
            Preferences.Instance.configFile.Add("IgSelectionType", GuiPreferences.Instance.IgSelectionType.ToString());//ig type, threshold or voxel amount
            Preferences.Instance.configFile.Add("NudMovingWindow", GuiPreferences.Instance.NudMovingWindow.ToString());//window
            XMLSerializer.serializeArrayToFile<Dictionary<string, string>>(GuiPreferences.Instance.WorkDirectory + "config.xml", Preferences.Instance.configFile);
            
        }

        public static void ProtocolSafetyCheck()
        {
            string protocolFilenameOnly =
                GuiPreferences.Instance.ProtocolFile.Substring(GuiPreferences.Instance.ProtocolFile.LastIndexOf(@"\") + 1,
                                                               GuiPreferences.Instance.ProtocolFile.LastIndexOf(".") -
                                                               GuiPreferences.Instance.ProtocolFile.LastIndexOf(@"\") - 1);


            string configProtocolFilenameOnly =
                Preferences.Instance.configFile["ProtocolFile"].Substring(Preferences.Instance.configFile["ProtocolFile"].LastIndexOf(@"\") + 1,
                                                               Preferences.Instance.configFile["ProtocolFile"].LastIndexOf(".") -
                                                               Preferences.Instance.configFile["ProtocolFile"].LastIndexOf(@"\") - 1);

            if (GuiPreferences.Instance.ProtocolFile != Preferences.Instance.configFile["ProtocolFile"])
            {
                if (MessageBox.Show(
                    "You have loaded a different protocol FILENAME than the one in the CONFIG, Continue anyway? Prot: " + protocolFilenameOnly + " CFG: " + configProtocolFilenameOnly,
                    "My application", MessageBoxButtons.YesNo, MessageBoxIcon.Stop,
                    MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    Application.Exit();
                }
            }

        }

        public static void loadConfigFile()
        {
            Preferences.Instance.configFile = XMLSerializer.DeserializeFile<Dictionary<string, string>>(GuiPreferences.Instance.WorkDirectory + "config.xml");
            //GuiPreferences.Instance.WorkDirectory = Preferences.Instance.configFile["WorkDirectory"];


            //same protocol safety check
            ProtocolSafetyCheck();

            try
            {
                Preferences.Instance.dirList =
                    new List<string>(Preferences.Instance.configFile["dirList"].Split(new[] { ',' }));
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.setLog("WARNING: dirList missing must be an old version of config.xml, rerun this model");
            }

            try
            {
                GuiPreferences.Instance.NormalizedType = (NormalizationType)Enum.Parse(typeof(NormalizationType), Preferences.Instance.configFile["NormalizationType"]);                
                GuiPreferences.Instance.NudMovingWindow = Convert.ToDecimal(Preferences.Instance.configFile["NudMovingWindow"]);
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.NormalizedType = NormalizationType.MinMax;
                GuiPreferences.Instance.setLog("WARNING: NormalizationType missing must be an old version of config.xml, rerun this model, defaulting to minmax");
            }

            try
            {
                GuiPreferences.Instance.IgSelectionType = (IGType)Enum.Parse(typeof(IGType), Preferences.Instance.configFile["IgSelectionType"]);
                GuiPreferences.Instance.NudIGVoxelAmount = Convert.ToInt32(Preferences.Instance.configFile["NudIGVoxelAmount"]);
                GuiPreferences.Instance.NudIGThreshold = Convert.ToDecimal(Preferences.Instance.configFile["NudIGThreshold"]);

                if (GuiPreferences.Instance.IgSelectionType == IGType.Threshold)
                {
                    GuiPreferences.Instance.setLog("Filtering using IG threshold: " + GuiPreferences.Instance.NudIGThreshold.ToString());
                }
                else if (GuiPreferences.Instance.IgSelectionType == IGType.Voxels)
                {
                    GuiPreferences.Instance.setLog("Filtering using IG Voxel Amount of: " + GuiPreferences.Instance.NudIGVoxelAmount.ToString());
                }
            }
            catch (Exception e)
            {
                GuiPreferences.Instance.IgSelectionType = IGType.Threshold;
                GuiPreferences.Instance.NudIGThreshold = Convert.ToDecimal(Preferences.Instance.configFile["NudIGThreshold"]);
                GuiPreferences.Instance.NudIGVoxelAmount = 100; // default value
                GuiPreferences.Instance.setLog("WARNING: IG SELECTION TYPE is missing must be an old version of config.xml, rerun this model, defaulting to Threshold selection + correct threshold figure");
            }


            GuiPreferences.Instance.FileType = (DataType)Enum.Parse(typeof(DataType), Preferences.Instance.configFile["dataType.rawValue"]);
            GuiPreferences.Instance.NudThreshold = Convert.ToDecimal(Preferences.Instance.configFile["NudThreshold"]);
            GuiPreferences.Instance.NudExtractFromTR = Convert.ToDecimal(Preferences.Instance.configFile["NudExtractFromTR"]);
            GuiPreferences.Instance.NudExtractToTR = Convert.ToDecimal(Preferences.Instance.configFile["NudExtractToTR"]);
            GuiPreferences.Instance.NudClassifyUsingTR = Convert.ToDecimal(Preferences.Instance.configFile["NudClassifyUsingTR"]);
            GuiPreferences.Instance.NudFilterEyeSlices = Convert.ToDecimal(Preferences.Instance.configFile["NudFilterEyeSlices"]);
            GuiPreferences.Instance.NudEyeSliceFirstLines = Convert.ToDecimal(Preferences.Instance.configFile["NudEyeSliceFirstLines"]);
            GuiPreferences.Instance.CbPeekHigherTRsIGChecked = Convert.ToBoolean(Preferences.Instance.configFile["CbPeekHigherTRsIGChecked"]);
        }

    }
}
