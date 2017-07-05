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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;
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
        internal static HealthVaultPlatformRecord Current { get; private set; } = new HealthVaultPlatformRecord();

        /// <summary>
        /// Releases the authorization of the application on the health record.
        /// </summary>
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// <exception cref="HealthServiceException">
        /// Errors during the authorization release.
        /// </exception>
        ///
        /// <remarks>
        /// Once the application releases the authorization to the health record,
        /// calling any methods of this <see cref="HealthRecordAccessor"/> will result
        /// in a <see cref="HealthServiceAccessDeniedException"/>."
        /// </remarks>
        public virtual async Task RemoveApplicationAuthorizationAsync(IHealthVaultConnection connection)
        {
            await connection.ExecuteAsync(HealthVaultMethods.RemoveApplicationRecordAuthorization, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this  record.
        /// </summary>
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        /// <param name="healthRecordItemTypeIds">
        /// A collection of unique identifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        /// <returns>
        /// Returns a dictionary of <see cref="ThingTypePermission"/>
        /// with thing types as the keys.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty dictionary is
        /// returned. If for a thing type, the person has
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
        public virtual async Task<IDictionary<Guid, ThingTypePermission>> QueryPermissionsByTypesAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            HealthRecordPermissions recordPermissions = await QueryRecordPermissionsAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
            Collection<ThingTypePermission> typePermissions = recordPermissions.ItemTypePermissions;

            Dictionary<Guid, ThingTypePermission> permissions = new Dictionary<Guid, ThingTypePermission>();

            foreach (Guid typeId in healthRecordItemTypeIds)
            {
                if (!permissions.ContainsKey(typeId))
                {
                    permissions.Add(typeId, null);
                }
            }

            foreach (ThingTypePermission typePermission in typePermissions)
            {
                permissions[typePermission.TypeId] = typePermission;
            }

            return permissions;
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record.
        /// </summary>
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        /// <returns>
        /// A list of <see cref="ThingTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
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
        public virtual async Task<Collection<ThingTypePermission>> QueryPermissionsAsync(
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            HealthRecordPermissions permissions = await QueryRecordPermissionsAsync(connection, accessor, healthRecordItemTypeIds).ConfigureAwait(false);
            return permissions.ItemTypePermissions;
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record as well as the other permission settings such as MeaningfulUseOptIn.
        /// </summary>
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        /// <returns>
        /// A <see cref="HealthRecordPermissions"/> object
        /// which contains a collection of <see cref="ThingTypePermission"/> objects and
        /// other permission settings.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned for <see cref="HealthRecordPermissions"/> object's ItemTypePermissions property.
        /// If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
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
            IHealthVaultConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            Validator.ThrowIfArgumentNull(healthRecordItemTypeIds, nameof(healthRecordItemTypeIds), Resources.CtorhealthRecordItemTypeIdsArgumentNull);

            string parameters = GetQueryPermissionsParametersXml(healthRecordItemTypeIds);

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.QueryPermissions, 1, parameters).ConfigureAwait(false);

            XPathNavigator infoNav =
                responseData.InfoNavigator.SelectSingleNode(
                    GetQueryPermissionsInfoXPathExpression(
                        responseData.InfoNavigator));

            HealthRecordPermissions recordPermissions = new HealthRecordPermissions();
            XPathNodeIterator thingTypePermissionsNodes =
                infoNav.Select("thing-type-permission");

            foreach (XPathNavigator nav in thingTypePermissionsNodes)
            {
                ThingTypePermission thingTypePermissions =
                    ThingTypePermission.CreateFromXml(nav);
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
        }

        private static string GetQueryPermissionsParametersXml(IList<Guid> healthRecordItemTypeIds)
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

        private static readonly XPathExpression s_queryPermissionsInfoPath = XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetQueryPermissionsInfoXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.QueryPermissions");

            XPathExpression infoPathClone;
            lock (s_queryPermissionsInfoPath)
            {
                infoPathClone = s_queryPermissionsInfoPath.Clone();
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
        /// last updating application is authorized by the last updating person to
        /// read and delete the membership.
        /// </remarks>
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        /// <param name="applicationIds">
        /// A collection of unique application identifiers for which to
        /// search for group memberships.  For a null or empty application identifier
        /// list, return all valid group memberships for the record.  Otherwise,
        /// return only those group memberships last updated by one of the
        /// supplied application identifiers.
        /// </param>
        /// <returns>
        /// A List of things representing the valid group memberships.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// If an error occurs while contacting the HealthVault service.
        /// </exception>
        public virtual async Task<Collection<ThingBase>> GetValidGroupMembershipAsync(
            IHealthVaultConnection connection,
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

            var thingDeserializer = Ioc.Container.Locate<IThingDeserializer>(
                new
                {
                    connection = connection,
                    thingTypeRegistrar = Ioc.Get<IThingTypeRegistrar>()
                });

            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.GetValidGroupMembership, 1, parameters.ToString()).ConfigureAwait(false);

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    responseData.InfoNavigator,
                    "GetValidGroupMembership");

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(infoPath);

            Collection<ThingBase> memberships = new Collection<ThingBase>();

            XPathNodeIterator membershipIterator = infoNav.Select("thing");
            if (membershipIterator != null)
            {
                foreach (XPathNavigator membershipNav in membershipIterator)
                {
                    memberships.Add(thingDeserializer.Deserialize(membershipNav.OuterXml));
                }
            }

            return memberships;
        }
    }
}
