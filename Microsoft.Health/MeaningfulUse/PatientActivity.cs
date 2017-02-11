// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;

namespace Microsoft.Health.MeaningfulUse
{
    /// <summary>
    /// Provides patient activity information for the Meaningful Use VDT Report.
    /// </summary>
    public class PatientActivity
    {
        /// <summary>
        /// Constructor for PatientActivity.
        /// </summary>
        /// 
        /// <param name="source">
        /// The source that added the patient activity to HealthVault.
        /// This is either the application id or domain of the email sender if data came from Direct channel.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> is <b>null</b>.
        /// </exception>
        public PatientActivity(string source)
        {
            Validator.ThrowIfArgumentNull(source, "source", "SourceNull");
            Source = source;
        }

        /// <summary>
        /// Gets or sets the contributing source of this patient activity.
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
        }
    }
}
