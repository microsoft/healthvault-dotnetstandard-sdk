// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault.Application
{
    /// <meta name="MSHAttr" content="CommunityContent:1" />
    /// <summary>
    /// Defines the configuration for a HealthVault application.
    /// </summary>
    ///
    internal class ApplicationInfo
    {
        /// <summary>
        /// Constructs an <see cref="ApplicationInfo"/> instance with default values.
        /// </summary>
        ///
        public ApplicationInfo()
        {
        }

        /// <summary>
        /// Constructs an <see cref="ApplicationInfo"/> instance with the specified application
        /// name and public keys.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the application.
        /// </param>
        ///
        /// <param name="publicKeys">
        /// The public key(s) used to uniquely and positively identify the application to the
        /// HealthVault service. The application signs it's first request to HealthVault using it's
        /// application private which matches the public key being passed here. When HealthVault
        /// receives that request it validates the signature using the public key.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="publicKeys"/> is <b>null</b> or empty.
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public ApplicationInfo(string name, IList<byte[]> publicKeys)
        {
            Validator.ThrowIfStringNullOrEmpty(name, "name");

            if (publicKeys == null || publicKeys.Count == 0)
            {
                throw new ArgumentException(Resources.ApplicationInfoPublicKeysRequired, nameof(publicKeys));
            }

            CultureSpecificNames.DefaultValue = name;

            PublicKeys.Clear();

            foreach (byte[] publicKey in publicKeys)
            {
                PublicKeys.Add(publicKey);
            }
        }

        /// <summary>
        /// Creates an ApplicationInfo instance from the XML info section.
        /// </summary>
        ///
        /// <param name="app">
        /// The navigator.
        /// </param>
        ///
        /// <returns>
        /// A fully constructed ApplicationInfo object.
        /// </returns>
        ///
        internal static ApplicationInfo CreateFromInfoXml(XPathNavigator app)
        {
            ApplicationInfo appInfo = new ApplicationInfo { Id = new Guid(app.SelectSingleNode("id").Value) };

            appInfo.CultureSpecificNames.PopulateFromXml(app, "name");

            if (app.SelectSingleNode("restrict-app-users").ValueAsBoolean)
            {
                appInfo.SetApplicationOptions(ApplicationOptions.RestrictApplicationUsers);
            }

            if (app.SelectSingleNode("is-published").ValueAsBoolean)
            {
                appInfo.SetApplicationOptions(ApplicationOptions.PublishApplication);
            }

            string actionUrl = XPathHelper.GetOptNavValue(app, "action-url");
            if (!string.IsNullOrEmpty(actionUrl))
            {
                appInfo.ActionUrl = new Uri(actionUrl);
            }

            appInfo.CultureSpecificDescriptions.PopulateFromXml(app, "description");
            appInfo.CultureSpecificAuthorizationReasons.PopulateFromXml(app, "auth-reason");
            appInfo.DomainName = XPathHelper.GetOptNavValue(app, "domain-name");

            appInfo.LargeLogo = ApplicationBinaryConfiguration.CreateFromXml(
                app,
                "large-logo",
                "logo",
                "content-type");
            appInfo.SmallLogo = ApplicationBinaryConfiguration.CreateFromXml(
                app,
                "small-logo",
                "logo",
                "content-type");

            XPathNavigator persistentTokensNav = app.SelectSingleNode("persistent-tokens");
            if (persistentTokensNav != null)
            {
                bool persistentTokensEnabled = persistentTokensNav.SelectSingleNode("enabled")
                        .ValueAsBoolean;

                if (persistentTokensEnabled)
                {
                    appInfo.SetApplicationOptions(ApplicationOptions.PersistentTokensAllowed);

                    XPathNavigator tokensTtlSecondsNav = persistentTokensNav.SelectSingleNode(
                            "token-ttl-seconds");

                    if (tokensTtlSecondsNav != null)
                    {
                        appInfo.PersistentTokenTtlInSeconds = tokensTtlSecondsNav.ValueAsInt;
                    }
                }
            }

            appInfo.OnlineBaseAuthorizations = AuthorizationRule.CreateFromXml(
                    app.SelectSingleNode("person-online-base-auth-xml"));

            XPathNavigator personOfflineBaseAuthNav =
                    app.SelectSingleNode("person-offline-base-auth-xml");
            if (personOfflineBaseAuthNav != null)
            {
                appInfo.OfflineBaseAuthorizations = AuthorizationRule.CreateFromXml(
                        personOfflineBaseAuthNav);
            }

            appInfo.PrivacyStatement = ApplicationBinaryConfiguration.CreateFromXml(
                app,
                "privacy-statement",
                "statement",
                "content-type");
            appInfo.TermsOfUse = ApplicationBinaryConfiguration.CreateFromXml(
                app,
                "terms-of-use",
                "statement",
                "content-type");
            appInfo.DtcSuccessMessage = ApplicationBinaryConfiguration.CreateFromXml(
                app,
                "dtc-success-message",
                "statement",
                "content-type");

            XPathNavigator attributesNav = app.SelectSingleNode("app-attributes");
            if (attributesNav != null)
            {
                XPathNodeIterator attributesIterator = attributesNav.Select("app-attribute");
                foreach (XPathNavigator attributeNav in attributesIterator)
                {
                    appInfo.ApplicationAttributes.Add(attributeNav.Value);
                }
            }

            XPathNavigator ipPrefixesNav = app.SelectSingleNode("valid-ip-prefixes");
            if (ipPrefixesNav != null)
            {
                appInfo.ValidIPPrefixes = ipPrefixesNav.Value;
            }

            appInfo.ClientServiceToken =
                XPathHelper.GetOptNavValueAsGuid(app, "client-service-token");

            XPathNavigator vocabularyAuthorizationsNav =
                app.SelectSingleNode("vocabulary-authorizations");
            if (vocabularyAuthorizationsNav != null)
            {
                appInfo.VocabularyAuthorizations =
                    VocabularyAuthorization.CreateFromXml(vocabularyAuthorizationsNav);
            }

            XPathNavigator childVocabularyAuthCeilingNav =
                app.SelectSingleNode("child-vocabulary-authorizations-ceiling");
            if (childVocabularyAuthCeilingNav != null)
            {
                appInfo.ChildVocabularyAuthorizationsCeiling =
                    VocabularyAuthorization.CreateFromXml(vocabularyAuthorizationsNav);
            }

            XPathNavigator supportedRecordLocationsIterator =
                app.SelectSingleNode("supported-record-locations");
            if (supportedRecordLocationsIterator != null)
            {
                LocationCollection locationCollection = new LocationCollection();
                locationCollection.ParseXml(supportedRecordLocationsIterator);
                appInfo.SupportedRecordLocations = locationCollection;
            }

            foreach (XPathNavigator instanceIdNav in app.Select("supported-instances/instance-id"))
            {
                appInfo.SupportedHealthVaultInstances.Add(instanceIdNav.Value);
            }

            XPathNavigator instancesNode = app.SelectSingleNode("supported-instances");
            string supportAllInstancesString = instancesNode?.GetAttribute("support-all-instances", string.Empty);
            if (!string.IsNullOrEmpty(supportAllInstancesString))
            {
                appInfo.SupportAllHealthVaultInstances = XmlConvert.ToBoolean(supportAllInstancesString);
            }

            foreach (XPathNavigator meaningfulUseSourceNav in app.Select("meaningful-use-sources/source"))
            {
                appInfo.MeaningfulUseSources.Add(meaningfulUseSourceNav.Value);
            }

            return appInfo;
        }

        private void SetApplicationOptions(ApplicationOptions options)
        {
            if (ConfigurationOptions == null)
            {
                ConfigurationOptions = options;
            }
            else
            {
                ConfigurationOptions |= options;
            }
        }

        internal string GetRequestParameters(Guid appId)
        {
            StringBuilder result = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                if (appId != Guid.Empty)
                {
                    writer.WriteElementString("id", appId.ToString());
                }

                CultureSpecificNames.AppendLocalizedElements(writer, "name");

                if (PublicKeys.Count > 0)
                {
                    writer.WriteStartElement("public-keys");

                    foreach (byte[] publicKey in PublicKeys)
                    {
                        string hexString = BitConverter.ToString(publicKey);
                        hexString = hexString.Replace("-", string.Empty);

                        writer.WriteElementString("public-key", hexString);
                    }

                    writer.WriteEndElement();
                }

                if (OnlineBaseAuthorizations.Count > 0)
                {
                    writer.WriteStartElement("person-online-base-auth");
                    writer.WriteRaw(AuthorizationRule.GetRulesXml(OnlineBaseAuthorizations));
                    writer.WriteEndElement();
                }

                if (OfflineBaseAuthorizations.Count > 0)
                {
                    writer.WriteStartElement("person-offline-base-auth");
                    writer.WriteRaw(AuthorizationRule.GetRulesXml(OfflineBaseAuthorizations));
                    writer.WriteEndElement();
                }

                if (CallableMethods.Count > 0)
                {
                    string methodString = string.Empty;
                    bool methodAdded = false;
                    foreach (HealthVaultMethods method in CallableMethods)
                    {
                        if (methodAdded)
                        {
                            methodString = methodString + "," + method;
                        }
                        else
                        {
                            methodString = method.ToString();
                            methodAdded = true;
                        }
                    }

                    writer.WriteElementString("methods", methodString);
                }

                if (ActionUrl != null)
                {
                    writer.WriteElementString("action-url", ActionUrl.OriginalString);
                }

                CultureSpecificDescriptions.AppendLocalizedElements(
                    writer, "description");

                CultureSpecificAuthorizationReasons.AppendLocalizedElements(
                    writer, "auth-reason");

                if (!string.IsNullOrEmpty(DomainName))
                {
                    writer.WriteElementString("domain-name", DomainName);
                }

                LargeLogo?.AppendRequestParameters(writer, "large-logo", "logo");

                SmallLogo?.AppendRequestParameters(writer, "small-logo", "logo");

                if ((ConfigurationOptions & ApplicationOptions.PersistentTokensAllowed) ==
                        ApplicationOptions.PersistentTokensAllowed &&
                    PersistentTokenTtlInSeconds != null)
                {
                    writer.WriteStartElement("persistent-tokens");

                    writer.WriteElementString("enabled", "true");
                    writer.WriteElementString("token-ttl-seconds", PersistentTokenTtlInSeconds.Value.ToString());

                    writer.WriteEndElement();
                }

                PrivacyStatement?.AppendRequestParameters(writer, "privacy-statement", "statement");

                TermsOfUse?.AppendRequestParameters(writer, "terms-of-use", "statement");

                if ((ConfigurationOptions & ApplicationOptions.ApplicationAuthorizationRequired) ==
                        ApplicationOptions.ApplicationAuthorizationRequired)
                {
                    writer.WriteElementString("app-auth-required", "true");
                }

                if ((ConfigurationOptions & ApplicationOptions.RestrictApplicationUsers) == ApplicationOptions.RestrictApplicationUsers)
                {
                    writer.WriteElementString("restrict-app-users", "true");
                }

                if ((ConfigurationOptions & ApplicationOptions.PublishApplication) ==
                        ApplicationOptions.PublishApplication)
                {
                    writer.WriteElementString("is-published", "true");
                }

                DtcSuccessMessage?.AppendRequestParameters(
                    writer, "dtc-success-message", "statement");

                if (ApplicationAttributes.Count != 0)
                {
                    writer.WriteStartElement("app-attributes");
                    foreach (string attribute in ApplicationAttributes)
                    {
                        if (!string.IsNullOrEmpty(attribute))
                        {
                            writer.WriteElementString("app-attribute", attribute);
                        }
                    }

                    writer.WriteEndElement();
                }

                if (!string.IsNullOrEmpty(ValidIPPrefixes))
                {
                    writer.WriteElementString("valid-ip-prefixes", ValidIPPrefixes);
                }

                if (VocabularyAuthorizations.Count > 0)
                {
                    writer.WriteStartElement("vocabulary-authorizations");
                    foreach (VocabularyAuthorization auth in VocabularyAuthorizations)
                    {
                        auth.WriteXml(writer);
                    }

                    writer.WriteEndElement();
                }

                SupportedRecordLocations.WriteXml(writer, "supported-record-locations");

                if (SupportedHealthVaultInstances.Count > 0 || SupportAllHealthVaultInstances)
                {
                    writer.WriteStartElement("supported-instances");

                    if (SupportAllHealthVaultInstances)
                    {
                        writer.WriteAttributeString("support-all-instances", SDKHelper.XmlFromBool(SupportAllHealthVaultInstances));
                    }
                    else
                    {
                        foreach (string instanceId in SupportedHealthVaultInstances)
                        {
                            writer.WriteElementString("instance-id", instanceId);
                        }
                    }

                    writer.WriteEndElement();
                }

                if (MeaningfulUseSources.Count > 0)
                {
                    writer.WriteStartElement("meaningful-use-sources");
                    foreach (string source in MeaningfulUseSources)
                    {
                        writer.WriteElementString("source", source);
                    }

                    writer.WriteEndElement();
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        ///
        /// <remarks>
        /// On retrieving an application this value will be set to the application id.
        /// When creating or updating an application, this value is ignored.
        /// </remarks>
        ///
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        ///
        /// <remarks>
        /// On updating an application configuration the name will only be updated if it
        /// is not null.
        /// </remarks>
        ///
        public string Name
        {
            get
            {
                return CultureSpecificNames.BestValue;
            }

            set
            {
                CultureSpecificNames.DefaultValue = value;
            }
        }

        /// <summary>
        ///      Gets a dictionary of language specifiers and localized names of the application.
        /// </summary>
        public CultureSpecificStringDictionary CultureSpecificNames { get; } = new CultureSpecificStringDictionary();

        /// <summary>
        /// Gets a collection of the public keys for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The public key(s) are used to uniquely and positively identify the application to the
        /// HealthVault service. The application signs its first request to HealthVault using its
        /// application private which matches the public key being passed here. When HealthVault
        /// receives that request it validates the signature using the public key.
        ///
        /// On updating an application configuration the public key(s) will only be updated if the
        /// collection is not empty.
        /// </remarks>
        ///
        public Collection<byte[]> PublicKeys { get; } = new Collection<byte[]>();

        /// <summary>
        /// Gets a collection of the online base authorization rules for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The online base authorization rules represent the set of data that the application
        /// can access when a user is logged on.
        ///
        /// On updating an application configuration the online base authorization rules will only
        /// be updated if the collection is not empty.
        /// </remarks>
        ///
        public Collection<AuthorizationRule> OnlineBaseAuthorizations { get; private set; } = new Collection<AuthorizationRule>();

        /// <summary>
        /// Gets a collection of the offline base authorization rules for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The offline base authorization rules represent the set of data that the application
        /// can access when a user is not logged on.
        ///
        /// On updating an application configuration the offline base authorization rules will only
        /// be updated if the collection is not empty.
        /// </remarks>
        ///
        public Collection<AuthorizationRule> OfflineBaseAuthorizations { get; private set; } = new Collection<AuthorizationRule>();

        /// <summary>
        /// Gets a collection of the HealthVault methods the application can call.
        /// </summary>
        ///
        /// <remarks>
        /// On updating an application configuration the callable methods will only
        /// be updated if the collection is not empty.
        /// </remarks>
        ///
        public Collection<HealthVaultMethods> CallableMethods { get; } = new Collection<HealthVaultMethods>();

        /// <summary>
        /// Gets a collection of authorizations to HealthVault vocabularies, that the application
        ///  has access to.
        /// </summary>
        ///
        public Collection<VocabularyAuthorization> VocabularyAuthorizations { get; private set; } = new Collection<VocabularyAuthorization>();

        /// <summary>
        /// Gets a collection of authorizations to HealthVault vocabularies. This represents the
        /// maximum authorization set that the application can grant to its child applications.
        /// </summary>
        ///
        public Collection<VocabularyAuthorization> ChildVocabularyAuthorizationsCeiling { get; private set; } = new Collection<VocabularyAuthorization>();

        /// <summary>
        /// Gets or sets the action URL for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The action URL is the single point of re-entry to the application from the HealthVault
        /// Shell. The implementation of the action URL must use the query string parameters to
        /// determine what the correct page is to show the user. See
        /// Microsoft.Health.Web.HealthServiceActionPage for more information.
        ///
        /// On updating an application configuration the action URL will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public Uri ActionUrl { get; set; }

        /// <summary>
        /// Gets or sets a description of the application which is shown to the user when
        /// authorizing the application to their record.
        /// </summary>
        ///
        /// <remarks>
        /// On updating an application configuration the description will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public string Description
        {
            get
            {
                return CultureSpecificDescriptions.BestValue;
            }

            set
            {
                CultureSpecificDescriptions.DefaultValue = value;
            }
        }

        /// <summary>
        ///      Dictionary of language specifiers and localized descriptions of the application.
        /// </summary>
        public CultureSpecificStringDictionary CultureSpecificDescriptions { get; } = new CultureSpecificStringDictionary();

        /// <summary>
        /// Gets or sets the reason the application requires the base online and offline
        /// authorization rules it is requesting.
        /// </summary>
        ///
        /// <remarks>
        /// The authorization reason is shown to the user when they are authorizing the application
        /// to use their health record.
        ///
        /// On updating an application configuration the authorization reason will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public string AuthorizationReason
        {
            get
            {
                return CultureSpecificAuthorizationReasons.BestValue;
            }

            set
            {
                CultureSpecificAuthorizationReasons.DefaultValue = value;
            }
        }

        /// <summary>
        ///     Dictionary of language specifiers and localized authorization reasons of the
        ///     application.
        /// </summary>
        public CultureSpecificStringDictionary CultureSpecificAuthorizationReasons { get; } = new CultureSpecificStringDictionary();

        /// <summary>
        /// Gets or sets the domain name for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The domain name is used when the application calls
        /// ApplicationConnection.SendInsecureMessageFromApplication along with the
        /// specified mailbox as the sending party. For example, if the domain name of the application
        /// is "microsoft.com" and the mailbox is "example", then the user will get an email from
        /// "example@microsoft.com".
        ///
        /// On updating an application configuration the domain name will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the large logo for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The large logo is shown at various times to the user when interacting with the
        /// HealthVault Shell. The large logo must be 120x60 pixels or smaller, and is limited
        /// to 160kb in size.
        ///
        /// On updating an application configuration the large logo will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public ApplicationBinaryConfiguration LargeLogo { get; set; }

        /// <summary>
        /// Gets or sets the small logo for the application.
        /// </summary>
        ///
        /// <remarks>
        /// The small logo is shown at various times to the user when interacting with the
        /// HealthVault Shell. The small logo is limited to 40kb in size.
        ///
        /// On updating an application configuration the small logo will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public ApplicationBinaryConfiguration SmallLogo { get; set; }

        /// <summary>
        /// Gets or sets the application's privacy statement.
        /// </summary>
        ///
        /// <remarks>
        /// A link is provided from the HealthVault Shell to the application's privacy statement.
        ///
        /// On updating an application configuration the privacy statement will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public ApplicationBinaryConfiguration PrivacyStatement { get; set; }

        /// <summary>
        /// Gets or sets the application's terms of use.
        /// </summary>
        ///
        /// <remarks>
        /// A link is provided from the HealthVault Shell to the application's terms of use.
        ///
        /// On updating an application configuration the terms of use will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public ApplicationBinaryConfiguration TermsOfUse { get; set; }

        /// <summary>
        /// Gets or sets the application's Direct To Clinical success message.
        /// </summary>
        ///
        /// <remarks>
        /// On updating an application configuration the dtc success message will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public ApplicationBinaryConfiguration DtcSuccessMessage { get; set; }

        /// <summary>
        /// Gets or sets the application attributes.
        /// </summary>
        ///
        /// <remarks>
        /// See <see cref="ExpectedApplicationAttributes"/> for a list of expected values.
        /// </remarks>
        ///
        public Collection<string> ApplicationAttributes { get; } = new Collection<string>();

        /// <summary>
        /// The list of strings that are currently have meaning in
        /// <see cref="ApplicationAttributes"/>.
        /// </summary>
        /// <remarks>
        /// More expected values may be added at any time.
        /// </remarks>
        public static string[] ExpectedApplicationAttributes { get; } = { "hipaa", "clinicaltrial" };

        /// <summary>
        /// Gets or sets various configuration options that applications can use.
        /// </summary>
        ///
        /// <remarks>
        /// The configuration options tell HealthVault of any special behaviors that it should
        /// allow or enforce for this application. See <see cref="ApplicationOptions"/> for more
        /// information.
        ///
        /// On updating an application configuration the configuration options will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        public ApplicationOptions? ConfigurationOptions { get; set; }

        /// <summary>
        /// Gets or sets the length of time a user token will persist if they choose the "Keep me
        /// logged in on this computer" checkbox during HealthVault login.
        /// </summary>
        ///
        /// <remarks>
        /// This value is only used if <see cref="ApplicationOptions.PersistentTokensAllowed"/> is
        /// specified in the <see cref="ConfigurationOptions"/>.
        ///
        /// On updating an application configuration the persistent token ttl will only
        /// be updated if the value is not null.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public int? PersistentTokenTtlInSeconds
        {
            get { return _persistentTokenTtlInSeconds; }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PersistentTokenTtlInSeconds), Resources.PersistentTokenTtlInSecondsNotPositive);
                }

                _persistentTokenTtlInSeconds = value;
            }
        }

        private int? _persistentTokenTtlInSeconds;

        /// <summary>
        /// Gets or sets the IP address masks from which the application can
        /// call HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// A comma separated list of IP address masks from which the
        /// application can call HealthVault.
        /// HealthVault provides some added security to applications by supporting
        /// calls that come from valid IP addresses of the application. IP addresses
        /// can be specified using a specific IP address and 32 bit mask like
        /// 192.168.0.1/32 or by specifying a subnet and mask like 192.168.0.0/16.
        /// You can have more than one IP address or mask by comma separating them.
        /// </remarks>
        public string ValidIPPrefixes { get; set; }

        /// <summary>
        /// Gets a collection of the record locations supported by this application.
        /// </summary>
        public LocationCollection SupportedRecordLocations
        {
            get { return _supportedRecordLocations; }

            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _supportedRecordLocations = value;
            }
        }

        private LocationCollection _supportedRecordLocations = new LocationCollection();

        /// <summary>
        /// Gets the instance identifiers for the HealthVault instances the application supports.
        /// </summary>
        ///
        /// <remarks>
        /// Use the <see cref="ServiceInfo"/> class to get instance information using these
        /// instance identifiers.
        ///
        /// When updating a child application, if the supported instances is left blank, only the
        /// instance being connected to will be configured as the supported instances.
        /// </remarks>
        ///
        public Collection<string> SupportedHealthVaultInstances { get; } = new Collection<string>();

        /// <summary>
        /// Gets or sets whether the application supports all HealthVault instances.
        /// </summary>
        ///
        /// <remarks>
        /// If set to true, the value of <see cref="P:SupportedHealthVaultInstances" /> is ignored.
        /// </remarks>
        ///
        public bool SupportAllHealthVaultInstances { get; set; }

        /// <summary>
        /// Gets the list of Meaningful Use sources associated with this application.
        /// </summary>
        ///
        /// <remarks>
        /// Gets the list of Meaningful Use sources associated with this application.
        /// For partners that contribute CCDA documents to HealthVault using Direct Messaging and wish to retrieve Meaningful Use reports,
        /// HealthVault allows associating the Direct Messaging domain of the sender to an application ID that will retrieve the corresponding
        /// Meaningful Use reports. The MeaningfulUseSources field specifies these Direct Messaging domains as a list of semi-colon delimited strings.
        /// Developers can associate Direct Messaging domains with an application ID, using the HealthVault Application Configuration Center.
        /// </remarks>
        ///
        public Collection<string> MeaningfulUseSources { get; } = new Collection<string>();

        /// <summary>
        /// Gets the client service token.
        /// </summary>
        ///
        /// <remarks>
        /// The client service token is used by browser scripts to access HealthVault client services
        /// such as the vocabulary search service. The value of the client service token is set in
        /// the application configuration center.
        /// </remarks>
        ///
        public Guid? ClientServiceToken { get; private set; }
    }
}