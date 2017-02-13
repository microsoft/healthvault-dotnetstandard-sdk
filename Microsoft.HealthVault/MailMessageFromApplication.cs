// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.HealthVault
{
    internal class MailMessageFromApplication : MailMessage
    {
        internal MailMessageFromApplication(
            IList<MailRecipient> mailRecipients,
            IList<Guid> personIds,
            Nullable<Guid> recordId,
            bool addressMustBeValidated,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
            : base(
                mailRecipients,
                personIds,
                recordId,
                addressMustBeValidated,
                subject,
                textBody,
                htmlBody)
        {
            Validator.ThrowIfStringNullOrEmpty(senderMailboxName, "senderMailboxName");
            Validator.ThrowIfStringNullOrEmpty(senderDisplayName, "senderDisplayName");

            _senderMailboxName = senderMailboxName;
            _senderDisplayName = senderDisplayName;
        }

        internal override int GetApproximateSize()
        {
            int approxSize =
                ((_senderMailboxName != null)
                    ? _senderMailboxName.Length
                    : 0)
              + ((_senderDisplayName != null)
                    ? _senderDisplayName.Length
                    : 0);

            return approxSize + base.GetApproximateSize();
        }

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("from-mailbox");
            writer.WriteElementString("mailbox-name", _senderMailboxName);
            writer.WriteElementString("name", _senderDisplayName);
            writer.WriteEndElement();

            base.WriteXml(writer);
        }

        private string _senderMailboxName;
        private string _senderDisplayName;
    }
}
