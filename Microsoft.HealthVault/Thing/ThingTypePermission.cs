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
    /// record item type (<see cref="ThingTypeDefinition"/>) in a
    /// health record in the context of an application.
    /// </summary>
    ///
    public class ThingTypePermission
    {
        /// <summary>
        /// Creates an instance of
        /// <see cref="ThingTypePermission"/> from XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the <see cref="ThingTypePermission"/>
        /// information.
        /// </param>
        ///
        /// <returns>
        /// A new instance of <see cref="ThingTypePermission"/>
        /// populated with the information in the XML.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The XPathNavigator intended to contain the XML information is <b>null</b>.
        /// </exception>
        ///
        public static ThingTypePermission CreateFromXml(
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            ThingTypePermission permissions
                = new ThingTypePermission();
            permissions.ParseXml(navigator);

            return permissions;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the thing
        /// type associated with the permissions.
        /// </summary>
        ///
        /// <returns>
        /// The GUID of the thing type.
        /// </returns>
        ///
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the permissions for online access for the person, for the
        /// thing type in the health record in the context of
        /// the application.
        /// </summary>
        ///
        /// <returns>
        /// The <see cref="ThingPermissions"/> for online access.
        /// </returns>
        ///
        public ThingPermissions OnlineAccessPermissions { get; set; }

        /// <summary>
        /// Gets or sets the permissions for offline access for the person, for the
        /// thing type in the health record in the context of
        /// the application.
        /// </summary>
        ///
        /// <returns>
        /// The <see cref="ThingPermissions"/> for offline access.
        /// </returns>
        ///
        public ThingPermissions OfflineAccessPermissions { get; set; }

        internal void ParseXml(XPathNavigator navigator)
        {
            this.TypeId = new Guid(navigator.SelectSingleNode(
                            "thing-type-id").Value);

            XPathNavigator onlinePermissions
                = navigator.SelectSingleNode("online-access-permissions");
            this.OnlineAccessPermissions = ThingPermissions.None;

            if (onlinePermissions != null)
            {
                XPathNodeIterator nodes
                    = onlinePermissions.Select("permission");
                try
                {
                    foreach (XPathNavigator navPerms in nodes)
                    {
                        this.OnlineAccessPermissions
                            |= (ThingPermissions)Enum.Parse(
                                    typeof(ThingPermissions),
                                    navPerms.Value);
                    }
                }
                catch (ArgumentException)
                {
                    this.OnlineAccessPermissions
                        = ThingPermissions.None;
                }
            }

            XPathNavigator offlinePermissions
                = navigator.SelectSingleNode("offline-access-permissions");
            this.OfflineAccessPermissions = ThingPermissions.None;

            if (offlinePermissions != null)
            {
                XPathNodeIterator nodes
                    = offlinePermissions.Select("permission");
                try
                {
                    foreach (XPathNavigator navPerms in nodes)
                    {
                        this.OfflineAccessPermissions
                            |= (ThingPermissions)Enum.Parse(
                                    typeof(ThingPermissions),
                                    navPerms.Value);
                    }
                }
                catch (ArgumentException)
                {
                    this.OfflineAccessPermissions
                        = ThingPermissions.None;
                }
            }
        }
    }
}
