using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OriBrainLearnerCore;

namespace OriBrainLearner
{
    public partial class Form2 : Form
    {
        public int defaultFrom, defaultTo, maxTo, minTo;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            defaultFrom = GuiPreferences.Instance.FromTR;
            defaultTo = GuiPreferences.Instance.ToTR;
            maxTo = GuiPreferences.Instance.MaxTR;
            minTo = GuiPreferences.Instance.MinTR;

            if (Preferences.Instance.svmLoaded)
            {
                nudTo.Maximum = GuiPreferences.Instance.MaxTR;
                nudTo.Value = GuiPreferences.Instance.ToTR;
                nudTo.Minimum = GuiPreferences.Instance.MinTR;
            }
            else
            {
                nudTo.Value = GuiPreferences.Instance.ToTR;
                nudTo.Minimum = GuiPreferences.Instance.MinTR;
                nudTo.Maximum = GuiPreferences.Instance.MaxTR;
            }
            GuiPreferences.Instance.TrUpdateEvent += new EventHandler<GuiPreferences.TrChangeEventArg>(Instance_TrUpdateEvent);
        }

        private void Instance_TrUpdateEvent(object sender, GuiPreferences.TrChangeEventArg t)
        {
            nudFrom.Maximum = t._maxTr;
            nudFrom.Value = t._fromTR;
            nudFrom.Minimum = t._minTR;

            nudTo.Maximum = t._maxTr; 
            nudTo.Value = t._toTR;
            nudTo.Minimum = t._minTR;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GuiPreferences.Instance.FromTR = defaultFrom;
            GuiPreferences.Instance.ToTR = defaultTo;
            GuiPreferences.Instance.MaxTR = maxTo;
            GuiPreferences.Instance.MinTR = minTo;
            this.Close();
        }

        private void nudFrom_ValueChanged(object sender, EventArgs e)
        {
            int changedValue = Convert.ToInt32(nudFrom.Value);
            GuiPreferences.Instance.FromTR = changedValue;
            if (GuiPreferences.Instance.FromTR != changedValue)
                nudFrom.Value = GuiPreferences.Instance.FromTR;
        }

        private void nudTo_ValueChanged(object sender, EventArgs e)
        {
            int changedValue = Convert.ToInt32(nudTo.Value);
            GuiPreferences.Instance.ToTR = changedValue;
            if (GuiPreferences.Instance.ToTR != changedValue)
                nudTo.Value = GuiPreferences.Instance.ToTR;
        }


    }
}
