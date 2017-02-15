// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Text;
using System.Xml;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents an email address that can be used when calling
    /// SendInsecureMessageFromApplication.
    /// </summary>
    ///
    /// <remarks>
    /// The <see cref="MailRecipient"/> class contains the email address and
    /// the display name of the person to whom the mail is being sent. It
    /// provides the methods to get the XML representation that adheres to the
    /// schema for the SendInsecureMessageFromApplication method.
    /// </remarks>
    ///
    public class MailRecipient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MailRecipient"/> class
        /// with the specified email address and display name.
        /// </summary>
        ///
        /// <param name="emailAddress">
        /// The email address of the person who is to receive the email.
        /// </param>
        ///
        /// <param name="displayName">
        /// The display name of the person who is to receive the email.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="emailAddress"/> or
        /// <paramref name="displayName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <remarks>
        /// </remarks>
        ///
        public MailRecipient(
            string emailAddress,
            string displayName)
        {
            Validator.ThrowIfStringNullOrEmpty(emailAddress, "emailAddress");
            Validator.ThrowIfStringNullOrEmpty(displayName, "displayName");

            _emailAddress = emailAddress;
            _displayName = displayName;
        }

        /// <summary>
        /// Gets or sets the email address of the person who receives the email.
        /// </summary>
        ///
        /// <value>
        /// A string representing the email address who receives the email.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The value is <b>null</b> or empty.
        /// </exception>
        ///
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "EmailAddress");
                _emailAddress = value;
            }
        }
        private string _emailAddress;

        /// <summary>
        /// Gets or sets the display name of the person to which the email
        /// will be sent.
        /// </summary>
        ///
        /// <value>
        /// The display name of the person to which the message will be sent.
        /// This gets displayed in the To field of the message.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The value is <b>null</b> or empty.
        /// </exception>
        ///
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "displayName");
                _displayName = value;
            }
        }
        private string _displayName;

        /// <summary>
        /// Writes the XML representation of the email recipient into the
        /// specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer that receives the XML representation of the email
        /// recipient.
        /// </param>
        ///
        /// <remarks>
        /// The XML written by this method adheres to the schema for the
        /// SendInsecureMessageFromApplication method.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement("rcpt-address");

            writer.WriteStartElement("address");
            writer.WriteValue(EmailAddress);
            writer.WriteEndElement();

            writer.WriteStartElement("name");
            writer.WriteValue(DisplayName);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets the XML representation of the email recipient as a string.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the XML representation of the email recipient.
        /// </returns>
        ///
        /// <remarks>
        /// The XML written by this method adheres to the schema for the
        /// SendInsecureMessageFromApplication method.
        /// </remarks>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(128);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                WriteXml(writer);
                writer.Flush();
            }

            return result.ToString();
        }
    }
}
