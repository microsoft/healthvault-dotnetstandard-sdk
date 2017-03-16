// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Security;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Provides information about a person who has access to a HealthVault record.
    /// </summary>
    ///
    internal class ActiveRecordAuthorization : RecordAuthorization
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
