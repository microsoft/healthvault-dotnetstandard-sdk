// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault.Web.Vocabulary
{
    /// <summary>
    /// Represents an API set to create URLs that will handle HTTP GET queries for searches on
    /// a specified vocabulary.
    /// </summary>
    /// <remarks>
    /// The urls provide a means for browser based access to the vocabulary search in HealthVault.
    /// The response is returned in JSON format.
    /// </remarks>
    internal static class VocabularySearchHelper
    {
        private static HealthVaultConfiguration configuration = Ioc.Get<HealthVaultConfiguration>();

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
                    configuration.GetHealthClientServiceUrl().OriginalString +
                    "?" +
                    queryString);
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
                    configuration.GetHealthClientServiceUrl().OriginalString +
                    "?" +
                    queryString);
        }

        private static void AppendVocabularySearchServiceParameters(
            StringBuilder queryString,
            string jsonCallbackName)
        {
            queryString.Append("service=searchVocab&searchMode=fulltext&output=JSON");
            queryString.AppendFormat("&callback={0}", WebUtility.UrlEncode(jsonCallbackName));
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
                configuration.MasterApplicationId);

            X509Certificate2 certificate
                = ApplicationCertificateStore.Current.ApplicationCertificate;
            queryString.AppendFormat(
                "&thumbprint={0}",
                WebUtility.UrlEncode(certificate.Thumbprint));

            DateTime expirationTime = DateTime.UtcNow.AddMinutes(timeToLiveMinutes);
            string dateTimeStr =
                expirationTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFZ", CultureInfo.InvariantCulture);
            byte[] raw = new byte[Encoding.UTF8.GetByteCount(dateTimeStr) + 1];
            raw[0] = 1;
            Encoding.UTF8.GetBytes(dateTimeStr, 0, dateTimeStr.Length, raw, 1);

            queryString.AppendFormat(
                "&raw={0}",
                WebUtility.UrlEncode(Convert.ToBase64String(raw)));

            RSACryptoServiceProvider rsaProvider =
                (RSACryptoServiceProvider)certificate.GetRSAPrivateKey();
            byte[] signature = rsaProvider.SignData(raw, "SHA1");

            queryString.AppendFormat(
                "&signature={0}",
                WebUtility.UrlEncode(Convert.ToBase64String(signature)));
        }

        private static void AppendVocabularySearchParameters(
            StringBuilder queryString,
            VocabularySearchParameters searchParameters)
        {
            queryString.AppendFormat(
                "&vocabName={0}",
                WebUtility.UrlEncode(searchParameters.Vocabulary.Name));

            if (!string.IsNullOrEmpty(searchParameters.Vocabulary.Family))
            {
                queryString.AppendFormat(
                    "&vocabFamily={0}",
                    WebUtility.UrlEncode(searchParameters.Vocabulary.Family));
            }

            if (!string.IsNullOrEmpty(searchParameters.Vocabulary.Version))
            {
                queryString.AppendFormat(
                    "&vocabVersion={0}",
                    WebUtility.UrlEncode(searchParameters.Vocabulary.Version));
            }

            if (searchParameters.MaxResults.HasValue)
            {
                queryString.AppendFormat("&maxResults={0}", searchParameters.MaxResults.Value);
            }

            if (searchParameters.Culture != null)
            {
                queryString.AppendFormat(
                    "&culture={0}", WebUtility.UrlEncode(searchParameters.Culture.Name));
            }
        }
    }
}