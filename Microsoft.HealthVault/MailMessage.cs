// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Class representing common methods/data for SDK mail messages
    /// </summary>
    /// 
    internal class MailMessage
    {
        internal MailMessage(
            IList<MailRecipient> mailRecipients,
            IList<Guid> personIds,
            Nullable<Guid> recordId,
            bool addressMustBeValidated,
            string subject,
            string textBody,
            string htmlBody)
        {
            Validator.ThrowIfStringNullOrEmpty(subject, "subject");

            Validator.ThrowArgumentExceptionIf(
                String.IsNullOrEmpty(textBody) &&
                String.IsNullOrEmpty(htmlBody),
                "textBody",
                "SendInsecureMessageNoMessage");

            Validator.ThrowArgumentExceptionIf(
                !recordId.HasValue &&
                (mailRecipients == null || mailRecipients.Count < 1) &&
                (personIds == null || personIds.Count < 1),
                "mailRecipients/personIds",
                "SendInsecureMessageNoRecipient");

            Validator.ThrowArgumentExceptionIf(
                recordId.HasValue && recordId.Value == Guid.Empty,
                "recordId",
                "SendInsecureMessageEmptyRecordId");

            _mailRecipients = mailRecipients;
            _personIds = personIds;
            _recordId = recordId;
            _addressMustBeValidated = addressMustBeValidated;
            _subject = subject;
            _textBody = textBody;
            _htmlBody = htmlBody;
        }

        internal virtual int GetApproximateSize()
        {
            int approxSize =
                ((_mailRecipients != null)
                        ? _mailRecipients.Count * 128
                        : 0)
              + ((_personIds != null)
                        ? _personIds.Count * 64
                        : 0)
              + ((_recordId.HasValue)
                        ? 64
                        : 0)
              + ((_subject != null)
                        ? _subject.Length
                        : 0)
              + ((_textBody != null)
                        ? _textBody.Length
                        : 0)
              + ((_htmlBody != null)
                        ? _htmlBody.Length * 2 // html encoding space
                        : 0);

            return approxSize;
        }

        internal virtual void WriteXml(XmlWriter writer)
        {
            if (_mailRecipients != null &&
                    _mailRecipients.Count > 0)
            {
                foreach (MailRecipient recipient in _mailRecipients)
                {
                    recipient.WriteXml(writer);
                }
            }

            if (_personIds != null &&
                _personIds.Count > 0)
            {
                foreach (Guid personId in _personIds)
                {
                    writer.WriteStartElement("rcpt-person");
                    writer.WriteStartAttribute("validated");
                    writer.WriteValue(
                        SDKHelper.XmlFromBool(_addressMustBeValidated));
                    writer.WriteEndAttribute();
                    writer.WriteValue(personId.ToString());
                    writer.WriteEndElement();
                }
            }

            if (_recordId.HasValue)
            {
                writer.WriteStartElement("rcpt-record");
                writer.WriteStartAttribute("validated");
                writer.WriteValue(
                        SDKHelper.XmlFromBool(_addressMustBeValidated));
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("subject");
            writer.WriteValue(_subject);
            writer.WriteEndElement();

            if (!String.IsNullOrEmpty(_textBody))
            {
                writer.WriteStartElement("text-body");
                writer.WriteValue(_textBody);
                writer.WriteEndElement();
            }

            if (!String.IsNullOrEmpty(_htmlBody))
            {
                writer.WriteStartElement("html-body");
                writer.WriteValue(_htmlBody);
                writer.WriteEndElement();
            }
        }

        protected IList<MailRecipient> _mailRecipients;

        protected IList<Guid> _personIds;

        protected Nullable<Guid> _recordId;

        protected bool _addressMustBeValidated;

        protected string _subject;

        protected string _textBody;

        protected string _htmlBody;
    }
}
