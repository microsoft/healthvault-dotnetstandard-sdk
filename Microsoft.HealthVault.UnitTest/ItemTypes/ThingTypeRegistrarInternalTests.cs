// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.ItemTypes
{
    [TestClass]
    public class ThingTypeRegistrarInternalTests
    {
        [TestMethod]
        public void WhenInitialized_ThenDefaultTypesAreRegistered()
        {
            var thingTypeRegistrarInternal = new ThingTypeRegistrar();

            Assert.IsNotNull(thingTypeRegistrarInternal.RegisteredTypeHandlers);
            Assert.IsTrue(thingTypeRegistrarInternal.RegisteredTypeHandlers.Count > 1);
        }

        [TestMethod]
        public void WhenApplicationSpecificTypesRegistered()
        {
            var thingTypeRegistrarInternal = new ThingTypeRegistrar();
            thingTypeRegistrarInternal.RegisterApplicationSpecificHandler("some", "some-tag", typeof(CustomThingType));

            Assert.IsTrue(thingTypeRegistrarInternal.RegisteredAppSpecificHandlers.Count == 1);
        }

        [TestMethod]
        public void WhenExtensionHandlerRegistered()
        {
            var thingTypeRegistrarInternal = new ThingTypeRegistrar();
            thingTypeRegistrarInternal.RegisterExtensionHandler("some", typeof(CustomExtensionType));

            Assert.IsTrue(thingTypeRegistrarInternal.RegisteredExtensionHandlers.Count == 1);
        }

        [TestMethod]
        public void WhenSerializesXml_ThenCorrectStringReturned()
        {           
            Weight weight = new Weight(new HealthServiceDateTime(new NodaTime.LocalDateTime(2017, 6, 1, 12, 0, 0)), new WeightValue(60.0));
            var serialized = weight.Serialize();
            Assert.IsFalse(string.IsNullOrWhiteSpace(serialized));
            Assert.IsTrue(serialized.Contains("<type-id>3d34d87e-7fc1-4153-800f-f56592cb0d17</type-id>"));
            Assert.IsTrue(serialized.Contains("<when><date><y>2017</y><m>6</m><d>1</d></date><time><h>12</h><m>0</m><s>0</s><f>0</f></time></when>"));
            Assert.IsTrue(serialized.Contains("<kg>60</kg>"));
        }
    }
}
