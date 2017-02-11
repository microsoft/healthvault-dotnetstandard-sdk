// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;

namespace Microsoft.Health.MeaningfulUse
{
    /// <summary>
    /// Provides DOPU document receipt information for Meaningful Use Timely Access Report.
    /// </summary>
    public class DOPUDocumentReceipt
    {
        /// <summary>
        /// Constructor for DOPUDocumentReceipt.
        /// </summary>
        /// 
        /// <param name="source">
        /// The source that added the DOPU package to HealthVault.
        /// This is either the application id or domain of the email sender if data came from Direct channel.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> is <b>null</b>.
        /// </exception>
        public DOPUDocumentReceipt(string source)
        {
            Validator.ThrowIfArgumentNull(source, "source", "SourceNull");
            Source = source;
        }

        /// <summary>
        /// Gets or sets the contributing source of this package.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the unique identifier of the application that added the data
        /// or the domain of the sender if data was contributed via Direct channel.
        /// </value>
        /// 
        public string Source { get; private set; }

        /// <summary>
        /// Gets the identifier of the DOPU package associated with this report entry.
        /// </summary>
        ///
        /// <value>
        /// A string representing the identifier of the package which
        /// is returned to the application from the call to CreateConnectPackage.
        /// </value>
        /// 
        public string PackageId { get; private set; }

        /// <summary>
        /// Gets the date the DOPU package associated with this report entry was made available in HealthVault.
        /// </summary>
        /// 
        /// <value>
        /// The date the DOPU package associated with this report entry was made available in HealthVault.
        /// </value>
        /// 
        public DateTime AvailableDate { get; private set; }

        /// <summary>
        /// Populate the <see cref="DOPUDocumentReceipt"/> instance from the supplied <see cref="XPathNavigator"/>.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The <see cref="XPathNavigator"/> to read the data from.
        /// </param>
        internal void ParseXml(XPathNavigator navigator)
        {
            PackageId =
                navigator.GetAttribute("package-id", String.Empty);

            AvailableDate = navigator.SelectSingleNode("available-date").ValueAsDateTime;
        }
    }
}
