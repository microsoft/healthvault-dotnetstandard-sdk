// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Web.UnitTest.Utilities
{
    [TestClass]
    public class ShellUrlBuilderUnitTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Ioc.Container = new DependencyInjectionContainer();
        }

        [TestMethod]
        public void WhenAppIdParamIsNotSet_ThenAppIdIsUpdatedFromWebConfiguration()
        {
            // Arrange
            Guid masterAppId = Guid.NewGuid();

            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration { MasterApplicationId = masterAppId };
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConfiguration).As<WebHealthVaultConfiguration>());

            Uri uri = new Uri("http://www.bing.com", UriKind.Absolute);

            // Create a dictionary - we are not setting app id as part of the parameters
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ShellUrlBuilder urlBuilder = new ShellUrlBuilder(
                shellUri: uri,
                target: "Action",
                applicationPath: "/Test",
                parameters: parameters);

            // Act
            urlBuilder.EnsureAppId();

            // Assert
            Assert.AreEqual(masterAppId, parameters["appid"]);
        }

        [TestMethod]
        public void WhenAppIdParamIsSet_ThenAppIdIsSetFromParams()
        {
            // Arrange
            Guid masterAppId = Guid.NewGuid();

            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration { MasterApplicationId = masterAppId };
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConfiguration).As<WebHealthVaultConfiguration>());

            Uri uri = new Uri("http://www.bing.com", UriKind.Absolute);

            // Create a dictionary - we are not setting app id as part of the parameters
            Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { "Appid", "test-id" } };
            ShellUrlBuilder urlBuilder = new ShellUrlBuilder(
                shellUri: uri,
                target: "Action",
                applicationPath: "/Test",
                parameters: parameters);

            // Act
            urlBuilder.EnsureAppId();

            // Assert
            Assert.AreEqual("test-id", parameters["appid"]);
        }

        [TestMethod]
        public void WhenActionQsAreNotInParams_ThenActionsQsPopulatedFromPathAndQuery()
        {
            // Arrange
            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration();
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConfiguration).As<WebHealthVaultConfiguration>());

            Uri uri = new Uri("http://www.bing.com/redirect.aspx?actionqs=test", UriKind.Absolute);

            // Create a dictionary - we are not setting app id as part of the parameters
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ShellUrlBuilder urlBuilder = new ShellUrlBuilder(
                shellUri: uri,
                target: "Action",
                applicationPath: "/Test",
                parameters: parameters);

            // Act
            urlBuilder.EnsureAppQs();

            // Assert
            Assert.AreEqual("/redirect.aspx?actionqs=test", parameters["actionqs"]);
        }

        [TestMethod]
        public void WhenRedirectOverrideSetInConfiguration_ThenParamaeterOverrideIsSetFromConfiguration()
        {
            // Arrange
            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration();
            webHealthVaultConfiguration.ActionUrlRedirectOverride = new Uri("http://localhost", UriKind.Absolute);
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConfiguration).As<WebHealthVaultConfiguration>());

            Uri uri = new Uri("http://www.bing.com/redirect.aspx?actionqs=test", UriKind.Absolute);

            // Create a dictionary - we are not setting app id as part of the parameters
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ShellUrlBuilder urlBuilder = new ShellUrlBuilder(
                shellUri: uri,
                target: "Action",
                applicationPath: "/Test",
                parameters: parameters);

            // Act
            urlBuilder.EnsureRedirect();

            // Assert
            Assert.AreEqual("http://localhost", parameters["redirect"]);
        }

        [TestMethod]
        public void WhenRedirectOverrideNotSetInConfiguration_ThenParamaeterOverrideIsNotSet()
        {
            // Arrange
            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration();
            Ioc.Container.Configure(c => c.ExportInstance(webHealthVaultConfiguration).As<WebHealthVaultConfiguration>());

            Uri uri = new Uri("http://www.bing.com/redirect.aspx?actionqs=test", UriKind.Absolute);

            // Create a dictionary - we are not setting app id as part of the parameters
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ShellUrlBuilder urlBuilder = new ShellUrlBuilder(
                shellUri: uri,
                target: "Action",
                applicationPath: "/Test",
                parameters: parameters);

            // Act
            urlBuilder.EnsureRedirect();

            // Assert
            Assert.IsFalse(parameters.ContainsKey("redirect"));
        }
    }
}
