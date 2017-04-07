using System;

namespace Microsoft.HealthVault.Web.Constants
{
    internal static class HealthVaultWebConstants
    {
        internal static class ConfigKeys
        {
            internal const string ActionPagePrefix = "HV_Action";
            internal const string AllowedRedirectSites = "HV_AllowedRedirectSites";
            internal const string AppId = "HV_ApplicationId";
            internal const string ApplicationCertificateFileName = "HV_ApplicationCertificateFilename";
            internal const string ApplicationCertificatePassword = "HV_ApplicationCertificatePassword";
            internal const string CertSubject = "HV_AppCertSubject";
            internal const string CookieDomain = "HV_CookieDomain";
            internal const string CookieEncryptionKey = "HV_CookieEncryptionKey";
            internal const string CookiePath = "HV_CookiePath";
            internal const string CookieTimeoutMinutes = "HV_CookieTimeoutMinutes";
            internal const string DefaultInlineBlobHashBlockSize = "HV_DefaultInlineBlobHashBlockSize";
            internal const string DefaultRequestTimeoutSeconds = "HV_DefaultRequestTimeoutSeconds";
            internal const string DefaultRequestTimeToLiveSeconds = "HV_DefaultRequestTimeToLiveSeconds";
            internal const string HealthServiceUrl = "HV_HealthServiceUrl";
            internal const string IsMra = "HV_IsMRA";
            internal const string IsSignupCodeRequired = "HV_IsSignupCodeRequired";
            internal const string MultiInstanceAware = "HV_MultiInstanceAware";
            internal const string NonProductionActionUrlRedirectOverride = "HV_NonProductionActionUrlRedirectOverride";
            internal const string RequestRetryOnInternal500Count = "HV_RequestRetryOnInternal500Count";
            internal const string RequestRetryOnInternal500SleepSeconds = "HV_RequestRetryOnInternal500SleepSeconds";
            internal const string RestHealthServiceUrl = "HV_RestHealthServiceUrl";
            internal const string ShellUrl = "HV_ShellUrl";
            internal const string SupportedType = "HV_SupportedTypeVersions";
            internal const string UseAspSession = "HV_UseAspSession";
            internal const string UseLegacyTypeVersionSupport = "HV_UseLegacyTypeVersionSupport";
            internal const string UseSslForSecurity = "HV_SSLForSecure";
        }

        internal static class ConfigDefaults
        {
            internal const string CookieNameSuffix = "_HV";
            internal const bool IsMra = false;
            internal const string CookieDomain = "";
            internal const string CookiePath = "";
            internal static readonly TimeSpan CookieTimeoutDuration = TimeSpan.FromMinutes(20);
            internal const bool IsSignupCodeRequired = false;
            internal const bool UseAspSession = false;
            internal const bool UseSslForSecurity = true;
            internal const string ShellRedirectorLocation = "redirect.aspx?target=";
        }

        internal static class Urls
        {
            internal const string BlobStreamUrlSuffix = "/streaming/wildcatblob.ashx";
            internal const string HealthClientServiceSuffix = "hvclientservice.ashx";
            internal const string TypeSchemaSuffix = "type-xsd/";
        }

        internal static class ShellTargetQsReturnParameters
        {
            internal const string WcToken = "wctoken";
            internal const string InstanceId = "instanceid";
            internal const string SuggestedTokenTtl = "suggestedtokenttl";
        }
    }
}
