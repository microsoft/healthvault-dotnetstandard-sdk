// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// A class to make processing event notifications simpler.
    /// </summary>
    /// <remarks>
    /// To use this class, create an instance and then hook a delegate to the
    /// event the application is interested in. Then, call the <see cref="ProcessNotificationXml" />
    /// method and pass in the notication XML from the HealthVault service. The
    /// method will parse the XML, find the notifications in it, and fire the 
    /// appropriate event. 
    /// </remarks>
    public class NotificationHandler
    {

        private static XPathExpression _notificationsPath =
            XPathExpression.Compile("/wc:notifications");

        private static XPathExpression GetNotificationsXPathExpression(XPathNavigator navigator)
        {
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(navigator.NameTable);

            xmlNamespaceManager.AddNamespace("wc", "urn:com.microsoft.wc.notification");

            XPathExpression notificationsPathClone = null;
            lock (_notificationsPath)
            {
                notificationsPathClone = _notificationsPath.Clone();
            }

            notificationsPathClone.SetContext(xmlNamespaceManager);

            return notificationsPathClone;
        }

        /// <summary>
        /// Parses the notification XML from the HealthVault service and dispatches to event handlers.
        /// </summary>
        /// <param name="notificationXml">The notification XML from the HealthVault service.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notificationXml"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="notificationXml"/> does not contain a notifications node.
        /// </exception>
        /// 
        public void ProcessNotificationXml(string notificationXml)
        {
            Validator.ThrowIfArgumentNull(notificationXml, "notificationXml", "StringNull");
            
            using (XmlReader reader = SDKHelper.GetXmlReaderForXml(notificationXml, SDKHelper.XmlReaderSettings))
            {
                XPathDocument document = new XPathDocument(reader);

                XPathNavigator nav = document.CreateNavigator();

                XPathExpression notificationsExpression = GetNotificationsXPathExpression(nav);

                XPathNavigator navNotifications = nav.SelectSingleNode(notificationsExpression);
                Validator.ThrowArgumentExceptionIf(navNotifications == null, "notificationXml", "MissingNotificationsNode");

                XPathNodeIterator iterNotification = navNotifications.Select("notification");

                foreach (XPathNavigator navNotification in iterNotification)
                {
                    ProcessNotification(navNotification);
                }
            }
        }

        private void ProcessNotification(XPathNavigator navNotification)
        {
            CommonNotificationData common = new CommonNotificationData();
            common.ParseXml(navNotification);

            foreach (XPathNavigator child in navNotification.SelectChildren(XPathNodeType.Element))
            {
                switch (child.Name)
                {
                    case "record-change-notification":
                        ProcessRecordChangeNotification(common, child);
                        break;
                }
            }
        }

        private void ProcessRecordChangeNotification(
            CommonNotificationData common,
            XPathNavigator navRecordChangedNotification)
        {
            HealthRecordItemChangedEventArgs eventArgs = new HealthRecordItemChangedEventArgs();
            eventArgs.Common = common;

            XPathNavigator navPersonId = navRecordChangedNotification.SelectSingleNode("person-id");
            eventArgs.PersonId = new Guid(navPersonId.Value);

            XPathNavigator navRecordId = navRecordChangedNotification.SelectSingleNode("record-id");
            eventArgs.RecordId = new Guid(navRecordId.Value);

            XPathNodeIterator iterThings = navRecordChangedNotification.Select("things/thing");

            eventArgs.ChangedItems = new Collection<HealthRecordItemChangedItem>();

            foreach (XPathNavigator navThing in iterThings)
            {
                XPathNavigator navThingId = navThing.SelectSingleNode("thing-id");

                HealthRecordItemChangedItem item = new HealthRecordItemChangedItem();
                item.Id = new Guid(navThingId.Value);

                eventArgs.ChangedItems.Add(item);
            }

            if (RecordChanged != null)
            {
                RecordChanged(this, eventArgs);
            }
        }

        /// <summary>
        /// The record changed event.
        /// </summary>
        /// <remarks>
        /// This event is called when the <see cref="ProcessNotificationXml"/> method is called and the 
        /// XML that is passed to it contains record changed notifications.
        /// </remarks>
        public event EventHandler<HealthRecordItemChangedEventArgs> RecordChanged;
    }
}
