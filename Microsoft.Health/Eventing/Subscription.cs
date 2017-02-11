// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// The base class for all subscriptions.
    /// </summary>
    public abstract class Subscription
    {
        /// <summary>
        /// Create a subscription instance with the specified common data.
        /// </summary>
        /// <param name="commonSubscriptionData">The common data to use.</param>
        internal Subscription(CommonSubscriptionData commonSubscriptionData)
        {
            _commonItemSubscriptionData = commonSubscriptionData;
        }

        internal abstract void WriteXml(XmlWriter xmlWriter);

        internal abstract void ParseXml(XPathNavigator navigator);

        /// <summary>
        /// Create an instance of a Subscription by deserializing the subscription XML.
        /// </summary>
        /// <remarks>
        /// The returned type will be a type derived from Subscription, such as the <see cref="HealthRecordItemChangedSubscription"/> class. 
        /// </remarks>
        /// <param name="subscriptionNavigator">The xml representation to deserialize.</param>
        /// <returns>The subscription.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="subscriptionNavigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="subscriptionNavigator"/> parameter XML does not contain 
        /// a subscription node.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="subscriptionNavigator"/> parameter XML has an unrecognized 
        /// subscription type.
        /// </exception>
        public static Subscription Deserialize(XPathNavigator subscriptionNavigator)
        {
            Validator.ThrowIfNavigatorNull(subscriptionNavigator);

            // find out what kind of node we have, and go from there...
            Subscription subscription = null;
            CommonSubscriptionData commonSubscriptionData = null;
            foreach (XPathNavigator typeSpecificNav in subscriptionNavigator.SelectChildren(XPathNodeType.Element))
            {
                switch (typeSpecificNav.Name)
                {
                    case "record-item-changed-event":
                        subscription = new HealthRecordItemChangedSubscription(null);
                        subscription.ParseXml(subscriptionNavigator);
                        break;

                    case "common":
                        commonSubscriptionData = new CommonSubscriptionData();
                        commonSubscriptionData.ParseXml(subscriptionNavigator);
                        break;

                    default:
                        throw new InvalidOperationException(
                            String.Format(
                                CultureInfo.InvariantCulture,
                                ResourceRetriever.GetResourceString("UnrecognizedSubscriptionType"),
                                typeSpecificNav.Name));
                }
            }
            if (subscription != null)
            {
                subscription.CommonSubscriptionData = commonSubscriptionData;
            }

            return subscription;
        }

        /// <summary>
        /// Save the subscription to XML.
        /// </summary>
        /// <param name="xmlWriter">The XmlWriter to write the data to.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="xmlWriter"/> parameter is <b>null</b>.
        /// </exception>        
        /// 
        /// <exception cref="InvalidOperationException">
        /// The <seealso name="CommonItemSubscriptionData" /> property is <b>null</b>.
        /// </exception>
        public void Serialize(XmlWriter xmlWriter)
        {
            Validator.ThrowIfWriterNull(xmlWriter);
            Validator.ThrowInvalidIfNull(_commonItemSubscriptionData, "CommonItemSubscriptionDataRequired");

            xmlWriter.WriteStartElement("subscription");
            {
                _commonItemSubscriptionData.WriteXml(xmlWriter);
                WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the <see cref="CommonSubscriptionData"/> that is associated with this subscription.
        /// </summary>
        public CommonSubscriptionData CommonSubscriptionData
        {
            get { return _commonItemSubscriptionData; }
            set { _commonItemSubscriptionData = value; }
        }
        private CommonSubscriptionData _commonItemSubscriptionData;
    }
}
