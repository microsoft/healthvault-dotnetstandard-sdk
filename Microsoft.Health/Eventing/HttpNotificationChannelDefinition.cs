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
    /// A <see cref="NotificationChannel"/> type that uses an HTTP POST as the method
    /// of communicating notifications to the application.
    /// </summary>
    public class HttpNotificationChannel : NotificationChannel
    {
        /// <summary>
        /// Creates an instance of the HttpNotificationChannel class. 
        /// </summary>
        public HttpNotificationChannel()
        {
        }

        /// <summary>
        /// Creates an instance of the HttpNotificationChannel class and
        /// sets the URL to send the notifications to. 
        /// </summary>
        public HttpNotificationChannel(Uri notificationUrl)
        {
            _uri = notificationUrl;
        }

        internal override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator channelNav = navigator.SelectSingleNode("http-notification-channel");

            Validator.ThrowArgumentExceptionIf(channelNav == null, "navigator", "MissingHttpNotificationChannelNode");

            XPathNavigator urlNav = channelNav.SelectSingleNode("url");
            if (urlNav != null)
            {
                _uri = new Uri(urlNav.Value);
            }
        }

        internal override void WriteXml(XmlWriter xmlWriter)
        {
            Validator.ThrowInvalidIfNull(_uri, "UrlNull");

            xmlWriter.WriteStartElement("http-notification-channel");
            {
                xmlWriter.WriteElementString("url", _uri.OriginalString);
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the URL that will receive the notifications.
        /// </summary>
        public Uri Url
        {
            get { return _uri; }
            set { _uri = value; }
        }
        private Uri _uri;

        /// <summary>
        /// Returns a string representation of this class.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return _uri.OriginalString;
        }
    }
}
