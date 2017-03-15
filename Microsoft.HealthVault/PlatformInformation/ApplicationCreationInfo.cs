using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Represents information to provision a newly created application instance for the SODA flow.
    /// </summary>
    public class ApplicationCreationInfo
    {
        internal static ApplicationCreationInfo Create(XPathNavigator nav)
        {
            ApplicationCreationInfo creationInfo = new ApplicationCreationInfo();

            creationInfo.ParseXml(nav);
            return creationInfo;
        }

        internal void ParseXml(XPathNavigator nav)
        {
            this.AppInstanceId = Guid.Parse(nav.SelectSingleNode("app-id").Value);
            this.SharedSecret = nav.SelectSingleNode("shared-secret").Value;
            this.AppCreationToken = nav.SelectSingleNode("app-token").Value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationCreationInfo"/> class with default values.
        /// </summary>
        public ApplicationCreationInfo()
        {
        }

        //[XmlElement("app-id", Order = 1)]
        public Guid AppInstanceId
        {
            get; set;
        }

        //[XmlElement("shared-secret", Order = 2)]
        public string SharedSecret
        {
            get; set;
        }

        //[XmlElement("app-token", Order = 3)]
        public string AppCreationToken
        {
            get; set;
        }
    }
}
