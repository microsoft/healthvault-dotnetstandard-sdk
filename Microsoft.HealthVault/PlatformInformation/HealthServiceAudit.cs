// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
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
        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceAudit"/>
        /// class using default values.
        /// </summary>
        ///
        public HealthServiceAudit()
        {
        }

        internal void ParseXml(XPathNavigator auditNav)
        {
            _timestamp =
                auditNav.SelectSingleNode("timestamp").ValueAsDateTime;

            XPathNavigator nav = auditNav.SelectSingleNode("app-id");
            _applicationId = new Guid(nav.Value);
            _applicationName = nav.GetAttribute("name", String.Empty);

            nav = auditNav.SelectSingleNode("person-id");
            _personId = new Guid(nav.Value);
            _personName = nav.GetAttribute("name", String.Empty);

            nav = auditNav.SelectSingleNode("impersonator-id");
            if (nav != null)
            {
                _impersonatorId = new Guid(nav.Value);
                _impersonatorName = nav.GetAttribute("name", String.Empty);
            }

            nav = auditNav.SelectSingleNode("access-avenue");
            if (nav != null)
            {
                try
                {
                    _accessAvenue =
                        (HealthServiceAuditAccessAvenue)Enum.Parse(
                            typeof(HealthServiceAuditAccessAvenue), nav.Value);
                }
                catch (ArgumentException)
                {
                    _accessAvenue = HealthServiceAuditAccessAvenue.Unknown;
                }
            }

            nav = auditNav.SelectSingleNode("audit-action");

            try
            {
                _auditAction =
                    (HealthServiceAuditAction)Enum.Parse(
                        typeof(HealthServiceAuditAction), nav.Value);
            }
            catch (ArgumentException)
            {
                _auditAction = HealthServiceAuditAction.Unknown;
            }

            nav = auditNav.SelectSingleNode("master-app-id");
            if (nav != null)
            {
                _masterAppId = new Guid(nav.Value);
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("timestamp", SDKHelper.XmlFromDateTime(_timestamp));

            writer.WriteStartElement("app-id");
            writer.WriteAttributeString("name", _applicationName);
            writer.WriteValue(_applicationId.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("person-id");
            writer.WriteAttributeString("name", _personName);
            writer.WriteValue(_personId.ToString());
            writer.WriteEndElement();

            if (_impersonatorId != null)
            {
                writer.WriteStartElement("impersonator-id");
                if (!String.IsNullOrEmpty(_impersonatorName))
                {
                    writer.WriteAttributeString("name", _impersonatorName);
                }
                writer.WriteValue(_impersonatorId.ToString());
                writer.WriteEndElement();
            }

            if (_accessAvenue != HealthServiceAuditAccessAvenue.Unknown)
            {
                writer.WriteElementString("access-avenue", _accessAvenue.ToString());
            }

            if (_auditAction != HealthServiceAuditAction.Unknown)
            {
                writer.WriteElementString("audit-action", _auditAction.ToString());
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
        public DateTime Timestamp
        {
            get { return _timestamp; }
        }
        private DateTime _timestamp = DateTime.Now;

        /// <summary>
        /// Gets the unique identifier of the application.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the unique identifier of the application.
        /// </value>
        ///
        public Guid ApplicationId
        {
            get { return _applicationId; }
        }
        private Guid _applicationId;

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name of the application.
        /// </value>
        ///
        public string ApplicationName
        {
            get { return _applicationName; }
        }
        private string _applicationName;

        /// <summary>
        /// Gets the unique identifier of the person.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the unique identifier of the person.
        /// </value>
        ///
        public Guid PersonId
        {
            get { return _personId; }
        }
        private Guid _personId;

        /// <summary>
        /// Gets the name of the person.
        /// </summary>
        ///
        public string PersonName
        {
            get { return _personName; }
        }
        private string _personName;

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
        /// change, or delete the <see cref="HealthRecordItem"/>.
        /// </remarks>
        ///
        public Guid? ImpersonatorId
        {
            get { return _impersonatorId; }
        }
        private Guid? _impersonatorId;

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
        /// change, or delete the <see cref="HealthRecordItem"/>.
        /// </remarks>
        ///
        public string ImpersonatorName
        {
            get { return _impersonatorName; }
        }
        private string _impersonatorName;

        /// <summary>
        /// Gets the access avenue used to create, change, or delete the
        /// <see cref="HealthRecordItem"/>.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceAuditAccessAvenue"/>.
        /// </value>
        ///
        public HealthServiceAuditAccessAvenue AccessAvenue
        {
            get { return _accessAvenue; }
        }
        private HealthServiceAuditAccessAvenue _accessAvenue =
            HealthServiceAuditAccessAvenue.Online;

        /// <summary>
        /// Gets the action performed.
        /// </summary>
        ///
        public HealthServiceAuditAction AuditAction
        {
            get { return _auditAction; }
        }
        private HealthServiceAuditAction _auditAction;

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
        public Guid MasterApplicationId
        {
            get { return _masterAppId; }
        }
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
            return
                String.Format(
                    CultureInfo.CurrentCulture,
                    toStringFormat,
                    _timestamp,
                    _applicationId,
                    _personId,
                    _auditAction);
        }

        // {0} - timestamp
        // {1} - app-id
        // {2} - person-id
        // {3} - audit-action
        //
        private const string toStringFormat = "{0}, {1}, {2}, {3}";
    }
}
