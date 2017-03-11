// Copyright(c) Microsoft Corporation.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.HealthVault;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Core
{
    [TestClass]
    public class HealthServiceRequestTest
    {
        #region Ctor tests

        [TestMethod]
        public void Ctor()
        {
            IConnectionInternal mock = Substitute.For<IConnectionInternal>();

            HealthServiceRequest req = new HealthServiceRequest(mock, HealthVaultMethods.GetPersonInfo, 5);

            Assert.AreEqual(req.Method, HealthVaultMethods.GetPersonInfo);
            Assert.AreEqual(req.MethodVersion, 5);
        }

        [TestMethod]
        public void CtorNullConnection()
        {
            try
            {
                HealthServiceRequest req = new HealthServiceRequest(null, HealthVaultMethods.GetPersonInfo, 5);
                Assert.Fail("Expecting an ArgumentNullException.");
            }
            catch (ArgumentNullException)
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
            IConnectionInternal mock = Substitute.For<IConnectionInternal>();
            HealthServiceRequest req = new HealthServiceRequest(mock, HealthVaultMethods.GetPersonInfo, 5);
            return req;
        }

        #endregion Helpers
    }
}
