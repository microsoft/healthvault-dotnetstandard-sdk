// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;

namespace Microsoft.Health.MeaningfulUse
{
    /// <summary>
    /// Provides document receipt information for Meaningful Use Timely Access Report.
    /// </summary>
    public class DocumentReceipt
    {
        /// <summary>
        /// Constructor for DocumentReceipt.
        /// </summary>
        /// 
        /// <param name="source">
        /// The source that added the document to HealthVault.
        /// This is either the application id or domain of the email sender if data came from Direct channel.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> is <b>null</b>.
        /// </exception>
        public DocumentReceipt(string source)
        {
            Validator.ThrowIfArgumentNull(source, "source", "SourceNull");
            Source = source;
        }

        /// <summary>
        /// Gets or sets the contributing source of this document.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the unique identifier of the application that added the data
        /// or the domain of the sender if data was contributed via Direct channel.
        /// </value>
        /// 
        public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the string identifier of the patient.
        /// </summary>
        ///
        /// <value>
        /// A string representing the identifier of the patient which 
        /// is set by the calling application or extracted from the health record item
        /// such as CCDA.
        /// </value>
        /// 
        public string PatientId { get; private set; }

        /// <summary>
        /// Gets or sets the string identifier of the organization.
        /// </summary>
        ///
        /// <value>
        /// A string representing the identifier of the provider which
        /// is extracted from the health record item such as CCDA.
        /// </value>
        /// 
        public string OrganizationId { get; private set; }

        /// <summary>
        /// Gets or sets the visit date (ambulatory) or the discharge date (inpatient) associated with this document.
        /// </summary>
        /// 
        /// <value>
        /// A DateTime representing either the patient visit date (ambulatory), or the discharge date (inpatient).
        /// </value>
        /// 
        public DateTime EventDate { get; private set; }

        /// <summary>
        /// Gets or sets the available date associated with this document.
        /// </summary>
        /// 
        /// <value>
        /// A DateTime representing the date in which the document was available in HealthVault.
        /// </value>
        /// 
        public DateTime AvailableDate { get; private set; }

        /// <summary>
        /// Populate the <see cref="PatientActivity"/> instance from the supplied <see cref="XPathNavigator"/>.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The <see cref="XPathNavigator"/> to read the data from.
        /// </param>
        internal void ParseXml(XPathNavigator navigator)
        {
            PatientId =
                navigator.GetAttribute("patient-id", String.Empty);

            string orgId = navigator.GetAttribute("organization-id", String.Empty);
            if (!String.IsNullOrEmpty(orgId))
            {
                OrganizationId = orgId;
            }

            EventDate =
                XPathHelper.ParseAttributeAsDateTime(navigator, "event-date", DateTime.MinValue);

            AvailableDate = navigator.SelectSingleNode("available-date").ValueAsDateTime;
        }
    }
}
