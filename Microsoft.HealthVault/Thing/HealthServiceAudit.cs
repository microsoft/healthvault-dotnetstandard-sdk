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
            Timestamp =
                auditNav.SelectSingleNode("timestamp").ValueAsDateTime;

            XPathNavigator nav = auditNav.SelectSingleNode("app-id");
            _applicationId = new Guid(nav.Value);
            ApplicationName = nav.GetAttribute("name", string.Empty);

            nav = auditNav.SelectSingleNode("person-id");
            _personId = new Guid(nav.Value);
            PersonName = nav.GetAttribute("name", string.Empty);

            nav = auditNav.SelectSingleNode("impersonator-id");
            if (nav != null)
            {
                ImpersonatorId = new Guid(nav.Value);
                ImpersonatorName = nav.GetAttribute("name", string.Empty);
            }

            nav = auditNav.SelectSingleNode("access-avenue");
            if (nav != null)
            {
                try
                {
                    AccessAvenue =
                        (HealthServiceAuditAccessAvenue)Enum.Parse(
                            typeof(HealthServiceAuditAccessAvenue), nav.Value);
                }
                catch (ArgumentException)
                {
                    AccessAvenue = HealthServiceAuditAccessAvenue.Unknown;
                }
            }

            nav = auditNav.SelectSingleNode("audit-action");

            try
            {
                AuditAction =
                    (HealthServiceAuditAction)Enum.Parse(
                        typeof(HealthServiceAuditAction), nav.Value);
            }
            catch (ArgumentException)
            {
                AuditAction = HealthServiceAuditAction.Unknown;
            }

            nav = auditNav.SelectSingleNode("master-app-id");
            if (nav != null)
            {
                _masterAppId = new Guid(nav.Value);
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("timestamp", SDKHelper.XmlFromDateTime(Timestamp));

            writer.WriteStartElement("app-id");
            writer.WriteAttributeString("name", ApplicationName);
            writer.WriteValue(_applicationId.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("person-id");
            writer.WriteAttributeString("name", PersonName);
            writer.WriteValue(_personId.ToString());
            writer.WriteEndElement();

            if (ImpersonatorId != null)
            {
                writer.WriteStartElement("impersonator-id");
                if (!string.IsNullOrEmpty(ImpersonatorName))
                {
                    writer.WriteAttributeString("name", ImpersonatorName);
                }

                writer.WriteValue(ImpersonatorId.ToString());
                writer.WriteEndElement();
            }

            if (AccessAvenue != HealthServiceAuditAccessAvenue.Unknown)
            {
                writer.WriteElementString("access-avenue", AccessAvenue.ToString());
            }

            if (AuditAction != HealthServiceAuditAction.Unknown)
            {
                writer.WriteElementString("audit-action", AuditAction.ToString());
            }

            if (_masterAppId != Guid.Empty)
            {
                writer.WriteElementString("master-app-id", _masterAppId.ToString());
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
        public Guid ApplicationId => _applicationId;

        private Guid _applicationId;

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
        public Guid PersonId => _personId;

        private Guid _personId;

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
        public Guid MasterApplicationId => _masterAppId;

        private Guid _masterAppId;

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
            return $"{Timestamp}, {_applicationId}, {_personId}, {AuditAction}";
        }
    }
}
