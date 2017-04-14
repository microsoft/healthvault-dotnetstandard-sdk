// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Net.Http;
using Microsoft.HealthVault.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.Rest
{
    [TestClass]
    public class GivenARestRequest
    {
        [TestMethod]
        public void WhenGettingActionPlans_ThenAResponseShoundBeReturned()
        {
            var client = RestClientBuilder.Create()
                .WithResponseMessage("{ \"Name\": \"My Plan\" }")
                .Build();

            dynamic response = client.ExecuteAsync(new HealthVaultRestMessage<dynamic>(new Uri("/v3/ActionPlans", UriKind.Relative), Guid.Empty, HttpMethod.Get)).Result;

            Assert.AreEqual("My Plan", (string)response.Name);
        }

        [TestMethod]
        public void WhenAnHttpErrorIsReturned_ThenAnExceptionIsThrown()
        {
            string userDoesNotExist = "User does not exist.";

            var client = RestClientBuilder.Create()
                .WithErrorMessage(userDoesNotExist)
                .Build();

            try
            {
                dynamic response = client.ExecuteAsync(new HealthVaultRestMessage<dynamic>(new Uri("/v3/ActionPlans", UriKind.Relative), Guid.Empty, HttpMethod.Get)).Result;
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(userDoesNotExist, ex.InnerException.Message);
            }
        }
    }
}