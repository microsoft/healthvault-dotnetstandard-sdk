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

namespace Microsoft.HealthVault.PlatformPrimitives
{

    /// <summary>
    /// Provides low-level access to the HealthVault record operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set 
    /// HealthVaultPlatformRecord.Current to a derived class to intercept all message calls.
    /// </remarks>

    public class HealthVaultPlatformRecord
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
        internal static HealthVaultPlatformRecord Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformRecord _current = new HealthVaultPlatformRecord();
        private static HealthVaultPlatformRecord _saved;

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
        public virtual void RemoveApplicationAuthorization(
            ApplicationConnection connection,
            HealthRecordAccessor accessor)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "RemoveApplicationRecordAuthorization", 1, accessor);

            request.Execute();
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
        public virtual IDictionary<Guid, HealthRecordItemTypePermission> QueryPermissionsByTypes(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            Collection<HealthRecordItemTypePermission> typePermissions =
                QueryRecordPermissions(connection, accessor, healthRecordItemTypeIds).ItemTypePermissions;

            Dictionary<Guid, HealthRecordItemTypePermission> permissions = new Dictionary<Guid, HealthRecordItemTypePermission>();

            for (int i = 0; i < healthRecordItemTypeIds.Count; ++i)
            {
                if (!permissions.ContainsKey(healthRecordItemTypeIds[i]))
                {
                    permissions.Add(healthRecordItemTypeIds[i], null);
                }
            }

            foreach (HealthRecordItemTypePermission typePermission in typePermissions)
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
        public virtual Collection<HealthRecordItemTypePermission> QueryPermissions(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            return QueryRecordPermissions(connection, accessor, healthRecordItemTypeIds).ItemTypePermissions;
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
        public virtual HealthRecordPermissions QueryRecordPermissions(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> healthRecordItemTypeIds)
        {
            Validator.ThrowIfArgumentNull(healthRecordItemTypeIds, "healthRecordItemTypeIds", "CtorhealthRecordItemTypeIdsArgumentNull");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "QueryPermissions", 1, accessor);

            request.Parameters =
                GetQueryPermissionsParametersXml(healthRecordItemTypeIds);

            request.Execute();

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(
                    GetQueryPermissionsInfoXPathExpression(
                        request.Response.InfoNavigator));

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
        }

        private static string GetQueryPermissionsParametersXml(
            IList<Guid> healthRecordItemTypeIds)
        {
            StringBuilder parameters = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(parameters, settings);
                for (int i = 0; i < healthRecordItemTypeIds.Count; ++i)
                {
                    writer.WriteElementString(
                        "thing-type-id",
                        healthRecordItemTypeIds[i].ToString());
                }
                writer.Flush();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                writer = null;
            }
            return parameters.ToString();
        }
        private static XPathExpression _queryPermissionsInfoPath =
            XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetQueryPermissionsInfoXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.QueryPermissions");

            XPathExpression infoPathClone = null;
            lock (_queryPermissionsInfoPath)
            {
                infoPathClone = _queryPermissionsInfoPath.Clone();
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
        public virtual Collection<HealthRecordItem> GetValidGroupMembership(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<Guid> applicationIds)
        {
            StringBuilder parameters = new StringBuilder(128);
            if (applicationIds != null)
            {
                XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
                using (XmlWriter writer = XmlWriter.Create(parameters, settings))
                {
                    for (int i = 0; i < applicationIds.Count; i++)
                    {
                        writer.WriteElementString(
                            "application-id",
                            applicationIds[i].ToString());
                    }
                }
            }

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetValidGroupMembership", 1, accessor);

            request.Parameters = parameters.ToString();
            request.Execute();

            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "GetValidGroupMembership");

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(infoPath);

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
        }
    }
}

