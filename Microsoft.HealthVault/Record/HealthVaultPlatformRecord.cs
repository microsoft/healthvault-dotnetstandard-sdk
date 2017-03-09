// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Things;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Provides low-level access to the HealthVault record operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformRecord.Current to a derived class to intercept all message calls.
    /// </remarks>
    internal class HealthVaultPlatformRecord
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
        public static void EnableMock(HealthVaultPlatformRecord mock)
        {
            Validator.ThrowInvalidIf(saved != null, "ClassAlreadyMocked");

            saved = Current;
            Current = mock;
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
            Validator.ThrowInvalidIfNull(saved, "ClassIsntMocked");

            Current = saved;
            saved = null;
        }

        internal static HealthVaultPlatformRecord Current { get; private set; } = new HealthVaultPlatformRecord();

        private static HealthVaultPlatformRecord saved;

        /// <summary>
        /// Releases the authorization of the application on the health record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <exception cref="HealthServiceException">
        /// Errors during the authorization release.
        /// </exception>
        ///
        /// <remarks>
        /// Once the application releases the authorization to the health record,
        /// calling any methods of this <see cref="HealthRecordAccessor"/> will result
        /// in a <see cref="HealthServiceAccessDeniedException"/>."
        /// </remarks>
        public virtual async Task RemoveApplicationAuthorizationAsync(
            IConnection connection,
            HealthRecordAccessor accessor)
        {
            // TODO: IConnection-ify this.
            // HealthServiceRequest request = new HealthServiceRequest(connection, "RemoveApplicationRecordAuthorization", 1, accessor);

            // await request.ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this  record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of unique identifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// Returns a dictionary of <see cref="HealthRecordItemTypePermission"/>
        /// with health record item types as the keys.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty dictionary is
        /// returned. If for a health record item type, the person has
        /// neither online access nor offline access permissions,
        /// <b> null </b> will be returned for that type in the dictionary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an exception during executing the request to HealthVault.
        /// </exception>
        ///
        public virtual async Task<IDictionary<Guid, HealthRecordItemTypePermission>> QueryPermissionsByTypesAsync(
            IConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            // TODO: IConnection-ify this.
            /*
            HealthRecordPermissions recordPermissions = await this.QueryRecordPermissionsAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
            Collection<HealthRecordItemTypePermission> typePermissions = recordPermissions.ItemTypePermissions;

            Dictionary<Guid, HealthRecordItemTypePermission> permissions = new Dictionary<Guid, HealthRecordItemTypePermission>();

            foreach (Guid typeId in healthRecordItemTypeIds)
            {
                if (!permissions.ContainsKey(typeId))
                {
                    permissions.Add(typeId, null);
                }
            }

            foreach (HealthRecordItemTypePermission typePermission in typePermissions)
            {
                permissions[typePermission.TypeId] = typePermission;
            }

            return permissions;
            */

            return null;
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A list of <see cref="HealthRecordItemTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty list is
        /// returned. If for a health record item type, the person has
        /// neither online access nor offline access permissions,
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an exception during executing the request to HealthVault.
        /// </exception>
        ///
        public virtual async Task<Collection<HealthRecordItemTypePermission>> QueryPermissionsAsync(
            IConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            HealthRecordPermissions permissions = await this.QueryRecordPermissionsAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
            return permissions.ItemTypePermissions;
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record as well as the other permission settings such as MeaningfulUseOptIn.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthRecordPermissions"/> object
        /// which contains a collection of <see cref="HealthRecordItemTypePermission"/> objects and
        /// other permission settings.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty list is
        /// returned for <see cref="HealthRecordPermissions"/> object's ItemTypePermissions property.
        /// If for a health record item type, the person has
        /// neither online access nor offline access permissions,
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// </exception>
        ///
        public virtual async Task<HealthRecordPermissions> QueryRecordPermissionsAsync(
            IConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            Validator.ThrowIfArgumentNull(healthRecordItemTypeIds, "healthRecordItemTypeIds", "CtorhealthRecordItemTypeIdsArgumentNull");

            // TODO: IConnection-ify this.
            /*
            HealthServiceRequest request =
            new HealthServiceRequest(connection, "QueryPermissions", 1, accessor)
                {
                    Parameters = GetQueryPermissionsParametersXml(healthRecordItemTypeIds)
                };

            HealthServiceResponseData responseData = await request.ExecuteAsync().ConfigureAwait(false);

            XPathNavigator infoNav =
                responseData.InfoNavigator.SelectSingleNode(
                    GetQueryPermissionsInfoXPathExpression(
                        responseData.InfoNavigator));

            HealthRecordPermissions recordPermissions = new HealthRecordPermissions();
            XPathNodeIterator thingTypePermissionsNodes =
                infoNav.Select("thing-type-permission");

            foreach (XPathNavigator nav in thingTypePermissionsNodes)
            {
                HealthRecordItemTypePermission thingTypePermissions =
                    HealthRecordItemTypePermission.CreateFromXml(nav);
                recordPermissions.ItemTypePermissions.Add(thingTypePermissions);
            }

            XPathNavigator valueNav = infoNav.SelectSingleNode("other-settings/meaningfuluse-opt-in");
            if (valueNav != null)
            {
                recordPermissions.MeaningfulUseOptIn = valueNav.ValueAsBoolean;
            }
            else
            {
                recordPermissions.MeaningfulUseOptIn = null;
            }

            return recordPermissions;
            */

            return null;
        }

        private static string GetQueryPermissionsParametersXml(
            IList<Guid> healthRecordItemTypeIds)
        {
            StringBuilder parameters = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                foreach (Guid guid in healthRecordItemTypeIds)
                {
                    writer.WriteElementString(
                        "thing-type-id",
                        guid.ToString());
                }

                writer.Flush();
            }

            return parameters.ToString();
        }

        private static readonly XPathExpression QueryPermissionsInfoPath =
            XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetQueryPermissionsInfoXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.QueryPermissions");

            XPathExpression infoPathClone;
            lock (QueryPermissionsInfoPath)
            {
                infoPathClone = QueryPermissionsInfoPath.Clone();
            }

            infoPathClone.SetContext(infoXmlNamespaceManager);

            return infoPathClone;
        }

        /// <summary>
        /// Gets valid group memberships for a record.
        /// </summary>
        ///
        /// <remarks>
        /// Group membership thing types allow an application to signify that the
        /// record belongs to an application defined group.  A record in the group may be
        /// eligible for special programs offered by other applications, for example.
        /// Applications then need a away to query for valid group memberships.
        /// <br/>
        /// Valid group memberships are those memberships which are not expired, and whose
        /// last updating application is authorized by the the last updating person to
        /// read and delete the membership.
        /// </remarks>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="applicationIds">
        /// A collection of unique application identifiers for which to
        /// search for group memberships.  For a null or empty application identifier
        /// list, return all valid group memberships for the record.  Otherwise,
        /// return only those group memberships last updated by one of the
        /// supplied application identifiers.
        /// </param>
        ///
        /// <returns>
        /// A List of HealthRecordItems representing the valid group memberships.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// If an error occurs while contacting the HealthVault service.
        /// </exception>
        public virtual async Task<Collection<HealthRecordItem>> GetValidGroupMembershipAsync(
            IConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> applicationIds)
        {
            StringBuilder parameters = new StringBuilder(128);
            if (applicationIds != null)
            {
                XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
                using (XmlWriter writer = XmlWriter.Create(parameters, settings))
                {
                    foreach (Guid guid in applicationIds)
                    {
                        writer.WriteElementString(
                            "application-id",
                            guid.ToString());
                    }
                }
            }

            // TODO: IConnection-ify this.
            /*
            HealthServiceRequest request =
            new HealthServiceRequest(connection, "GetValidGroupMembership", 1, accessor)
                {
                    Parameters = parameters.ToString()
                };

            HealthServiceResponseData responseData = await request.ExecuteAsync().ConfigureAwait(false);

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    responseData.InfoNavigator,
                    "GetValidGroupMembership");

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(infoPath);

            Collection<HealthRecordItem> memberships = new Collection<HealthRecordItem>();

            XPathNodeIterator membershipIterator = infoNav.Select("thing");
            if (membershipIterator != null)
            {
                foreach (XPathNavigator membershipNav in membershipIterator)
                {
                    memberships.Add(ItemTypeManager.DeserializeItem(membershipNav.OuterXml));
                }
            }

            return memberships;
            */

            return null;
        }
    }
}
