// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents an audit trail of creations or updates to
    /// information in the HealthVault service.
    /// </summary>
    ///
    /// <remarks>
    /// The audit records when the operation
    /// happened, by which application, and by which person.
    /// </remarks>
    ///
    public class HealthServiceAudit
    {
        internal void ParseXml(XPathNavigator auditNav)
        {
            this.Timestamp =
                auditNav.SelectSingleNode("timestamp").ValueAsDateTime;

            XPathNavigator nav = auditNav.SelectSingleNode("app-id");
            this.applicationId = new Guid(nav.Value);
            this.ApplicationName = nav.GetAttribute("name", string.Empty);

            nav = auditNav.SelectSingleNode("person-id");
            this.personId = new Guid(nav.Value);
            this.PersonName = nav.GetAttribute("name", string.Empty);

            nav = auditNav.SelectSingleNode("impersonator-id");
            if (nav != null)
            {
                this.ImpersonatorId = new Guid(nav.Value);
                this.ImpersonatorName = nav.GetAttribute("name", string.Empty);
            }

            nav = auditNav.SelectSingleNode("access-avenue");
            if (nav != null)
            {
                try
                {
                    this.AccessAvenue =
                        (HealthServiceAuditAccessAvenue)Enum.Parse(
                            typeof(HealthServiceAuditAccessAvenue), nav.Value);
                }
                catch (ArgumentException)
                {
                    this.AccessAvenue = HealthServiceAuditAccessAvenue.Unknown;
                }
            }

            nav = auditNav.SelectSingleNode("audit-action");

            try
            {
                this.AuditAction =
                    (HealthServiceAuditAction)Enum.Parse(
                        typeof(HealthServiceAuditAction), nav.Value);
            }
            catch (ArgumentException)
            {
                this.AuditAction = HealthServiceAuditAction.Unknown;
            }

            nav = auditNav.SelectSingleNode("master-app-id");
            if (nav != null)
            {
                this.masterAppId = new Guid(nav.Value);
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("timestamp", SDKHelper.XmlFromDateTime(this.Timestamp));

            writer.WriteStartElement("app-id");
            writer.WriteAttributeString("name", this.ApplicationName);
            writer.WriteValue(this.applicationId.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("person-id");
            writer.WriteAttributeString("name", this.PersonName);
            writer.WriteValue(this.personId.ToString());
            writer.WriteEndElement();

            if (this.ImpersonatorId != null)
            {
                writer.WriteStartElement("impersonator-id");
                if (!string.IsNullOrEmpty(this.ImpersonatorName))
                {
                    writer.WriteAttributeString("name", this.ImpersonatorName);
                }

                writer.WriteValue(this.ImpersonatorId.ToString());
                writer.WriteEndElement();
            }

            if (this.AccessAvenue != HealthServiceAuditAccessAvenue.Unknown)
            {
                writer.WriteElementString("access-avenue", this.AccessAvenue.ToString());
            }

            if (this.AuditAction != HealthServiceAuditAction.Unknown)
            {
                writer.WriteElementString("audit-action", this.AuditAction.ToString());
            }

            if (this.masterAppId != Guid.Empty)
            {
                writer.WriteElementString("master-app-id", this.masterAppId.ToString());
            }
        }

        /// <summary>
        /// Gets the time stamp of the audit.
        /// </summary>
        ///
        /// <value>
        /// A DateTime in UTC representing when the operation happened.
        /// </value>
        ///
        /// <remarks>
        /// The application must convert the value to local time
        /// if needed.
        /// </remarks>
        ///
        public DateTime Timestamp { get; private set; } = DateTime.Now;

        /// <summary>
        /// Gets the unique identifier of the application.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the unique identifier of the application.
        /// </value>
        ///
        public Guid ApplicationId => this.applicationId;

        private Guid applicationId;

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name of the application.
        /// </value>
        ///
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the person.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the unique identifier of the person.
        /// </value>
        ///
        public Guid PersonId => this.personId;

        private Guid personId;

        /// <summary>
        /// Gets the name of the person.
        /// </summary>
        ///
        public string PersonName { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the impersonator.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the identifier of the impersonator.
        /// </value>
        ///
        /// <remarks>
        /// The value is <b>null</b> if impersonation was not used to create,
        /// change, or delete the <see cref="ThingBase"/>.
        /// </remarks>
        ///
        public Guid? ImpersonatorId { get; private set; }

        /// <summary>
        /// Gets the name of the impersonator.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name of the impersonator.
        /// </value>
        ///
        /// <remarks>
        /// The value is <b>null</b> if impersonation was not used to create,
        /// change, or delete the <see cref="ThingBase"/>.
        /// </remarks>
        ///
        public string ImpersonatorName { get; private set; }

        /// <summary>
        /// Gets the access avenue used to create, change, or delete the
        /// <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceAuditAccessAvenue"/>.
        /// </value>
        ///
        public HealthServiceAuditAccessAvenue AccessAvenue { get; private set; } = HealthServiceAuditAccessAvenue.Online;

        /// <summary>
        /// Gets the action performed.
        /// </summary>
        ///
        public HealthServiceAuditAction AuditAction { get; private set; }

        /// <summary>
        /// The unique application identifier for the code base of the
        /// application that created the audit.
        /// </summary>
        ///
        /// <remarks>
        /// When an application performs a write action on an item the audit
        /// is stamped with the applications unique identifier. In the case of
        /// master/child applications, or SODA applications, the <see cref="ApplicationId"/>
        /// is the identifier of the child or installation of the application.
        /// The <see cref="MasterApplicationId"/> is the application identifier for the root of
        /// the configuration for that application. In the case of master/child
        /// applications, this is the application identifier of the master. For
        /// SODA applications, this is the application identifier for the root
        /// configuration of the application.
        /// </remarks>
        ///
        public Guid MasterApplicationId => this.masterAppId;

        private Guid masterAppId;

        /// <summary>
        /// Gets a string representation of the object.
        /// </summary>
        ///
        /// <returns>
        /// A string in the following format:
        ///
        ///     "&lt;timestamp&gt;, &lt;app-id&gt;, &lt;person-id&gt;"
        /// </returns>
        ///
        public override string ToString()
        {
            return $"{this.Timestamp}, {this.applicationId}, {this.personId}, {this.AuditAction}";
        }
    }
}
