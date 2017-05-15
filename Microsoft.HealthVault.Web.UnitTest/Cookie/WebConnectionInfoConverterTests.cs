// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Web.Cookie;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;

namespace Microsoft.HealthVault.Web.UnitTest.Cookie
{
    /// <summary>
    /// Contains tests for <see cref="WebConnectionInfoConverter"/> class
    /// </summary>
    [TestClass]
    public class WebConnectionInfoConverterTests
    {
        /// <summary>
        /// Verifies when serialized WebConnectionInfo is less than
        /// acceptable cookie size, WebConnectionInfoConverter serializes all
        /// the properties - though we only are testing serviceInstanceId, it
        /// should suffice the assumption that the converter is not doing any
        /// additional changes.
        /// </summary>
        [TestMethod]
        public void WhenWebConnectionInfoIsLessThanCookieSize()
        {
            // Arrange
            WebConnectionInfo webConnectionInfo = CreateWebConnectionInfo();
            JsonWriter mockedJsonWriter = Substitute.For<JsonWriter>();

            // Act
            Serialize(mockedJsonWriter, webConnectionInfo);

            // Assert
            mockedJsonWriter.Received().WriteValue(Arg.Is<string>(x => x.Contains("ServiceInstanceId")));
        }

        /// <summary>
        /// Verifies when application settings make the serialized string greater than cookie size, then
        /// the converter minimizes the application settings and sets the MinimizedPersonInfoApplicationSettings
        /// property to true.
        /// </summary>
        [TestMethod]
        public void WhenApplicationSettingsMakeWebConnectionInfoGreaterThanCookieSize()
        {
            // Arrange
            var applicationSettingDocument = CreateApplicationSettingDocumentBiggerThanCookieSize();
            WebConnectionInfo webConnectionInfo = CreateWebConnectionInfo(null, applicationSettingDocument);
            JsonWriter mockedJsonWriter = Substitute.For<JsonWriter>();

            // Act
            Serialize(mockedJsonWriter, webConnectionInfo);

            // Assert
            // Verify that person info application settings has been minimized
            mockedJsonWriter.Received().WriteValue(
                Arg.Is<string>(x => x.Contains("\"MinimizedPersonInfoApplicationSettings\":true")));
        }

        /// <summary>
        /// Verifies when authorized records make the serialized string greater than cookie size, then
        /// the converter minimizes the authorized records and sets the MinimizedPersonInfoRecords
        /// property to true.
        /// </summary>
        [TestMethod]
        public void WhenAuthorizedRecordsMakeWebConnectionInfoGreaterThanCookieSize()
        {
            // Arrange
            var healthRecordInfos = CreateHealthRecordInfosToMakeCookieSizeLarge();
            WebConnectionInfo webConnectionInfo = CreateWebConnectionInfo(healthRecordInfos);
            JsonWriter mockedJsonWriter = Substitute.For<JsonWriter>();

            // Act
            Serialize(mockedJsonWriter, webConnectionInfo);

            // Assert
            // Verify that person info authorized records have been minimized
            mockedJsonWriter.Received().WriteValue(
                Arg.Is<string>(x => x.Contains("\"MinimizedPersonInfoRecords\":true")));
        }

        /// <summary>
        /// Verifies when application settings and authorized records exist in the person info and make
        /// the serialized string greater than cookie size, then the converter
        /// minimizes the application settings and authorized records and sets the MinimizedPersonInfoApplicationSettings
        /// and MinimizedPersonInfoRecords property to true.
        /// </summary>
        [TestMethod]
        public void WhenBothAuthorizedRecordsAndApplicationSettingsMakeCookieSizeBig()
        {
            var applicationSettingDocument = CreateApplicationSettingDocumentBiggerThanCookieSize();
            var healthRecordInfos = CreateHealthRecordInfosToMakeCookieSizeLarge();
            WebConnectionInfo webConnectionInfo = CreateWebConnectionInfo(healthRecordInfos, applicationSettingDocument);
            JsonWriter mockedJsonWriter = Substitute.For<JsonWriter>();

            Serialize(mockedJsonWriter, webConnectionInfo);

            // Verify that person info authorized records have been minimized
            mockedJsonWriter.Received().WriteValue(
                Arg.Is<string>(x => x.Contains("\"MinimizedPersonInfoRecords\":true")
                && x.Contains("\"MinimizedPersonInfoApplicationSettings\":true")));
        }

        #region helpers

        private WebConnectionInfo CreateWebConnectionInfo(
            IDictionary<Guid, HealthRecordInfo> healthRecordInfos = null,
            XDocument applicationSettingsDocument = null)
        {
            return new WebConnectionInfo
            {
                UserAuthToken = Guid.NewGuid().ToString(),
                ServiceInstanceId = "1",
                SessionCredential = new SessionCredential { SharedSecret = "someSecret", Token = "someToken" },
                PersonInfo = new PersonInfo
                {
                    PersonId = Guid.NewGuid(),
                    ApplicationSettingsDocument = applicationSettingsDocument,
                    AuthorizedRecords = healthRecordInfos ?? new Dictionary<Guid, HealthRecordInfo> { { Guid.NewGuid(), new HealthRecordInfo() } },
                    HasMoreApplicationSettings = false,
                    HasMoreRecords = false,
                    Location = new Location("US", "WA"),
                    Name = "some",
                    PreferredCulture = "en-US",
                    PreferredUICulture = "en-US",
                    SelectedRecord = new HealthRecordInfo()
                }
            };
        }

        private XDocument CreateApplicationSettingDocumentBiggerThanCookieSize()
        {
            StringBuilder applicationSettings = new StringBuilder();
            applicationSettings.Append("<root>");

            // Build an applicationSetting string to be greater than 4000, cookie size being 4093, creating
            // applicationSettings xml to be greater than 4000 should minimize application settings
            for (int i = 0; i < 4000; i++)
            {
                applicationSettings.Append("c");
            }
            applicationSettings.Append("</root>");
            XDocument applicationSettingDocument = XDocument.Parse(applicationSettings.ToString());
            return applicationSettingDocument;
        }

        private Dictionary<Guid, HealthRecordInfo> CreateHealthRecordInfosToMakeCookieSizeLarge()
        {
            Dictionary<Guid, HealthRecordInfo> healthRecordInfos = new Dictionary<Guid, HealthRecordInfo>(1000);

            // Create authorized records to be greater than 1000. This will make the cooie size of webconnectioninfo to
            // be greater than 4093.
            for (int i = 0; i < 1000; i++)
            {
                Guid id = Guid.NewGuid();

                HealthRecordInfo healthRecordInfo = new HealthRecordInfo
                {
                    Id = id,
                    IsCustodian = false
                };

                healthRecordInfos.Add(id, healthRecordInfo);
            }

            // Set the first record in the authorized records to be self record
            healthRecordInfos.ElementAt(0).Value.RelationshipType = RelationshipType.Self;
            healthRecordInfos.ElementAt(0).Value.DateAuthorizationExpires = DateTime.MaxValue;
            return healthRecordInfos;
        }

        private void Serialize(JsonWriter mockedJsonWriter, WebConnectionInfo webConnectionInfo)
        {
            WebConnectionInfoConverter converter = new WebConnectionInfoConverter();
            converter.WriteJson(mockedJsonWriter, webConnectionInfo, JsonSerializer.Create());
        }

        #endregion
    }
}
