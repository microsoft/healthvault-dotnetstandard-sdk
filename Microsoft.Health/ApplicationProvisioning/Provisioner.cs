// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.Health.Web;

namespace Microsoft.Health.ApplicationProvisioning
{
    /// <summary>
    /// An application provisioning application can use the method of this class to 
    /// provision and update child applications.
    /// </summary>
    /// 
    /// <remarks>
    /// HealthVault allows certain types of application to be used to instantiate, configure, and
    /// update applications that are related to it.  For example, one implementation of an application
    /// may be used and installed as many different application instances. To allow for these 
    /// separate applications to be instantiated HealthVault enables a "parent" application to
    /// tell HealthVault about a new installation of the application and provide certain configuration
    /// values for it.  That application will then be able access HealthVault with a unique 
    /// application identifier.
    /// </remarks>
    /// 
    public static class Provisioner
    {
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
        public static ApplicationInfo GetApplication(
            OfflineWebApplicationConnection connection,
            Guid childApplicationId)
        {
            return HealthVaultPlatform.GetChildApplication(connection, childApplicationId);
        }

        /// <summary>
        /// Adds a HealthVault application instance for a "child" application of the calling
        /// application.
        /// </summary>
        /// 
        /// <param name="connection">
        /// A HealthVault connection instantiated as the provisioning application.
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
        public static Guid AddApplication(
            OfflineWebApplicationConnection connection,
            ApplicationInfo applicationConfigurationInformation)
        {
            return HealthVaultPlatform.AddChildApplication(connection, applicationConfigurationInformation);
        }
    }
}
