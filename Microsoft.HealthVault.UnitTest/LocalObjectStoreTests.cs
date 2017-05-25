// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest
{
    [TestClass]
    public class LocalObjectStoreTests
    {
        private const string ServiceInstanceKey = "Test";

        private const string ServiceInstanceSampleFile = "StoredServiceInstance.json";

        private ISecretStore _subSecretStore;

        [TestInitialize]
        public void TestInitialize()
        {
            _subSecretStore = Substitute.For<ISecretStore>();
        }

        [TestMethod]
        public async Task NormalWrite()
        {
            var serviceInstance = new HealthServiceInstance
            {
                Id = "test",
                Name = "Test",
                Description = "description",
                HealthServiceUrl = new Uri("http://contoso.com"),
                ShellUrl = new Uri("http://contoso.com/shell"),
            };

            LocalObjectStore localObjectStore = CreateLocalObjectStore();
            await localObjectStore.WriteAsync(ServiceInstanceKey, serviceInstance);

            await _subSecretStore.Received().WriteAsync(ServiceInstanceKey, SampleUtils.GetSampleContent(ServiceInstanceSampleFile));
        }

        [TestMethod]
        public async Task NormalRead()
        {
            _subSecretStore.ReadAsync(ServiceInstanceKey).Returns(SampleUtils.GetSampleContent(ServiceInstanceSampleFile));

            LocalObjectStore localObjectStore = CreateLocalObjectStore();
            HealthServiceInstance serviceInstance = await localObjectStore.ReadAsync<HealthServiceInstance>(ServiceInstanceKey);

            Assert.AreEqual("test", serviceInstance.Id);
            Assert.AreEqual("Test", serviceInstance.Name);
            Assert.AreEqual("description", serviceInstance.Description);
            Assert.AreEqual(new Uri("http://contoso.com"), serviceInstance.HealthServiceUrl);
            Assert.AreEqual(new Uri("http://contoso.com/shell"), serviceInstance.ShellUrl);
        }

        private LocalObjectStore CreateLocalObjectStore()
        {
            return new LocalObjectStore(
                _subSecretStore);
        }
    }
}
