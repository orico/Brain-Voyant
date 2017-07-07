using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OriBrainLearnerCore
{
    public class XMLSerializer
    {

        /// <summary>
        /// save an int[] to file, serialized as XML. intended to save the top1000 IG features.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="features"></param>
        public static void serializeArrayToFile<T>(string filename, T features)
        {
            /*using (
                var stream =
                    File.Create(filename))
            {
                var serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(stream, features);
            }*/

            using (var stream =
                    File.Create(filename))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));//dict.GetType());
                serializer.WriteObject(stream, features);

            }
        }
        
        public static T DeserializeFile<T>(string filename)
        {
            try
            {
                T features;
                using (
                    var stream =
                        File.OpenRead(filename))
                {

                    //var serializer = new XmlSerializer(typeof(T));

                    var serializer = new DataContractSerializer(typeof(T));
                    features = (T)serializer.ReadObject(stream);
                }
                return features;
            }
            catch (Exception ex)
            {
                //exception by design we can safely ignore http://stackoverflow.com/questions/2209443/c-sharp-xmlserializer-bindingfailure
                GuiPreferences.Instance.setLog("exception by design, on stackoverflow, if you see this then check the code, we are returning null and it is not right.");
                return default(T);
            }
        }


        /*public static void serializeArrayToFile(string filename, double[][] features)
        {
            using (
                var stream =
                    File.Create(filename))
            {
                var serializer = new XmlSerializer(typeof(double[][]));
                serializer.Serialize(stream, features);
            }
        }*/

        /// <summary>
        /// load int[] from an XML file, intended to load top 1000 IG features.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /*public static int[]  DeserializeArrayToFileInt(string filename)
        {
            try {
                int[] features;
                using (
                    var stream =
                        File.OpenRead(filename))
                {
                
                        var serializer = new XmlSerializer(typeof(int[]));
                        features = (int[])serializer.Deserialize(stream);
                
                }
                return features;
            } 
            catch(Exception ex)
            {
                   //exception by design we can safely ignore http://stackoverflow.com/questions/2209443/c-sharp-xmlserializer-bindingfailure
               MessageBox.Show("exception by design, on stackoverflow, if you see this then check the code, we are returning null and it is not right.");
               return new int[]{};
            }
            
        }*/

    }
}
