using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.UnitTest.Samples
{
    static class SampleUtils
    {
        /// <summary>
        /// Gets file stored in the Samples directory and marked as embedded resource
        /// </summary>
        /// <param name="sampleFilename">the file's name</param>
        /// <returns></returns>
        public static string GetSampleContent(string sampleFilename)
        {
            string resourceName = $"Microsoft.HealthVault.UnitTest.Samples.{sampleFilename}";
            var assemblies = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
