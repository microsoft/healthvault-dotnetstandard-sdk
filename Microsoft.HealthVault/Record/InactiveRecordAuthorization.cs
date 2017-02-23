// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Provides information about a person who has not yet accepted an invitation to share a
    /// HealthVault record.
    /// </summary>
    ///
    public class InactiveRecordAuthorization : RecordAuthorization
    {
        /// <summary>
        /// Populates the class members with data from the specified
        /// active person information XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the person information from.
        /// </param>
        ///
        internal override void ParseXml(XPathNavigator navigator)
        {
            base.ParseXml(navigator);

            this.Email = navigator.SelectSingleNode("email-address").Value;

            this.RecordAuthorizationState =
                XPathHelper.GetEnumByName(
                    navigator,
                    "authorized-record-state",
                    AuthorizedRecordState.ActivationPending);

            this.GrantorName = navigator.SelectSingleNode("grantor-name").Value;
            this.GranteeName = navigator.SelectSingleNode("grantee-name").Value;
        }

        #region public properties

        /// <summary>
        /// Gets the name of the person that sent the invitation to share their HealthVault record.
        /// </summary>
        ///
        public string GrantorName { get; private set; }

        /// <summary>
        /// Gets the name of the person that was invited to share the HealthVault record.
        /// </summary>
        public string GranteeName { get; private set; }

        #endregion public properties
    }
}
