// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformPrimitives
{
    /// <summary>
    /// Provides low-level access to the HealthVault message operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformPerson.Current to a derived class to intercept all message calls.
    /// </remarks>
    public class HealthVaultPlatformPerson
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
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
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
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }

        internal static HealthVaultPlatformPerson Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformPerson _current = new HealthVaultPlatformPerson();
        private static HealthVaultPlatformPerson _saved;

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
        public virtual ApplicationSettings GetApplicationSettings(HealthServiceConnection connection)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetApplicationSettings", 1);

            request.Execute();

            XPathExpression xPathExpression
                = GetPersonAppSettingsXPathExpression(
                            request.Response.InfoNavigator);

            XPathNavigator appSettingsNav
                = request.Response.InfoNavigator
                    .SelectSingleNode(xPathExpression);

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
        public virtual void SetApplicationSettings(
            HealthServiceConnection connection,
            IXPathNavigable applicationSettings)
        {
            string requestParameters =
                GetSetApplicationSettingsParameters(applicationSettings);

            SetApplicationSettings(connection, requestParameters);
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
        public virtual void SetApplicationSettings(
            HealthServiceConnection connection,
            string requestParameters)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "SetApplicationSettings", 1);

            request.Parameters = requestParameters;
            request.Execute();
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

        private static XPathExpression _infoPersonAppSettingsPath =
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
            lock (_infoPersonAppSettingsPath)
            {
                infoPersonAppSettingsPathClone
                    = _infoPersonAppSettingsPath.Clone();
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
        public virtual PersonInfo GetPersonInfo(ApplicationConnection connection)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetPersonInfo", 1);

            request.Execute();

            XPathExpression personPath =
                GetPersonXPathExpression(request.Response.InfoNavigator);

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(personPath);

            return PersonInfo.CreateFromXml(connection, infoNav);
        }

        private static XPathExpression _infoPersonPath =
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
            lock (_infoPersonPath)
            {
                infoPersonPathClone = _infoPersonPath.Clone();
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
        public virtual Collection<HealthRecordInfo> GetAuthorizedRecords(
            ApplicationConnection connection,
            IList<Guid> recordIds)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetAuthorizedRecords", 1);

            StringBuilder parameters = new StringBuilder(128);
            foreach (Guid id in recordIds)
            {
                parameters.Append(
                    "<id>" + id.ToString() + "</id>");
            }
            request.Parameters = parameters.ToString();

            request.Execute();

            Collection<HealthRecordInfo> results =
                new Collection<HealthRecordInfo>();

            XPathNodeIterator records =
                request.Response.InfoNavigator.Select(
                    GetRecordXPathExpression(request.Response.InfoNavigator));

            foreach (XPathNavigator recordNav in records)
            {
                results.Add(HealthRecordInfo.CreateFromXml(connection, recordNav));
            }
            return results;
        }

        private static XPathExpression _infoRecordPath =
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
            lock (_infoRecordPath)
            {
                infoRecordPathClone = _infoRecordPath.Clone();
            }

            infoRecordPathClone.SetContext(infoXmlNamespaceManager);

            return infoRecordPathClone;
        }

        #endregion GetAuthorizedRecords
    }
}
