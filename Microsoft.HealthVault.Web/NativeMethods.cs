// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.HealthVault.Web.Certificate;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Wrapper class for all native methods
    /// </summary>
    internal static class NativeMethods
    {
        #region Certificate Name methods

        /// <summary>
        /// 	Convert a X.500 string to an encoded certificate name
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/certstrtoname.asp
        /// </remarks>
        /// <param name="dwCertEncodingType">Encoding type to use</param>
        /// <param name="pszX500">X.500 string to convert</param>
        /// <param name="dwStrType">Type of the input string</param>
        /// <param name="pvReserved">Must be null</param>
        /// <param name="pbEncoded">Buffer to recieve the encoded structure</param>
        /// <param name="pcbEncoded">Size of the buffer</param>
        /// <param name="ppszError">Set to the point in the string where a parsing error occured</param>
        /// <returns>True on success, false on error</returns>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CertStrToName(CertEncodingType dwCertEncodingType,
                                                    [MarshalAs(UnmanagedType.LPWStr)]string pszX500,
                                                    StringType dwStrType,
                                                    IntPtr pvReserved,
                                                    byte[] pbEncoded,
                                                    [In, Out]ref int pcbEncoded,
                                                    [MarshalAs(UnmanagedType.LPWStr)]ref StringBuilder ppszError);

        // Disable "field not used" warning to leave consts for documentation purposes
#pragma warning disable 414

        /// <summary>
        /// 	Way to encode the certificate name
        /// </summary>
        [Flags]
        internal enum CertEncodingType
        {
            X509AsnEncoding = 0x00000001,
            PKCS7AsnEncoding = 0x00010000
        }

        /// <summary>
        /// String format for the certificate name
        /// </summary>
        [Flags]
        internal enum StringType
        {
            SimpleNameString = 1,
            OIDNameString = 2,
            X500NameString = 3,

            CommaFlag = 0x04000000,
            SemicolonFlag = 0x40000000,
            CRLFFlag = 0x08000000,
            NoPlusFlag = 0x20000000,
            NoQuotingFlag = 0x10000000,
            ReverseFlag = 0x02000000,
            DisableIE4UTF8Flag = 0x00010000,
            EnableT61UnicodeFlag = 0x00020000,
            EnableUTF8UnicodeFlag = 0x00040000
        }

#pragma warning restore
        #endregion

        #region Certificate cleanup methods

        /// <summary>
        /// 	Free a certificate context by decrementing its reference count
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/certfreecertificatecontext.asp
        /// </remarks>
        /// <param name="pCertContext">CERT_CONTEXT to free</param>
        /// <returns>always true</returns>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CertFreeCertificateContext(IntPtr pCertContext);

        /// <summary>
        /// 	Delete a certificate context from the certificate store
        /// </summary>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CertDeleteCertificateFromStore(IntPtr pCertContext);

        /// <summary>
        /// 	Close a certificate store handle
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/certclosestore.asp
        /// </remarks>
        /// <param name="hCertStore">handle of the store to be closed</param>
        /// <param name="dwFlags">flags to close with</param>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CertCloseStore(IntPtr hCertStore, int dwFlags);

        /// <summary>
        /// 	Close a key handle
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/cryptdestroykey.asp
        /// </remarks>
        /// <param name="hKey">key handle to close</param>
        [DllImport("Advapi32.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDestroyKey(IntPtr hKey);

        /// <summary>
        /// 	Close a key container handle
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/cryptreleasecontext.asp
        /// </remarks>
        /// <param name="hProv">key container to close</param>
        /// <param name="dwFlags">reserved, must be zero</param>
        [DllImport("Advapi32.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

        #endregion

        #region Create/Get/Export cetificate methods

        /// <summary>
        /// 	Build a self signed certificate
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/certcreateselfsigncertificate.asp
        /// </remarks>
        /// <param name="hProv">[optional] Handle of the cryptographic provider used to sign the certificate</param>
        /// <param name="pSubjectIssuerBlob">Pointer to the blob containing the DN of the certificate</param>
        /// <param name="dwFlags">Flags to override the default function behavior</param>
        /// <param name="pKeyProvInfo">[optional] information on the key provider for the certificate</param>
        /// <param name="pSignatureAlgorithm">[optional] A pointer to a CRYPT_ALGORITHM_IDENTIFIER structure.
        ///                                             If NULL, the default algorithm, SHA1RSA, is used.</param>
        /// <param name="pStartTime">[optional] begining of the certificate's validity</param>
        /// <param name="pEndTime">[optional] end of the certificate's validity</param>
        /// <param name="pExtensions">[optional] certificate extensions</param>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [SecurityCritical]
        internal static extern CertificateHandle CertCreateSelfSignCertificate(
                                                    KeyContainerHandle hProv,
                                                    CryptoApiBlob pSubjectIssuerBlob,
                                                    SelfSignFlags dwFlags,
                                                    IntPtr pKeyProvInfo,
                                                    IntPtr pSignatureAlgorithm,
                                                    [In] ref SystemTime pStartTime,
                                                    [In] ref SystemTime pEndTime,
                                                    IntPtr pExtensions);

        /// <summary>
        /// 	Open a certificate store
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/certopenstore.asp
        /// </remarks>
        /// <param name="lpszStoreProvider">Store provider type</param>
        /// <param name="dwMsgAndCertEncodingType">Encoding of the certificate store</param>
        /// <param name="hCryptProv">handle to a crypto provider</param>
        /// <param name="dwFlags">general characteristics of the store</param>
        /// <param name="pvPara">extra data for the provider</param>
        /// <returns>handle to the store, NULL on error</returns>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [SecurityCritical]
        internal static extern CertificateStoreHandle CertOpenStore(
                                                    IntPtr lpszStoreProvider,
                                                    int dwMsgAndCertEncodingType,
                                                    IntPtr hCryptProv,
                                                    int dwFlags,
                                                    IntPtr pvPara);

        /// <summary>
        /// 	Add a certificate to a certificate store
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/certaddcertificatecontexttostore.asp
        /// </remarks>
        /// <param name="hCertStore">handle of a certificate store</param>
        /// <param name="pCertContext">certificate to add to the store</param>
        /// <param name="dwAddDisposition">action to take if a matching cert is already in the store</param>
        /// <param name="ppStoreContext">[out, optional] pointer to the copy of the cert added to the store</param>
        /// <returns>true on success, false on error</returns>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool CertAddCertificateContextToStore(
                                                    CertificateStoreHandle hCertStore,
                                                    CertificateHandle pCertContext,
                                                    AddDisposition dwAddDisposition,
                                                    [Out]out CertificateHandle ppStoreContext);

        /// <summary>
        /// 	Create a key container
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/cryptacquirecontext.asp
        /// </remarks>
        /// <param name="phProv">[out]key container handle</param>
        /// <param name="pszContainer">key container name</param>
        /// <param name="pszProvider">name of the CSP to use</param>
        /// <param name="dwProvType">type of CSP to aquire</param>
        /// <param name="dwFlags">flag values</param>
        /// <returns>true on success, false on error</returns>
        [DllImport("Advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool CryptAcquireContext(
                                                    [Out]out KeyContainerHandle phProv,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string pszContainer,
                                                    [MarshalAs(UnmanagedType.LPWStr)]string pszProvider,
                                                    ProviderType dwProvType,
                                                    ContextFlags dwFlags);

        /// <summary>
        /// 	Generate a random key
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/cryptgenkey.asp
        /// </remarks>
        /// <param name="hProv">key container to put the key into</param>
        /// <param name="algId">algorithm the key to be generated is for</param>
        /// <param name="dwFlags">type of key to generate</param>
        /// <param name="phKey">[out]generated key</param>
        /// <returns>true on success, false on error</returns>
        [DllImport("Advapi32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool CryptGenKey(
                                                    KeyContainerHandle hProv,
                                                    AlgorithmType algId,
                                                    KeyFlags dwFlags,
                                                    [Out]out KeyHandle phKey);

        /// <summary>
        /// 	Export the certificates and private keys from a store
        /// </summary>
        /// <remarks>
        /// 	See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/pfxexportcertstoreex.asp
        /// </remarks>
        /// <param name="hStore">handle of the store to export certs from</param>
        /// <param name="pPFX">blob to contain the PFX data</param>
        /// <param name="szPassword">password to encrypt and verify the PFX data with</param>
        /// <param name="pvReserved">reserved for future use</param>
        /// <param name="dwFlags">export flags</param>
        /// <returns>true on success, false on error</returns>
        [DllImport("Crypt32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool PFXExportCertStoreEx(
                                                    CertificateStoreHandle hStore,
                                                    IntPtr pPFX,
                                                    IntPtr szPassword,
                                                    IntPtr pvReserved,
                                                    PfxExportFlags dwFlags);

        // Disable "field not used" warning to leave consts for documentation purposes
#pragma warning disable 414

        [Flags]
        internal enum SelfSignFlags
        {
            NoKeyInfo = 2,
            NoSign = 1,
            None = 0
        }

        internal enum AddDisposition
        {
            New = 1,
            UseExisting = 2,
            ReplaceExisting = 3,
            Always = 4,
            ReplaceExistingInheritProperties = 5
        }

        [Flags]
        internal enum PfxExportFlags
        {
            ReportNoPrivateKey = 0x00000001,
            ReportNotAbleToExportPrivateKey = 0x00000002,
            ExportPrivateKeys = 0x00000004
        }

        internal enum ProviderType
        {
            RsaFull = 1,
            RsaSignature = 2,
            Dss = 3,
            Fortezza = 4,
            MsExchange = 5,
            Ssl = 6,
            RsaSecureChannel = 12,
            DssDiffieHellman = 13,
            EcDsaSignature = 14,
            EcNraSignature = 15,
            EcDsaFull = 16,
            EcNraFull = 17,
            DiffieHellmanSecureChannel = 18,
            SpyrusLynks = 20,
            RandomNumberGenerator = 21,
            IntelSec = 22,
            ReplaceOwf = 23,
            RsaAes = 24
        }

        [Flags]
        internal enum ContextFlags : uint
        {
            VerifyContext = 0xF0000000,
            NewKeySet = 0x00000008,
            DeleteKeySet = 0x00000010,
            MachineKeySet = 0x00000020,
            Silent = 0x00000040
        }

        internal enum AlgorithmType
        {
            KeyExchange = 1,
            Signature = 2
        }

        internal enum KeyFlags
        {
            Exportable = 0x00000001,
            UserProtected = 0x00000002,
            CreateSalt = 0x00000004,
            UpdateKey = 0x00000008,
            NoSalt = 0x00000010,
            PreGenerate = 0x00000040,
            Online = 0x00000080,
            Sf = 0x00000100,
            CreateIv = 0x00000200,
            KeyExchangeKey = 0x00000400,
            DataKey = 0x00000800,
            Volatile = 0x00001000,
            SgcKey = 0x00002000,
            Archivable = 0x00004000
        }

        internal enum CertSystemStoreFlags
        {
            CurrentUserId = 1,
            LocalMachineId = 2,
            CurrentServiceId = 4,
            ServicesId = 5,
            UsersId = 6,
            CurrentUserGroupPolicyId = 7,
            LocalMachineGroupPolicyId = 8,
            LocalMachineEnterpriseId = 9,
            CurrentUser = CurrentUserId << 16,
            LocalMachine = LocalMachineId << 16
        }

        /// <summary>
        /// Constants for store providers
        /// From WinCrypt.h
        /// </summary>
        internal const int CERT_STORE_PROV_MSG = 1;
        internal const int CERT_STORE_PROV_MEMORY = 2;
        internal const int CERT_STORE_PROV_FILE = 3;
        internal const int CERT_STORE_PROV_REG = 4;
        internal const int CERT_STORE_PROV_PKCS7 = 5;
        internal const int CERT_STORE_PROV_SERIALIZED = 6;
        internal const int CERT_STORE_PROV_FILENAME = 8;
        internal const int CERT_STORE_PROV_SYSTEM = 10;
        internal const int CERT_STORE_PROV_COLLECTION = 11;
        internal const int CERT_STORE_PROV_SYSTEM_REGISTRY = 13;
        internal const int CERT_STORE_PROV_PHYSICAL = 14;
        internal const int CERT_STORE_PROV_SMART_CARD = 15;
        internal const int CERT_STORE_PROV_LDAP = 16;
        internal const int CERT_STORE_PROV_PKCS12 = 17;

#pragma warning restore

        [StructLayout(LayoutKind.Sequential)]
        internal struct SystemTime
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;

            public SystemTime(DateTime date)
            {
                wYear = (short)date.Year;
                wMonth = (short)date.Month;
                wDayOfWeek = (short)date.DayOfWeek;
                wDay = (short)date.Day;
                wHour = (short)date.Hour;
                wMinute = (short)date.Minute;
                wSecond = (short)date.Second;
                wMilliseconds = (short)date.Millisecond;
            }
        }

        #endregion
    }
}
