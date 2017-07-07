using System; 
using System.Media;
using System.Runtime.InteropServices; 
using System.Windows.Forms;

namespace OriBrainLearnerCore
{
    public class Sound
    {
        public static void PlayTada()
        {
            if (GuiPreferences.Instance.CbSoundONChecked)
            {
                try
                {
                    SoundPlayer sndplayr = new
                             SoundPlayer(OriBrainLearnerCore.Properties.Resources.tada);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(),
                                   "Error");
                }
            }
        }

        public static void PlayImportingFinished()
        {
            if (GuiPreferences.Instance.CbSoundONChecked)
            {
                try
                {
                    SoundPlayer sndplayr = new
                             SoundPlayer(OriBrainLearnerCore.Properties.Resources.ImportingFinished);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(),
                                   "Error");
                }
            }
        }

        public static void PlayProcessingFinished()
        {
            if (GuiPreferences.Instance.CbSoundONChecked)
            {
                try
                {
                    SoundPlayer sndplayr = new
                             SoundPlayer(OriBrainLearnerCore.Properties.Resources.ProcessingFinished);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(),
                                   "Error");
                }
            }
        }

        public static void PlayTrainingFinished()
        {
            if (GuiPreferences.Instance.CbSoundONChecked)
            {
                try
                {
                    SoundPlayer sndplayr = new
                             SoundPlayer(OriBrainLearnerCore.Properties.Resources.TrainingFinished);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(),
                                   "Error");
                }
            }
        }

        public static void PlayTestingFinished()
        {
            if (GuiPreferences.Instance.CbSoundONChecked)
            {
                try
                {
                    SoundPlayer sndplayr = new
                             SoundPlayer(OriBrainLearnerCore.Properties.Resources.TestingFinished);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(),
                                   "Error");
                }
            }
        }

        public static void PlayEverythingFinished()
        {
            if (GuiPreferences.Instance.CbSoundONChecked)
            {
                try
                {
                    SoundPlayer sndplayr = new
                             SoundPlayer(OriBrainLearnerCore.Properties.Resources.EverythingFinished);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(),
                                   "Error");
                }
            }
        }
        
        public enum beepType
        {
            /// <summary>
            /// A simple windows beep
            /// </summary>            
            SimpleBeep  = -1,
            /// <summary>
            /// A standard windows OK beep
            /// </summary>
            OK    = 0x00,
            /// <summary>
            /// A standard windows Question beep
            /// </summary>
            Question  = 0x20,
            /// <summary>
            /// A standard windows Exclamation beep
            /// </summary>
            Exclamation  = 0x30,
            /// <summary>
            /// A standard windows Asterisk beep
            /// </summary>
            Asterisk  = 0x40,
        }
        [DllImport("User32.dll", ExactSpelling=true)]
        private static extern bool MessageBeep(uint type);

        static void testall()
        {
            beep(beepType.Asterisk);
            
            beep(beepType.Exclamation);
            
            beep(beepType.OK);
            
            beep(beepType.Question);
            
            beep(beepType.SimpleBeep);
        }
        public static void beep(beepType type)
        {
            MessageBeep((uint)type);
        }
    }
}
