using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyApp
{
    class cFiles
    {
        public static string loadFile(string fileName)
        {
            string fileData = "";

            if (fileExists(fileName))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        fileData = sr.ReadToEnd();
                    }

                }
                catch (Exception e)
                {
                    cTools.logException(e);
                }
            }

            return (fileData);
        }
        public static bool fileExists(string fileName)
        {
            bool fileFound = false;

            try
            {
                if (System.IO.File.Exists(fileName))
                    fileFound = true;
            }
            catch (Exception e)
            {
                cTools.logException(e);
            }

            return (fileFound);
        }

        public static void writeFile(string fileName, string fileData, bool appendToFile = false)
        {
            try
            {
                StreamWriter fileWriter = new StreamWriter(fileName, appendToFile);  // last param makes it overwrite the file..
                fileWriter.Write(fileData);
                fileWriter.Close();
            }
            catch (Exception ex)
            {
                cTools.logException(ex);
            }
        }
    }
}
