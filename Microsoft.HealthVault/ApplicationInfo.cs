// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{

    /// <meta name="MSHAttr" content="CommunityContent:1" />
    /// <summary>
    /// Defines the configuration for a HealthVault application.
    /// </summary>
    /// 
    public class ApplicationInfo
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
        public ApplicationInfo(string name, IList<Byte[]> publicKeys)
        {
            Validator.ThrowIfStringNullOrEmpty(name, "name");

            Validator.ThrowArgumentExceptionIf(
                publicKeys == null || publicKeys.Count == 0,
                "publicKeys",
                "ApplicationInfoPublicKeysRequired");

            CultureSpecificNames.DefaultValue = name;

            _publicKeys.Clear();
            foreach (Byte[] publicKey in publicKeys)
            {
                _publicKeys.Add(publicKey);
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
            ApplicationInfo appInfo = new ApplicationInfo();

            appInfo.Id = new Guid(app.SelectSingleNode("id").Value);
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
            if (!String.IsNullOrEmpty(actionUrl))
            {
                appInfo.ActionUrl = new Uri(actionUrl);
            }

            appInfo.CultureSpecificDescriptions.PopulateFromXml(app, "description");
            appInfo.CultureSpecificAuthorizationReasons.PopulateFromXml(app, "auth-reason");
            appInfo.DomainName = XPathHelper.GetOptNavValue(app, "domain-name");

            appInfo.LargeLogo = ApplicationBinaryConfiguration.CreateFromXml(app, "large-logo",
                    "logo", "content-type");
            appInfo.SmallLogo = ApplicationBinaryConfiguration.CreateFromXml(app, "small-logo",
                    "logo", "content-type");

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

            appInfo._onlineBaseAuthorizations = AuthorizationRule.CreateFromXml(
                    app.SelectSingleNode("person-online-base-auth-xml"));

            XPathNavigator personOfflineBaseAuthNav =
                    app.SelectSingleNode("person-offline-base-auth-xml");
            if (personOfflineBaseAuthNav != null)
            {
                appInfo._offlineBaseAuthorizations = AuthorizationRule.CreateFromXml(
                        personOfflineBaseAuthNav);
            }

            appInfo.PrivacyStatement = ApplicationBinaryConfiguration.CreateFromXml(app,
                    "privacy-statement", "statement", "content-type");
            appInfo.TermsOfUse = ApplicationBinaryConfiguration.CreateFromXml(app,
                    "terms-of-use", "statement", "content-type");
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

            appInfo._clientServiceToken =
                XPathHelper.GetOptNavValueAsGuid(app, "client-service-token");

            XPathNavigator vocabularyAuthorizationsNav =
                app.SelectSingleNode("vocabulary-authorizations");
            if (vocabularyAuthorizationsNav != null)
            {
                appInfo._vocabularyAuthorizations =
                    VocabularyAuthorization.CreateFromXml(vocabularyAuthorizationsNav);
            }

            XPathNavigator childVocabularyAuthCeilingNav =
                app.SelectSingleNode("child-vocabulary-authorizations-ceiling");
            if (childVocabularyAuthCeilingNav != null)
            {
                appInfo._childVocabularyAuthorizationsCeiling =
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
            if (instancesNode != null)
            {
                string supportAllInstancesString = instancesNode.GetAttribute("support-all-instances", String.Empty);
                if (!String.IsNullOrEmpty(supportAllInstancesString))
                {
                    appInfo.SupportAllHealthVaultInstances = XmlConvert.ToBoolean(supportAllInstancesString);
                }
            }

            foreach (XPathNavigator meaningfulUseSourceNav in app.Select("meaningful-use-sources/source"))
            {
                appInfo._meaningfulUseSources.Add(meaningfulUseSourceNav.Value);
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

        /// <summary>
        /// Updates the application's configuration in HealthVault.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to call HealthVault.
        /// </param>
        /// 
        /// <remarks>
        /// This method makes a remote call to the HealthVault service.
        /// The calling application in the <paramref name="connection"/> must be the same as
        /// the application specified by this ApplicationInfo instance or its master application.
        /// Note, this update will replace all configuration elements for the application. It is 
        /// advised that <see cref="ApplicationProvisioning.Provisioner.GetApplication"/> is 
        /// called to retrieve the existing application configuration before changing values and 
        /// calling Update.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If <see cref="Id"/> is <see cref="Guid.Empty"/>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        public void Update(ApplicationConnection connection)
        {
            HealthVaultPlatform.UpdateChildApplication(connection, this);
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

                    foreach (Byte[] publicKey in PublicKeys)
                    {
                        string hexString = BitConverter.ToString(publicKey);
                        hexString = hexString.Replace("-", "");

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
                    string methodString = String.Empty;
                    bool methodAdded = false;
                    foreach (HealthVaultMethods method in CallableMethods)
                    {
                        if (methodAdded)
                        {
                            methodString = methodString + "," + method.ToString();
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

                if (!String.IsNullOrEmpty(DomainName))
                {
                    writer.WriteElementString("domain-name", DomainName);
                }

                if (LargeLogo != null)
                {
                    LargeLogo.AppendRequestParameters(writer, "large-logo", "logo");
                }

                if (SmallLogo != null)
                {
                    SmallLogo.AppendRequestParameters(writer, "small-logo", "logo");
                }

                if ((ConfigurationOptions & ApplicationOptions.PersistentTokensAllowed) ==
                        ApplicationOptions.PersistentTokensAllowed &&
                    PersistentTokenTtlInSeconds != null)
                {

                    writer.WriteStartElement("persistent-tokens");

                    writer.WriteElementString("enabled", "true");
                    writer.WriteElementString("token-ttl-seconds",
                        PersistentTokenTtlInSeconds.Value.ToString());

                    writer.WriteEndElement();
                }

                if (PrivacyStatement != null)
                {
                    PrivacyStatement.AppendRequestParameters(writer, "privacy-statement", "statement");
                }

                if (TermsOfUse != null)
                {
                    TermsOfUse.AppendRequestParameters(writer, "terms-of-use", "statement");
                }

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

                if (DtcSuccessMessage != null)
                {
                    DtcSuccessMessage.AppendRequestParameters(
                        writer, "dtc-success-message", "statement");
                }

                if (ApplicationAttributes.Count != 0)
                {
                    writer.WriteStartElement("app-attributes");
                    for (int i = 0; i < ApplicationAttributes.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(ApplicationAttributes[i]))
                        {
                            writer.WriteElementString("app-attribute", ApplicationAttributes[i]);
                        }
                    }
                    writer.WriteEndElement();
                }

                if (!String.IsNullOrEmpty(ValidIPPrefixes))
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
        public Guid Id
        {
            get { return _appId; }
            set { _appId = value; }
        }
        private Guid _appId;

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
        public CultureSpecificStringDictionary CultureSpecificNames
        {
            get { return _cultureSpecificNames; }
        }
        private CultureSpecificStringDictionary _cultureSpecificNames = new CultureSpecificStringDictionary();

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
        public Collection<Byte[]> PublicKeys
        {
            get { return _publicKeys; }
        }
        private Collection<Byte[]> _publicKeys = new Collection<Byte[]>();

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
        public Collection<AuthorizationRule> OnlineBaseAuthorizations
        {
            get { return _onlineBaseAuthorizations; }
        }
        private Collection<AuthorizationRule> _onlineBaseAuthorizations =
            new Collection<AuthorizationRule>();

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
        public Collection<AuthorizationRule> OfflineBaseAuthorizations
        {
            get { return _offlineBaseAuthorizations; }
        }
        private Collection<AuthorizationRule> _offlineBaseAuthorizations =
            new Collection<AuthorizationRule>();

        /// <summary>
        /// Gets a collection of the HealthVault methods the application can call.
        /// </summary>
        /// 
        /// <remarks>
        /// On updating an application configuration the callable methods will only 
        /// be updated if the collection is not empty.
        /// </remarks>
        /// 
        public Collection<HealthVaultMethods> CallableMethods
        {
            get { return _callableMethods; }
        }
        private Collection<HealthVaultMethods> _callableMethods =
            new Collection<HealthVaultMethods>();

        /// <summary>
        /// Gets a collection of authorizations to HealthVault vocabularies, that the application
        ///  has access to.
        /// </summary>
        /// 
        public Collection<VocabularyAuthorization> VocabularyAuthorizations
        {
            get { return _vocabularyAuthorizations; }
        }
        private Collection<VocabularyAuthorization> _vocabularyAuthorizations =
            new Collection<VocabularyAuthorization>();

        /// <summary>
        /// Gets a collection of authorizations to HealthVault vocabularies. This represents the
        /// maximum authorization set that the application can grant to its child applications.
        /// </summary>
        /// 
        public Collection<VocabularyAuthorization> ChildVocabularyAuthorizationsCeiling
        {
            get { return _childVocabularyAuthorizationsCeiling; }
        }
        private Collection<VocabularyAuthorization> _childVocabularyAuthorizationsCeiling =
            new Collection<VocabularyAuthorization>();

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
        public Uri ActionUrl
        {
            get { return _actionUrl; }
            set { _actionUrl = value; }
        }
        private Uri _actionUrl;

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
        public CultureSpecificStringDictionary CultureSpecificDescriptions
        {
            get { return _cultureSpecificDescriptions; }
        }
        private CultureSpecificStringDictionary _cultureSpecificDescriptions =
            new CultureSpecificStringDictionary();

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
        public CultureSpecificStringDictionary CultureSpecificAuthorizationReasons
        {
            get { return _cultureSpecificAuthorizationReasons; }
        }
        private CultureSpecificStringDictionary _cultureSpecificAuthorizationReasons =
            new CultureSpecificStringDictionary();

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
        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }
        private string _domainName;

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
        public ApplicationBinaryConfiguration LargeLogo
        {
            get { return _largeLogo; }
            set { _largeLogo = value; }
        }
        private ApplicationBinaryConfiguration _largeLogo;

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
        public ApplicationBinaryConfiguration SmallLogo
        {
            get { return _smallLogo; }
            set { _smallLogo = value; }
        }
        private ApplicationBinaryConfiguration _smallLogo;

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
        public ApplicationBinaryConfiguration PrivacyStatement
        {
            get { return _privacyStatement; }
            set { _privacyStatement = value; }
        }
        private ApplicationBinaryConfiguration _privacyStatement;

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
        public ApplicationBinaryConfiguration TermsOfUse
        {
            get { return _termsOfUse; }
            set { _termsOfUse = value; }
        }
        private ApplicationBinaryConfiguration _termsOfUse;

        /// <summary>
        /// Gets or sets the application's Direct To Clinical success message.
        /// </summary>
        /// 
        /// <remarks>
        /// On updating an application configuration the dtc success message will only 
        /// be updated if the value is not null.
        /// </remarks>
        /// 
        public ApplicationBinaryConfiguration DtcSuccessMessage
        {
            get { return _dtcSuccessMessage; }
            set { _dtcSuccessMessage = value; }
        }
        private ApplicationBinaryConfiguration _dtcSuccessMessage;

        /// <summary>
        /// Gets or sets the application attributes.
        /// </summary>
        /// 
        /// <remarks>
        /// See <see cref="ExpectedApplicationAttributes"/> for a list of expected values.
        /// </remarks>
        /// 
        public Collection<string> ApplicationAttributes
        {
            get { return _applicationAttributes; }
        }
        private Collection<string> _applicationAttributes = new Collection<string>();

        /// <summary>
        /// The list of strings that are currently have meaning in
        /// <see cref="ApplicationAttributes"/>.
        /// </summary>
        /// <remarks>
        /// More expected values may be added at any time.
        /// </remarks>
        public static string[] ExpectedApplicationAttributes = { "hipaa", "clinicaltrial" };

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
        public ApplicationOptions? ConfigurationOptions
        {
            get { return _configurationOptions; }
            set { _configurationOptions = value; }
        }
        private ApplicationOptions? _configurationOptions;

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
                Validator.ThrowArgumentOutOfRangeIf(
                    value <= 0,
                    "PersistentTokenTtlInSeconds",
                    "PersistentTokenTtlInSecondsNotPositive");
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
        public string ValidIPPrefixes
        {
            get { return _validIPPrefixes; }
            set { _validIPPrefixes = value; }
        }
        private string _validIPPrefixes;

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
                    throw new ArgumentNullException("value");
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
        public Collection<string> SupportedHealthVaultInstances
        {
            get { return _supportedInstances; }
        }
        private Collection<string> _supportedInstances = new Collection<string>();

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
        public Collection<string> MeaningfulUseSources
        {
            get { return _meaningfulUseSources; }
        }
        private Collection<string> _meaningfulUseSources = new Collection<string>();

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
        public Guid? ClientServiceToken
        {
            get { return _clientServiceToken; }
        }
        private Guid? _clientServiceToken;
    }
}