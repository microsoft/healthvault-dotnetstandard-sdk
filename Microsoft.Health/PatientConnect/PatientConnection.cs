// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using Microsoft.Health.Web;

namespace Microsoft.Health.PatientConnect
{
    /// <summary>
    /// Methods for accessing the patient connection APIs of HealthVault.
    /// </summary>
    /// 
    /// <remarks>
    /// Some HealthVault applications maintain some of their own data storage but need a way to
    /// link their account/person identifier to a HealthVault identifier. The application can do
    /// this by calling <see cref="Create"/> and passing the application's 
    /// identifier and some information that is specific to the user. The user can then go to 
    /// HealthVault Shell and validate the connection with their appropriate health record. The
    /// application can then query for all validated connections (usually on a daily basis) by
    /// calling <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection)"/>
    /// or <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection, DateTime)"/> 
    /// which returns instances of <see cref="ValidatedPatientConnection"/>.
    /// <br/><br/>
    /// Validated connect requests are removed by HealthVault after 90 days. It is advised 
    /// that applications call <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection)"/>
    /// or <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection, DateTime)"/> 
    /// daily or weekly to ensure that all validated connect requests are retrieved.
    /// </remarks>
    /// 
    public static class PatientConnection
    {
        /// <summary>
        /// Asks HealthVault to create a pending patient connection for the application specified
        /// by the connection with the specified user specific parameters.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The application connection to HealthVault. The application ID in the connection is used
        /// when making the patient connection.
        /// </param>
        /// 
        /// <param name="friendlyName">
        /// A friendly name for the patient connection which will be shown to the user when they
        /// go to HealthVault Shell to validate the connection.
        /// </param>
        /// 
        /// <param name="securityQuestion">
        /// A question (usually provided by the patient) to which the patient must provide the 
        /// answer when they go to validate the connection in the HealthVault Shell.
        /// </param>
        /// 
        /// <param name="securityAnswer">
        /// The answer to the <paramref name="securityQuestion"/> which the patient must use
        /// when validating the connection in HealthVault Shell. The answer is case-insensitive but
        /// otherwise must match exactly. In most cases it is recommended that this is a single 
        /// word to prevent entry problems when validating the connection.
        /// </param>
        /// 
        /// <param name="callbackUrl">
        /// Not yet implemented. May be null.
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application specific identifier for the user. This identifier is used to uniquely
        /// identify the user in the application data storage whereas the HealthVault person ID is
        /// used to identify the person in HealthVault.
        /// </param>
        /// 
        /// <returns>
        /// A token that the application must give to the patient to use when validating the
        /// connection request.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="friendlyName"/>, <paramref name="securityQuestion"/>,
        /// <paramref name="securityAnswer"/>, or <paramref name="applicationPatientId"/> is
        /// <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static string Create(
            OfflineWebApplicationConnection connection,
            string friendlyName,
            string securityQuestion,
            string securityAnswer,
            Uri callbackUrl,
            string applicationPatientId)
        {
            return HealthVaultPlatform.CreatePatientConnection(
                connection,
                friendlyName,
                securityQuestion,
                securityAnswer,
                callbackUrl,
                applicationPatientId);
        }

        /// <summary>
        /// Deletes a request for a connection that has been made by the calling application but
        /// has not been validated by the user.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to HealthVault to use for this operation. 
        /// </param>
        /// 
        /// <param name="applicationPatientId">
        /// The application's identifier for the user which was used to create the connection 
        /// request.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationPatientId"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void DeletePending(
            OfflineWebApplicationConnection connection,
            string applicationPatientId)
        {
            HealthVaultPlatform.DeletePendingPatientConnection(
                connection,
                applicationPatientId);
        }

        /// <summary>
        /// Updates an existing pending patient connection with a new application patient identifier.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The HealthVault connection to use for the operation.
        /// </param>
        /// 
        /// <param name="oldApplicationPatientId">
        /// The application patient identifier that was used to make the initial connection request.
        /// </param>
        /// 
        /// <param name="newApplicationPatientId">
        /// The new application patient identifier.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="oldApplicationPatientId"/> or <paramref name="newApplicationPatientId"/>
        /// is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// If an error occurs when contacting HealthVault.
        /// </exception>
        /// 
        public static void UpdateApplicationPatientId(
            OfflineWebApplicationConnection connection,
            string oldApplicationPatientId,
            string newApplicationPatientId)
        {
            HealthVaultPlatform.UpdatePatientConnectionApplicationPatientId(
                connection,
                oldApplicationPatientId,
                newApplicationPatientId);
        }

        /// <summary>
        /// Gets the connections for the application that people have accepted.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The application's connection to HealthVault.
        /// </param>
        /// 
        /// <returns>
        /// A collection of the connections that people have accepted.
        /// </returns>
        /// 
        /// <remarks>
        /// Validated connect requests are removed by HealthVault after 90 days. It is advised 
        /// that applications call <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection)"/>
        /// or <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection, DateTime)"/> 
        /// daily or weekly to ensure that all validated connect requests are retrieved.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        public static Collection<ValidatedPatientConnection> GetValidatedConnections(
            OfflineWebApplicationConnection connection)
        {
            return GetValidatedConnections(connection, DateTime.MinValue);
        }

        /// <summary>
        /// Gets the connections for the application that people have accepted since the specified
        /// date.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The application's connection to HealthVault.
        /// </param>
        /// 
        /// <param name="validatedSince">
        /// Connections that have been validated since this date will be returned. The date passed
        /// should be in UTC time.
        /// </param>
        /// 
        /// <returns>
        /// A collection of the connections that people have accepted.
        /// </returns>
        /// 
        /// <remarks>
        /// Validated connect requests are removed by HealthVault after 90 days. It is advised 
        /// that applications call <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection)"/>
        /// or <see cref="GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection, DateTime)"/> 
        /// daily or weekly to ensure that all validated connect requests are retrieved.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        public static Collection<ValidatedPatientConnection> GetValidatedConnections(
            OfflineWebApplicationConnection connection,
            DateTime validatedSince)
        {
            return HealthVaultPlatform.GetValidatedPatientConnections(
                connection,
                validatedSince);
        }
    }
}
