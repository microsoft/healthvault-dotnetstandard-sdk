// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Package;
using Microsoft.HealthVault.PatientConnect;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Web;

namespace Microsoft.HealthVault.PlatformPrimitives
{

    /// <summary>
    /// Provides low-level access to the HealthVault patient connect operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set 
    /// HealthVaultPlatformPatientConnect.Current to a derived class to intercept all message calls.
    /// </remarks>

    public class HealthVaultPlatformPatientConnect
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
        public static void EnableMock(HealthVaultPlatformPatientConnect mock)
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
        internal static HealthVaultPlatformPatientConnect Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformPatientConnect _current = new HealthVaultPlatformPatientConnect();
        private static HealthVaultPlatformPatientConnect _saved;

        #region PatientConnect
        /// <summary>
        /// Asks HealthVault to create a pending patient connection for the application specified
        /// by the connection with the specified user specific parameters.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault. The application ID in the connection is used
        /// when making the patient connection.
        /// </param>
        /// 
        /// <param name="friendlyName">
        /// A friendly name for the patient connection which will be shown to the user when they
        /// go to HealthVault Shell to validate the connection.
        /// </param>
        /// 
        /// <param name="securityQuestion">
        /// A question (usually provided by the patient) to which the patient must provide the 
        /// answer when they go to validate the connection in the HealthVault Shell.
        /// </param>
        /// 
        /// <param name="securityAnswer">
        /// The answer to the <paramref name="securityQuestion"/> which the patient must use
        /// when validating the connection in HealthVault Shell. The answer is case-insensitive but
        /// otherwise must match exactly. In most cases it is recommended that this is a single 
        /// word to prevent entry problems when validating the connection.
        /// </param>
        /// 
        /// <param name="callbackUrl">
        /// Not yet implemented. May be null.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="friendlyName"/>, <paramref name="securityQuestion"/>,
        /// <paramref name="securityAnswer"/>, or <paramref name="applicationPatientId"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual string CreatePatientConnection(
            OfflineWebApplicationConnection connection,
            string friendlyName,
            string securityQuestion,
            string securityAnswer,
            Uri callbackUrl,
            string applicationPatientId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(friendlyName, "friendlyName");
            Validator.ThrowIfStringNullOrEmpty(securityQuestion, "securityQuestion");
            Validator.ThrowIfStringNullOrEmpty(securityAnswer, "securityAnswer");
            Validator.ThrowIfStringNullOrEmpty(applicationPatientId, "applicationPatientId");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "CreateConnectRequest", 1);

            request.Parameters =
                GetCreateConnectRequestParameters(
                    friendlyName,
                    securityQuestion,
                    securityAnswer,
                    callbackUrl,
                    applicationPatientId);

            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "CreateConnectRequest");

            XPathNavigator infoNav = request.Response.InfoNavigator.SelectSingleNode(infoPath);
            return infoNav.SelectSingleNode("identity-code").Value;
        }

        private static string GetCreateConnectRequestParameters(
            string friendlyName,
            string securityQuestion,
            string securityAnswer,
            Uri callbackUrl,
            string applicationPatientId)
        {
            StringBuilder result = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                writer.WriteElementString("friendly-name", friendlyName);
                writer.WriteElementString("question", securityQuestion);
                writer.WriteElementString("answer", securityAnswer);
                writer.WriteElementString("external-id", applicationPatientId);

                if (callbackUrl != null)
                {
                    writer.WriteElementString("call-back-url", callbackUrl.OriginalString);
                }
                writer.Flush();
                writer.Close();
            }
            return result.ToString();
        }

        /// <summary>
        /// Deletes a request for a connection that has been made by the calling application but
        /// has not been validated by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application's identifier for the user which was used to create the connection 
        /// request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationPatientId"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual void DeletePendingPatientConnection(
            OfflineWebApplicationConnection connection,
            string applicationPatientId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(applicationPatientId, "applicationPatientId");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "DeletePendingConnectRequest", 1);

            request.Parameters = "<external-id>" + applicationPatientId + "</external-id>";

            request.Execute();
        }

        /// <summary>
        /// Updates an existing pending patient connection with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="oldApplicationPatientId">
        /// The application patient identifier that was used to make the initial connection request.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="oldApplicationPatientId"/> or <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual void UpdatePatientConnectionApplicationPatientId(
            OfflineWebApplicationConnection connection,
            string oldApplicationPatientId,
            string newApplicationPatientId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(oldApplicationPatientId, "oldApplicationPatientId");
            Validator.ThrowIfStringNullOrEmpty(newApplicationPatientId, "newApplicationPatientId");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "UpdateExternalId", 1);

            request.Parameters = String.Join(
                String.Empty,
                new string[] { 
                    "<old-external-id>", 
                    oldApplicationPatientId, 
                    "</old-external-id>", 
                    "<new-external-id>", 
                    newApplicationPatientId, 
                    "</new-external-id>" });

            request.Execute();
        }

        /// <summary>
        /// Gets the connections for the application that people have accepted since the specified
        /// date.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The application's connection to HealthVault.
        /// </param>
        /// 
        /// <param name="validatedSince">
        /// Connections that have been validated since this date will be returned. The date passed
        /// should be in UTC time.
        /// </param>
        /// 
        /// <returns>
        /// A collection of the connections that people have accepted.
        /// </returns>
        /// 
        /// <remarks>
        /// Validated connect requests are removed by HealthVault after 90 days. It is advised 
        /// that applications call <see cref="GetValidatedPatientConnections(OfflineWebApplicationConnection, DateTime)"/> 
        /// daily or weekly to ensure that all validated connect requests are retrieved.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        public virtual Collection<ValidatedPatientConnection> GetValidatedPatientConnections(
            OfflineWebApplicationConnection connection,
            DateTime validatedSince)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetAuthorizedConnectRequests", 1);

            if (validatedSince != DateTime.MinValue)
            {
                request.Parameters =
                    "<authorized-connect-requests-since>" +
                    SDKHelper.XmlFromDateTime(validatedSince) +
                    "</authorized-connect-requests-since>";
            }

            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "GetAuthorizedConnectRequests");

            XPathNavigator infoNav = request.Response.InfoNavigator.SelectSingleNode(infoPath);

            Collection<ValidatedPatientConnection> result =
                new Collection<ValidatedPatientConnection>();

            XPathNodeIterator validatedConnectionsIterator = infoNav.Select("connect-request");

            foreach (XPathNavigator nav in validatedConnectionsIterator)
            {
                ValidatedPatientConnection validatedConnection = new ValidatedPatientConnection();
                validatedConnection.ParseXml(nav);

                result.Add(validatedConnection);
            }
            return result;
        }
        #endregion PatientConnect

        #region ConnectPackage

        /// <summary>
        /// Asks HealthVault to create a pending package for the application specified
        /// by the connection with the specified user specific parameters and the pre-allocated
        /// identity code.
        /// </summary>
        /// 
        /// <remarks>
        /// The password protected package supports 2 encryption algorithms, AES256 (recommended)
        /// and TripleDES. 
        /// <br/><br/>
        /// For AES256, the supported key size is 256 bits, the blocksize is 256 bits, the IV 
        /// length is 32 bytes.
        /// <br/><br/>
        /// For TripleDES, the supported key size is 192 bits, the blocksize is 64 bits, the IV 
        /// length is 8 bytes.
        /// <br/><br/>
        /// The encryption key should be derived using the answer, the salt, and the number of hash 
        /// iterations. The decryption will generate this key via the 
        /// <see cref="Rfc2898DeriveBytes"/> class, hence, encryption should use a similar or 
        /// identical process. To ensure case-insensitivity, the answer should be converted to its
        /// lower cased form using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The salt should be at least 8 bytes long.
        /// <br/><br/>
        /// It is recommended that the number of hash iterations be at least 10000.
        /// </remarks>
        /// 
        /// <param name="creationParameters">
        /// The parameters to use when creating the package. 
        /// </param>
        /// 
        /// <param name="connectPackage">
        /// The pending connect package that the user will add to his/her record. 
        /// This package's
        /// <see cref="HealthRecordItem"/>'s <see cref="BlobStore"/> must be an encrypted 
        /// blob of xml that represents a list of HealthRecordItems. This xml blob
        /// must be a sequence of <thing/> elements, each wrapping the XML representation of a 
        /// single HealthRecordItem. Each <thing/> element may be generated by calling 
        /// <see cref="HealthRecordItem.GetItemXml()"/>.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        public virtual string CreateConnectPackage(
            ConnectPackageCreationParameters creationParameters,
            PasswordProtectedPackage connectPackage)
        {
            return CreateConnectPackage(creationParameters, connectPackage, null);
        }

        /// <summary>
        /// Asks HealthVault to create a pending package for the application specified
        /// by the connection with the specified user specific parameters and the pre-allocated
        /// identity code.
        /// </summary>
        /// 
        /// <remarks>
        /// The password protected package supports 2 encryption algorithms, AES256 (recommended)
        /// and TripleDES. 
        /// <br/><br/>
        /// For AES256, the supported key size is 256 bits, the blocksize is 256 bits, the IV 
        /// length is 32 bytes.
        /// <br/><br/>
        /// For TripleDES, the supported key size is 192 bits, the blocksize is 64 bits, the IV 
        /// length is 8 bytes.
        /// <br/><br/>
        /// The encryption key should be derived using the answer, the salt, and the number of hash 
        /// iterations. The decryption will generate this key via the 
        /// <see cref="Rfc2898DeriveBytes"/> class, hence, encryption should use a similar or 
        /// identical process. To ensure case-insensitivity, the answer should be converted to its
        /// lower cased form using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The salt should be at least 8 bytes long.
        /// <br/><br/>
        /// It is recommended that the number of hash iterations be at least 10000.
        /// </remarks>
        /// 
        /// <param name="creationParameters">
        /// The parameters to use when creating the package. 
        /// </param>
        /// 
        /// <param name="connectPackage">
        /// The pending connect package that the user will add to his/her record. 
        /// This package's
        /// <see cref="HealthRecordItem"/>'s <see cref="BlobStore"/> must be an encrypted 
        /// blob of xml that represents a list of HealthRecordItems. This xml blob
        /// must be a sequence of <thing/> elements, each wrapping the XML representation of a 
        /// single HealthRecordItem. Each <thing/> element may be generated by calling 
        /// <see cref="HealthRecordItem.GetItemXml()"/>.
        /// </param>
        /// 
        /// <param name="packageContentsBlobUrls">
        /// URLs of the streamed blobs of the package contents.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        public virtual string CreateConnectPackage(
            ConnectPackageCreationParameters creationParameters,
            PasswordProtectedPackage connectPackage,
            IEnumerable<Uri> packageContentsBlobUrls)
        {
            Validator.ThrowIfArgumentNull(
                connectPackage,
                "connectPackage",
                "PackageCreatePPPMissingMandatory");

            var packageHelper = new ConnectPackageHelper(creationParameters, connectPackage, packageContentsBlobUrls);
            return packageHelper.CreateConnectPackage();
        }

        /// <summary>
        /// Deletes all packages that have been created by the calling application 
        /// for the applicationPatientId and have not been accepted by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific user ID that was used to create the connection 
        /// request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationPatientId"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual void DeletePendingConnectPackages(
            OfflineWebApplicationConnection connection,
            string applicationPatientId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(applicationPatientId, "applicationPatientId");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "DeletePendingConnectPackage", 1);

            request.Parameters = "<external-id>" + applicationPatientId + "</external-id>";

            request.Execute();
        }

        /// <summary>
        /// Deletes a single package that has been created by the calling application but has not been 
        /// accepted by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="identityCode">
        /// The unique token that identifies the package.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="identityCode"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual void DeletePendingConnectionPackageForIdentityCode(
            OfflineWebApplicationConnection connection,
            string identityCode)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(identityCode, "identityCode");

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;

            StringBuilder requestBuilder = new StringBuilder(256);
            using (XmlWriter writer = XmlWriter.Create(requestBuilder, writerSettings))
            {
                writer.WriteElementString("identity-code", identityCode);
            }

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "DeletePendingConnectPackage", 1);
            request.Parameters = requestBuilder.ToString();

            request.Execute();
        }

        /// <summary>
        /// Updates existing pending packages with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="oldApplicationPatientId">
        /// The application patient identifier that was used to create the initial package.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="oldApplicationPatientId"/> or <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual void UpdateConnectPackageApplicationPatientId(
            OfflineWebApplicationConnection connection,
            string oldApplicationPatientId,
            string newApplicationPatientId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(oldApplicationPatientId, "oldApplicationPatientId");
            Validator.ThrowIfStringNullOrEmpty(newApplicationPatientId, "newApplicationPatientId");

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;

            StringBuilder requestBuilder = new StringBuilder(256);

            using (XmlWriter writer = XmlWriter.Create(requestBuilder, writerSettings))
            {
                writer.WriteElementString("old-external-id", oldApplicationPatientId);
                writer.WriteElementString("new-external-id", newApplicationPatientId);
            }

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "UpdateExternalId", 1);

            request.Parameters = requestBuilder.ToString();
            request.Execute();
        }

        /// <summary>
        /// Updates an existing pending package with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="identityCode">
        /// The unique token that identifies the package.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public virtual void UpdateConnectPackageApplicationPatientIdForIdentityCode(
            OfflineWebApplicationConnection connection,
            string identityCode,
            string newApplicationPatientId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");
            Validator.ThrowIfStringNullOrEmpty(identityCode, "identityCode");
            Validator.ThrowIfStringNullOrEmpty(newApplicationPatientId, "newApplicationPatientId");

            StringBuilder requestBuilder = new StringBuilder(256);

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestBuilder, writerSettings))
            {
                writer.WriteElementString("identity-code", identityCode);
                writer.WriteElementString("new-external-id", newApplicationPatientId);
            }

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "UpdateExternalId", 1);

            request.Parameters = requestBuilder.ToString();

            request.Execute();
        }

        /// <summary>
        /// Allocates a package ID within HealthVault and returns it.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The package ID is allocated as a place holder for information that 
        /// is identifiable but not yet available through the HealthVault service.
        /// The returned package ID token should be stored or given to a patient, 
        /// then used in a call to CreateConnectPackage()
        /// to send the package data to the HealthVault service.</para>
        /// <para>The package ID is not a GUID.  It uses a shorter format that is more
        /// convenient for offline delivery and manual data entry.  The HealthVault 
        /// service guarantees that each package ID is unique for the lifetime of the 
        /// package.  Once the package has been accepted by the patient using the 
        /// HealthVault Shell, or explicitly deleted using the API, the package ID is 
        /// deallocated and may be reused.</para>
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        public virtual String AllocateConnectPackageId(
            OfflineWebApplicationConnection connection)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "AllocatePackageId", 1);

            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "AllocatePackageId");

            XPathNavigator infoNav = request.Response.InfoNavigator.SelectSingleNode(infoPath);
            return infoNav.SelectSingleNode("identity-code").Value;
        }

        #endregion
    }
}

