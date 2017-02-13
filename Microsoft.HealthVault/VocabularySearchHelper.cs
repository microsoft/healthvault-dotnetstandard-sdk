// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents an API set to create URLs that will handle HTTP GET queries for searches on 
    /// a specified vocabulary.
    /// </summary>
    /// <remarks>
    /// The urls provide a means for browser based access to the vocabulary search in HealthVault.
    /// The response is returned in JSON format.
    /// </remarks>
    public static class VocabularySearchHelper
    {
        /// <summary>
        /// Creates a vocabulary search request URL in which the application is identified using 
        /// its service token.
        /// </summary>
        /// <remarks>
        /// 1. This is a fast access mechanism to publicly available vocabularies in HealthVault.
        /// 2. The service token is available at <see cref="ApplicationInfo.ClientServiceToken"/>
        /// 3. Searches to restricted vocabularies that an application has access to will not succeed
        ///    with this method. For those searches please use 
        ///    <see cref="VocabularySearchHelper.CreateVocabularySearchRequestJsonAuthenticatedUrl"/>
        /// </remarks>
        /// <param name="searchParameters">
        /// The set of parameters that identify the vocabulary to search, maximum search results,
        /// the culture, etc.
        /// </param>
        /// <param name="jsonCallbackName">
        /// This is the name of the callback function that will be called to parse the JSON response.
        /// </param>
        /// <param name="serviceToken">
        /// A Guid token that is used to identify the application that is going to make the search call.
        /// </param>
        /// <returns>
        /// A URL which can be used in the browser to make AJAX calls to HealthVault vocabulary search.
        /// </returns>
        /// 
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            Justification = "JSON is a valid data interchange standard.")]
        public static Uri CreateVocabularySearchRequestJsonUrl(
            VocabularySearchParameters searchParameters,
            string jsonCallbackName,
            Guid serviceToken)
        {
            Validator.ThrowIfArgumentNull(searchParameters, "searchParameters", "VocabularySearchParametersNull");
            Validator.ThrowIfStringNullOrEmpty(jsonCallbackName, "jsonCallbackName");

            Validator.ThrowArgumentExceptionIf(
                serviceToken == Guid.Empty,
                "serviceToken",
                "ServiceTokenEmpty");

            StringBuilder queryString = new StringBuilder(1024);
            AppendVocabularySearchServiceParameters(queryString, jsonCallbackName);
            AppendClientServiceToken(queryString, serviceToken);
            AppendVocabularySearchParameters(queryString, searchParameters);
            return
                new Uri(
                    HealthApplicationConfiguration.Current.HealthClientServiceUrl.OriginalString +
                    "?" +
                    queryString.ToString());
        }

        /// <summary>
        /// Creates a vocabulary search request url in which the application is identified using 
        /// the application certificate as the authentication parameter.
        /// </summary>
        /// <remarks>
        /// 1. This is an authenticated access mechanism for public and restricted vocabularies in 
        ///    HealthVault.
        /// 2. The call will require more processing on the HealthVault server and it is recommended, 
        ///    that this mechanism be used to mainly access restricted vocabularies in HealthVault that
        ///    require authenticated access.
        /// </remarks>
        /// <param name="searchParameters">
        /// The set of parameters that identify the vocabulary to search, maximum search results,
        /// the culture etc.
        /// </param>
        /// <param name="jsonCallbackName">
        /// This is the name of the callback function that will be called to parse the JSON response.
        /// </param>
        /// <param name="timeToLiveMinutes">
        /// The amount of time in minutes that the URL will be valid for. 
        /// </param>
        /// <returns>
        /// A URL which can be used in the browser to make AJAX calls to HealthVault vocabulary search.
        /// </returns>
        ///
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            Justification = "JSON is a valid data interchange standard.")]
        public static Uri CreateVocabularySearchRequestJsonAuthenticatedUrl(
            VocabularySearchParameters searchParameters,
            string jsonCallbackName,
            int timeToLiveMinutes)
        {
            Validator.ThrowIfArgumentNull(searchParameters, "searchParameters", "VocabularySearchParametersNull");
            Validator.ThrowIfStringNullOrEmpty(jsonCallbackName, "jsonCallbackName");

            StringBuilder queryString = new StringBuilder(1024);
            AppendVocabularySearchServiceParameters(queryString, jsonCallbackName);
            AppendAuthenticationParameters(queryString, timeToLiveMinutes);
            AppendVocabularySearchParameters(queryString, searchParameters);
            return
                new Uri(
                    HealthApplicationConfiguration.Current.HealthClientServiceUrl.OriginalString +
                    "?" +
                    queryString.ToString());
        }

        private static void AppendVocabularySearchServiceParameters(
            StringBuilder queryString,
            string jsonCallbackName)
        {
            queryString.Append("service=searchVocab&searchMode=fulltext&output=JSON");
            queryString.AppendFormat("&callback={0}", HttpUtility.UrlEncode(jsonCallbackName));
        }

        private static void AppendClientServiceToken(
            StringBuilder queryString, Guid serviceToken)
        {
            queryString.AppendFormat("&serviceToken={0}", serviceToken);
        }

        private static void AppendAuthenticationParameters(
            StringBuilder queryString, int timeToLiveMinutes)
        {
            queryString.AppendFormat(
                "&appid={0}",
                HealthApplicationConfiguration.Current.ApplicationId);

            X509Certificate2 certificate
                = HealthApplicationConfiguration.Current.ApplicationCertificate;
            queryString.AppendFormat(
                "&thumbprint={0}",
                HttpUtility.UrlEncode(certificate.Thumbprint));

            DateTime expirationTime = DateTime.UtcNow.AddMinutes(timeToLiveMinutes);
            String dateTimeStr =
                expirationTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFZ", CultureInfo.InvariantCulture);
            Byte[] raw = new Byte[Encoding.UTF8.GetByteCount(dateTimeStr) + 1];
            raw[0] = 1;
            Encoding.UTF8.GetBytes(dateTimeStr, 0, dateTimeStr.Length, raw, 1);

            queryString.AppendFormat(
                "&raw={0}",
                HttpUtility.UrlEncode(Convert.ToBase64String(raw)));

            RSACryptoServiceProvider rsaProvider =
                (RSACryptoServiceProvider)certificate.PrivateKey;
            Byte[] signature = rsaProvider.SignData(raw, "SHA1");

            queryString.AppendFormat(
                "&signature={0}",
                HttpUtility.UrlEncode(Convert.ToBase64String(signature)));
        }

        private static void AppendVocabularySearchParameters(
            StringBuilder queryString,
            VocabularySearchParameters searchParameters)
        {
            queryString.AppendFormat(
                "&vocabName={0}",
                HttpUtility.UrlEncode(searchParameters.Vocabulary.Name));

            if (!String.IsNullOrEmpty(searchParameters.Vocabulary.Family))
            {
                queryString.AppendFormat(
                    "&vocabFamily={0}",
                    HttpUtility.UrlEncode(searchParameters.Vocabulary.Family));
            }

            if (!String.IsNullOrEmpty(searchParameters.Vocabulary.Version))
            {
                queryString.AppendFormat(
                    "&vocabVersion={0}",
                    HttpUtility.UrlEncode(searchParameters.Vocabulary.Version));
            }

            if (searchParameters.MaxResults.HasValue)
            {
                queryString.AppendFormat("&maxResults={0}", searchParameters.MaxResults.Value);
            }

            if (searchParameters.Culture != null)
            {
                queryString.AppendFormat(
                    "&culture={0}", HttpUtility.UrlEncode(searchParameters.Culture.Name));
            }
        }
    }
}