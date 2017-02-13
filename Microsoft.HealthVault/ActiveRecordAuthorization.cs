// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Security;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Provides information about a person who has access to a HealthVault record.
    /// </summary>
    /// 
    public class ActiveRecordAuthorization : RecordAuthorization
    {
        /// <summary>
        /// Constructs an instance of <see cref="ActiveRecordAuthorization"/> with default values.
        /// </summary>
        /// 
        public ActiveRecordAuthorization()
        {
        }

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

            Email = navigator.SelectSingleNode("contact-email").Value;

            RecordAuthorizationState = AuthorizedRecordState.Active;

            _name = navigator.SelectSingleNode("name").Value;
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
        public string Name
        {
            get
            {
                return _name;
            }
        }
        private string _name;

        #endregion public properties

    }
}
