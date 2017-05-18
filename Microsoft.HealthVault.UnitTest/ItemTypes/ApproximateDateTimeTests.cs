using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.ItemTypes
{
    [TestClass]
    public class ApproximateDateTimeTests
    {
        [TestMethod]
        public void TestEqualsWrongType()
        {
            ApproximateDateTime dateTime = new ApproximateDateTime();

            Assert.IsFalse(dateTime.Equals(this), "The equals should return false.");
        }

        [TestMethod]
        public void TestEqualsNull()
        {
            ApproximateDateTime dateTime = new ApproximateDateTime();

            Assert.IsFalse(dateTime.Equals(null), "The equals should return false.");
        }

        [TestMethod]
        public void TestEqualsSameDate()
        {
            ApproximateDate date = new ApproximateDate(2017);

            ApproximateDateTime dateTime1 = new ApproximateDateTime(date);
            ApproximateDateTime dateTime2 = new ApproximateDateTime(date);

            Assert.IsTrue(dateTime1.Equals(dateTime2), "The equals should return true.");
        }
    }
}
