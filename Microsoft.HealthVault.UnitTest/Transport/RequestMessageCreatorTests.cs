// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml.XPath;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Transport
{
    /// <summary>
    /// Contains unit tests for RequestMessageCreator
    /// </summary>
    [TestClass]
    public class RequestMessageCreatorTests
    {
        private IConnectionInternal _connection;
        private IServiceLocator _serviceLocator;

        [TestInitialize]
        public void InitializeTest()
        {
            _connection = Substitute.For<IConnectionInternal>();
            _serviceLocator = Substitute.For<IServiceLocator>();

            _serviceLocator.GetInstance<HealthVaultConfiguration>()
                .Returns(new HealthVaultConfiguration
                {
                    MasterApplicationId = Guid.NewGuid(),
                    RequestTimeToLiveDuration = new TimeSpan(hours: 0, minutes: 1, seconds: 5)
                });

            _serviceLocator.GetInstance<SdkTelemetryInformation>()
                .Returns(
                    new SdkTelemetryInformation() { Category = "test", FileVersion = "test", OsInformation = "test" });

            ICryptographer mockCryptographer = Substitute.For<ICryptographer>();
            CryptoData mockCryptoData = new CryptoData() { Algorithm = "some", Value = "some" };

            mockCryptographer.Hmac(Arg.Any<string>(), Arg.Any<byte[]>())
                .Returns(mockCryptoData);

            mockCryptographer.Hash(Arg.Any<byte[]>())
                .Returns(mockCryptoData);

            _serviceLocator.GetInstance<ICryptographer>().Returns(mockCryptographer);

            _connection.SessionCredential.Returns(
                new SessionCredential() { SharedSecret = "someSharedSecret", Token = "someToken" });
        }

        /// <summary>
        /// Verifies that the request gets serialized
        /// </summary>
        /// <remarks>
        ///     More detailed tests for the request sections are tested in other serializer tests
        ///     like - <see cref="RequestAuthSerializerTests"/>
        /// </remarks>
        [TestMethod]
        public void WhenRequestMethod_ThenSerialized()
        {
            AuthSession authSession = new AuthSession
            {
                AuthToken = "test",
                UserAuthToken = "test-user-token"
            };

            _connection.GetAuthSessionHeader().Returns(authSession);

            RequestMessageCreator creator = new RequestMessageCreator(_connection, _serviceLocator);

            Request request = new Request();

            string requestXml = creator.Create(
                method: HealthVaultMethods.GetThings,
                methodVersion: 2,
                isMethodAnonymous: false,
                parameters: "<parma/>",
                recordId: Guid.NewGuid(),
                appId: null);

            XPathNavigator requestNavigator = requestXml.AsXPathNavigator();

            XPathNavigator authNavigator = requestNavigator.SelectSingleNode("child::auth");
            XPathNavigator headerNavigator = requestNavigator.SelectSingleNode("child::header");
            XPathNavigator infoNavigator = requestNavigator.SelectSingleNode("child::info");

            Assert.IsNotNull(authNavigator);
            Assert.IsNotNull(headerNavigator);
            Assert.IsNotNull(infoNavigator);
        }

        [TestMethod]
        public void WhenRequestMethodIsAnonymous_ThenAppIdIsSetInRequestHeader()
        {
            RequestMessageCreator creator = new RequestMessageCreator(_connection, _serviceLocator);

            Guid appid = Guid.NewGuid();

            Request request = new Request();

            creator.SetRequestHeader(
                HealthVaultMethods.NewApplicationCreationInfo,
                2,
                true,
                recordId: Guid.NewGuid(),
                appId: appid,
                infoXml: "<parma/>",
                request: request);

            Assert.AreEqual(appid.ToString(), request.Header.AppId);
            Assert.IsNull(request.Header.AuthSession);
        }

        [TestMethod]
        public void WhenRequestMethodRequiresAuthentication_ThenAuthSessionIsSetInRequestHeader()
        {
            AuthSession authSession = new AuthSession
            {
                AuthToken = "test",
                UserAuthToken = "test-user-token"
            };

            _connection.GetAuthSessionHeader().Returns(authSession);

            RequestMessageCreator creator = new RequestMessageCreator(_connection, _serviceLocator);

            Guid appid = Guid.NewGuid();

            Request request = new Request();

            creator.SetRequestHeader(
                HealthVaultMethods.NewApplicationCreationInfo,
                2,
                false,
                recordId: Guid.NewGuid(),
                appId: appid,
                infoXml: "<parma/>",
                request: request);

            Assert.IsNull(request.Header.AppId);
            Assert.AreEqual(authSession.AuthToken, request.Header.AuthSession.AuthToken);
        }
    }
}
