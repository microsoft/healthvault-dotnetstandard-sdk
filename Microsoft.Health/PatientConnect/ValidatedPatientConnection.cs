// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;

namespace Microsoft.Health.PatientConnect
{
    /// <summary>
    /// Information about a validated patient connection.
    /// </summary>
    /// 
    /// <remarks>
    /// Some HealthVault applications maintain some of their own data storage but need a way to
    /// link their account/person identifier to a HealthVault identifier. The application can do
    /// this by calling <see cref="PatientConnection.Create"/> and passing the application's 
    /// identifier and some information that is specific to the user. The user can then go to 
    /// HealthVault Shell and validate the connection with their appropriate health record. The
    /// application can then query for all validated connections (usually on a daily basis) by
    /// calling <see cref="PatientConnection.GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection)"/>
    /// or <see cref="PatientConnection.GetValidatedConnections(Microsoft.Health.Web.OfflineWebApplicationConnection, DateTime)"/> 
    /// which returns instances of <see cref="ValidatedPatientConnection"/>.
    /// </remarks>
    /// 
    public class ValidatedPatientConnection
    {
        /// <summary>
        /// Constructs a <see cref="ValidatedPatientConnection"/> with default values.
        /// </summary>
        /// 
        public ValidatedPatientConnection()
        {
        }

        /// <summary>
        /// Parses the validated patient connection information from the specified XML.
        /// </summary>
        /// 
        /// <param name="nav"></param>
        /// 
        /// <exception cref="HealthServiceException">
        /// If the XML is missing one more mandatory element. The code will be InvalidXml.
        /// </exception>
        /// 
        internal void ParseXml(XPathNavigator nav)
        {
            Guid? optionalGuid = XPathHelper.GetOptNavValueAsGuid(nav, "person-id");
            if (optionalGuid != null)
            {
                _personId = optionalGuid.Value;
            }

            XPathNavigator recordNav = nav.SelectSingleNode("record-id");
            if (recordNav != null)
            {
                _applicationSpecificRecordId = recordNav.GetAttribute("app-specific-record-id", "");
                _recordId = new Guid(recordNav.Value);
            }

            optionalGuid = XPathHelper.GetOptNavValueAsGuid(nav, "app-id");
            if (optionalGuid != null)
            {
                _applicationId = optionalGuid.Value;
            }

            _applicationPatientId = XPathHelper.GetOptNavValue(nav, "external-id");

            if (_personId == Guid.Empty ||
                _recordId == Guid.Empty ||
                _applicationId == Guid.Empty ||
                _applicationPatientId == null)
            {
                throw new HealthServiceException(HealthServiceStatusCode.InvalidXml);
            }
        }

        /// <summary>
        /// Gets or sets the Application specific record id. 
        /// </summary>
        /// 
        public String ApplicationSpecificRecordId
        {
            get { return _applicationSpecificRecordId; }
            set { _applicationSpecificRecordId = value; }
        }
        private String _applicationSpecificRecordId;

        /// <summary>
        /// Gets or sets the person identifier for the patient that has validated their connection.
        /// </summary>
        /// 
        public Guid PersonId
        {
            get { return _personId; }
            set { _personId = value; }
        }
        private Guid _personId;

        /// <summary>
        /// Gets or sets the record identifier that the patient chose when validating their connection.
        /// </summary>
        /// 
        public Guid RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
        }
        private Guid _recordId;

        /// <summary>
        /// Gets or sets the application identifier for which the patient validated their connection.
        /// </summary>
        /// 
        public Guid ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }
        private Guid _applicationId;

        /// <summary>
        /// Gets or sets the application specific identifier that the application uses to connect the
        /// application data with the person's HealthVault record.
        /// </summary>
        /// 
        /// <remarks>
        /// This identifier was provided by the application when the original connection request
        /// was made.
        /// </remarks>
        /// 
        public string ApplicationPatientId
        {
            get { return _applicationPatientId; }
            set { _applicationPatientId = value; }
        }
        private string _applicationPatientId;

    }
}
