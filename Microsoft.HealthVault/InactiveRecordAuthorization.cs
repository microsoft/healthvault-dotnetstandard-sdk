// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Provides information about a person who has not yet accepted an invitation to share a
    /// HealthVault record.
    /// </summary>
    ///
    public class InactiveRecordAuthorization : RecordAuthorization
    {
        /// <summary>
        /// Constructs an instance of <see cref="InactiveRecordAuthorization"/> with default values.
        /// </summary>
        ///
        public InactiveRecordAuthorization()
        {
        }

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

            Email = navigator.SelectSingleNode("email-address").Value;

            RecordAuthorizationState =
                XPathHelper.GetEnumByName<AuthorizedRecordState>(
                    navigator,
                    "authorized-record-state",
                    AuthorizedRecordState.ActivationPending);

            _grantorName = navigator.SelectSingleNode("grantor-name").Value;
            _granteeName = navigator.SelectSingleNode("grantee-name").Value;
        }

        #region public properties

        /// <summary>
        /// Gets the name of the person that sent the invitation to share their HealthVault record.
        /// </summary>
        ///
        public string GrantorName
        {
            get { return _grantorName; }
        }
        private string _grantorName;

        /// <summary>
        /// Gets the name of the person that was invited to share the HealthVault record.
        /// </summary>
        public string GranteeName
        {
            get { return _granteeName; }
        }
        private string _granteeName;

        #endregion public properties
    }
}
