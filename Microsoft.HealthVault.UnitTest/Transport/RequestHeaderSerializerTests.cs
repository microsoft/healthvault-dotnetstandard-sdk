// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Transport.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.Transport
{
    /// <summary>
    /// Contains Tests for request header section serialization
    /// </summary>
    [TestClass]
    public class RequestHeaderSerializerTests
    {
        [TestMethod]
        public void WhenRequestHeaderIsSerialized()
        {
            RequestHeader requestHeader = this.CreateRequestHeader();

            RequestHeaderSerializer requestInfoSerializer = new RequestHeaderSerializer();
            string requestHeaderXml = requestInfoSerializer.Serialize(requestHeader);

            var navigator = requestHeaderXml.AsXPathNavigator();

            string rootName = navigator.Name;

            string methodName = navigator.SelectSingleNode("child::method").Value;
            string methodVersion = navigator.SelectSingleNode("child::method-version").Value;
            string targetPersonId = navigator.SelectSingleNode("child::target-person-id").Value;
            string recordId = navigator.SelectSingleNode("child::record-id").Value;
            string appId = navigator.SelectSingleNode("child::app-id").Value;
            string cultureCode = navigator.SelectSingleNode("child::culture-code").Value;
            string msgTime = navigator.SelectSingleNode("child::msg-time").Value;
            string msgTtl = navigator.SelectSingleNode("child::msg-ttl").Value;
            string version = navigator.SelectSingleNode("child::version").Value;
            string infoHash = navigator.SelectSingleNode("child::info-hash").Value;

            Assert.AreEqual("header", rootName);
            Assert.AreEqual("GetServiceDefinition", methodName);
            Assert.AreEqual("1", methodVersion);
            Assert.AreEqual("someTargetPersonId", targetPersonId);
            Assert.AreEqual("someRecordId", recordId);
            Assert.AreEqual("someAppId", appId);
            Assert.AreEqual("en-US", cultureCode);
            Assert.IsNotNull(msgTime);
            Assert.AreEqual("60", msgTtl);
            Assert.AreEqual("someVersion", version);
            Assert.IsNotNull(infoHash);
        }

        [TestMethod]
        public void WhenRequestHeaderWithUserAuthSessionIsSent_ThenAuthSessionExist()
        {
            RequestHeader requestHeader = this.CreateRequestHeader();
            requestHeader.AppId = null;
            requestHeader.AuthSession = new AuthSession
            {
                AuthToken = "someToken",
                UserAuthToken = "someUserAuthToken"
            };

            RequestHeaderSerializer requestInfoSerializer = new RequestHeaderSerializer();
            string requestHeaderXml = requestInfoSerializer.Serialize(requestHeader);

            var navigator = requestHeaderXml.AsXPathNavigator();

            XPathNavigator authSessionNavigator = navigator.SelectSingleNode("child::auth-session");

            string authToken = authSessionNavigator.SelectSingleNode("child::auth-token").Value;
            string userAuthToken = authSessionNavigator.SelectSingleNode("child::user-auth-token").Value;

            Assert.AreEqual("someToken", authToken);
            Assert.AreEqual("someUserAuthToken", userAuthToken);
        }

        [TestMethod]
        public void WhenRequestHeaderWithOfflineAuthSessionIsSent_ThenAuthSessionExist()
        {
            Guid offlinePersonGuid = Guid.NewGuid();

            RequestHeader requestHeader = this.CreateRequestHeader();
            requestHeader.AppId = null;
            requestHeader.AuthSession = new AuthSession
            {
                AuthToken = "someToken",
                Person = new OfflinePersonInfo { OfflinePersonId = offlinePersonGuid }
            };

            RequestHeaderSerializer requestInfoSerializer = new RequestHeaderSerializer();
            string requestHeaderXml = requestInfoSerializer.Serialize(requestHeader);

            var navigator = requestHeaderXml.AsXPathNavigator();

            XPathNavigator authSessionNavigator = navigator.SelectSingleNode("child::auth-session");

            string authToken = authSessionNavigator.SelectSingleNode("child::auth-token").Value;
            string offlinePersonId = authSessionNavigator
                .SelectSingleNode("child::offline-person-info")
                .SelectSingleNode("child::offline-person-id")
                .Value;

            Assert.AreEqual("someToken", authToken);
            Assert.AreEqual(offlinePersonGuid.ToString(), offlinePersonId);
        }

        private RequestHeader CreateRequestHeader()
        {
            RequestHeader requestHeader = new RequestHeader
            {
                Method = "GetServiceDefinition",
                MethodVersion = 1,
                TargetPersonId = "someTargetPersonId",
                RecordId = "someRecordId",
                AppId = "someAppId",
                CultureCode = "en-US",
                MessageTtl = 60,
                Version = "someVersion",
                InfoHash = new InfoHash
                {
                    HashData = new CryptoData { Algorithm = "someAlgorithm", Value = "someVaule" }
                }
            };


            return requestHeader;
        }
    }
}
