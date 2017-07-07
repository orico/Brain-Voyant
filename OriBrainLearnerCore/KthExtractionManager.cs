using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronPython.Hosting;
//using IronPythonDLL; //to use this remember to add reference to the Iron Python DLL solution.
using Microsoft.Scripting.Hosting; 

namespace OriBrainLearnerCore
{ 
    public class KthExtractionManager
    {
        public static void ExecuteSelectKthVectorScript(string CsharpFileName, string CsharpDirectory)
        {
            KthVectors instance;
            List<int> listK = new List<int>();
            //GuiPreferences.Instance.setLog(CsharpFileName);
            //GuiPreferences.Instance.setLog(CsharpDirectory);
            if (CsharpFileName!="")
            {
                instance = new KthVectors(CsharpFileName, CsharpDirectory);//, 5);
                for (decimal i = GuiPreferences.Instance.NudExtractFromTR; i <= GuiPreferences.Instance.NudExtractToTR;i++ )
                {
                    listK.Add(Convert.ToInt32(i));
                }
                instance.batchProcessPerK(listK);
            }
            //GuiPreferences.Instance.setLog(listK.ToString());

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
             
            //NOTE: python implementation replaced in order to get the '_eventStartLocations' into the config file, to pass them to the normalization stage
            //      in the running window med\iqr.
            //RunPythonScript(CsharpFileName,CsharpDirectory); 
        }

        //the old method used python.
        public static void RunPythonScript(string CsharpFileName, string CsharpDirectory)
        {

            //IMPORTANT: for this script to work, we need to includ "using IR2" in this file
            ScriptEngine engine = Python.CreateEngine();
            ScriptRuntime runtime = engine.Runtime;
            ScriptScope scope = engine.CreateScope();
            ScriptSource source;

            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 5; i++)
                rootDir = rootDir.Substring(0, rootDir.LastIndexOf(@"\"));

            GuiPreferences.Instance.setLog(@"Python Root Dir + \PythonScripts\MainSelectVectorKth.py in: " + rootDir);
            string scriptDirectory = Path.Combine(rootDir, @"PythonScripts\MainSelectVectorKth.py");

            scope.SetVariable("PythonScriptDirectory", Path.Combine(rootDir, @"PythonScripts"));
            scope.SetVariable("CsharpFileName", CsharpFileName);
            scope.SetVariable("CsharpDirectory", CsharpDirectory);

            //adding lists was found here http://www.codeproject.com/Articles/53611/Embedding-IronPython-in-a-C-Application
            List<int> ListTRsToExtract = new List<int>();
            for (decimal i = GuiPreferences.Instance.NudExtractFromTR; i <= GuiPreferences.Instance.NudExtractToTR; i++)
            {
                ListTRsToExtract.Add(Convert.ToInt16(i));
            }
            scope.SetVariable("ListTRsToExtract", ListTRsToExtract);

            source = engine.CreateScriptSourceFromFile(scriptDirectory);
            source.Execute(scope);
            
        }
    }
}
