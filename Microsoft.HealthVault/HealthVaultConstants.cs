// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Constants used by the HealthVault SDK
    /// </summary>
    internal static class HealthVaultConstants
    {
        internal static class Cryptography
        {
            internal static readonly string HashAlgorithm = "SHA256";
            internal static readonly string HmacAlgorithm = "HMACSHA256";
            internal static readonly string SignatureAlgorithmName = "RSA-SHA1";
            internal static readonly string DigestAlgorithm = "SHA1";
        }

        internal static class Storage
        {
            internal static readonly string DirectoryName = "HealthVault-SDK";
        }

        internal static class SdkTelemetryInformationCategories
        {
            internal static readonly string AndroidClient = "HV-Xamarin-Android";
            internal static readonly string WindowsClient = "HV-Xamarin-Windows";
            internal static readonly string IosClient = "HV-Xamarin-Ios";
            internal static readonly string Web = "HV-Web";
            internal static readonly string IntegrationTest = "HV-Integration-Test";
        }

        internal static class ShellRedirectTargets
        {
            internal const string Auth = "AUTH";
            internal const string AppSignOut = "APPSIGNOUT";
            internal const string CreateApplication = "CREATEAPPLICATION";
        }

        internal static class ShellRedirectTargetQueryStrings
        {
            internal const string CredentialToken = "credtoken";
        }
    }
}
