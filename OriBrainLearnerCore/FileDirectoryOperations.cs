using System.IO;

namespace OriBrainLearnerCore
{
    public class FileDirectoryOperations
    {
        /// <summary>
        /// Deletes a file if it exists.
        /// </summary>
        /// <param name="fname"></param>
        public static void DeleteFile(string fname)
        {
            if (File.Exists(fname))
            {
                File.Delete(fname);
            }
        }

        /// <summary>
        /// Creates a directory if it doesnt exist.
        /// </summary>
        /// <param name="Dir"></param>
        public static void CreateDirectory(string Dir)
        {
            if (!System.IO.Directory.Exists(Dir))
                System.IO.Directory.CreateDirectory(Dir);
        }
    }
}
