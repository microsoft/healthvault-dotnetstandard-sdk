// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Provides information about a person who has access or who has been invited to share a
    /// HealthVault record.
    /// </summary>
    ///
    public abstract class RecordAuthorization
    {
        /// <summary>
        /// Populates the class members with data from the specified
        /// active person information XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the active person information from.
        /// </param>
        [SecuritySafeCritical]
        internal virtual void ParseXml(XPathNavigator navigator)
        {
            _isRecordCustodian = navigator.SelectSingleNode("record-custodian").ValueAsBoolean;

            _recordDisplayName = navigator.SelectSingleNode("record-display-name").Value;
            _expires = XPathHelper.GetDateTime(navigator, "date-auth-expires");

            Collection<AuthorizationRule> authRules =
                AuthorizationRule.CreateFromXml(navigator.SelectSingleNode("auth-xml"));
            _accessRights = new ReadOnlyCollection<AuthorizationRule>(authRules);

            _relationshipType =
                XPathHelper.GetEnumByNumber<RelationshipType>(
                    navigator,
                    "rel-type",
                    RelationshipType.Unknown);

            _dateAuthFirstAccepted = XPathHelper.GetDateTime(navigator, "date-auth-created");
            _dateAuthChanged = XPathHelper.GetDateTime(navigator, "date-auth-updated");

            _canAccessAudit = navigator.SelectSingleNode("can-access-audit").ValueAsBoolean;
        }

        #region public properties

        /// <summary>
        /// Gets whether or not the person is a custodian of the HealthVault record.
        /// </summary>
        ///
        public bool IsRecordCustodian
        {
            get { return _isRecordCustodian; }
        }
        private bool _isRecordCustodian;

        /// <summary>
        /// Gets the person's email address.
        /// </summary>
        ///
        public string Email
        {
            get { return _email; }
            internal set { _email = value; }
        }
        private string _email;

        /// <summary>
        /// Gets the display name of the HealthVault record that the person has access to.
        /// </summary>
        ///
        public string RecordDisplayName
        {
            get { return _recordDisplayName; }
        }
        private string _recordDisplayName;

        /// <summary>
        /// Gets the state of the sharing request for a HealthVault record.
        /// </summary>
        ///
        /// <remarks>
        /// When a HealthVault record is shared by its custodian, a record authorization is created
        /// in the <see cref="AuthorizedRecordState.ActivationPending"/> state. If the person to
        /// whom the record is being shared accepts the invitation the record authorization moves
        /// to the <see cref="AuthorizedRecordState.Active"/> state. If the person rejects the
        /// invitation, the record authorization moves to the
        /// <see cref="AuthorizedRecordState.ActivationRejected"/> state.
        /// </remarks>
        ///
        public AuthorizedRecordState RecordAuthorizationState
        {
            get { return _recordAuthorizationState; }
            internal set { _recordAuthorizationState = value; }
        }
        private AuthorizedRecordState _recordAuthorizationState;

        /// <summary>
        /// Gets the date when authorization to the HealthVault record expires.
        /// </summary>
        ///
        public DateTime DateAuthorizationExpires
        {
            get { return _expires; }
        }
        private DateTime _expires;

        /// <summary>
        /// Gets the permissions the person has to the HealthVault record.
        /// </summary>
        ///
        public ReadOnlyCollection<AuthorizationRule> AccessRights
        {
            get { return _accessRights; }
        }
        private ReadOnlyCollection<AuthorizationRule> _accessRights;

        /// <summary>
        /// Gets the relationship the person authorized to view this record
        /// has with the "owner" of the record.
        /// </summary>
        ///
        /// <value>
        /// An enumeration value indicating the relationship between the
        /// record owner and the person authorized to use the record.
        /// </value>
        ///
        /// <remarks>
        /// See <see cref="RelationshipType"/> for more information on the
        /// relationships and what they mean.
        /// </remarks>
        ///
        public RelationshipType RelationshipType
        {
            get { return _relationshipType; }
        }
        private RelationshipType _relationshipType = RelationshipType.Unknown;

        /// <summary>
        /// Gets the date when the person first accepted access to the HealthVault record.
        /// </summary>
        ///
        /// <remarks>
        /// If the authorized person is the record owner, this date will be the date when the
        /// record was created. If the authorized person is not the record owner, the date will be
        /// when the person first accepted the sharing invitation.
        /// </remarks>
        ///
        public DateTime DateAuthorizationFirstAccepted
        {
            get { return _dateAuthFirstAccepted; }
        }
        private DateTime _dateAuthFirstAccepted;

        /// <summary>
        /// Gets the date when the person accepted changes to their access to the HealthVault
        /// record.
        /// </summary>
        ///
        public DateTime DateAuthorizationChanged
        {
            get { return _dateAuthChanged; }
        }
        private DateTime _dateAuthChanged;

        /// <summary>
        /// Gets whether the authorized person can access audit information in the HealthVault
        /// record.
        /// </summary>
        ///
        public bool CanAccessAuditInformation
        {
            get { return _canAccessAudit; }
        }
        private bool _canAccessAudit;

        #endregion public properties
    }
}
