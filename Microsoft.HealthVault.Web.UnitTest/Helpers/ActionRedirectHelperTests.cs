// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.Web.UnitTest.Helpers
{
    /// <summary>
    /// Tests functionality in <see cref="ActionRedirectHelper"/> class
    /// </summary>
    [TestClass]
    public class ActionRedirectHelperTests
    {
        /// <summary>
        /// Verifies that the action url set in <see cref="WebHealthVaultConfiguration"/>
        /// is returned when requested for that specific action key (here "Test")
        /// </summary>
        [TestMethod]
        public void WhenSpecificActionRequested_ThenActionUrlSetInConfigurationIsReturned()
        {
            // Arrange
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();

            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration
            {
                ActionPageUrls = new Dictionary<string, Uri> { { "Test", new Uri("/Test", UriKind.Relative) } }
            };

            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(webHealthVaultConfiguration);

            ActionRedirectHelper actionRedirectHelper = new ActionRedirectHelper(serviceLocator);

            // Act
            string targetLocation =  actionRedirectHelper.TryGetTargetLocation("Test", null);

            // Assert
            Assert.AreEqual("/Test", targetLocation);
        }

        [TestMethod]
        public void WhenSpecificActionRequestedWithQueryString_ThenActionUrlSetInConfigurationIsReturned()
        {
            // Arrange
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();

            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration
            {
                ActionPageUrls = new Dictionary<string, Uri> { { "Test", new Uri("/Test", UriKind.Relative) } }
            };

            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(webHealthVaultConfiguration);

            ActionRedirectHelper actionRedirectHelper = new ActionRedirectHelper(serviceLocator);

            // Act
            string targetLocation = actionRedirectHelper.TryGetTargetLocation(
                action: "Test",
                actionQueryString:  "query=test");

            // Asssert
            Assert.AreEqual("/Test?query=test", targetLocation);
        }

        [TestMethod]
        public void WhenSpecificActionRequestedWithFormattedQueryString_ThenActionUrlSetInConfigurationIsReturned()
        {
            // Arrange
            IServiceLocator serviceLocator = Substitute.For<IServiceLocator>();

            WebHealthVaultConfiguration webHealthVaultConfiguration = new WebHealthVaultConfiguration
            {
                ActionPageUrls = new Dictionary<string, Uri> { { "Test", new Uri("/Test", UriKind.Relative) } }
            };

            serviceLocator.GetInstance<WebHealthVaultConfiguration>().Returns(webHealthVaultConfiguration);

            ActionRedirectHelper actionRedirectHelper = new ActionRedirectHelper(serviceLocator);

            // Act
            string targetLocation = actionRedirectHelper.TryGetTargetLocation(
                action: "Test", 
                actionQueryString: "?query=test");

            // Assert
            Assert.AreEqual("/Test?query=test", targetLocation);
        }
    }
}
