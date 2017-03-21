using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Transport;

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

        public static HealthServiceResponseData GetResponseData(string fileName)
        {
            return new HealthServiceResponseData
            {
                InfoNavigator =
                    new XPathDocument(new StringReader(SampleUtils.GetSampleContent(fileName)))
                        .CreateNavigator(),
                ResponseText =
                    new ArraySegment<byte>(
                        Encoding.ASCII.GetBytes(SampleUtils.GetSampleContent(fileName)))
            };
        }
    }
}
