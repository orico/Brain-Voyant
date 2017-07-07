using java.io;

namespace LibSVMScale
{
    public interface NormalizationInterface
    {
        bool ConfigFileLoaded
        {
            get;
            set;
        }

        void exit_with_help();

        BufferedReader rewind(BufferedReader fp, System.String filename);

        void output_target(double value);

        void output(int index, double value);

        void outputRealTime(int index, ref double value);

        void outputRealTime_update(int index, ref double value);

        System.String readline(BufferedReader fp);

        //legacy function, has it all and its working.
        void run(System.String[] argv);
         
        void parseArguments(System.String[] argv);

        void validateValues();

        void openInputFile();

        void openOutputFile();

        //find max_index from the restore file
        void restoreCofingFileGetMaxIndex();

        void restoreConfigMinMaxGetValues();

        //find max_index from the restore file
        void restoreCofingFileGetMaxIndexCsharp();

        void restoreConfigMinMaxGetValuesCsharp();

        /// <summary>
        /// static function, returns max index, min values, max values arrays as a reference
        /// </summary>
        void getConfigFileMinMaxValues(string restore_filename, ref double[] feature_max,
                                                     ref double[] feature_min, ref int max_index);

        //needs to figure out why .16g thing isnt working in runtime..it is java after all and working inthe original.
        void saveConfigMinMax_java();

        //replacement for "saveConfigMinMax_java" and it appears to be working properly
        void saveConfigMinMax_CSharp();

        void saveConfigMinMax_CSharp(string save_filename, double[] feature_min, double[] feature_max,
                                                   int max_index, double lower, double upper);

        //allocates enough memory for ALL features in our feature space, for their min and for max values.
        void allocateFeaturesMinMaxMemory();

        void assignTemporaryMinMax();

        void getMaxFeaturesAndNonZeros();

        void getMinMaxValuesForFeatures();

        void Normalize();

        void saveNormalized();

        void runModularFunctions(System.String[] argv, double[] max, double[] min);

        void initSingleVectorConfig(System.String[] argv, int maxIndex);

        bool NormalizeSingleVector(int[] indices, ref double[] values);

        //static void Main(System.String[] argv);
    }
}
