// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.Health.ItemTypes;
using Microsoft.Health.Web;

namespace Microsoft.Health.Package
{
    /// <summary>
    /// Methods for accessing the package APIs of HealthVault.
    /// </summary>
    /// 
    /// <remarks>
    /// The ConnectPackage class allows applications to send user data to the user's
    /// HealthVault account, outside of the context of an ongoing interaction with the user.
    /// The application can do this by sending a data package to the HealthVault service
    /// along with the application's identifier and user specific information.
    /// The user can then go to HealthVault Shell and add the package's contents to their 
    /// appropriate health record.
    /// </remarks>
    /// 
    public static class ConnectPackage
    {
        /// <summary>
        /// Represents a package of user data that is created by HealthVault in order
        /// to be retrieved by a user using the HealthVault Shell.
        /// </summary>
        /// 
        /// <remarks>
        /// Package encryption is delegated to the .NET Crypto classes. The encryption algorithm 
        /// supported by default is AES256. If TripleDES is required, the caller should create 
        /// the custom Password Protected Package and call <see cref="Create(Microsoft.Health.Web.OfflineWebApplicationConnection, string, string, string, Microsoft.Health.ItemTypes.PasswordProtectedPackage)"/>.
        /// <br/><br/>
        /// The answer key provided is not the actual key to the decryption. A key is derived using 
        /// the answer, the salt, and the number of hash iterations (via the 
        /// <see cref="Rfc2898DeriveBytes"/> class). To ensure case-insensitivity, the answer 
        /// is lower cased using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The data must be appended to the hash, then encrypted and then Base64 
        /// encoded.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The application connection to HealthVault. The application ID in the connection is used
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
        /// <paramref name="securityAnswer"/> when they go to validate the connection in 
        /// the HealthVault Shell.
        /// </param>
        /// 
        /// <param name="securityAnswer">
        /// The answer to the <paramref name="securityQuestion"/> which the patient must use
        /// when adding the package to their record via HealthVault Shell. The answer is 
        /// case-insensitive but otherwise must match exactly. Additionally, it must be at least 
        /// 6 characters long.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage, whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <param name="packageContents">
        /// The list of HealthRecordItems that will be encrypted and added to the package that the 
        /// user will claim via HealthVault Shell.
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
        /// <paramref name="securityAnswer"/>, <paramref name="applicationPatientId"/> or
        /// any element in <paramref name="packageContents"/> are
        /// <b>null</b> or empty.        
        /// </exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="securityAnswer"/> is less than 6 characters.
        /// </exception>
        /// 
        /// <exception cref="NotSupportedException">
        /// One of the items in <paramref name="packageContents"/> is signed and contains
        /// streamed blobs. This is not supported.        
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static string Create(
            OfflineWebApplicationConnection connection,
            string friendlyName,
            string securityQuestion,
            string securityAnswer,
            string applicationPatientId,
            IList<HealthRecordItem> packageContents)
        {
            using (ConnectPackageCreationParameters creationParameters = new ConnectPackageCreationParameters(
                connection,
                friendlyName,
                securityQuestion,
                securityAnswer,
                applicationPatientId))
            {

                return CreatePackageWithContentsAllParameters(
                    creationParameters,
                    packageContents);
            }
        }

        /// <summary>
        /// Represents a package of user data that is created by HealthVault in order
        /// to be retrieved by a user using the HealthVault Shell.
        /// </summary>
        /// 
        /// <remarks>
        /// Package encryption is delegated to the .NET Crypto classes. The encryption algorithm 
        /// supported by default is AES256. If TripleDES is required, the caller should create 
        /// the custom Password Protected Package and call <see cref="Create(Microsoft.Health.Web.OfflineWebApplicationConnection, string, string, string, Microsoft.Health.ItemTypes.PasswordProtectedPackage)"/>.
        /// <br/><br/>
        /// The answer key provided is not the actual key to the decryption. A key is derived using 
        /// the answer, the salt, and the number of hash iterations (via the 
        /// <see cref="Rfc2898DeriveBytes"/> class). To ensure case-insensitivity, the answer 
        /// is lower cased using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The data must be appended to the hash, then encrypted and then Base64 
        /// encoded.
        /// </remarks>
        ///<param name="creationParameters">
        /// The <see cref="ConnectPackageCreationParameters"/> to be used while creating the 
        /// connect package.
        ///</param>
        ///
        /// <param name="packageContents">
        /// The list of HealthRecordItems that will be encrypted and added to the package that the 
        /// user will claim via HealthVault Shell.
        /// </param>
        /// 
        /// <exception cref="NotSupportedException">
        /// One of the items in <paramref name="packageContents"/> is signed and contains
        /// streamed blobs. This is not supported.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        public static string Create(
            ConnectPackageCreationParameters creationParameters,
            IList<HealthRecordItem> packageContents)
        {
            return CreatePackageWithContentsAllParameters(
                creationParameters,
                packageContents);
        }

        /// <summary>
        /// Asks HealthVault to create a pending package for the application specified
        /// by the connection with the specified user specific parameters.
        /// </summary>
        /// 
        /// <remarks>
        /// The encryption is delegated to the .NET Crypto classes. The encryption algorithm 
        /// supported by default is AES256. If TripleDES is required, the caller should create 
        /// the custom Password Protected Package and call <see cref="Create(Microsoft.Health.Web.OfflineWebApplicationConnection, string, string, string, Microsoft.Health.ItemTypes.PasswordProtectedPackage)"/>.
        /// <br/><br/>
        /// The answer key provided is not the actual key to the decryption. A key is derived using 
        /// the answer, the salt, and the number of hash iterations (via the 
        /// <see cref="Rfc2898DeriveBytes"/> class). To ensure case-insensitivity, the answer 
        /// is lower cased using <see cref="String.ToLowerInvariant()"/> (culturally-agnostic) 
        /// prior to generating the derived key.
        /// <br/><br/>
        /// The algorithm used has the following parameters:
        /// <ul>
        ///    <li>Mode = CipherMode.CBC</li>
        ///    <li>Padding = PaddingMode.ISO10126</li>
        /// </ul>
        /// <br/><br/>
        /// The salt supplied is used as the salt to the derived key as well as the key to the 
        /// supplied HMAC. The data must be appended to the hash, then encrypted and then Base64 
        /// encoded.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The application connection to HealthVault. The application ID in the connection is used
        /// when making the patient connection.
        /// </param>
        /// 
        /// <param name="identityCode">
        /// The application unique identifier of the package.
        /// </param>
        /// 
        /// <param name="friendlyName">
        /// A friendly name for the patient connection which will be shown to the user when they
        /// go to HealthVault Shell to validate the connection.
        /// </param>
        /// 
        /// <param name="securityQuestion">
        /// A question (usually provided by the patient) to which the patient must provide the 
        /// <paramref name="securityAnswer"/> when they go to validate the connection in 
        /// the HealthVault Shell.
        /// </param>
        /// 
        /// <param name="securityAnswer">
        /// The answer to the <paramref name="securityQuestion"/> which the patient must use
        /// when adding the package to their record via HealthVault Shell. The answer is 
        /// case-insensitive but otherwise must match exactly. Additionally, it must be at least 
        /// 6 characters long.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage, whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <param name="packageContents">
        /// The list of HealthRecordItems that will be encrypted and added to the package that the 
        /// user will claim via HealthVault Shell.
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
        /// If <paramref name="identityCode"/>, <paramref name="friendlyName"/>, <paramref name="securityQuestion"/>,
        /// <paramref name="securityAnswer"/>, <paramref name="applicationPatientId"/> or
        /// any element in <paramref name="packageContents"/> are
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="securityAnswer"/> is less than 6 characters.
        /// </exception>
        /// 
        /// <exception cref="NotSupportedException">
        /// One of the items in <paramref name="packageContents"/> is signed and contains
        /// streamed blobs. This is not supported.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'creationParameters' variable is returned to the caller.")]
        public static string CreatePackage(
            OfflineWebApplicationConnection connection,
            string identityCode,
            string friendlyName,
            string securityQuestion,
            string securityAnswer,
            string applicationPatientId,
            IList<HealthRecordItem> packageContents)
        {
            ConnectPackageCreationParameters creationParameters =
                new ConnectPackageCreationParameters(
                    connection,
                    identityCode,
                    friendlyName,
                    securityQuestion,
                    securityAnswer,
                    applicationPatientId);

            return CreatePackageWithContentsAllParameters(
                creationParameters,
                packageContents);
        }

        private static string CreatePackageWithContentsAllParameters(
            ConnectPackageCreationParameters creationParameters,
            IEnumerable<HealthRecordItem> packageContents)
        {
            var helper = new ConnectPackageHelper(creationParameters, packageContents);

            return HealthVaultPlatform.CreateConnectPackage(
                    creationParameters,
                    helper.CreatePasswordProtectedPackageItem(),
                    helper.GetPackageContentsStreamedBlobUrls());
        }

        /// <summary>
        /// Asks HealthVault to create a pending package for the application specified
        /// by the connection with the specified user specific parameters.
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
        /// <param name="connection">
        /// The application connection to HealthVault. The application ID in the connection is used
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
        /// answer when they go to validate the connection in the 
        /// HealthVault Shell.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <param name="connectPackage">
        /// The pending connect package that the user will add to his/her record. This package's
        /// <see cref="Blob"/> must be an encrypted and Base64 
        /// encoded blob of xml that represents a list of HealthRecordItems. This xml blob
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
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="friendlyName"/>, <paramref name="securityQuestion"/>,
        /// <paramref name="applicationPatientId"/>, or <paramref name="connectPackage"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'creationParameters' variable is returned to the caller.")]
        public static string Create(
            OfflineWebApplicationConnection connection,
            string friendlyName,
            string securityQuestion,
            string applicationPatientId,
            PasswordProtectedPackage connectPackage)
        {
            ConnectPackageCreationParameters creationParameters =
                new ConnectPackageCreationParameters(
                    connection,
                    null,
                    friendlyName,
                    securityQuestion,
                    null,
                    applicationPatientId,
                    false,
                    false);

            return HealthVaultPlatform.CreateConnectPackage(
                    creationParameters,
                    connectPackage);
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
        /// <param name="connection">
        /// The application connection to HealthVault. The application ID in the connection is used
        /// when making the patient connection.
        /// </param>
        /// 
        /// <param name="identityCode">
        /// A package identity token previously obtained from <see cref="ConnectPackage.AllocatePackageId"/>.  
        /// </param>
        /// 
        /// <param name="friendlyName">
        /// A friendly name for the patient connection which will be shown to the user when they
        /// go to HealthVault Shell to validate the connection.
        /// </param>
        /// 
        /// <param name="securityQuestion">
        /// A question (usually provided by the patient) to which the patient must provide the 
        /// answer when they go to validate the connection in the 
        /// HealthVault Shell.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
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
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="identityCode"/>, <paramref name="friendlyName"/>, 
        /// <paramref name="securityQuestion"/>,
        /// <paramref name="applicationPatientId"/>, or <paramref name="connectPackage"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "'creationParameters' variable is returned to the caller.")]
        public static string Create(
            OfflineWebApplicationConnection connection,
            string identityCode,
            string friendlyName,
            string securityQuestion,
            string applicationPatientId,
            PasswordProtectedPackage connectPackage)
        {
            Validator.ThrowIfStringNullOrEmpty(identityCode, "identityCode");

            ConnectPackageCreationParameters creationParameters =
                new ConnectPackageCreationParameters(
                    connection,
                    identityCode,
                    friendlyName,
                    securityQuestion,
                    null,
                    applicationPatientId,
                    false,
                    true);

            return HealthVaultPlatform.CreateConnectPackage(
                    creationParameters,
                    connectPackage);
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
        public static void DeletePending(
            OfflineWebApplicationConnection connection,
            string applicationPatientId)
        {
            HealthVaultPlatform.DeletePendingConnectPackages(
                connection,
                applicationPatientId);
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
        public static void DeletePendingForIdentityCode(
            OfflineWebApplicationConnection connection,
            string identityCode)
        {
            HealthVaultPlatform.DeletePendingConnectionPackageForIdentityCode(
                connection,
                identityCode);
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
        public static void UpdateApplicationPatientId(
            OfflineWebApplicationConnection connection,
            string oldApplicationPatientId,
            string newApplicationPatientId)
        {
            HealthVaultPlatform.UpdateConnectPackageApplicationPatientId(
                connection,
                oldApplicationPatientId,
                newApplicationPatientId);
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
        public static void UpdateApplicationPatientIdForIdentityCode(
            OfflineWebApplicationConnection connection,
            string identityCode,
            string newApplicationPatientId)
        {
            HealthVaultPlatform.UpdateConnectPackageApplicationPatientIdForIdentityCode(
                connection,
                identityCode,
                newApplicationPatientId);
        }

        /// <summary>
        /// Allocates a package ID within HealthVault and returns it.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The package ID is allocated as a place holder for information that 
        /// is identifiable but not yet available through the HealthVault service.
        /// The returned package ID token should be stored or given to a patient, 
        /// then used in a call to <see cref="ConnectPackage.Create(OfflineWebApplicationConnection, string, string, string, PasswordProtectedPackage)"/>
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
        public static String AllocatePackageId(
            OfflineWebApplicationConnection connection)
        {
            return HealthVaultPlatform.AllocateConnectPackageId(connection);
        }
    }
}
