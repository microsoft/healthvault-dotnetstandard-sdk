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
    /// The base class for all notification channel types.
    /// </summary>
    /// <remarks>
    /// A notification channel defines how event notifications are delivered.
    /// to the application.
    /// </remarks>
    public abstract class NotificationChannel
    {
        internal NotificationChannel()
        {
        }

        internal virtual void WriteXml(XmlWriter xmlWriter)
        {
        }

        internal virtual void ParseXml(XPathNavigator navigator)
        {
        }

        /// <summary>
        /// Creates a notification channel from the XML representation.
        /// </summary>
        /// <param name="navigator">The navigator to read the information from. </param>
        /// <returns>The notification channel.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The notification channel defined in the XML is not recognized.
        /// </exception>
        public static NotificationChannel Deserialize(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // find out what kind of node we have, and go from there...
            NotificationChannel channel = null;
            foreach (XPathNavigator typeSpecificNav in navigator.SelectChildren(XPathNodeType.Element))
            {
                switch (typeSpecificNav.Name)
                {
                    case "http-notification-channel":
                        channel = new HttpNotificationChannel();
                        channel.ParseXml(navigator);
                        break;

                    default:
                        throw new InvalidOperationException(
                            String.Format(
                                CultureInfo.InvariantCulture,
                                ResourceRetriever.GetResourceString("UnrecognizedNotificationChannelType"),
                                typeSpecificNav.Name));
                }
            }

            return channel;
        }
    }
}
