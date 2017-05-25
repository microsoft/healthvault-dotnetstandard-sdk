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
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates
    /// a collection of group membership information.
    /// </summary>
    ///
    public class GroupMembership : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GroupMembership"/> class
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public GroupMembership()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("66ac44c7-1d60-4e95-bb5b-d21490e91057");

        /// <summary>
        /// Populates this group membership instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the group membership data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a group membership node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("group-membership");

            Validator.ThrowInvalidIfNull(itemNav, Resources.GroupMembershipUnexpectedNode);

            XPathNodeIterator membershipIterator = itemNav.Select("membership");
            _groupMemberships.Clear();
            foreach (XPathNavigator membershipNav in membershipIterator)
            {
                GroupMembershipType groupMembership = new GroupMembershipType();
                groupMembership.ParseXml(membershipNav);
                _groupMemberships.Add(groupMembership);
            }

            _expires = XPathHelper.GetOptNavValue<HealthServiceDateTime>(itemNav, "expires");
        }

        /// <summary>
        /// Writes the group membership data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the group membership data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <group-membership>
            writer.WriteStartElement("group-membership");

            // <membership>
            for (int index = 0; index < _groupMemberships.Count; ++index)
            {
                _groupMemberships[index].WriteXml("membership", writer);
            }

            // <expires>
            XmlWriterHelper.WriteOpt(writer, "expires", _expires);

            // </group-membership>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a collection of group memberships of the record owner.
        /// </summary>
        ///
        public Collection<GroupMembershipType> GroupMemberships => _groupMemberships;

        private readonly Collection<GroupMembershipType> _groupMemberships =
            new Collection<GroupMembershipType>();

        /// <summary>
        /// Gets or sets the date/time when the group membership expires.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The default value is the current year, month, and day.
        /// Set the value to <b>null</b> if there is no expiration.
        /// </value>
        ///
        public HealthServiceDateTime Expires
        {
            get { return _expires; }

            set
            {
                _expires = value;
            }
        }

        private HealthServiceDateTime _expires;

        /// <summary>
        /// Gets a string representation of the group membership.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the group membership.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            for (int index = 0; index < GroupMemberships.Count; ++index)
            {
                if (index > 0)
                {
                    result.Append(Resources.ListSeparator);
                }

                result.Append(GroupMemberships[index]);
            }

            return result.ToString();
        }
    }
}
