// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.Record
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
            this.IsRecordCustodian = navigator.SelectSingleNode("record-custodian").ValueAsBoolean;

            this.RecordDisplayName = navigator.SelectSingleNode("record-display-name").Value;
            this.DateAuthorizationExpires = XPathHelper.GetDateTime(navigator, "date-auth-expires");

            Collection<AuthorizationRule> authRules =
                AuthorizationRule.CreateFromXml(navigator.SelectSingleNode("auth-xml"));
            this.AccessRights = new ReadOnlyCollection<AuthorizationRule>(authRules);

            this.RelationshipType =
                XPathHelper.GetEnumByNumber(
                    navigator,
                    "rel-type",
                    RelationshipType.Unknown);

            this.DateAuthorizationFirstAccepted = XPathHelper.GetDateTime(navigator, "date-auth-created");
            this.DateAuthorizationChanged = XPathHelper.GetDateTime(navigator, "date-auth-updated");

            this.CanAccessAuditInformation = navigator.SelectSingleNode("can-access-audit").ValueAsBoolean;
        }

        #region public properties

        /// <summary>
        /// Gets whether or not the person is a custodian of the HealthVault record.
        /// </summary>
        ///
        public bool IsRecordCustodian { get; private set; }

        /// <summary>
        /// Gets the person's email address.
        /// </summary>
        ///
        public string Email { get; internal set; }

        /// <summary>
        /// Gets the display name of the HealthVault record that the person has access to.
        /// </summary>
        ///
        public string RecordDisplayName { get; private set; }

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
        public AuthorizedRecordState RecordAuthorizationState { get; internal set; }

        /// <summary>
        /// Gets the date when authorization to the HealthVault record expires.
        /// </summary>
        ///
        public DateTime DateAuthorizationExpires { get; private set; }

        /// <summary>
        /// Gets the permissions the person has to the HealthVault record.
        /// </summary>
        ///
        public ReadOnlyCollection<AuthorizationRule> AccessRights { get; private set; }

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
        public RelationshipType RelationshipType { get; private set; } = RelationshipType.Unknown;

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
        public DateTime DateAuthorizationFirstAccepted { get; private set; }

        /// <summary>
        /// Gets the date when the person accepted changes to their access to the HealthVault
        /// record.
        /// </summary>
        ///
        public DateTime DateAuthorizationChanged { get; private set; }

        /// <summary>
        /// Gets whether the authorized person can access audit information in the HealthVault
        /// record.
        /// </summary>
        ///
        public bool CanAccessAuditInformation { get; private set; }

        #endregion public properties
    }
}
