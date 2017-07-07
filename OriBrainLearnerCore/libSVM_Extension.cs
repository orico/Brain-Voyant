using System;
using System.Linq; 
using System.Collections.Generic;

namespace libSVMWrapper
{
    public class libSVM_output
    {
        public double meanSquaredError = 0; 
        public double squaredCorrelationCoefficient = 0;
        public double accuracy = 0;
        //public double[] predictedLabels;
        private SortedDictionary<int, double> predictedLabels;

        public libSVM_output()
        {
            try
            {
                predictedLabels = new SortedDictionary<int, double>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //used in testAuto
        public void setValue(int key, double value)
        {
            if (!predictedLabels.ContainsKey(key))
            {
                predictedLabels.Add(key, value);
            }
            else
            {
                predictedLabels.Remove(key);
                predictedLabels.Add(key, value);
            }
        }

        //used in testAuto
        public double getValue(int i)
        {
            return predictedLabels[i];
        }

        //used in testAuto
        public double getValueByIndex(int i)
        {
            return predictedLabels.ElementAt(i).Value;
        }

        //used in testAuto
        public int getKeyByIndex(int i)
        {
            return predictedLabels.ElementAt(i).Key;
        }

        //used in predict / getaccuracy / cross validation
        public void setDoubleArray(double [] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                predictedLabels.Add(i, array[i]);
            }
        }

        public SortedDictionary<int, double> getPredictedLabels()
        {
            return predictedLabels;
        }

        public int getLength()
        {
            return predictedLabels.Count;
        }

        public SortedDictionary<int,double>.KeyCollection getKeys()
        {
            return predictedLabels.Keys;
        }

        public SortedDictionary<int, double>.ValueCollection getValues()
        {
            return predictedLabels.Values;
        }
        
    }

    /// <summary>
    /// custom changes to support mapping of predicted labels to actual labels and cross validation
    /// </summary>
    public class libSVM_Extension : libSVM
    {
        public libSVM_output output = new libSVM_output();


        /*public IntPtr getModel()
        {
            if (_model_ptr == IntPtr.Zero) throw new Exception("model neither loaded nor trained");
           return _model_ptr;
        }

        public void  setModel(IntPtr model)
        {   
            _model_ptr = model;
        }

        /// <summary>
        /// Reload libSVM model 
        /// added externally and is a duplication of the internal libsvm function
        /// </summary>
        /// <param name="filename">name of model file</param>
        public new void Reload(string filename)
        {
            Dispose();

            _model_ptr = svm_load_model(filename);

            if (_model_ptr == IntPtr.Zero) throw new Exception("bad model file");
        }

        /// <summary>
        ///  Load model from file
        /// added externally and is a duplication of the internal libsvm function
        /// </summary>
        /// <param name="filename">Model file</param>                
        /// <returns>libSVM</returns>
        public new static libSVM_Extension Load(string filename)
        {
            libSVM_Extension svm = new libSVM_Extension();

            svm.Reload(filename);

            return svm;
        }*/

        // private void ptr_to_array(int len, ref double[] array, IntPtr array_ptr)
        public void CrossValidate(libSVM_ExtendedProblem problem, libSVM_Parameter parameter, int nr_fold)
        { 
            int i;
            int total_correct = 0;
            double total_error = 0;
            double sumv = 0, sumy = 0, sumvv = 0, sumyy = 0, sumvy = 0;
            double[] predictedLabels = new double[problem.labels.Length]; 

            IntPtr _target_ptr = array_to_ptr(predictedLabels);
            IntPtr _problem_ptr = libSVM_Problem_to_svm_problem_ptr(problem);
            IntPtr _parameter_ptr = libSVM_Parameter_to_svm_parameter_ptr(parameter);
            svm_cross_validation(_problem_ptr, _parameter_ptr, nr_fold, _target_ptr);
            ptr_to_array(problem.labels.Length, ref predictedLabels, _target_ptr);
            output.setDoubleArray(predictedLabels);
            predictedLabels = null;

            if (parameter.svm_type == SVM_TYPE.EPSILON_SVR || parameter.svm_type == SVM_TYPE.NU_SVR)
            {
                for (i = 0; i < problem.labels.Length; i++)
                {

                    double y = problem.labels[i];
                    double v = output.getValue(i);
                    total_error += (v-y)*(v-y);
                    sumv += v;
                    sumy += y;
                    sumvv += v*v;
                    sumyy += y*y;
                    sumvy += v*y;
                } 
                output.meanSquaredError = total_error / problem.labels.Length;
                output.squaredCorrelationCoefficient = ((problem.labels.Length * sumvy - sumv * sumy) * (problem.labels.Length * sumvy - sumv * sumy)) /
                ((problem.labels.Length * sumvv - sumv * sumv) * (problem.labels.Length * sumyy - sumy * sumy));
            }
            else
            {
                for (i = 0; i < problem.labels.Length; i++)
                    if (output.getValue(i) == problem.labels[i])
                ++total_correct;
                //Console.Write("Cross Validation Accuracy = %g%%\n", 100.0 * total_correct / problem.labels.Length);
                output.accuracy = 100.0 * total_correct / problem.labels.Length;
            } 
        }   
 

        /// <summary>
        /// GetAccuracy for problem, returning predicted labels.
        /// </summary>
        /// <param name="problem">problem</param>
        /// <returns>accuracy in %</returns>
        ///  
        new public void GetAccuracy(libSVM_Problem problem)
        {

            if (problem.samples == null || problem.samples.Length == 0) throw new Exception("GetAccuracy no samples");
            if (problem.labels == null || problem.labels.Length == 0) throw new Exception("GetAccuracy no labels");

            if (problem.samples.Length != problem.labels.Length) throw new Exception("GetAccuracy labels.length!=samples.length");

            output = new libSVM_output();

            int correctPredictions = 0;
            double pred ;
            for (int i = 0; i < problem.samples.Length; i++)
            { 
                if (problem.samples[i] != null && problem.samples[i].Count > 0 )
                {
                    pred = Predict(problem.samples[i]);
                    output.setValue(i, pred);
                    if (pred == problem.labels[i])
                        correctPredictions++;
                }
            }

            output.accuracy = correctPredictions / (double)problem.samples.Length * 100.0;
        }
        
        /// <summary>
        /// wrapper function for GetAccuracy, the name was not logical and i have to maintain a subversion pristine copy.
        /// </summary>
        /// <param name="problem">problem</param>
        /// <returns>accuracy in %</returns>
        ///  
        public void GetAccuracyFromTest(libSVM_Problem problem)
        {
            GetAccuracy(problem);
        }


        /// <summary>
        /// wrapper function for GetAccuracy, the name was not logical and i have to maintain a subversion pristine copy.
        /// </summary>
        /// <param name="problem">problem</param>
        /// <returns>accuracy in %</returns>
        ///  
        public double GetAccuracyFromTestSingleSample(libSVM_Problem problem, libSVM svmModel)
        { 
            if (problem.samples == null || problem.samples.Length == 0) throw new Exception("GetAccuracy no samples");
            if (problem.labels == null || problem.labels.Length == 0) throw new Exception("GetAccuracy no labels");

            if (problem.samples.Length != problem.labels.Length) throw new Exception("GetAccuracy labels.length!=samples.length");

            output = new libSVM_output();

            int correctPredictions = 0;
            double pred ;

            int index = problem.samples.Length -1;
            if (problem.samples[index] != null && problem.samples[index].Count > 0)
            {
                pred = svmModel.Predict(problem.samples[index]);
                return pred;
                /*output.setValue(index, pred);
                if (pred == problem.labels[index])
                    correctPredictions++;*/
            }
            

            output.accuracy = correctPredictions / (double)problem.samples.Length * 100.0;
            return 1.0;
        }

        /// <summary>
        /// Trains model automatically using users grids. if your svm_type/kernel_type does not need it then set it to null
        /// </summary>
        /// <param name="fold">folds for samples >=2 </param>
        /// <param name="problem">samples and labels</param>
        /// <param name="parameter">model parameters</param>
        /// <param name="grid_c">grid for C</param>
        /// <param name="grid_gamma">grid for gamma</param>
        /// <param name="grid_p">grid for p</param>
        /// <param name="grid_nu">grid for nu</param>
        /// <param name="grid_coef0">grid for coef</param>
        /// <param name="grid_degree">grid for degree</param>
        /// <returns>Total Accuracy in %</returns>
        public void TrainAutoBestTotal(int fold, libSVM_Problem problem, libSVM_Parameter parameter, libSVM_Grid grid_c, libSVM_Grid grid_gamma, libSVM_Grid grid_p, libSVM_Grid grid_nu, libSVM_Grid grid_coef0, libSVM_Grid grid_degree)
        {
            Dispose();

            if (problem == null) throw new Exception("libSVMProblem not initialized");
            if (problem.samples == null) throw new Exception("libSVMProblem.samples = null");
            if (problem.labels == null) throw new Exception("libSVMProblem.labels = null");
            if (problem.samples.Length != problem.labels.Length) throw new Exception("libSVMProblem.samples.Length != libSVMProblem.labels.length");
            if (parameter == null) throw new Exception("libSVMParameter not initialized");
            if (parameter.weight == null && parameter.weight_label != null) throw new Exception("libSVMParameter.weight = null and libSVMParameter.weight_label != null");
            if (parameter.weight != null && parameter.weight_label == null) throw new Exception("libSVMParameter.weight_label = null and libSVMParameter.weight != null");
            if (parameter.weight != null && parameter.weight_label != null && parameter.weight_label.Length != parameter.weight.Length) throw new Exception("libSVMParameter.weight_label.Length != libSVMParameter.weight.Length");
            if (fold < 2 || fold > problem.samples.Length) throw new Exception("fold < 2 || fold > nr_samples");

            libSVM_Grid _grid_gamma = grid_gamma;
            libSVM_Grid _grid_coef0 = grid_coef0;
            libSVM_Grid _grid_degree = grid_degree;

            if (parameter.kernel_type == KERNEL_TYPE.LINEAR
                || parameter.kernel_type == KERNEL_TYPE.PRECOMPUTED)
            {
                _grid_gamma = new libSVM_Grid();
                _grid_coef0 = new libSVM_Grid();
                _grid_degree = new libSVM_Grid();
            }
            else if (parameter.kernel_type == KERNEL_TYPE.POLY)
            {
                if (_grid_gamma == null) throw new Exception("grid_gamma not set");
                if (_grid_coef0 == null) throw new Exception("grid_coef0 not set");
                if (_grid_degree == null) throw new Exception("grid_degree not set");

            }
            else if (parameter.kernel_type == KERNEL_TYPE.RBF)
            {
                if (_grid_gamma == null) throw new Exception("grid_gamma not set");

                _grid_coef0 = new libSVM_Grid();
                _grid_degree = new libSVM_Grid();
            }
            else if (parameter.kernel_type == KERNEL_TYPE.SIGMOID)
            {
                if (_grid_gamma == null) throw new Exception("grid_gamma not set");
                if (_grid_coef0 == null) throw new Exception("grid_coef not set");
                _grid_degree = new libSVM_Grid();
            }
            else throw new Exception("unknown kernel_type");

            libSVM_Grid _grid_c = grid_c;
            libSVM_Grid _grid_p = grid_p;
            libSVM_Grid _grid_nu = grid_nu;

            if (parameter.svm_type == SVM_TYPE.C_SVC
                || parameter.svm_type == SVM_TYPE.ONE_CLASS)
            {
                if (_grid_c == null) throw new Exception("grid_C not set");

                _grid_nu = new libSVM_Grid();
                _grid_p = new libSVM_Grid();
            }
            else if (parameter.svm_type == SVM_TYPE.NU_SVC || parameter.svm_type == SVM_TYPE.NU_SVR)
            {
                if (_grid_nu == null) throw new Exception("grid_nu not set");

                _grid_c = new libSVM_Grid();
                _grid_p = new libSVM_Grid();
            }
            else if (parameter.svm_type == SVM_TYPE.EPSILON_SVR)
            {
                if (_grid_p == null) throw new Exception("grid_p not set");

                _grid_c = new libSVM_Grid();
                _grid_nu = new libSVM_Grid();
            }
            else throw new Exception("unknown svm_type");

            double c = _grid_c.min;
            double p = _grid_p.min;
            double nu = _grid_nu.min;

            double gamma = _grid_gamma.min;
            double coef0 = _grid_coef0.min;
            double degree = _grid_degree.min;
            int f = 0;

            int test_probe = 0;
            int test_samples = 0;
            //ori
            int total_test_probe = 0;
            int total_test_samples = 0;
            libSVM_Parameter train_parameter = new libSVM_Parameter();

            //copy parameters set by user;
            train_parameter.weight = parameter.weight;
            train_parameter.weight_label = parameter.weight_label;
            train_parameter.shrinking = parameter.shrinking;
            train_parameter.svm_type = parameter.svm_type;
            train_parameter.kernel_type = parameter.kernel_type;
            train_parameter.probability = parameter.probability;
            train_parameter.cache_size = parameter.cache_size;

            libSVM_Problem train_problem = new libSVM_Problem();
            libSVM_Problem test_problem = new libSVM_Problem();

            //ori
            libSVM_output this_output = new libSVM_output();
             
            //fold
            do
            {
                train_problem.labels = new double[1];
                train_problem.samples = new SortedDictionary<int, double>[1];

                test_problem.labels = new double[1];
                test_problem.samples = new SortedDictionary<int, double>[1];

                int j = 0;
                int k = 0;
                for (int i = 0; i < problem.samples.Length; i++)
                    if ((i + 1 + f) % fold == 0)
                    {
                        Array.Resize(ref test_problem.labels, j + 1);
                        Array.Resize(ref test_problem.samples, j + 1);
                        this_output.setValue(i,999);

                        test_problem.labels[j] = problem.labels[i];
                        test_problem.samples[j] = problem.samples[i];
                        j++;
                    }
                    else
                    {
                        Array.Resize(ref train_problem.labels, k + 1);
                        Array.Resize(ref train_problem.samples, k + 1);

                        train_problem.labels[k] = problem.labels[i];
                        train_problem.samples[k] = problem.samples[i];
                        k++;
                    }

                //p
                p = _grid_p.min;
                do
                {
                    //nu
                    nu = _grid_nu.min;
                    do
                    {
                        //gamma
                        gamma = _grid_gamma.min;
                        do
                        {
                            //coef0
                            coef0 = _grid_coef0.min;
                            do
                            {
                                //degree

                                degree = _grid_degree.min;
                                do
                                {
                                    //c

                                    c = _grid_c.min;
                                    do
                                    {
                                        //svm_train alters problem_ptr so it's necessary to create it every time;
                                        IntPtr this_problem_ptr = libSVM_Problem_to_svm_problem_ptr(train_problem);

                                        //set generated parameters
                                        train_parameter.C = c;
                                        train_parameter.p = p;
                                        train_parameter.nu = nu;
                                        train_parameter.gamma = gamma;
                                        train_parameter.degree = (int)degree;
                                        train_parameter.coef0 = coef0;

                                        IntPtr this_parameter_ptr = libSVM_Parameter_to_svm_parameter_ptr(train_parameter);

                                        IntPtr error_ptr = svm_check_parameter(this_problem_ptr, this_parameter_ptr);

                                        if (error_ptr != IntPtr.Zero)
                                            throw new Exception(ptr_to_string(error_ptr));

                                        IntPtr this_model_ptr = svm_train(this_problem_ptr, this_parameter_ptr);

                                        int this_test_probe = 0;

                                        //count propperly recognized test_problem.samples
                                        for (int i = 0; i < test_problem.labels.Length; i++)
                                        {
                                            IntPtr svm_nodes_ptr = sample_to_svm_nodes_ptr(test_problem.samples[i]);

                                            //ori 
                                            
                                            double testPrediction = svm_predict(this_model_ptr, svm_nodes_ptr);

                                            this_output.setValue(this_output.getKeyByIndex(i), testPrediction); 
                                            total_test_samples++; // can also be problem.labels.length
                                            if (test_problem.labels[i] == testPrediction)
                                            {
                                                this_test_probe++;
                                                total_test_probe++;
                                            }

                                            Free_ptr(ref svm_nodes_ptr);
                                        }

                                        //if first run then just copy this
                                        if (_model_ptr == IntPtr.Zero)
                                        {
                                            _model_ptr = this_model_ptr;
                                            _parameter_ptr = this_parameter_ptr;
                                            _problem_ptr = this_problem_ptr;

                                            test_probe = this_test_probe;
                                            test_samples = test_problem.samples.Length;

                                            //ori
                                            output = this_output;

                                            //!-uncommenting this gets mapping of predicted labels only for best model 
                                            this_output = new libSVM_output(); 

                                            //!-uncommenting this gets only best model and not runs on all 10 folds (unless best is 10th) 
                                            //exit when no better solution could be found
                                            if (test_probe == test_problem.samples.Length) goto leave;
                                        }
                                        else
                                            //if model was better than previous then free previous data and copy this
                                            if (this_test_probe > test_probe)
                                            {
                                                Free_svm_parameter_ptr(ref _parameter_ptr);
                                                Free_svm_problem_ptr(ref _problem_ptr);
                                                svm_free_and_destroy_model(ref _model_ptr);

                                                _parameter_ptr = this_parameter_ptr;
                                                _problem_ptr = this_problem_ptr;
                                                _model_ptr = this_model_ptr;

                                                test_probe = this_test_probe;
                                                test_samples = test_problem.samples.Length;

                                                //ori
                                                output = this_output;

                                                //!-uncommenting this gets mapping of predicted labels only for best model 
                                                this_output = new libSVM_output();  

                                                //!-uncommenting this gets only best model and not runs on all 10 folds (unless best is 10th)
                                                //exit when no better solution could be found;
                                                if (test_probe == test_problem.samples.Length) goto leave;
                                            }
                                            //if not then free this model
                                            else
                                            {
                                                Free_svm_problem_ptr(ref this_problem_ptr);
                                                Free_svm_parameter_ptr(ref this_parameter_ptr);
                                                svm_free_and_destroy_model(ref this_model_ptr);

                                                //!-uncommenting this gets mapping of predicted labels only for best model 
                                                this_output = new libSVM_output();  
                                            }

                                        c *= _grid_c.step;
                                    } while (c < _grid_c.max);

                                    degree *= _grid_degree.step;
                                } while (degree < _grid_degree.max);

                                coef0 *= _grid_coef0.step;
                            } while (coef0 < _grid_coef0.max);

                            gamma *= _grid_gamma.step;
                        } while (gamma < _grid_gamma.max);

                        nu *= _grid_nu.step;
                    } while (nu < _grid_nu.max);

                    p *= _grid_p.step;
                } while (p < _grid_p.max);

                f++;
            } while (f < fold);

        leave:
            //return new double []{100.0 * test_probe / (double) test_samples,100.0 * total_test_probe / (double)total_test_samples};
            output.accuracy = 100.0 * test_probe / (double)test_samples;
            return;
        }

         
    }
}
