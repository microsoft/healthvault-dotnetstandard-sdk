// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.ItemTypes
{
    [TestClass]
    public class WeightTests
    {
        [TestMethod]
        public void WhenSerializesXml_ThenCorrectSerializationReturned()
        {
            var xml = @"<?xml version='1.0'?>
                        <thing>
	                        <thing-id version-stamp='672b1c15-4704-4bcd-97d6-3e183309af0a'>31501360-362b-4791-ae12-141386ac5da6</thing-id>
	                        <type-id name='Weight'>3d34d87e-7fc1-4153-800f-f56592cb0d17</type-id>
	                        <thing-state>Active</thing-state>
	                        <flags>0</flags>
	                        <eff-date>2017-06-02T00:00:00Z</eff-date>
	                        <created>
		                        <timestamp>2017-06-02T22:26:22.07Z</timestamp>
		                        <app-id name='Microsoft HealthVault'>9ca84d74-1473-471d-940f-2699cb7198df</app-id>
		                        <person-id name='Health  Insights'>64141445-0ed8-47eb-a9bc-6081628f9357</person-id>
		                        <access-avenue>Online</access-avenue>
		                        <audit-action>Created</audit-action>
	                        </created>
	                        <updated>
		                        <timestamp>2017-06-02T23:44:21.963Z</timestamp>
		                        <app-id name='Microsoft HealthVault'>9ca84d74-1473-471d-940f-2699cb7198df</app-id>
		                        <person-id name='Health  Insights'>64141445-0ed8-47eb-a9bc-6081628f9357</person-id>
		                        <access-avenue>Online</access-avenue>
		                        <audit-action>Updated</audit-action>
	                        </updated>
	                        <data-xml>
		                        <weight>
			                        <when>
				                        <date>
					                        <y>2017</y><m>6</m><d>2</d>
				                        </date>
			                        </when>
			                        <value>
				                        <kg>76</kg>
				                        <display units='kg' units-code='kg'>76</display>
			                        </value>
		                        </weight>
		                        <common />
	                        </data-xml>
                        </thing>";
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
