// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Reflection;
using System.Xml.XPath;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.UnitTest.Samples
{
    internal static class SampleUtils
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
            var response = new HealthServiceResponseData
            {
                InfoNavigator =
                    new XPathDocument(new StringReader(GetSampleContent(fileName)))
                        .CreateNavigator()
            };

            return response;
        }
    }
}
