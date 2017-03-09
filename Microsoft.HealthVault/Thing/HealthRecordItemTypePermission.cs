// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Provides online and offline access permissions to persons for a health
    /// record item type (<see cref="HealthRecordItemTypeDefinition"/>) in a
    /// health record in the context of an application.
    /// </summary>
    ///
    public class HealthRecordItemTypePermission
    {
        /// <summary>
        /// Creates an instance of
        /// <see cref="HealthRecordItemTypePermission"/> from XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the <see cref="HealthRecordItemTypePermission"/>
        /// information.
        /// </param>
        ///
        /// <returns>
        /// A new instance of <see cref="HealthRecordItemTypePermission"/>
        /// populated with the information in the XML.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The XPathNavigator intended to contain the XML information is <b>null</b>.
        /// </exception>
        ///
        public static HealthRecordItemTypePermission CreateFromXml(
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            HealthRecordItemTypePermission permissions
                = new HealthRecordItemTypePermission();
            permissions.ParseXml(navigator);

            return permissions;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the health record item
        /// type associated with the permissions.
        /// </summary>
        ///
        /// <returns>
        /// The GUID of the health record item type.
        /// </returns>
        ///
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the permissions for online access for the person, for the
        /// health record item type in the health record in the context of
        /// the application.
        /// </summary>
        ///
        /// <returns>
        /// The <see cref="HealthRecordItemPermissions"/> for online access.
        /// </returns>
        ///
        public HealthRecordItemPermissions OnlineAccessPermissions { get; set; }

        /// <summary>
        /// Gets or sets the permissions for offline access for the person, for the
        /// health record item type in the health record in the context of
        /// the application.
        /// </summary>
        ///
        /// <returns>
        /// The <see cref="HealthRecordItemPermissions"/> for offline access.
        /// </returns>
        ///
        public HealthRecordItemPermissions OfflineAccessPermissions { get; set; }

        internal void ParseXml(XPathNavigator navigator)
        {
            this.TypeId = new Guid(navigator.SelectSingleNode(
                            "thing-type-id").Value);

            XPathNavigator onlinePermissions
                = navigator.SelectSingleNode("online-access-permissions");
            this.OnlineAccessPermissions = HealthRecordItemPermissions.None;

            if (onlinePermissions != null)
            {
                XPathNodeIterator nodes
                    = onlinePermissions.Select("permission");
                try
                {
                    foreach (XPathNavigator navPerms in nodes)
                    {
                        this.OnlineAccessPermissions
                            |= (HealthRecordItemPermissions)Enum.Parse(
                                    typeof(HealthRecordItemPermissions),
                                    navPerms.Value);
                    }
                }
                catch (ArgumentException)
                {
                    this.OnlineAccessPermissions
                        = HealthRecordItemPermissions.None;
                }
            }

            XPathNavigator offlinePermissions
                = navigator.SelectSingleNode("offline-access-permissions");
            this.OfflineAccessPermissions = HealthRecordItemPermissions.None;

            if (offlinePermissions != null)
            {
                XPathNodeIterator nodes
                    = offlinePermissions.Select("permission");
                try
                {
                    foreach (XPathNavigator navPerms in nodes)
                    {
                        this.OfflineAccessPermissions
                            |= (HealthRecordItemPermissions)Enum.Parse(
                                    typeof(HealthRecordItemPermissions),
                                    navPerms.Value);
                    }
                }
                catch (ArgumentException)
                {
                    this.OfflineAccessPermissions
                        = HealthRecordItemPermissions.None;
                }
            }
        }
    }
}
