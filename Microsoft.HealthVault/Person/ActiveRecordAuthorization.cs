// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Security;
using System.Xml.XPath;
using Microsoft.HealthVault.Record;

namespace Microsoft.HealthVault.Person
{
    /// <summary>
    /// Provides information about a person who has access to a HealthVault record.
    /// </summary>
    ///
    public class ActiveRecordAuthorization : RecordAuthorization
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
        internal override void ParseXml(XPathNavigator navigator)
        {
            base.ParseXml(navigator);

            this.Email = navigator.SelectSingleNode("contact-email").Value;

            this.RecordAuthorizationState = AuthorizedRecordState.Active;

            this.Name = navigator.SelectSingleNode("name").Value;
        }

        #region public properties

        /// <summary>
        /// Gets the person's name.
        /// </summary>
        ///
        /// <value>
        /// The person's full name as it was entered into HealthVault.
        /// </value>
        ///
        public string Name { get; private set; }

        #endregion public properties
    }
}
