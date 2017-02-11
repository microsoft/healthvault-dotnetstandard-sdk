// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// Represents data that is common across all event notification.
    /// </summary>
    public class CommonNotificationData
    {
        /// <summary>
        /// Creates an empty instance of the CommonNotificationData class. 
        /// </summary>
        public CommonNotificationData()
        {
        }

        /// <summary>
        /// Creates an instance of the CommonNotificationData class and sets
        /// the unique identifier for the subscription.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The <paramref name="subscriptionId"/> is equal to Guid.Empty.
        /// </exception>
        /// 
        public CommonNotificationData(Guid subscriptionId)
        {
            Validator.ThrowArgumentExceptionIf(subscriptionId == Guid.Empty, "subscriptionId", "EmptyGuid");

            _subscriptionId = subscriptionId;
        }

        /// <summary>
        /// Populate the CommonNotificationData instance from XML.
        /// </summary>
        /// <param name="navigator">The XPathNavigator.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="navigator"/> parameter does not contain a common node or a subscription-id node.
        /// </exception>

        public void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator commonNav = navigator.SelectSingleNode("common");
            Validator.ThrowArgumentExceptionIf(commonNav == null, "navigator", "MissingCommonNode");

            XPathNavigator idNav = commonNav.SelectSingleNode("subscription-id");
            Validator.ThrowArgumentExceptionIf(idNav == null, "navigator", "MissingSubscriptionIdNode");

            _subscriptionId = new Guid(idNav.Value);
        }

        /// <summary>
        /// Gets or sets the unique identifier for the subscription.
        /// </summary>
        public Guid SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = value; }
        }
        private Guid _subscriptionId;
    }
}
