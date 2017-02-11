// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// Represents data that is common across all event subscriptions.
    /// </summary>
    public class CommonSubscriptionData
    {
        /// <summary>
        /// Creates an empty instance of the CommonSubscriptionData class. 
        /// </summary>
        public CommonSubscriptionData()
        {
        }

        /// <summary>
        /// Creates an instance of the CommonSubscriptionData class and sets the notification channel and the
        /// shared key information.
        /// </summary>
        /// 
        /// <param name="notificationChannel">The channel to use for notifications.</param>
        /// <param name="sharedKeyInfo">The shared key to use for authentication.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notificationChannel"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="sharedKeyInfo"/> parameter is <b>null</b>.
        /// </exception>
        public CommonSubscriptionData(
                NotificationChannel notificationChannel,
                NotificationAuthenticationSharedKeyInfo sharedKeyInfo)
        {
            Validator.ThrowIfArgumentNull(notificationChannel, "notificationChannel", "NotificationChannelNull");
            Validator.ThrowIfArgumentNull(sharedKeyInfo, "sharedKeyInfo", "ArgumentNull");

            _notificationChannel = notificationChannel;
            _sharedKeyInfo = sharedKeyInfo;
        }

        /// <summary>
        /// Creates an instance of the CommonSubscriptionData class and sets the notification channel, the shared
        /// key information and the unique identifier for the subscription.
        /// </summary>
        /// 
        /// <param name="notificationChannel">The channel to use for notifications.</param>
        /// <param name="sharedKeyInfo">The shared key to use for authentication.</param>
        /// <param name="id">The id of the subscription.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notificationChannel"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="sharedKeyInfo"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="id"/> parameter is an empty GUID.
        /// </exception>
        public CommonSubscriptionData(
                NotificationChannel notificationChannel,
                NotificationAuthenticationSharedKeyInfo sharedKeyInfo,
            Guid id)
            : this(notificationChannel, sharedKeyInfo)
        {
            Validator.ThrowArgumentExceptionIf(id == Guid.Empty, "id", "EmptyGuid");

            _id = id;
        }

        /// <summary>
        /// Write a representation of the CommonSubscriptionData instance to XML.
        /// </summary>
        /// <param name="xmlWriter">The XmlWriter.</param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The <see cref="NotificationChannel"/> property is <b>null</b> or the 
        /// <see cref="SharedKeyInfo"/> property is null.
        /// </exception>
        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("common");
            {
                WriteXmlInternal(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Internal implementation of <see cref="WriteXml(XmlWriter)"/>.
        /// </summary>
        /// <param name="xmlWriter">The XmlWriter</param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="NotificationChannel"/> property is <b>null</b> or the 
        /// <see cref="SharedKeyInfo"/> property is null.
        /// </exception>
        protected virtual void WriteXmlInternal(XmlWriter xmlWriter)
        {
            if (_id != null)
            {
                xmlWriter.WriteElementString("id", _id.ToString());
            }

            Validator.ThrowInvalidIfNull(_sharedKeyInfo, "SharedKeyInfoNull");

            xmlWriter.WriteStartElement("notification-authentication-info");
            {
                _sharedKeyInfo.WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();

            Validator.ThrowInvalidIfNull(_notificationChannel, "NotificationChannelNull");

            xmlWriter.WriteStartElement("notification-channel");
            {
                _notificationChannel.WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Populate the CommonSubscriptionData instance from XML.
        /// </summary>
        /// <param name="navigator">The XPathNavigator.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="navigator"/> parameter does not contain a "common" node.
        /// </exception>

        public void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator commonNav = navigator.SelectSingleNode("common");
            Validator.ThrowArgumentExceptionIf(commonNav == null, "navigator", "MissingCommonNode");

            XPathNavigator idNav = commonNav.SelectSingleNode("id");
            if (idNav != null)
            {
                _id = new Guid(idNav.Value);
            }

            XPathNavigator notificationAuthNode = commonNav.SelectSingleNode("notification-authentication-info");
            if (notificationAuthNode != null)
            {
                XPathNavigator sharedKeyNav = notificationAuthNode.SelectSingleNode("hv-eventing-shared-key");

                _sharedKeyInfo = new NotificationAuthenticationSharedKeyInfo();
                _sharedKeyInfo.ParseXml(sharedKeyNav);
            }

            XPathNavigator channelNav = commonNav.SelectSingleNode("notification-channel");
            if (channelNav != null)
            {
                XPathNodeIterator children = channelNav.SelectChildren(XPathNodeType.Element);
                XPathNavigator child = children.Current;
                _notificationChannel = NotificationChannel.Deserialize(child);
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the subscription.
        /// </summary>
        public Guid? Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private Guid? _id;

        /// <summary>
        /// Gets or sets the notification channel that is used to deliver events from
        /// the subscription.
        /// </summary>
        public NotificationChannel NotificationChannel
        {
            get { return _notificationChannel; }
            set { _notificationChannel = value; }
        }
        private NotificationChannel _notificationChannel;

        private NotificationAuthenticationSharedKeyInfo _sharedKeyInfo;

        /// <summary>
        /// Gets or sets the information used to validate the source of notifications.
        /// </summary>
        /// <remarks>
        /// For notification channels where the HealthVault service pushes 
        /// notifications to an application's end-point (ie. an HTTP 
        /// notification channel), this information can be used by the 
        /// application to ensure the incoming notification was sent by the 
        /// HealthVault service.
        /// </remarks>
        public NotificationAuthenticationSharedKeyInfo SharedKeyInfo
        {
            get { return _sharedKeyInfo; }
            set { _sharedKeyInfo = value; }
        }
    }
}
