// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.ItemTypes
{
    [TestClass]
    public class HealthServiceDateTimeTests
    {
        [TestMethod]
        public void TestEqualsWrongType()
        {
            HealthServiceDateTime dateTime = new HealthServiceDateTime();

            Assert.IsFalse(dateTime.Equals(this), "The equals should return false.");
        }

        [TestMethod]
        public void TestEqualsNull()
        {
            HealthServiceDateTime dateTime = new HealthServiceDateTime();

            Assert.IsFalse(dateTime.Equals(null), "The equals should return false.");
        }

        [TestMethod]
        public void TestEqualsSameDate()
        {
            HealthServiceDate date = new HealthServiceDate(2017, 1, 12);

            HealthServiceDateTime dateTime1 = new HealthServiceDateTime(date);
            HealthServiceDateTime dateTime2 = new HealthServiceDateTime(date);

            Assert.IsTrue(dateTime1.Equals(dateTime2), "The equals should return true.");
        }
    }
}
