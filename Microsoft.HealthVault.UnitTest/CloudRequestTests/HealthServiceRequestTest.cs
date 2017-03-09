// Copyright(c) Microsoft Corporation.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Core
{
    [TestClass]
    public class HealthServiceRequestTest
    {
        #region Ctor tests

        [TestMethod]
        public void Ctor()
        {
            Mock<IConnectionInternal> mock = new Mock<IConnectionInternal>();

            HealthServiceRequest req = new HealthServiceRequest(mock.Object, "my-test-method", 5);

            Assert.AreEqual(req.MethodName, "my-test-method");
            Assert.AreEqual(req.MethodVersion, 5);
        }

        [TestMethod]
        public void CtorNullConnection()
        {
            try
            {
                HealthServiceRequest req = new HealthServiceRequest(null, "my-test-method", 5);
                Assert.Fail("Expecting an ArgumentNullException.");
            }
            catch (ArgumentNullException)
            {
                // pass
            }
        }

        [TestMethod]
        public void CtorNullMethodName()
        {
            Mock<IConnectionInternal> mock = new Mock<IConnectionInternal>();

            try
            {
                HealthServiceRequest req = new HealthServiceRequest(mock.Object, null, 5);
                Assert.Fail("Expecting an ArgumentException.");
            }
            catch (ArgumentException)
            {
                // pass
            }
        }

        [TestMethod]
        public void CtorEmptyStringMethodName()
        {
            Mock<IConnectionInternal> mock = new Mock<IConnectionInternal>();
            try
            {
                HealthServiceRequest req = new HealthServiceRequest(mock.Object, String.Empty, 5);
                Assert.Fail("Expecting an ArgumentException.");
            }
            catch (ArgumentException)
            {
                // pass
            }
        }

        #endregion Ctor tests

        #region Properties

        [TestMethod]
        public void TimeoutSeconds_setNegative()
        {
            HealthServiceRequest req = CreateDefault();

            try
            {
                req.TimeoutSeconds = -1;
                Assert.Fail("Expecting an ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // pass
            }
        }

        [TestMethod]
        public void TimeoutSeconds_setZero()
        {
            HealthServiceRequest req = CreateDefault();

            req.TimeoutSeconds = 0;
            Assert.AreEqual(req.TimeoutSeconds, 0, "TimeoutSeconds");
        }

        [TestMethod]
        public void TimeoutSeconds_setPositive()
        {
            HealthServiceRequest req = CreateDefault();

            req.TimeoutSeconds = 1;
            Assert.AreEqual(req.TimeoutSeconds, 1, "TimeoutSeconds");
        }

        #endregion Properties

        #region Helpers
        private HealthServiceRequest CreateDefault()
        {
            Mock<IConnectionInternal> mock = new Mock<IConnectionInternal>();
            HealthServiceRequest req = new HealthServiceRequest(mock.Object, "my-test-method", 5);
            return req;
        }

        #endregion Helpers
    }
}
