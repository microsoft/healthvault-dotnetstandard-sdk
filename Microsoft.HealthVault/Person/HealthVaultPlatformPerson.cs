// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Person
{
    /// <summary>
    /// Provides low-level access to the HealthVault message operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformPerson.Current to a derived class to intercept all message calls.
    /// </remarks>
    internal class HealthVaultPlatformPerson
    {
        internal static HealthVaultPlatformPerson Current { get; private set; } = new HealthVaultPlatformPerson();

        #region ApplicationSettings

        /// <summary>
        /// Gets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// The complete set application settings including the XML settings, selected record ID, etc.
        /// </returns>
        ///
        public virtual async Task<ApplicationSettings> GetApplicationSettingsAsync(IHealthVaultConnection connection)
        {
            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetApplicationSettings, 1).ConfigureAwait(false);

            XPathExpression xPathExpression = GetPersonAppSettingsXPathExpression(responseData.InfoNavigator);

            XPathNavigator appSettingsNav = responseData.InfoNavigator.SelectSingleNode(xPathExpression);

            ApplicationSettings settings = null;
            if (appSettingsNav != null)
            {
                settings = new ApplicationSettings();
                settings.ParseXml(appSettingsNav);
            }

            return settings;
        }

        /// <summary>
        /// Sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="applicationSettings">
        /// The application settings XML.
        /// </param>
        ///
        /// <remarks>
        /// This may be <b>null</b> if no application settings have been
        /// stored for the application or user.
        /// </remarks>
        ///
        public virtual async Task SetApplicationSettingsAsync(
            IHealthVaultConnection connection,
            IXPathNavigable applicationSettings)
        {
            string requestParameters =
                GetSetApplicationSettingsParameters(applicationSettings);

            await SetApplicationSettingsAsync(connection, requestParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="requestParameters">
        /// The request parameters.
        /// </param>
        ///
        /// <remarks>
        /// This may be <b>null</b> if no application settings have been
        /// stored for the application or user.
        /// </remarks>
        ///
        public virtual async Task SetApplicationSettingsAsync(
            IHealthVaultConnection connection,
            string requestParameters)
        {
            await connection.ExecuteAsync(HealthVaultMethods.SetApplicationSettings, 1, requestParameters).ConfigureAwait(false);
        }

        internal static string GetSetApplicationSettingsParameters(
            IXPathNavigable applicationSettings)
        {
            string result = null;
            if (applicationSettings == null)
            {
                result = "<app-settings />";
            }
            else
            {
                result = applicationSettings.CreateNavigator().OuterXml;
            }

            return result;
        }

        private static XPathExpression s_infoPersonAppSettingsPath =
            XPathExpression.Compile("/wc:info");

        private static XPathExpression GetPersonAppSettingsXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.GetApplicationSettings");

            XPathExpression infoPersonAppSettingsPathClone = null;
            lock (s_infoPersonAppSettingsPath)
            {
                infoPersonAppSettingsPathClone
                    = s_infoPersonAppSettingsPath.Clone();
            }

            infoPersonAppSettingsPathClone.SetContext(infoXmlNamespaceManager);

            return infoPersonAppSettingsPathClone;
        }

        #endregion

        #region GetPersonInfo

        /// <summary>
        /// Gets the information about the person specified.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// Information about the person's HealthVault account.
        /// </returns>
        ///
        /// <remarks>
        /// This method always calls the HealthVault service to get the latest
        /// information. It is recommended that the calling application cache
        /// the return value and only call this method again if it needs to
        /// refresh the cache.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public virtual async Task<PersonInfo> GetPersonInfoAsync(IHealthVaultConnection connection)
        {
            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetPersonInfo, 1).ConfigureAwait(false);

            XPathExpression personPath = GetPersonXPathExpression(responseData.InfoNavigator);

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(personPath);

            return PersonInfo.CreateFromXml(infoNav);
        }

        private static XPathExpression s_infoPersonPath =
            XPathExpression.Compile("/wc:info/person-info");

        private static XPathExpression GetPersonXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.GetPersonInfo");

            XPathExpression infoPersonPathClone = null;
            lock (s_infoPersonPath)
            {
                infoPersonPathClone = s_infoPersonPath.Clone();
            }

            infoPersonPathClone.SetContext(infoXmlNamespaceManager);

            return infoPersonPathClone;
        }

        #endregion GetPersonInfo

        #region GetAuthorizedRecords

        /// <summary>
        /// Gets the <see cref="HealthRecordInfo"/> for the records identified
        /// by the specified <paramref name="recordIds"/>.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="recordIds">
        /// The unique identifiers for the records to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A collection of the records matching the specified record
        /// identifiers and authorized for the authenticated person.
        /// </returns>
        ///
        /// <remarks>
        /// This method is useful in cases where the application is storing
        /// record identifiers and needs access to the functionality provided
        /// by the object model.
        /// </remarks>
        ///
        public virtual async Task<Collection<HealthRecordInfo>> GetAuthorizedRecordsAsync(
            IHealthVaultConnection connection,
            IList<Guid> recordIds)
        {
            StringBuilder parameters = new StringBuilder(128);
            foreach (Guid id in recordIds)
            {
                parameters.Append(
                    "<id>" + id + "</id>");
            }

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetAuthorizedRecords, 1, parameters.ToString()).ConfigureAwait(false);

            Collection<HealthRecordInfo> results =
                new Collection<HealthRecordInfo>();

            XPathNodeIterator records =
                responseData.InfoNavigator.Select(
                    GetRecordXPathExpression(responseData.InfoNavigator));

            foreach (XPathNavigator recordNav in records)
            {
                results.Add(HealthRecordInfo.CreateFromXml(connection, recordNav));
            }

            return results;
        }

        private static XPathExpression s_infoRecordPath =
            XPathExpression.Compile("/wc:info/record");

        private static XPathExpression GetRecordXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);
            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.GetAuthorizedRecords");

            XPathExpression infoRecordPathClone = null;
            lock (s_infoRecordPath)
            {
                infoRecordPathClone = s_infoRecordPath.Clone();
            }

            infoRecordPathClone.SetContext(infoXmlNamespaceManager);

            return infoRecordPathClone;
        }

        #endregion GetAuthorizedRecords
    }
}
