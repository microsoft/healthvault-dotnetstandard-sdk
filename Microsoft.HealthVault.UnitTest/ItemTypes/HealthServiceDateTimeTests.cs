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
