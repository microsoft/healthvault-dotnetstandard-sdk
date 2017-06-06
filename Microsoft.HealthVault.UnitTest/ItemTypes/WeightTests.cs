// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.ItemTypes
{
    [TestClass]
    public class WeightTests
    {
        [TestMethod]
        public void WhenSerializesXml_ThenCorrectSerializationReturned()
        {
            var xml = SampleUtils.GetSampleContent("ThingSampleWeight.xml");
            var weight = Weight.Deserialize(xml) as Weight;

            Assert.IsNotNull(weight);
            Assert.AreEqual(new Guid("31501360-362b-4791-ae12-141386ac5da6"), weight.Key.Id);
            Assert.AreEqual(new Guid("672b1c15-4704-4bcd-97d6-3e183309af0a"), weight.Key.VersionStamp);
            Assert.AreEqual(new NodaTime.LocalDateTime(2017, 6, 2, 0, 0, 0), weight.EffectiveDate);
            Assert.AreEqual(new HealthServiceDateTime(new HealthServiceDate(2017, 6, 2)), weight.When);
            Assert.AreEqual(76, weight.Value.Kilograms);
            Assert.AreEqual("kg", weight.Value.DisplayValue.Units);
            Assert.AreEqual("kg", weight.Value.DisplayValue.UnitsCode);
        }
    }
}
