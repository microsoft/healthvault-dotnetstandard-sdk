// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Health.ItemTypes;
using Microsoft.Health.Web;

namespace Microsoft.Health.Package
{
    /// <summary>
    /// The parameters that need to be supplied in order to create 
    /// a <see cref="PasswordProtectedPackage" /> through the <see cref="ConnectPackage" />
    /// creation API.
    /// </summary>
    public class ConnectPackageCreationParameters : IDisposable
    {
        /// <summary>
        /// Constructs an <see cref="ConnectPackageCreationParameters"/> instance 
        /// with supplied values.
        /// </summary>
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
        /// <paramref name="securityAnswer"/> when they go to validate the connection in the 
        /// HealthVault Shell.
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
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="friendlyName"/>, 
        /// <paramref name="securityQuestion"/>, or
        /// <paramref name="applicationPatientId"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="securityAnswer"/> is less than 6 characters.
        /// </exception>
        /// 
        public ConnectPackageCreationParameters(
            OfflineWebApplicationConnection connection,
            String friendlyName,
            String securityQuestion,
            String securityAnswer,
            String applicationPatientId) :
            this(
                connection,
                null,
                friendlyName,
                securityQuestion,
                securityAnswer,
                applicationPatientId,
                true,
                false)
        {
        }

        /// <summary>
        /// Constructs an <see cref="ConnectPackageCreationParameters"/> instance 
        /// with supplied values.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The application connection to HealthVault. The application ID in the connection is used
        /// when making the patient connection.
        /// </param>
        /// 
        /// <param name="identityCode">
        /// A package identity token previously obtained from 
        /// <see cref="ConnectPackage.AllocatePackageId"/>.  
        /// </param>
        /// 
        /// <param name="friendlyName">
        /// A friendly name for the patient connection which will be shown to the user when they
        /// go to HealthVault Shell to validate the connection.
        /// </param>
        /// 
        /// <param name="securityQuestion">
        /// A question (usually provided by the patient) to which the patient must provide the 
        /// <paramref name="securityAnswer"/> when they go to validate the connection in the 
        /// HealthVault Shell.
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
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="identityCode"/>, <paramref name="friendlyName"/>, 
        /// <paramref name="securityQuestion"/>, or
        /// <paramref name="applicationPatientId"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="securityAnswer"/> is less than 6 characters.
        /// </exception>
        /// 
        public ConnectPackageCreationParameters(
            OfflineWebApplicationConnection connection,
            String identityCode,
            String friendlyName,
            String securityQuestion,
            String securityAnswer,
            String applicationPatientId) :
            this(
                connection,
                identityCode,
                friendlyName,
                securityQuestion,
                securityAnswer,
                applicationPatientId,
                true,
                true)
        {
        }

        internal ConnectPackageCreationParameters(
            OfflineWebApplicationConnection connection,
            String identityCode,
            String friendlyName,
            String securityQuestion,
            String securityAnswer,
            String applicationPatientId,
            Boolean isSecurityAnswerAvailable,
            Boolean isIdentityCodeRequired)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectPackageConnectionNull");

            Validator.ThrowArgumentExceptionIf(
                isIdentityCodeRequired && String.IsNullOrEmpty(identityCode),
                "identityCode",
                "PackageCreateHRIMissingMandatory");

            Validator.ThrowIfStringNullOrEmpty(friendlyName, "friendlyName");
            Validator.ThrowIfStringNullOrEmpty(securityQuestion, "friendlyName");

            if (isSecurityAnswerAvailable)
            {
                Validator.ThrowIfStringNullOrEmpty(securityAnswer, "securityAnswer");
                Validator.ThrowArgumentOutOfRangeIf(
                    securityAnswer.Length < 6,
                    "securityAnswer",
                    "PackageCreateHRIAnswerLength");
            }

            Validator.ThrowIfStringNullOrEmpty(applicationPatientId, "applicationPatientId");

            _connection = connection;
            _identityCode = identityCode;
            _friendlyName = friendlyName;
            _securityQuestion = securityQuestion;
            _securityAnswer = securityAnswer;
            _applicationPatientId = applicationPatientId;
            _salt = Guid.NewGuid().ToString();
            _passwordProtectAlgorithm = PasswordProtectAlgorithm.HmacSha256Aes256;
            _connectPackageEncryptionAlgorithmName = "AES256";
            _blobChunkEncryptionAlgorithmName = "AES256";

            _encryptionParameterGenerationIterationCount = 20000;
            if (isSecurityAnswerAvailable)
            {
                SetupEncryptionParameters();
            }
        }

        private void SetupEncryptionParameters()
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(_salt);

            // Convert to lowercase 
            String answer = _securityAnswer.ToLowerInvariant();
            SetupConnectPackageHMACAlgorithm(saltBytes);

            using (Rfc2898DeriveBytes rdbytes =
                new Rfc2898DeriveBytes(
                    Encoding.UTF8.GetBytes(answer),
                    saltBytes,
                    _encryptionParameterGenerationIterationCount))
            {
                byte[] encryptionKey = rdbytes.GetBytes(32);
                byte[] encryptionIV = rdbytes.GetBytes(32);
                SetupBlobEncryptionAlgorithm(encryptionKey, encryptionIV);
                SetupConnectPackageEncryptionAlgorithm(encryptionKey, encryptionIV);
            }
        }

        private void SetupConnectPackageHMACAlgorithm(byte[] saltBytes)
        {
            // Compute the hash
            _connectPackageHMACAlgorithm = new HMACSHA256();
            _connectPackageHMACAlgorithm.Key = saltBytes;
        }

        private void SetupBlobEncryptionAlgorithm(
            byte[] encryptionKey,
            byte[] encryptionIV)
        {
            // Set up the blob encryption algorithm
            _blobChunkEncryptionAlgorithm = new RijndaelManaged();
            _blobChunkEncryptionAlgorithm.Mode = CipherMode.CBC;
            _blobChunkEncryptionAlgorithm.Padding = PaddingMode.ISO10126;
            _blobChunkEncryptionAlgorithm.KeySize = 256;
            _blobChunkEncryptionAlgorithm.BlockSize = 256;
            _blobChunkEncryptionAlgorithm.Key = encryptionKey;
            _blobChunkEncryptionAlgorithm.IV = encryptionIV;
        }

        private void SetupConnectPackageEncryptionAlgorithm(
            byte[] encryptionKey,
            byte[] encryptionIV)
        {
            // Set up the blob encryption algorithm
            _connectPackageEncryptionAlgorithm = new RijndaelManaged();
            _connectPackageEncryptionAlgorithm.Mode = CipherMode.CBC;
            _connectPackageEncryptionAlgorithm.Padding = PaddingMode.ISO10126;
            _connectPackageEncryptionAlgorithm.KeySize = 256;
            _connectPackageEncryptionAlgorithm.BlockSize = 256;
            _connectPackageEncryptionAlgorithm.Key = encryptionKey;
            _connectPackageEncryptionAlgorithm.IV = encryptionIV;
        }

        /// <summary>
        /// Gets the package identity token previously obtained from 
        /// <see cref="ConnectPackage.AllocatePackageId"/>.  
        /// </summary>
        public string IdentityCode
        {
            get { return _identityCode; }
        }
        private string _identityCode;

        /// <summary>
        /// Gets the friendly name for the patient connection which will be shown to the user 
        /// when they go to HealthVault Shell to validate the connection.
        /// </summary>
        public string FriendlyName
        {
            get { return _friendlyName; }
        }
        private string _friendlyName;

        /// <summary>
        /// Gets the question (usually provided by the patient) to which the patient must provide 
        /// the  <see cref="SecurityAnswer"/> when they go to validate the connection in the 
        /// HealthVault Shell.
        /// </summary>
        public string SecurityQuestion
        {
            get { return _securityQuestion; }
        }
        private string _securityQuestion;

        /// <summary>
        /// Gets the answer to the <see cref="SecurityQuestion"/> which the patient must use
        /// when adding the package to their record via HealthVault Shell.
        /// </summary>
        public string SecurityAnswer
        {
            get { return _securityAnswer; }
        }
        private string _securityAnswer;

        /// <summary>
        /// Gets the application specific identifier for the user. This identifier is used to 
        /// uniquely identify the user in the application data storage, whereas the HealthVault 
        /// person ID is used to identify the person in HealthVault.
        /// </summary>
        public string ApplicationPatientId
        {
            get { return _applicationPatientId; }
        }
        private string _applicationPatientId;

        /// <summary>
        /// Gets the salt used to encrypt the entities within the connect package.
        /// </summary>
        public string Salt
        {
            get { return _salt; }
        }
        private string _salt;

        /// <summary>
        /// Gets the name of the algorithm used to generate the HMAC for the connect package.
        /// </summary>
        public string ConnectPackageHMACAlgorithmName
        {
            get { return _connectPackageHMACAlgorithmName; }
        }
        private string _connectPackageHMACAlgorithmName = "SHA256";

        /// <summary>
        /// Gets the algorithm used to generate the HMAC for the connect package.
        /// </summary>
        public HMAC ConnectPackageHMACAlgorithm
        {
            get { return _connectPackageHMACAlgorithm; }
        }
        private HMAC _connectPackageHMACAlgorithm;

        /// <summary>
        /// Gets the <see cref="PasswordProtectAlgorithm" /> to be used in the connect package.
        /// </summary>
        public PasswordProtectAlgorithm PasswordProtectAlgorithm
        {
            get { return _passwordProtectAlgorithm; }
        }
        private PasswordProtectAlgorithm _passwordProtectAlgorithm;

        /// <summary>
        /// Gets the name of the algorithm used to encrypt the <see cref="HealthRecordItem"/>s, 
        /// that form the contents of the connect package.
        /// </summary>
        public string ConnectPackageEncryptionAlgorithmName
        {
            get { return _connectPackageEncryptionAlgorithmName; }
        }
        private string _connectPackageEncryptionAlgorithmName = "AES256";

        /// <summary>
        /// Gets the algorithm used to encrypt the <see cref="HealthRecordItem"/>s, 
        /// that form the contents of the connect package.
        /// </summary>
        public SymmetricAlgorithm ConnectPackageEncryptionAlgorithm
        {
            get { return _connectPackageEncryptionAlgorithm; }
        }
        private SymmetricAlgorithm _connectPackageEncryptionAlgorithm;

        /// <summary>
        /// Gets the name of the algorithm used to encrypt the <see cref="BlobStream"/>s
        /// that belong to the <see cref="HealthRecordItem"/>s  within the connect package.
        /// </summary>
        public string BlobChunkEncryptionAlgorithmName
        {
            get { return _blobChunkEncryptionAlgorithmName; }
        }
        private string _blobChunkEncryptionAlgorithmName = "AES256";

        /// <summary>
        /// Gets the algorithm used to encrypt the <see cref="BlobStream"/>s
        /// that belong to the <see cref="HealthRecordItem"/>s  within the connect package.
        /// </summary>
        public SymmetricAlgorithm BlobChunkEncryptionAlgorithm
        {
            get { return _connectPackageEncryptionAlgorithm; }
        }
        private SymmetricAlgorithm _blobChunkEncryptionAlgorithm;

        /// <summary>
        /// Gets the application connection to HealthVault. The application ID in the connection 
        /// is used when making the patient connection.
        /// </summary>
        public OfflineWebApplicationConnection Connection
        {
            get { return _connection; }
        }
        OfflineWebApplicationConnection _connection;

        /// <summary>
        /// Explicit clean up.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Last resort clean up.
        /// </summary>
        ~ConnectPackageCreationParameters()
        {
            Dispose(false);
        }

        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                ((IDisposable)_connectPackageEncryptionAlgorithm).Dispose();
                ((IDisposable)_connectPackageHMACAlgorithm).Dispose();
                ((IDisposable)_blobChunkEncryptionAlgorithm).Dispose();
            }
        }

        private int _encryptionParameterGenerationIterationCount = 20000;
    }
}