using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using libsvm;
using System.Windows.Forms;
using com.sun.codemodel.@internal.util;
using java.io;
using java.util;
using java.lang;
using java.text;
using weka.classifiers;
using weka.core;
using weka.classifiers.functions;
using weka.core.converters;
using weka.filters;
using OriBrainLearnerCore;

namespace LibSVMScale
{
    //min = test baseline median - 2nd smallest (training baseline median - training min)
    //max = test baseline median + 2nd smallest (training baseline median - training max)

    //1. test baseline median is calculated for K IG features after the baseline event finishes.
    //2. training min/max are saved for EVERY feature using the MINMAX class.
    //3. training baseline median is saved for EVEREY feature using the statistics class. the only reason to save all of the values for every feature is
    //that median is HARD to calculate from a stream without keeping all the data. but for the K baseline TRs its not RAM heavy.

    public class Normalize_RawMinusMedWinIqrWin : NormalizationInterface
    {
        //for single vector normalization, config file only loaded ONCE
        private bool configFileLoaded = false;

        private System.String line = null;
        private double lower = -1.0;
        private double upper = 1.0;
        private double y_lower;
        private double y_upper;
        private bool y_scaling = false;
        private double[] feature_max;
        private double[] feature_min;
        private double y_max = -System.Double.MaxValue;
        private double y_min = System.Double.MaxValue;
        private int max_index;
        private long num_nonzeros = 0;
        private long zeros = 0;
        private long new_num_nonzeros = 0;
        int counter = 0;


        BufferedReader fp = null, fp_restore_java = null;
        System.IO.StreamReader fp_restore = null;
        System.String save_filename = null;
        System.String restore_filename = null;
        System.String data_filename = null;
        System.String save_filenameoutput = null;
        private string TR;
        private int currentLine;
        FileStream ostrm = null;
        StreamWriter writer = null;
        TextWriter oldOut = System.Console.Out;

        public bool ConfigFileLoaded
        {
            get { return configFileLoaded; }
            set { configFileLoaded = value; }
        }


        public void exit_with_help()
	    {
            // do what you want here.
            if (MessageBox.Show("An unexpected error has occurred in svm_scale_java. \n possibly due to WHITE SPACE in the directory name or other input argument error\n Continue?\n" 
                + "svm-scale converstion to csharp using (java IKVM Dlls), Ori Cohen orioric@gmail.com\n"
                + "Usage: svm-scale [options] data_filename\n"
                + "options:\n"
                + "-l lower : x scaling lower limit (default -1)\n"
                + "-u upper : x scaling upper limit (default +1)\n"
                + "-y y_lower y_upper : y scaling limits (default: no y scaling)\n"
                + "-s save_filename : save scaling parameters to save_filename\n"
                + "-r restore_filename : restore scaling parameters from restore_filename\n"
                    // my addition - ori
                + "-o save_filenameoutput : redirects output to save_filenameoutput\n",
                "My application", MessageBoxButtons.YesNo, MessageBoxIcon.Stop,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                Application.Exit();
            }
	    }

        public BufferedReader rewind(BufferedReader fp, System.String filename) 
        {
            fp.close();
            return new BufferedReader(new FileReader(filename));
        }

        public void output_target(double value)
        {
            // not needed in raw-med etc
		    /*if(y_scaling)
		    {
			    if(value == y_min)
				    value = y_lower;
			    else if(value == y_max)
				    value = y_upper;
			    else
				    value = y_lower + (y_upper-y_lower) *
				    (value-y_min) / (y_max-y_min);
		    }*/

		    System.Console.Write(value + " ");
	    }


        private bool useGrowingWindow = false; //was true 17\2\2014
        /// <summary>
        /// keeps the entire range of features, even if low==high min==max.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void output(int index, double value)
        {
            //in range we sometime get low==high. 

            if (index==158271)
            {
                int stop = 1;
            }

            //if (Preferences.Instance.currentClassifiedVector < Preferences.Instance.events.eventList[0].var2)
                //return;
            double[] medianIQR = Preferences.Instance.TrainingBaselineMediansRunningWindow[Preferences.Instance.currentProcessedRun].
                TrainingCalcMedIQRRunningWindow(index - 1, TR, currentLine, useGrowingWindow);

            double normalized_value;
            if (medianIQR[1] == 0f)
            {
                //GuiPreferences.Instance.setLog("Dividing by zero in Normalization IQR. bug, or just something else? " + index.ToString() + "  : " + value.ToString());
                normalized_value = 0;
                zeros++;
            }
            else
                normalized_value = (value - medianIQR[0]) / medianIQR[1];
             
            value = normalized_value;
            System.Console.Write(index + ":" + value + " "); //console redirected to output to a file 
            //actually non zero and positive numbers
            new_num_nonzeros++;
        }
        
        public void outputRealTime(int index, ref double value)
        {}

        private double min, max;
        
        public void outputRealTime_update(int index, ref double value)
        {

            if (Preferences.Instance.currentClassifiedVector < Preferences.Instance.events.eventList[0].var2)
                return; 

            //if (Preferences.Instance.currentClassifiedVector < Preferences.Instance.events.eventList[0].var2)
            //return;
            double[] medianIQR = Preferences.Instance.TestingBaselineMediansRunningWindow.TestingCalcMedIQRRunningWindow(index - 1, Preferences.Instance.currentClassifiedVector,useGrowingWindow);

            double normalized_value;
            if (medianIQR[1] == 0f)
            {
                GuiPreferences.Instance.setLog("Dividing by zero in Normalization IQR. bug, or just something else? " + index.ToString() + "  : " + value.ToString());
                normalized_value = 0;
                zeros++;
            }
            else
                normalized_value = (value - medianIQR[0]) / medianIQR[1];

            value = normalized_value;
            System.Console.Write(index + ":" + value + " "); //console redirected to output to a file 
            //actually non zero and positive numbers
            new_num_nonzeros++;
        }


        public System.String readline(BufferedReader fp) 
        {
            line = fp.readLine();
            return line;
        }

        //legacy function, has it all and its working.
        public void run(System.String[] argv)
	    {
		    int i,index;
		    BufferedReader fp = null, fp_restore = null;
		    System.String save_filename = null;
		    System.String restore_filename = null;
		    System.String data_filename = null;
		    System.String save_filenameoutput = null;

		    for(i=0;i<argv.Count();i++)
		    {
			    if (argv[i][0] != '-')	break;
			    ++i;

                if (argv[i-1][1]=='l')
                { 
                    lower = System.Double.Parse(argv[i]); 
                }
                else if (argv[i-1][1]=='u')
			    {  
                    upper = System.Double.Parse(argv[i]); 
                }
                else if (argv[i-1][1]=='y')
			    { 
                    y_lower = System.Double.Parse(argv[i]);
                    ++i;
                    y_upper = System.Double.Parse(argv[i]);
                    y_scaling = true; 
                }
                else if (argv[i-1][1]=='s')
			    { 
                    save_filename = argv[i];	 
                }
                else if (argv[i-1][1]=='r')
			    { 
                    restore_filename = argv[i];	 
                }
                else if (argv[i - 1][1] == 'o')
                {
                    save_filenameoutput = argv[i];
                }
                else
                {
                    System.Console.Error.WriteLine("unknown option");
                    exit_with_help();
                }

			    /*switch(argv[i-1][1])
			    {
				    case 'l': lower = System.Double.Parse(argv[i]);	break;
                    case 'u': upper = System.Double.Parse(argv[i]); break;
				    case 'y':
                        y_lower = System.Double.Parse(argv[i]);
					      ++i;
                          y_upper = System.Double.Parse(argv[i]);
					      y_scaling = true;
					      break;
				    case 's': save_filename = argv[i];	break;
				    case 'r': restore_filename = argv[i];	break;
				    default:
					      System.Console.Error.WriteLine("unknown option");
					      exit_with_help();
			    }*/
		    }

		    if(!(upper > lower) || (y_scaling && !(y_upper > y_lower)))
		    {
                System.Console.Error.WriteLine("inconsistent lower/upper specification");
                //Environment.Exit(1);
		    }
		    if(restore_filename != null && save_filename != null)
		    {
                System.Console.Error.WriteLine("cannot use -r and -s simultaneously");
                //Environment.Exit(1);
		    }

		    if(argv.Count() != i+1)
			    exit_with_help();

		    data_filename = argv[i];
		    try {
			    fp = new BufferedReader(new FileReader(data_filename));
		    } catch (java.lang.Exception e) {
                System.Console.Error.WriteLine("can't open file " + data_filename);
                //Environment.Exit(1);
		    }

		    /* assumption: min index of attributes is 1 */
		    /* pass 1: find out max index of attributes */
		    max_index = 0;

		    if(restore_filename != null)
		    {
			    int idx, c;

			    try {
				    fp_restore = new BufferedReader(new FileReader(restore_filename));
			    }
                catch (java.lang.Exception e)
                {
                    System.Console.Error.WriteLine("can't open file " + restore_filename);
                    //Environment.Exit(1);
			    }
			    if((c = fp_restore.read()) == 'y')
			    {
				    fp_restore.readLine();
				    fp_restore.readLine();		
				    fp_restore.readLine();		
			    }
			    fp_restore.readLine();
			    fp_restore.readLine();

			    System.String restore_line = null;
			    while((restore_line = fp_restore.readLine())!=null)
			    {
				    java.util.StringTokenizer st2 = new java.util.StringTokenizer(restore_line);
				    idx = Int32.Parse(st2.nextToken());
                    max_index = System.Math.Max(max_index, idx);
			    }
			    fp_restore = rewind(fp_restore, restore_filename);
		    }

		    while (readline(fp) != null)
		    {
			    java.util.StringTokenizer st = new java.util.StringTokenizer(line," \t\n\r\f:");
			    st.nextToken();
			    while(st.hasMoreTokens())
			    {
				    index = Int32.Parse(st.nextToken());
                    max_index = System.Math.Max(max_index, index);
				    st.nextToken();
				    num_nonzeros++;
			    }
		    }

		    try {
			    feature_max = new double[(max_index+1)];
			    feature_min = new double[(max_index+1)];
		    } catch(OutOfMemoryException e) {
                System.Console.Error.WriteLine("can't allocate enough memory");
                //Environment.Exit(1);
		    }

		    for(i=0;i<=max_index;i++)
		    {
                feature_max[i] = -System.Double.MaxValue;
                feature_min[i] = System.Double.MaxValue;
		    }

		    fp = rewind(fp, data_filename);

		    /* pass 2: find out min/max value */
		    while(readline(fp) != null)
		    {
			    int next_index = 1;
			    double target;
			    double value;

			    java.util.StringTokenizer st = new java.util.StringTokenizer(line," \t\n\r\f:");
                target = System.Double.Parse(st.nextToken());
                y_max = System.Math.Max(y_max, target);
                y_min = System.Math.Min(y_min, target);

			    while (st.hasMoreTokens())
			    {
				    index = Int32.Parse(st.nextToken());
                    value = System.Double.Parse(st.nextToken());

				    for (i = next_index; i<index; i++)
				    {
                        feature_max[i] = System.Math.Max(feature_max[i], 0);
                        feature_min[i] = System.Math.Min(feature_min[i], 0);
				    }

                    feature_max[index] = System.Math.Max(feature_max[index], value);
                    feature_min[index] = System.Math.Min(feature_min[index], value);
				    next_index = index + 1;
			    }

			    for(i=next_index;i<=max_index;i++)
			    {
                    feature_max[i] = System.Math.Max(feature_max[i], 0);
                    feature_min[i] = System.Math.Min(feature_min[i], 0);
			    }
		    }

		    fp = rewind(fp, data_filename);

		    /* pass 2.5: save/restore feature_min/feature_max */
		    if(restore_filename != null)
		    {
			    // fp_restore rewinded in finding max_index 
			    int idx, c;
			    double fmin, fmax;

			    fp_restore.mark(2);				// for reset
			    if((c = fp_restore.read()) == 'y')
			    {
				    fp_restore.readLine();		// pass the '\n' after 'y'
				    java.util.StringTokenizer st = new java.util.StringTokenizer(fp_restore.readLine());
                    y_lower = System.Double.Parse(st.nextToken());
                    y_upper = System.Double.Parse(st.nextToken());
				    st = new java.util.StringTokenizer(fp_restore.readLine());
                    y_min = System.Double.Parse(st.nextToken());
                    y_max = System.Double.Parse(st.nextToken());
				    y_scaling = true;
			    }
			    else
				    fp_restore.reset();

			    if(fp_restore.read() == 'x') {
				    fp_restore.readLine();		// pass the '\n' after 'x'
				    java.util.StringTokenizer st = new java.util.StringTokenizer(fp_restore.readLine());
                    lower = System.Double.Parse(st.nextToken());
                    upper = System.Double.Parse(st.nextToken());
				    System.String restore_line = null;
				    while((restore_line = fp_restore.readLine())!=null)
				    {
					    java.util.StringTokenizer st2 = new java.util.StringTokenizer(restore_line);
					    idx = Int32.Parse(st2.nextToken());
                        fmin = System.Double.Parse(st2.nextToken());
                        fmax = System.Double.Parse(st2.nextToken());
					    if (idx <= max_index)
					    {
						    feature_min[idx] = fmin;
						    feature_max[idx] = fmax;
					    }
				    }
			    }
			    fp_restore.close();
		    }

            // needs to figure out why .16g thing isnt working in runtime..it is java after all and working inthe original.
		    /*if(save_filename != null)
		    {
                java.util.Formatter formatter = new java.util.Formatter(new java.lang.StringBuilder());
                java.io.BufferedWriter fp_save = null;

			    try {
                    fp_save = new java.io.BufferedWriter(new java.io.FileWriter(save_filename));
			    } catch(java.io.IOException e) {
                    System.Console.Error.WriteLine("can't open file " + save_filename);
                    Environment.Exit(1);
			    }

			    if(y_scaling)
			    {
				    formatter.format("y\n");
				    formatter.format("%.16g %.16g\n", y_lower, y_upper);
				    formatter.format("%.16g %.16g\n", y_min, y_max);
			    }
			    formatter.format("x\n");
			    formatter.format("%.16g %.16g\n", lower, upper);
			    for(i=1;i<=max_index;i++)
			    {
				    if(feature_min[i] != feature_max[i]) 
					    formatter.format("%d %.16g %.16g\n", i, feature_min[i], feature_max[i]);
			    }
			    fp_save.write(formatter.toString());
			    fp_save.close();
		    }*/

            FileStream ostrm = null;
            StreamWriter writer = null; 
            TextWriter oldOut = System.Console.Out;

            if (save_filenameoutput != null)
            {
                try
                {
                    ostrm = new FileStream(save_filenameoutput, FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new StreamWriter(ostrm);
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine("Cannot open Redirect.txt for writing");
                    System.Console.WriteLine(e.Message);
                    return;
                }
                System.Console.SetOut(writer);
            }

		    /* pass 3: scale */
		    while(readline(fp) != null)
		    {
			    int next_index = 1;
			    double target;
			    double value;

			    java.util.StringTokenizer st = new java.util.StringTokenizer(line," \t\n\r\f:");
                target = System.Double.Parse(st.nextToken());
			    output_target(target);
			    while(st.hasMoreElements())
			    {
				    index = Int32.Parse(st.nextToken());
                    value = System.Double.Parse(st.nextToken());
				    for (i = next_index; i<index; i++)
					    output(i, 0);
				    output(index, value);
				    next_index = index + 1;
			    }

			    for(i=next_index;i<= max_index;i++)
				    output(i, 0);
			    System.Console.Write("\n");
		    }


            if (save_filenameoutput != null)
            {
                if (writer!=null)
                    writer.Close();
                if (ostrm != null)
                    ostrm.Close();
                System.Console.SetOut(oldOut);
            }

		    if (new_num_nonzeros > num_nonzeros)
			    System.Console.Error.WriteLine(
			     "WARNING: original #nonzeros " + num_nonzeros+"\n"
			    +"         new      #nonzeros " + new_num_nonzeros+"\n"
			    +"Use -l 0 if many original feature values are zeros\n");

		    fp.close();

	    }
         
        public void parseArguments(System.String[] argv)
        { 
            int i; 

            //parse arguments
            for (i = 0; i < argv.Count(); i++)
            {
                if (argv[i][0] != '-') break;
                ++i;

                if (argv[i - 1][1] == 'l')
                {
                    lower = System.Double.Parse(argv[i]);
                }
                else if (argv[i - 1][1] == 'u')
                {
                    upper = System.Double.Parse(argv[i]);
                }
                else if (argv[i - 1][1] == 'y')
                {
                    y_lower = System.Double.Parse(argv[i]);
                    ++i;
                    y_upper = System.Double.Parse(argv[i]);
                    y_scaling = true;
                }
                else if (argv[i - 1][1] == 's') //file to save configuration from.
                {
                    save_filename = argv[i];
                }
                else if (argv[i - 1][1] == 'r') //file to restore configuration from.
                {
                    restore_filename = argv[i];
                }
                else if (argv[i - 1][1] == 'o') //normalized output file
                {
                    save_filenameoutput = argv[i];
                } 
                else if (argv[i - 1][1] == 'T') //custom option i added which TR are we evaluating.
                {
                    TR = argv[i];
                }
                else
                {
                    GuiPreferences.Instance.setLog("unknown option " + argv[i]); 
                }
            }

            //validate if upper lower and scaling values are logical
            validateValues();

            if (argv.Count() != i + 1)
                exit_with_help();

            data_filename = argv[i];

        }

        public void validateValues()
        {
            // check if values are logical in size.
            if (!(upper > lower) || (y_scaling && !(y_upper > y_lower)))
            {
                System.Console.Error.WriteLine("inconsistent lower/upper specification");
                //Environment.Exit(1);
            }

            if (restore_filename != null && save_filename != null)
            {
                System.Console.Error.WriteLine("cannot use -r and -s simultaneously");
                //Environment.Exit(1);
            }
        }

        public void openInputFile()
        {
            try
            {
                fp = new BufferedReader(new FileReader(data_filename));
            }
            catch (java.lang.Exception e)
            {
                System.Console.Error.WriteLine("can't open file " + data_filename);
                //Environment.Exit(1);
            }
        } 

        public void openOutputFile()
        { 
            if (save_filenameoutput != null)
            {
                try
                {
                    ostrm = new FileStream(save_filenameoutput, FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new StreamWriter(ostrm);
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine("Cannot open Redirect.txt for writing");
                    System.Console.WriteLine(e.Message);
                    return;
                }
                System.Console.SetOut(writer);
            }
        }

        //find max_index from the restore file
        public void restoreCofingFileGetMaxIndex()
        {
            if (restore_filename != null)
            {
                int idx, c;

                try
                {
                    fp_restore_java = new BufferedReader(new FileReader(restore_filename));
                }
                catch (java.lang.Exception e)
                {
                    System.Console.Error.WriteLine("can't open file " + restore_filename);
                    //Environment.Exit(1);
                }
                if ((c = fp_restore_java.read()) == 'y')
                {
                    fp_restore_java.readLine();
                    fp_restore_java.readLine();
                    fp_restore_java.readLine();
                }
                fp_restore_java.readLine();
                fp_restore_java.readLine();

                System.String restore_line = null;
                while ((restore_line = fp_restore_java.readLine()) != null)
                {
                    java.util.StringTokenizer st2 = new java.util.StringTokenizer(restore_line);
                    idx = Int32.Parse(st2.nextToken());
                    max_index = System.Math.Max(max_index, idx);
                }
                fp_restore_java = rewind(fp_restore_java, restore_filename);
            }
        }

        public void restoreConfigMinMaxGetValues()
        {
            if (restore_filename != null)
            {
                // fp_restore rewinded in finding max_index 
                int idx, c;
                double fmin, fmax;

                fp_restore_java.mark(2);				// for reset
                if ((c = fp_restore_java.read()) == 'y')
                {
                    fp_restore_java.readLine();		// pass the '\n' after 'y'
                    java.util.StringTokenizer st = new java.util.StringTokenizer(fp_restore_java.readLine());
                    y_lower = System.Double.Parse(st.nextToken());
                    y_upper = System.Double.Parse(st.nextToken());
                    st = new java.util.StringTokenizer(fp_restore_java.readLine());
                    y_min = System.Double.Parse(st.nextToken());
                    y_max = System.Double.Parse(st.nextToken());
                    y_scaling = true;
                }
                else
                    fp_restore_java.reset();

                if (fp_restore_java.read() == 'x')
                {
                    fp_restore_java.readLine();		// pass the '\n' after 'x'
                    java.util.StringTokenizer st = new java.util.StringTokenizer(fp_restore_java.readLine());
                    lower = System.Double.Parse(st.nextToken());
                    upper = System.Double.Parse(st.nextToken());
                    System.String restore_line = null;
                    while ((restore_line = fp_restore_java.readLine()) != null)
                    {
                        java.util.StringTokenizer st2 = new java.util.StringTokenizer(restore_line);
                        idx = Int32.Parse(st2.nextToken());
                        fmin = System.Double.Parse(st2.nextToken());
                        fmax = System.Double.Parse(st2.nextToken());
                        if (idx <= max_index)
                        {
                            feature_min[idx] = fmin;
                            feature_max[idx] = fmax;
                        }
                    }
                }
                fp_restore_java.close();
            }
        }

        //find max_index from the restore file
        public void restoreCofingFileGetMaxIndexCsharp()
        {

            fp_restore = null;
            if (restore_filename != null)
            {
                int idx, c;

                try
                {
                    fp_restore = new StreamReader(restore_filename);
                }
                catch (System.Exception e)
                {
                    System.Console.Error.WriteLine("can't open file " + restore_filename);
                    //Environment.Exit(1);
                }
                if (fp_restore.Read() == 'y')
                {
                    fp_restore.ReadLine();
                    fp_restore.ReadLine();
                    fp_restore.ReadLine();
                }
                fp_restore.ReadLine();
                fp_restore.ReadLine();

                string restore_line = null;
                while ((restore_line = fp_restore.ReadLine()) != null)
                {
                    idx = int.Parse(restore_line.Split(',')[0]);
                    max_index = System.Math.Max(max_index, idx);
                }

                //rewind stream.
                fp_restore.BaseStream.Position = 0;
                fp_restore.DiscardBufferedData();
            }
        }


        public void restoreConfigMinMaxGetValuesCsharp()
        {
            if (restore_filename != null)
            {
                // fp_restore rewinded in finding max_index 
                int idx;
                double fmin, fmax;
                string restore_line; 

                if (fp_restore.Read() == 'y')
                {
                    fp_restore.ReadLine();		                // pass the '\n' after 'y'
                    restore_line = fp_restore.ReadLine();		
                    string [] restore_line_ar = restore_line.Split(',');
                    y_lower = double.Parse(restore_line_ar[0]);
                    y_upper = double.Parse(restore_line_ar[1]);

                    restore_line = fp_restore.ReadLine();		
                    restore_line_ar = restore_line.Split(',');
                    y_min = double.Parse(restore_line_ar[0]);
                    y_max = double.Parse(restore_line_ar[1]); 
                    y_scaling = true;
                }
                else
                {
                    fp_restore.BaseStream.Position = 0;
                    fp_restore.DiscardBufferedData();
                }

                if (fp_restore.Read() == 'x')
                {
                    fp_restore.ReadLine();		// pass the '\n' after 'x' 
                    restore_line = fp_restore.ReadLine();
                    string[] restore_line_ar = restore_line.Split(',');
                    lower = double.Parse(restore_line_ar[0]);
                    upper = double.Parse(restore_line_ar[1]);

                    restore_line = null;
                    restore_line_ar = null;
                    while ((restore_line = fp_restore.ReadLine()) != null)
                    { 
                        restore_line_ar = restore_line.Split(',');
                        idx  = int.Parse(restore_line_ar[0]);
                        fmin = double.Parse(restore_line_ar[1]);
                        fmax = double.Parse(restore_line_ar[2]);
                        if (idx <= max_index) // this will ignore index 204801 if the max is 204800
                        {
                            feature_min[idx] = fmin;
                            feature_max[idx] = fmax;
                        }
                    }
                }
                fp_restore.Close();
            }
        }


        /// <summary>
        /// was a static function, returns max index, min values, max values arrays as a reference
        /// </summary>
        public void getConfigFileMinMaxValues(string restore_filename, ref double[] feature_max, ref double[] feature_min, ref int max_index)
        {
            max_index = -1;
            StreamReader fp_restore;
            fp_restore = null;
            if (restore_filename != null)
            {
                int idx, c; 
                double fmin, fmax;
                string restore_line; 
                double lower = -1.0;
                double upper = 1.0;
                double y_lower;
                double y_upper;
                bool y_scaling = false; 
                double y_max = -System.Double.MaxValue;
                double y_min = System.Double.MaxValue; 

                //open file.
                try
                {
                    fp_restore = new StreamReader(restore_filename);
                }
                catch (System.Exception e)
                {
                    System.Console.Error.WriteLine("can't open file " + restore_filename);
                }

                //STEP - 1: find max index
                if (fp_restore.Read() == 'y')
                {
                    fp_restore.ReadLine();
                    fp_restore.ReadLine();
                    fp_restore.ReadLine();
                }
                fp_restore.ReadLine();
                fp_restore.ReadLine();

                restore_line = null;
                while ((restore_line = fp_restore.ReadLine()) != null)
                {
                    idx = int.Parse(restore_line.Split(',')[0]);
                    max_index = System.Math.Max(max_index, idx);
                }

                //STEP2: allocate memory
                try
                {
                    feature_max = new double[(max_index + 1)];
                    feature_min = new double[(max_index + 1)];
                }
                catch (OutOfMemoryException e)
                {
                    System.Console.Error.WriteLine("can't allocate enough memory");
                }

                //rewind stream.
                fp_restore.BaseStream.Position = 0;
                fp_restore.DiscardBufferedData();
                
                //STEP - 3: find all min/max values
                if (fp_restore.Read() == 'y')
                {
                    fp_restore.ReadLine();		                // pass the '\n' after 'y'
                    restore_line = fp_restore.ReadLine();
                    string[] restore_line_ar = restore_line.Split(',');
                    y_lower = double.Parse(restore_line_ar[0]);
                    y_upper = double.Parse(restore_line_ar[1]);

                    restore_line = fp_restore.ReadLine();
                    restore_line_ar = restore_line.Split(',');
                    y_min = double.Parse(restore_line_ar[0]);
                    y_max = double.Parse(restore_line_ar[1]);
                    y_scaling = true;
                }
                else
                {
                    fp_restore.BaseStream.Position = 0;
                    fp_restore.DiscardBufferedData();
                }

                if (fp_restore.Read() == 'x')
                {
                    fp_restore.ReadLine();		// pass the '\n' after 'x' 
                    restore_line = fp_restore.ReadLine();
                    string[] restore_line_ar = restore_line.Split(',');
                    lower = double.Parse(restore_line_ar[0]);
                    upper = double.Parse(restore_line_ar[1]);

                    restore_line = null;
                    restore_line_ar = null;
                    while ((restore_line = fp_restore.ReadLine()) != null)
                    {
                        restore_line_ar = restore_line.Split(',');
                        idx = int.Parse(restore_line_ar[0]);
                        fmin = double.Parse(restore_line_ar[1]);
                        fmax = double.Parse(restore_line_ar[2]);
                        if (idx <= max_index) // this will ignore index 204801 if the max is 204800
                        {
                            feature_min[idx] = fmin;
                            feature_max[idx] = fmax;
                        }
                    }
                }
                fp_restore.Close();
            }
        }

        //needs to figure out why .16g thing isnt working in runtime..it is java after all and working inthe original.
        public void saveConfigMinMax_java()
        {
            if (save_filename != null)
            {
                java.util.Formatter formatter = new java.util.Formatter(new java.lang.StringBuilder());
                java.io.BufferedWriter fp_save = null;

                try
                {
                    fp_save = new java.io.BufferedWriter(new java.io.FileWriter(save_filename));
                }
                catch (java.io.IOException e)
                {
                    System.Console.Error.WriteLine("can't open file " + save_filename);
                    //Environment.Exit(1);
                }

                if (y_scaling)
                {
                    formatter.format("y\n");
                    formatter.format("%.16g %.16g\n", y_lower, y_upper);
                    formatter.format("%.16g %.16g\n", y_min, y_max);
                }
                formatter.format("x\n");
                formatter.format("%.16g %.16g\n", lower, upper);
                for (int i = 1; i <= max_index; i++)
                {
                    if (feature_min[i] != feature_max[i])
                        formatter.format("%d %.16g %.16g\n", i, feature_min[i], feature_max[i]);
                }
                fp_save.write(formatter.toString());
                fp_save.close();
            }
        }

        //replacement for "saveConfigMinMax_java" and it appears to be working properly
        public void saveConfigMinMax_CSharp()
        {
            if (save_filename != null)
            { 
                System.IO.StreamWriter fp_save = null; 
                try
                {
                    fp_save = new System.IO.StreamWriter(save_filename);
                }
                catch (System.Exception e)
                {
                    System.Console.Error.WriteLine("can't open file " + save_filename);
                    //Environment.Exit(1);
                }

                if (y_scaling)
                {
                    fp_save.WriteLine("y");
                    fp_save.WriteLine(y_lower.ToString() + "," + y_upper.ToString());
                    fp_save.WriteLine(y_min.ToString() + "," + y_max.ToString());  
                }

                fp_save.WriteLine("x");
                fp_save.WriteLine(lower.ToString() + "," + upper.ToString()); 
                for (int i = 1; i <= max_index; i++)
                {
                    if (feature_min[i] != feature_max[i])
                        fp_save.WriteLine(i.ToString() + "," + feature_min[i].ToString() + "," + feature_max[i].ToString());  
                } 
                fp_save.Close();
            }
        }

        public void saveConfigMinMax_CSharp(string save_filename, double[] feature_min, double[] feature_max, int max_index, double lower, double upper)
        {
            if (save_filename != null)
            {
                System.IO.StreamWriter fp_save = null;
                try
                {
                    fp_save = new System.IO.StreamWriter(save_filename);
                }
                catch (System.Exception e)
                {
                    System.Console.Error.WriteLine("can't open file " + save_filename);
                    //Environment.Exit(1);
                }

                /*if (y_scaling)
                {
                    fp_save.WriteLine("y");
                    fp_save.WriteLine(y_lower.ToString() + "," + y_upper.ToString());
                    fp_save.WriteLine(y_min.ToString() + "," + y_max.ToString());
                }*/

                fp_save.WriteLine("x");
                fp_save.WriteLine(lower.ToString() + "," + upper.ToString());
                for (int i = 1; i <= max_index; i++)
                {
                    if (feature_min[i] != feature_max[i])
                        fp_save.WriteLine(i.ToString() + "," + feature_min[i].ToString() + "," + feature_max[i].ToString());
                }
                fp_save.Close();
            }
        }

        //allocates enough memory for ALL features in our feature space, for their min and for max values.
        public void allocateFeaturesMinMaxMemory()
        {
            try
            {
                feature_max = new double[(max_index + 1)];
                feature_min = new double[(max_index + 1)];
            }
            catch (OutOfMemoryException e)
            {
                System.Console.Error.WriteLine("can't allocate enough memory");
                //Environment.Exit(1);
            }
        }

        public void assignTemporaryMinMax()
        {
            for (int i = 0; i <= max_index; i++)
            {
                feature_max[i] = -System.Double.MaxValue;
                feature_min[i] = System.Double.MaxValue;
            }
        }

        public void getMaxFeaturesAndNonZeros()
        {
            int index;
            while (readline(fp) != null)
            {
                java.util.StringTokenizer st = new java.util.StringTokenizer(line, " \t\n\r\f:");
                st.nextToken();
                while (st.hasMoreTokens())
                {
                    index = Int32.Parse(st.nextToken());
                    max_index = System.Math.Max(max_index, index);
                    st.nextToken();
                    num_nonzeros++;
                }
            }
        }

        public void getMinMaxValuesForFeatures()
        {
            fp = rewind(fp, data_filename);
            while (readline(fp) != null)
            {
                int next_index = 1;
                double target;
                double value;

                java.util.StringTokenizer st = new java.util.StringTokenizer(line, " \t\n\r\f:");
                target = System.Double.Parse(st.nextToken());
                y_max = System.Math.Max(y_max, target);
                y_min = System.Math.Min(y_min, target);

                while (st.hasMoreTokens())
                {
                    int index = Int32.Parse(st.nextToken());
                    value = System.Double.Parse(st.nextToken());

                    for (int i = next_index; i < index; i++)
                    {
                        feature_max[i] = System.Math.Max(feature_max[i], 0);
                        feature_min[i] = System.Math.Min(feature_min[i], 0);
                    }

                    feature_max[index] = System.Math.Max(feature_max[index], value);
                    feature_min[index] = System.Math.Min(feature_min[index], value);
                    next_index = index + 1;
                }

                for (int i = next_index; i <= max_index; i++)
                {
                    feature_max[i] = System.Math.Max(feature_max[i], 0);
                    feature_min[i] = System.Math.Min(feature_min[i], 0);
                }
            }
        }

        public void Normalize()
        {
            int index,i;
            fp = rewind(fp, data_filename);
            currentLine = 0;
            while (readline(fp) != null)
            {
                int next_index = 1;
                double target;
                double value;

                java.util.StringTokenizer st = new java.util.StringTokenizer(line, " \t\n\r\f:");
                target = System.Double.Parse(st.nextToken());
                output_target(target);
                while (st.hasMoreElements())
                {
                    index = Int32.Parse(st.nextToken());
                    value = System.Double.Parse(st.nextToken());
                    //NOTE: inside output there is a formula that used to work with zero value indices, however since we are injecting a full 204K normalization vector of minmax, this no longer works and the formula gives megative nums and needs to be removed for zero value indices.
                    //for (i = next_index; i < index; i++)
                    //    output(i, 0);
                    output(index, value);
                    next_index = index + 1;
                }

                for (i = next_index; i <= max_index; i++)
                    output(i, 0);
                System.Console.Write("\n");
                currentLine++;
            }
            GuiPreferences.Instance.setLog("Counted IQR = Zero: " + zeros.ToString());
        }

        public void saveNormalized()
        {
            if (save_filenameoutput != null)
            {
                if (writer != null)
                    writer.Close();
                if (ostrm != null)
                    ostrm.Close();
                System.Console.SetOut(oldOut);
            }
        }

        public void runModularFunctions(System.String[] argv, double [] max, double[] min)
        {
            //parse configuration arguments.
            parseArguments(argv);

            //open data_filename 
            openInputFile();

            /* assumption: min index of attributes is 1 */
            /* pass 1: find out max index of attributes */
            max_index = 0;

            //restore the saved configuration file
            restoreCofingFileGetMaxIndexCsharp();

            //calculate maxfeatures (used to allocate memory afterwards) and also non zeros
            getMaxFeaturesAndNonZeros(); 

            if (max==null && min==null)
            {
                //allocate memory for all mins and maxs of all features.
                allocateFeaturesMinMaxMemory();

                //temporaily assign -Max and Max to min and max for all features.
                assignTemporaryMinMax();

                /* pass 2: find out min/max value */
                getMinMaxValuesForFeatures();
            }
            else
            {
                feature_max = max;
                feature_min = min;
            }

            /* pass 2.5: restore feature_min/feature_max */
            restoreConfigMinMaxGetValuesCsharp();
            
            // /* pass 2.5: save feature_min/feature_max */
            saveConfigMinMax_CSharp();

            //open output file for writing
            openOutputFile();

            /* pass 3: scale */
            Normalize(); 

            //save
            saveNormalized();

            GuiPreferences.Instance.setLog("zeros: " + zeros.ToString());
            //print information
            if (new_num_nonzeros > num_nonzeros)
                System.Console.Error.WriteLine(
                 "WARNING: original #nonzeros " + num_nonzeros + "\n"
                + "         new      #nonzeros " + new_num_nonzeros + "\n"
                + "Use -l 0 if many original feature values are zeros\n");

            //close file
            fp.close();

        }

        public void initSingleVectorConfig(System.String[] argv,int maxIndex)
        { 
            //parse configuration arguments.
            parseArguments(argv); 

            /* assumption: min index of attributes is 1 */
            /* pass 1: find out max index of attributes */
            max_index = 0;

            //restore the saved configuration file 
            restoreCofingFileGetMaxIndexCsharp();

            if (max_index == maxIndex + 1)
            {
                max_index = maxIndex;
            }
            else
            {
                string mistake = "error";
            }

            //allocate memory for all mins and maxs of all features.
            allocateFeaturesMinMaxMemory();

            //temporaily assign -Max and Max to min and max for all features.
            assignTemporaryMinMax();

            /* pass 2.5: restore feature_min/feature_max */ 
            restoreConfigMinMaxGetValuesCsharp();
            configFileLoaded = true; 
        }

        public bool  NormalizeSingleVector(int[] indices, ref double[] values)
        {
            GuiPreferences.Instance.setLog("start: " + DateTime.Now.ToString());
            counter = 0;
            double [] newvalues = new double[values.Length];
            if (!configFileLoaded)
                return false;
                
            //target = label? // output_target is done for the label and this is what they call y_scaling, since i dont use it right now there is no need to implement it here.                
            //output_target(target);

            for (int v = 0; v<values.Length - 1;v++)
            {
                outputRealTime_update(indices[v], ref values[v]);
            }
            GuiPreferences.Instance.setLog("finish: " + DateTime.Now.ToString());
            return true;
         }
        
        public static void Main(System.String[] argv) 
        {
            Normalize_Range s = new Normalize_Range();
            s.run(argv);
        }
    }
}
