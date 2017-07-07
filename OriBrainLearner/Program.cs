using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace OriBrainLearner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Form1 mainForm;
        [STAThread]

        static void Main()
        { 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new Form1();
            Application.ThreadException += new ThreadExceptionEventHandler(mainForm.UnhandledThreadExceptionHandler);
            Application.Run(mainForm);
        }
    }
}
