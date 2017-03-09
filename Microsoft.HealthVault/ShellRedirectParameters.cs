// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Parameters for constructing a HealthVault Shell redirect URL.
    /// </summary>
    ///
    /// <remarks>
    /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a> for
    /// complete listing of all parameters supported by Shell.
    /// </remarks>
    public class ShellRedirectParameters
    {
        private const string ShellRedirectPage = "/redirect.aspx";
        private IConfiguration configuration = Ioc.Get<IConfiguration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellRedirectParameters"/> class that is empty.
        /// </summary>
        public ShellRedirectParameters()
        {
            this.ActionParameters = new NameValueCollection();
            this.TargetParameters = new NameValueCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellRedirectParameters"/> class with
        /// the specified Shell redirector URL.
        /// </summary>
        ///
        public ShellRedirectParameters(string shellRedirectorUrl)
            : this()
        {
            this.ShellRedirectorUrl = shellRedirectorUrl;
        }

        /// <summary>
        /// Gets or sets the base Shell redirector URL.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// For example, "https://account.healthvault.com/redirect.aspx".
        /// </p>
        ///
        /// <p>
        /// If the specified URL does not end with "/redirect.aspx", when
        /// <see cref="ConstructRedirectUrl"/> is called, "/redirect.aspx"
        /// is appended to the URL used for redirection.
        /// </p>
        ///
        /// <p>
        /// If not specified, the value of the <see cref="ConfigurationBase.HealthVaultShellUrl"/> configuration is
        /// used for constructing the Shell redirect URL.
        /// </p>
        /// </remarks>
        public string ShellRedirectorUrl { get; set; }

        /// <summary>
        /// Gets or sets the parameter that specifies whether
        /// the application simultaneously deals with multiple records
        /// for the same person.
        /// </summary>
        ///
        /// <remarks>
        /// If set to true, this is encoded as the value of the <b>ismra</b> parameter in the
        /// <b>targetqs</b> query string parameter.
        ///
        /// <p>
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </p>
        /// </remarks>
        ///
        public bool? IsMultiRecordApplication { get; set; }

        /// <summary>
        /// Gets or sets the signup code parameter for creating a HealthVault account.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// This is required for applications in locations with limited access to HealthVault.
        /// Signup codes may be obtained from
        /// <see cref="ApplicationConnection.NewSignupCode" />
        /// </p>
        ///
        /// <p>
        /// If specified, this is encoded as the value of the <b>signupcode</b> parameter in the
        /// <b>targetqs</b> query string parameter.
        /// </p>
        ///
        /// <p>
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </p>
        /// </remarks>
        ///
        public string SignupCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location to redirect to in HealthVault Shell.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a> for
        /// complete listing of target locations and their parameters.
        /// </p>
        ///
        /// <p>
        /// This is encoded as the value of the <b>target</b> query string parameter in the
        /// redirect URL.
        /// </p>
        /// </remarks>
        ///
        public string TargetLocation { get; set; }

        /// <summary>
        /// Gets the collection of target-specific parameters to pass to the target location.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a> for
        /// complete listing of target locations and their parameters.
        /// </p>
        ///
        /// <p>
        /// This is encoded as a key-value query string (application/x-www-form-urlencoded) along with <see cref="P:TargetQueryString"/>
        /// as the value of the <b>targetqs</b> query string parameter in the redirect URL.
        /// </p>
        /// </remarks>
        ///
        public NameValueCollection TargetParameters { get; private set; }

        /// <summary>
        /// Gets or sets the target-specific query string that is passed to the target location.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// When specified, this value is used along with the <see cref="P:TargetParameters"/> name-value collection as the value
        /// of the <b>targetqs</b> query string parameter.
        /// </p>
        ///
        /// <p>
        /// See <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface</a> for
        /// complete listing of target locations and their parameters.
        /// </p>
        /// </remarks>
        ///
        public string TargetQueryString { get; set; }

        /// <summary>
        /// Gets or sets the URL that the non-production HealthVault Shell will redirect
        /// back to after the user interaction for the requested target
        /// location is complete.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// This parameter is intended to make development in non-production HealthVault
        /// environments easier.  In the production HealthVault environment, redirects
        /// will return to HealthVault's action URL configuration for the application ID.
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </p>
        ///
        /// <p>
        /// If specified or configured, this is encoded as the value of the <b>redirect</b> parameter in the
        /// <b>targetqs</b> query string parameter.
        /// </p>
        /// </remarks>
        ///
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets the collection of user-defined parameters which are included in the return redirect
        /// when HealthVault Shell redirects back to the application.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// These parameters allow the application to transfer its state through the
        /// HealthVault Shell redirect.
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </p>
        ///
        /// <p>
        /// This is encoded as a key-value query string (application/x-www-form-urlencoded) along with <see cref="P:ActionQueryString"/>
        /// as the value of the <b>actionqs</b> parameter in the <b>targetqs</b> query string parameter.
        /// </p>
        /// </remarks>
        ///
        public NameValueCollection ActionParameters { get; private set; }

        /// <summary>
        /// Gets or sets the query string which is included in the return redirect when HealthVault Shell
        /// redirects back to the application.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// This query string allows the application to transfer its state through the
        /// HealthVault Shell redirect.
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </p>
        ///
        /// <p>
        /// When specified, this value is used along with the <see cref="P:ActionParameters"/> name-value collection as the value
        /// of the <b>actionqs</b> parameter in the <b>targetqs</b> query string parameter.
        /// </p>
        /// </remarks>
        ///
        public string ActionQueryString { get; set; }

        /// <summary>
        /// Gets or sets the application identifier parameter.
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the parameter that specifies whether the target instance of HealthVault Shell can redirect the
        /// user to another instance of HealthVault Shell where the user's account might
        /// reside.
        /// </summary>
        ///
        /// <remarks>
        /// <p>
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </p>
        /// <p>
        /// If set to true, this is encoded as the value of the <b>aib</b> parameter in the
        /// <b>targetqs</b> query string parameter.
        /// </p>
        /// </remarks>
        public bool? AllowInstanceBounce { get; set; }

        /// <summary>
        /// Gets or sets the parameter that specifies the token redirection method.
        /// </summary>
        ///
        /// <remarks>
        /// This is the HTTP method HealthVault Shell will use when redirecting user back
        /// to the application with the auth token.
        /// See the <a href="http://msdn.microsoft.com/en-us/library/ff803620.aspx">Shell redirect interface documentation</a>.
        /// </remarks>
        ///
        public string TokenRedirectionMethod { get; set; }

        /// <summary>
        /// Makes a copy of the redirect parameters object.
        /// </summary>
        ///
        /// <returns>
        /// A copy of the redirect parameters object.
        /// </returns>
        public ShellRedirectParameters Clone()
        {
            var result = new ShellRedirectParameters
            {
                ActionParameters = CloneParams(this.ActionParameters),
                ActionQueryString = this.ActionQueryString,
                AllowInstanceBounce = this.AllowInstanceBounce,
                ApplicationId = this.ApplicationId,
                IsMultiRecordApplication = this.IsMultiRecordApplication,
                ReturnUrl = this.ReturnUrl,
                ShellRedirectorUrl = this.ShellRedirectorUrl,
                SignupCode = this.SignupCode,
                TargetLocation = this.TargetLocation,
                TargetParameters = CloneParams(this.TargetParameters),
                TargetQueryString = this.TargetQueryString,
                TokenRedirectionMethod = this.TokenRedirectionMethod
            };

            return result;
        }

        /// <summary>
        /// Copies a NameValueCollection object.
        /// </summary>
        ///
        private static NameValueCollection CloneParams(NameValueCollection source)
        {
            var result = new NameValueCollection();
            for (int i = 0; i < source.Count; i++)
            {
                result.Add(source.GetKey(i), source[i]);
            }

            return result;
        }

        /// <summary>
        /// Constructs a Shell redirect URL for the redirect parameters.
        /// </summary>
        ///
        /// <returns>
        /// The constructed URL.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// No <see cref="TargetLocation"/> is specified.
        /// </exception>
        ///
        /// <exception cref="InvalidConfigurationException">
        /// No <see cref="ShellRedirectorUrl"/> specified or
        /// Shell URL configured for the application
        /// (<see cref="ConfigurationBase.HealthVaultShellUrl"/>).
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// The specified parameters construct an invalid URL.
        /// </exception>
        ///
        public Uri ConstructRedirectUrl()
        {
            // figure out Shell redirector URL
            string shellRedirectorUrl = this.ShellRedirectorUrl;
            if (string.IsNullOrEmpty(shellRedirectorUrl))
            {
                if (this.configuration.HealthVaultShellUrl == null)
                {
                    throw Validator.InvalidConfigurationException("ShellUrlRequired");
                }

                // get from config
                shellRedirectorUrl = this.configuration.HealthVaultShellUrl.OriginalString;
            }

            if (!shellRedirectorUrl.EndsWith(ShellRedirectPage, StringComparison.OrdinalIgnoreCase))
            {
                if (shellRedirectorUrl.EndsWith("/", StringComparison.Ordinal))
                {
                    // no need to prefix with slash
                    shellRedirectorUrl += ShellRedirectPage.Substring(1);
                }
                else
                {
                    shellRedirectorUrl += ShellRedirectPage;
                }
            }

            return new Uri(shellRedirectorUrl + "?" + this.ConstructRedirectorQueryString());
        }

        /// <summary>
        /// Construct the query string for the Shell redirect URL.
        /// </summary>
        ///
        /// <returns>
        /// The Shell redirect query string.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// <see cref="TargetLocation"/> is not set.
        /// </exception>
        ///
        public string ConstructRedirectorQueryString()
        {
            Validator.ThrowInvalidIf(string.IsNullOrEmpty(this.TargetLocation), "ShellTargetRequired");

            var qs = new NameValueCollection();
            var targetQs = this.ConstructTargetQueryString();

            qs.Add("target", this.TargetLocation);
            if (!string.IsNullOrEmpty(targetQs))
            {
                qs.Add("targetqs", "?" + targetQs);
            }

            return CollectionToQueryString(qs);
        }

        /// <summary>
        /// Constructs the query string for the target.
        /// </summary>
        ///
        /// <returns>
        /// The Shell redirect query string.
        /// </returns>
        ///
        public string ConstructTargetQueryString()
        {
            // fold properties back into target params
            return CombineQueryStrings(this.TargetQueryString, this.FlattenTargetParameters());
        }

        /// <summary>
        /// Combines two query strings.
        /// </summary>
        private static string CombineQueryStrings(string query1, NameValueCollection query2)
        {
            // remove leading '?'
            if (query1 != null && query1.StartsWith("?", StringComparison.Ordinal))
            {
                query1 = query1.Substring(1);
            }

            // if second query string has nothing, just return query1
            if (query2.Count == 0)
            {
                return query1 ?? string.Empty;
            }

            string query2Str = CollectionToQueryString(query2);

            // if first query string has nothing, just return query2
            if (string.IsNullOrEmpty(query1))
            {
                return query2Str;
            }

            // otherwise combine
            return string.Join("&", query1, query2Str);
        }

        /// <summary>
        /// Folds targetqs-based property values into TargetParameters.
        /// </summary>
        private NameValueCollection FlattenTargetParameters()
        {
            // combine action query string and action parameters
            var actionQueryString = CombineQueryStrings(this.ActionQueryString, this.ActionParameters);

            NameValueCollection targetParameters = CloneParams(this.TargetParameters);

            // app id
            if (this.ApplicationId != null)
            {
                targetParameters["appid"] = this.ApplicationId.Value.ToString();
            }

            // return URL
            if (!string.IsNullOrEmpty(this.ReturnUrl))
            {
                targetParameters["redirect"] = this.ReturnUrl;
            }

            // signup code
            if (!string.IsNullOrEmpty(this.SignupCode))
            {
                targetParameters["signupcode"] = this.SignupCode;
            }

            // action query string
            if (!string.IsNullOrEmpty(this.ActionQueryString))
            {
                targetParameters["actionqs"] = actionQueryString;
            }

            // ismra
            if (this.IsMultiRecordApplication != null && this.IsMultiRecordApplication.Value)
            {
                targetParameters["ismra"] = "true";
            }

            // aib
            if (this.AllowInstanceBounce != null && this.AllowInstanceBounce.Value)
            {
                targetParameters["aib"] = "true";
            }

            // trm
            if (!string.IsNullOrEmpty(this.TokenRedirectionMethod))
            {
                targetParameters["trm"] = this.TokenRedirectionMethod;
            }

            return targetParameters;
        }

        /// <summary>
        /// Serializes a name-value collection into a key/value query string
        /// </summary>
        internal static string CollectionToQueryString(NameValueCollection parameters)
        {
            StringBuilder qs = new StringBuilder(parameters.Count * 32);

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0)
                {
                    qs.Append("&");
                }

                string key = parameters.GetKey(i);
                string value = parameters[i];

                qs.Append(Uri.EscapeDataString(key));
                qs.Append("=");
                qs.Append(Uri.EscapeDataString(value));
            }

            return qs.ToString();
        }
    }
}