// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Represents the credential for those using Passport.
    /// </summary>
    ///
    internal class PassportCredential : Credential
    {
        #region ctor

        internal PassportCredential()
        {
        }

        internal PassportCredential(Guid appId)
        {
            this.appId = appId;
        }

        internal string AuthenticationToken
        {
            get { return this.authToken; }

            set
            {
                this.authToken = value;
                CreateAuthenticationTokenResult result =
                    new CreateAuthenticationTokenResult
                    {
                        ApplicationRecordAuthorizationAction = ApplicationRecordAuthorizationAction.NoActionRequired,
                        AuthenticationToken = this.authToken,
                        Status = AuthenticationTokenCreationStatus.Success,
                        ApplicationId = this.appId
                    };

                this.UpdateAuthenticationResults(result);
            }
        }

        private readonly Guid appId;
        private string authToken;

        /// <summary>
        /// Creates a new instance of a PassportCredential.
        /// </summary>
        ///
        /// <param name="appId">
        /// A GUID representing the application identifier.
        /// </param>
        ///
        /// <remarks>
        /// A new instance of a PassportCredential is created by first
        /// calling into the Passport APIs and getting a ticket from the
        /// Passport service for the person logging on. Once the ticket has
        /// been verified by the Shell, it passes it to HealthVault to create
        /// the session token.
        /// </remarks>
        ///
        internal static PassportCredential Create(Guid appId)
        {
            PassportCredential cred = new PassportCredential(appId);
            cred.GetToken();
            return cred;
        }

        #endregion

        /// <summary>
        /// Calls the CardSpace service and establishes the client side of the
        /// authenticated session.
        /// </summary>
        ///
        internal void GetToken()
        {
            this.SharedSecret = new CryptoHmac();
        }

        /// <summary>
        /// Represents the XMLWriter that receives credential information.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XMLWriter.
        /// </param>
        ///
        public override void WriteInfoXml(XmlWriter writer)
        {
            writer.WriteStartElement("passport");

            writer.WriteStartElement("shared-secret");
            this.SharedSecret.WriteInfoXml(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
