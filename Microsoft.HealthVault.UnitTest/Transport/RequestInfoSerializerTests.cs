// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Xml.XPath;
using Microsoft.HealthVault.Transport.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.Transport
{
    /// <summary>
    /// Contains Tests for request info section serialization
    /// </summary>
    [TestClass]
    public class RequestInfoSerializerTests
    {
        [TestMethod]
        public void WhenParameterIsNull_ThenInfoXmlHasEmptyBody()
        {
            RequestInfoSerializer requestInfoSerializer = new RequestInfoSerializer();

            string infoXml = requestInfoSerializer.Serialize(null);

            XPathNavigator navigator = infoXml.AsXPathNavigator();

            string rootName = navigator.Name;
            bool hasChildren = navigator.HasChildren;

            Assert.AreEqual("info", rootName);
            Assert.IsTrue(!hasChildren, "infoxml shouldn't have any children when no parameters are set");
        }

        [TestMethod]
        public void WhenParameterIsEmpty_ThenInfoXmlHasEmptyBody()
        {
            RequestInfoSerializer requestInfoSerializer = new RequestInfoSerializer();

            string infoXml = requestInfoSerializer.Serialize(string.Empty);

            XPathNavigator navigator = infoXml.AsXPathNavigator();

            string rootName = navigator.Name;
            bool hasChildren = navigator.HasChildren;

            Assert.AreEqual("info", rootName);
            Assert.IsTrue(!hasChildren, "infoxml shouldn't have any children when no parameters are set");
        }

        [TestMethod]
        public void WhenParameterExist_ThenInfoXmlHasBody()
        {
            RequestInfoSerializer requestInfoSerializer = new RequestInfoSerializer();

            string infoXml = requestInfoSerializer.Serialize("<test>value</test>");
            XPathNavigator navigator = infoXml.AsXPathNavigator();

            string rootName = navigator.Name;
            bool hasChildren = navigator.HasChildren;

            XPathNavigator testPathNavigator = navigator.SelectSingleNode("//test");

            Assert.AreEqual("info", rootName);
            Assert.IsTrue(hasChildren, "infoxml should have children when parameters are set");
            Assert.IsNotNull(testPathNavigator, "parameter sent have been updated as infoxml children");
        }
    }
}
