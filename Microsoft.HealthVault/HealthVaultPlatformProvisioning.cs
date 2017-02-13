// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml.XPath;
using Microsoft.HealthVault.Web;

namespace Microsoft.HealthVault.PlatformPrimitives
{

    /// <summary>
    /// Provides low-level access to the HealthVault provisioning operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set 
    /// HealthVaultPlatformProvisioning.Current to a derived class to intercept all provisioning calls.
    /// </remarks>

    public class HealthVaultPlatformProvisioning
    {
        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        /// 
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked. 
        /// </remarks>
        /// 
        /// <param name="mock">The mocking class.</param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        /// 
        public static void EnableMock(HealthVaultPlatformProvisioning mock)
        {
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        /// 
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        /// 
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }
        internal static HealthVaultPlatformProvisioning Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformProvisioning _current = new HealthVaultPlatformProvisioning();
        private static HealthVaultPlatformProvisioning _saved;

        /// <summary>
        /// Updates the application's configuration in HealthVault.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to make the update.
        /// </param>
        /// 
        /// <param name="applicationInfo">
        /// The <see cref="ApplicationInfo"/> to update.
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
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        public virtual void UpdateChildApplication(
            ApplicationConnection connection,
            ApplicationInfo applicationInfo)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ProvisionerNullConnection");
            Validator.ThrowInvalidIf(applicationInfo.Id == Guid.Empty, "ProvisionerEmptyAppId");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "UpdateApplication", 2);

            request.Parameters = applicationInfo.GetRequestParameters(applicationInfo.Id);
            request.Execute();
        }

        /// <summary>
        /// Gets the configuration information for the specified child application ID.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to get the application information.
        /// </param>
        /// 
        /// <param name="childApplicationId">
        /// The unique application identifier for the child application to get the configuration
        /// information for.
        /// </param>
        /// 
        /// <returns>
        /// Configuration information for the specified child application.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="childApplicationId"/> is <see cref="Guid.Empty"/>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceAccessDeniedException">
        /// If the application specified in the <paramref name="connection"/> is not a master
        /// application, or if <paramref name="childApplicationId"/> does not identify a child
        /// application of the calling application.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If there is an error when the HealthVault service is called.
        /// </exception>
        /// 
        public virtual ApplicationInfo GetChildApplication(
            OfflineWebApplicationConnection connection,
            Guid childApplicationId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ProvisionerNullConnection");

            Validator.ThrowArgumentExceptionIf(
                childApplicationId == Guid.Empty,
                "childApplicationId",
                "ProvisionerEmptyAppId");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetApplicationInfo", 2);

            request.Parameters =
                String.Format(
                    CultureInfo.InvariantCulture,
                    "<all-languages>true</all-languages><child-app-id>{0}</child-app-id>",
                    childApplicationId);

            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "GetApplicationInfo");

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(infoPath);

            XPathNavigator appInfoNav = infoNav.SelectSingleNode("application");

            ApplicationInfo appInfo = null;
            if (appInfoNav != null)
            {
                appInfo = ApplicationInfo.CreateFromInfoXml(appInfoNav);
            }

            return appInfo;
        }

        /// <summary>
        /// Adds a HealthVault application instance for a "child" application of the calling
        /// application.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to use to add the application.
        /// </param>
        /// 
        /// <param name="applicationConfigurationInformation">
        /// Configuration information about the application being provisioned.
        /// </param>
        /// 
        /// <returns>
        /// The new application identifier for the new application provided by HealthVault.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> or <paramref name="applicationConfigurationInformation"/>
        /// is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <see cref="ApplicationInfo.Name"/>, <see cref="ApplicationInfo.PublicKeys"/>,
        /// <see cref="ApplicationInfo.OfflineBaseAuthorizations"/>, <see cref="ApplicationInfo.Description"/>,
        /// <see cref="ApplicationInfo.AuthorizationReason"/>, or <see cref="ApplicationInfo.LargeLogo"/> 
        /// is not specified.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If there is an error when the HealthVault service is called.
        /// </exception>
        /// 
        public virtual Guid AddChildApplication(
            OfflineWebApplicationConnection connection,
            ApplicationInfo applicationConfigurationInformation)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ProvisionerNullConnection");
            Validator.ThrowIfArgumentNull(
                applicationConfigurationInformation,
                "applicationConfigurationInformation",
                "ProvisionerNullApplicationInfo");

            Validator.ThrowIfStringNullOrEmpty(applicationConfigurationInformation.Name, "applicationConfigurationInformation");

            Validator.ThrowArgumentExceptionIf(
                applicationConfigurationInformation.PublicKeys.Count < 1,
                    "applicationConfigurationInformation",
                    "AddApplicationPublicKeysMandatory");

            Validator.ThrowArgumentExceptionIf(
                applicationConfigurationInformation.OfflineBaseAuthorizations.Count < 1,
                "applicationConfigurationInformation",
                "AddApplicationOfflineBaseAuthorizationsMandatory");

            Validator.ThrowIfStringNullOrEmpty(
                applicationConfigurationInformation.Description,
                "applicationConfigurationInformation.Description");

            Validator.ThrowIfStringNullOrEmpty(
                applicationConfigurationInformation.AuthorizationReason,
                "applicationConfigurationInformation.AuthorizationReason");

            Validator.ThrowIfArgumentNull(
                applicationConfigurationInformation.LargeLogo,
                "applicationConfigurationInformation.LargeLogo",
                "AddApplicationLargeLogoMandatory");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "AddApplication", 2);

            request.Parameters = applicationConfigurationInformation.GetRequestParameters(Guid.Empty);

            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "AddApplication");

            XPathNavigator infoNav = request.Response.InfoNavigator.SelectSingleNode(infoPath);
            return new Guid(infoNav.SelectSingleNode("id").Value);
        }
    }
}

