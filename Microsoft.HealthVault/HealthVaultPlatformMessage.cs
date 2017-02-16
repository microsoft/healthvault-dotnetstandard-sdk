// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.PlatformPrimitives
{
    /// <summary>
    /// Provides low-level access to the HealthVault message operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformMessage.Current to a derived class to intercept all message calls.
    /// </remarks>
    public class HealthVaultPlatformMessage
    {
        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        ///
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked.
        /// </remarks>
        ///
        /// <param name="mock">The mocking class.</param>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        ///
        public static void EnableMock(HealthVaultPlatformMessage mock)
        {
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        ///
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }

        internal static HealthVaultPlatformMessage Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformMessage _current = new HealthVaultPlatformMessage();
        private static HealthVaultPlatformMessage _saved;

        /// <summary>
        /// Sends an insecure message to the specified message recipients.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        ///
        /// <param name="mailRecipient">
        /// The addresses and display names of the people to send the
        /// message to.
        /// </param>
        ///
        /// <param name="personIds">
        /// The unique identifiers of the people to which the message should be
        /// sent.
        /// </param>
        ///
        /// <param name="recordId">
        /// The unique identifier of the health record for which the
        /// custodians should be sent the message.
        /// </param>
        ///
        /// <param name="addressMustBeValidated">
        /// <b>true</b> if HealthVault ensures that the person has validated
        /// their message address before sending the mail; <b>false</b> if the
        /// message will be sent even if the person's address has not been
        /// validated.
        /// </param>
        ///
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        ///
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        ///
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        ///
        /// <remarks>
        /// If both the <paramref name="textBody"/> and
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="mailRecipient"/> property is <b>null</b> or empty,
        /// or the <paramref name="subject"/> parameter is <b>null</b> or empty, or
        /// the <paramref name="textBody"/> and <paramref name="htmlBody"/>
        /// parameters are both <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The server returned a failure when making the request.
        /// </exception>
        ///
        public virtual async Task SendInsecureMessageAsync(
            HealthServiceConnection connection,
            IList<MailRecipient> mailRecipient,
            IList<Guid> personIds,
            Nullable<Guid> recordId,
            bool addressMustBeValidated,
            string subject,
            string textBody,
            string htmlBody)
        {
            //Todo verify that we have an application connection here...

            MailMessage mailMessage = new MailMessage(
                mailRecipient, personIds, recordId, addressMustBeValidated,
                subject, textBody, htmlBody);

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "SendInsecureMessage", 1);

            request.Parameters =
                GetSendInsecureMessageParameters(mailMessage);

            if (recordId.HasValue)
            {
                request.RecordId = recordId.Value;
            }

            await request.ExecuteAsync().ConfigureAwait(false);
        }

        private static string GetSendInsecureMessageParameters(
            MailMessage mailMessage)
        {
            StringBuilder result = null;
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            int approxParamLength = mailMessage.GetApproximateSize();

            result = new StringBuilder(approxParamLength + 128);

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                mailMessage.WriteXml(writer);
                writer.Flush();
            }

            return result.ToString();
        }

        /// <summary>
        /// Sends an insecure message to the specified message recipients.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use for this operation. The connection must
        /// have application capabilities.
        /// </param>
        ///
        /// <param name="mailRecipient">
        /// The addresses and display names of the people to send the
        /// message to.
        /// </param>
        ///
        /// <param name="personIds">
        /// The unique identifiers of the people to which the message should be
        /// sent.
        /// </param>
        ///
        /// <param name="recordId">
        /// The unique identifier of the health record for which the
        /// custodians should be sent the message.
        /// </param>
        ///
        /// <param name="addressMustBeValidated">
        /// <b>true</b> if HealthVault ensures that the person has validated
        /// their message address before sending the mail; <b>false</b> if the
        /// message will be sent even if the person's address has not been
        /// validated.
        /// </param>
        ///
        /// <param name="senderMailboxName">
        /// An application specified mailbox name that's sending the message.
        /// The mailbox name is appended to the application's domain name to
        /// form the From email address of the message. This parameter should
        /// only contain the characters before the @ symbol of the email
        /// address.
        /// </param>
        ///
        /// <param name="senderDisplayName">
        /// The message sender's display name.
        /// </param>
        ///
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        ///
        /// <param name="textBody">
        /// The text body of the message.
        /// </param>
        ///
        /// <param name="htmlBody">
        /// The HTML body of the message.
        /// </param>
        ///
        /// <remarks>
        /// If both the <paramref name="textBody"/> and
        /// <paramref name="htmlBody"/> of the message is specified then a
        /// multi-part message will be sent so that the html body will be used
        /// and fallback to text if not supported by the client.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="mailRecipient"/> property is <b>null</b> or empty,
        /// or the <paramref name="subject"/> parameter is <b>null</b> or empty, or
        /// the <paramref name="textBody"/> and <paramref name="htmlBody"/>
        /// parameters are both <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The server returned a failure when making the request.
        /// </exception>
        ///
        public virtual async Task SendInsecureMessageFromApplicationAsync(
            HealthServiceConnection connection,
            IList<MailRecipient> mailRecipient,
            IList<Guid> personIds,
            Nullable<Guid> recordId,
            bool addressMustBeValidated,
            string senderMailboxName,
            string senderDisplayName,
            string subject,
            string textBody,
            string htmlBody)
        {
            MailMessageFromApplication mailMessage =
                new MailMessageFromApplication(
                    mailRecipient, personIds, recordId,
                    addressMustBeValidated, senderMailboxName,
                    senderDisplayName, subject, textBody, htmlBody);

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "SendInsecureMessageFromApplication", 1);

            request.Parameters =
                GetSendInsecureMessageFromApplicationParameters(mailMessage);

            if (recordId.HasValue)
            {
                request.RecordId = recordId.Value;
            }

            await request.ExecuteAsync().ConfigureAwait(false);
        }

        private static string GetSendInsecureMessageFromApplicationParameters(
            MailMessageFromApplication mailMessage)
        {
            StringBuilder result = null;
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            int approxParamLength = mailMessage.GetApproximateSize();

            result = new StringBuilder(approxParamLength + 128);

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                mailMessage.WriteXml(writer);
                writer.Flush();
            }

            return result.ToString();
        }
    }
}
