// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

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
        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        ///
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked.
        /// </remarks>
        ///
        /// <param name="mock">The mocking class.</param>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        ///
        public static void EnableMock(HealthVaultPlatformPerson mock)
        {
            Validator.ThrowInvalidIf(saved != null, "ClassAlreadyMocked");

            saved = Current;
            Current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        ///
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(saved, "ClassIsntMocked");

            Current = saved;
            saved = null;
        }

        internal static HealthVaultPlatformPerson Current { get; private set; } = new HealthVaultPlatformPerson();

        private static HealthVaultPlatformPerson saved;

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
        public virtual async Task<ApplicationSettings> GetApplicationSettingsAsync(IConnectionInternal connection)
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
            IConnectionInternal connection,
            IXPathNavigable applicationSettings)
        {
            string requestParameters =
                GetSetApplicationSettingsParameters(applicationSettings);

            await this.SetApplicationSettingsAsync(connection, requestParameters).ConfigureAwait(false);
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
            IConnectionInternal connection,
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

        private static XPathExpression infoPersonAppSettingsPath =
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
            lock (infoPersonAppSettingsPath)
            {
                infoPersonAppSettingsPathClone
                    = infoPersonAppSettingsPath.Clone();
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
        public virtual async Task<PersonInfo> GetPersonInfoAsync(IConnectionInternal connection)
        {
            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetPersonInfo, 1).ConfigureAwait(false);

            XPathExpression personPath = GetPersonXPathExpression(responseData.InfoNavigator);

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(personPath);

            return PersonInfo.CreateFromXml(infoNav);
        }

        private static XPathExpression infoPersonPath =
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
            lock (infoPersonPath)
            {
                infoPersonPathClone = infoPersonPath.Clone();
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
            IConnectionInternal connection,
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

        private static XPathExpression infoRecordPath =
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
            lock (infoRecordPath)
            {
                infoRecordPathClone = infoRecordPath.Clone();
            }

            infoRecordPathClone.SetContext(infoXmlNamespaceManager);

            return infoRecordPathClone;
        }

        #endregion GetAuthorizedRecords
    }
}
