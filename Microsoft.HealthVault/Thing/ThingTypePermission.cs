// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            TypeId = new Guid(navigator.SelectSingleNode(
                            "thing-type-id").Value);

            XPathNavigator onlinePermissions
                = navigator.SelectSingleNode("online-access-permissions");
            OnlineAccessPermissions = ThingPermissions.None;

            if (onlinePermissions != null)
            {
                XPathNodeIterator nodes
                    = onlinePermissions.Select("permission");
                try
                {
                    foreach (XPathNavigator navPerms in nodes)
                    {
                        OnlineAccessPermissions
                            |= (ThingPermissions)Enum.Parse(
                                    typeof(ThingPermissions),
                                    navPerms.Value);
                    }
                }
                catch (ArgumentException)
                {
                    OnlineAccessPermissions
                        = ThingPermissions.None;
                }
            }

            XPathNavigator offlinePermissions
                = navigator.SelectSingleNode("offline-access-permissions");
            OfflineAccessPermissions = ThingPermissions.None;

            if (offlinePermissions != null)
            {
                XPathNodeIterator nodes
                    = offlinePermissions.Select("permission");
                try
                {
                    foreach (XPathNavigator navPerms in nodes)
                    {
                        OfflineAccessPermissions
                            |= (ThingPermissions)Enum.Parse(
                                    typeof(ThingPermissions),
                                    navPerms.Value);
                    }
                }
                catch (ArgumentException)
                {
                    OfflineAccessPermissions
                        = ThingPermissions.None;
                }
            }
        }
    }
}
