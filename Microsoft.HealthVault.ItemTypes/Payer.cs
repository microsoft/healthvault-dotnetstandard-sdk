// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates information for
    /// one who pays for health and/or medical expenses.
    /// </summary>
    ///
    public class Payer : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Payer"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Payer()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Payer"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="planName">
        /// The name of the plan that pays for health and/or medical expenses.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="planName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public Payer(string planName)
            : base(TypeId)
        {
            this.PlanName = planName;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("9366440c-ec81-4b89-b231-308a4c4d70ed");

        /// <summary>
        /// Populates this <see cref="Payer"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the payer data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> parameter is not
        /// a payer node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator payerNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("payer");

            Validator.ThrowInvalidIfNull(payerNav, "PayerUnexpectedNode");

            this.planName = payerNav.SelectSingleNode("plan-name").Value;

            this.coverageType =
                XPathHelper.GetOptNavValue<CodableValue>(payerNav, "coverage-type");

            XPathNavigator carrierIdNav =
                payerNav.SelectSingleNode("carrier-id");
            if (carrierIdNav != null)
            {
                this.carrierId = carrierIdNav.Value;
            }

            XPathNavigator groupNumNav =
                payerNav.SelectSingleNode("group-num");
            if (groupNumNav != null)
            {
                this.groupNumber = groupNumNav.Value;
            }

            XPathNavigator planCodeNav =
                payerNav.SelectSingleNode("plan-code");
            if (planCodeNav != null)
            {
                this.planCode = planCodeNav.Value;
            }

            XPathNavigator subscriberIdNav =
                payerNav.SelectSingleNode("subscriber-id");
            if (subscriberIdNav != null)
            {
                this.subscriberId = subscriberIdNav.Value;
            }

            XPathNavigator personCodeNav =
                payerNav.SelectSingleNode("person-code");
            if (personCodeNav != null)
            {
                this.personCode = personCodeNav.Value;
            }

            XPathNavigator subscriberNameNav =
                payerNav.SelectSingleNode("subscriber-name");
            if (subscriberNameNav != null)
            {
                this.subscriberName = subscriberNameNav.Value;
            }

            XPathNavigator subscriberDobNav =
                payerNav.SelectSingleNode("subscriber-dob");
            if (subscriberDobNav != null)
            {
                this.subscriberDateOfBirth = new HealthServiceDateTime();
                this.subscriberDateOfBirth.ParseXml(subscriberDobNav);
            }

            XPathNavigator isPrimaryNav =
                payerNav.SelectSingleNode("is-primary");
            if (isPrimaryNav != null)
            {
                this.isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            XPathNavigator expirationDateNav =
                payerNav.SelectSingleNode("expiration-date");
            if (expirationDateNav != null)
            {
                this.expirationDate = new HealthServiceDateTime();
                this.expirationDate.ParseXml(expirationDateNav);
            }

            XPathNavigator contactNav =
                payerNav.SelectSingleNode("contact");
            if (contactNav != null)
            {
                this.contact = new ContactInfo();
                this.contact.ParseXml(contactNav);
            }
        }

        /// <summary>
        /// Writes the payer data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the payer data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="PlanName"/> parameter has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(
                this.planName,
                "PayerPlanNameNotSet");

            // <payer>
            writer.WriteStartElement("payer");

            writer.WriteElementString("plan-name", this.planName);

            // <coverage-type>
            XmlWriterHelper.WriteOpt(
                writer,
                "coverage-type",
                this.coverageType);

            if (!string.IsNullOrEmpty(this.carrierId))
            {
                writer.WriteElementString("carrier-id", this.carrierId);
            }

            if (!string.IsNullOrEmpty(this.groupNumber))
            {
                writer.WriteElementString("group-num", this.groupNumber);
            }

            if (!string.IsNullOrEmpty(this.planCode))
            {
                writer.WriteElementString("plan-code", this.planCode);
            }

            if (!string.IsNullOrEmpty(this.subscriberId))
            {
                writer.WriteElementString("subscriber-id", this.subscriberId);
            }

            if (!string.IsNullOrEmpty(this.personCode))
            {
                writer.WriteElementString("person-code", this.personCode);
            }

            if (!string.IsNullOrEmpty(this.subscriberName))
            {
                writer.WriteElementString("subscriber-name", this.subscriberName);
            }

            if (this.subscriberDateOfBirth != null)
            {
                this.subscriberDateOfBirth.WriteXml("subscriber-dob", writer);
            }

            if (this.isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary",
                    SDKHelper.XmlFromBool((bool)this.isPrimary));
            }

            if (this.expirationDate != null)
            {
                this.expirationDate.WriteXml("expiration-date", writer);
            }

            if (this.contact != null)
            {
                this.contact.WriteXml("contact", writer);
            }

            // </payer>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the plan name.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the name.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace when set.
        /// </exception>
        ///
        public string PlanName
        {
            get { return this.planName; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "PlanName");
                Validator.ThrowIfStringIsWhitespace(value, "PlanName");
                this.planName = value;
            }
        }

        private string planName;

        /// <summary>
        /// Gets or sets the coverage type, such as medical, dental, and so on.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the type.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the coverage type should not be stored.
        /// </remarks>
        ///
        public CodableValue CoverageType
        {
            get { return this.coverageType; }
            set { this.coverageType = value; }
        }

        private CodableValue coverageType;

        /// <summary>
        /// Gets or sets the carrier identifier.
        /// </summary>
        /// <returns>
        /// A string representing the identifier.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the carrier identifier should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string CarrierId
        {
            get { return this.carrierId; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "CarrierId");
                this.carrierId = value;
            }
        }

        private string carrierId;

        /// <summary>
        /// Gets or sets the group number.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the group number.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the group number should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string GroupNumber
        {
            get { return this.groupNumber; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "GroupNumber");
                this.groupNumber = value;
            }
        }

        private string groupNumber;

        /// <summary>
        /// Gets or sets the plan code.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the plan code.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the plan code should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PlanCode
        {
            get { return this.planCode; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PlanCode");
                this.planCode = value;
            }
        }

        private string planCode;

        /// <summary>
        /// Gets or sets the subscriber identifier.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the identifier.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the subscriber identifier should
        /// not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string SubscriberId
        {
            get { return this.subscriberId; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SubscriberId");
                this.subscriberId = value;
            }
        }

        private string subscriberId;

        /// <summary>
        /// Gets or sets the person code.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the code.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the person code should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PersonCode
        {
            get { return this.personCode; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PersonCode");
                this.personCode = value;
            }
        }

        private string personCode;

        /// <summary>
        /// Gets or sets the subscriber name.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the name.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the subscriber name should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string SubscriberName
        {
            get { return this.subscriberName; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SubscriberName");
                this.subscriberName = value;
            }
        }

        private string subscriberName;

        /// <summary>
        /// Gets or sets the subscriber's date of birth.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the subscriber's date of birth should not
        /// be stored.
        /// </remarks>
        ///
        public HealthServiceDateTime SubscriberDateOfBirth
        {
            get { return this.subscriberDateOfBirth; }
            set { this.subscriberDateOfBirth = value; }
        }

        private HealthServiceDateTime subscriberDateOfBirth;

        /// <summary>
        /// Gets or sets a value indicating whether this is the primary coverage for the
        /// person.
        /// </summary>
        ///
        /// <returns>
        /// <b>true</b> if this is primary coverage; otherwise, <b>false</b>.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the is primary should not be stored.
        /// </remarks>
        ///
        public bool? IsPrimary
        {
            get { return this.isPrimary; }
            set { this.isPrimary = value; }
        }

        private bool? isPrimary;

        /// <summary>
        /// Gets or sets the date the coverage expires.
        /// </summary>
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the expiration date should not
        /// be stored.
        /// </remarks>
        ///
        public HealthServiceDateTime ExpirationDate
        {
            get { return this.expirationDate; }
            set { this.expirationDate = value; }
        }

        private HealthServiceDateTime expirationDate;

        /// <summary>
        /// Gets or sets the payer contact information.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="ContactInfo"/> instance representing the information.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the payer contact information
        /// should not be stored.
        /// </remarks>
        ///
        public ContactInfo Contact
        {
            get { return this.contact; }
            set { this.contact = value; }
        }

        private ContactInfo contact;

        /// <summary>
        /// Gets a string representation of the payer item.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the payer item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = this.PlanName;

            if (this.CoverageType != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PayerToStringFormat"),
                        this.PlanName,
                        this.CoverageType.Text);
            }

            return result;
        }
    }
}
