// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
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
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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

            _planName = payerNav.SelectSingleNode("plan-name").Value;

            _coverageType =
                XPathHelper.GetOptNavValue<CodableValue>(payerNav, "coverage-type");

            XPathNavigator carrierIdNav =
                payerNav.SelectSingleNode("carrier-id");
            if (carrierIdNav != null)
            {
                _carrierId = carrierIdNav.Value;
            }

            XPathNavigator groupNumNav =
                payerNav.SelectSingleNode("group-num");
            if (groupNumNav != null)
            {
                _groupNumber = groupNumNav.Value;
            }

            XPathNavigator planCodeNav =
                payerNav.SelectSingleNode("plan-code");
            if (planCodeNav != null)
            {
                _planCode = planCodeNav.Value;
            }

            XPathNavigator subscriberIdNav =
                payerNav.SelectSingleNode("subscriber-id");
            if (subscriberIdNav != null)
            {
                _subscriberId = subscriberIdNav.Value;
            }

            XPathNavigator personCodeNav =
                payerNav.SelectSingleNode("person-code");
            if (personCodeNav != null)
            {
                _personCode = personCodeNav.Value;
            }

            XPathNavigator subscriberNameNav =
                payerNav.SelectSingleNode("subscriber-name");
            if (subscriberNameNav != null)
            {
                _subscriberName = subscriberNameNav.Value;
            }

            XPathNavigator subscriberDobNav =
                payerNav.SelectSingleNode("subscriber-dob");
            if (subscriberDobNav != null)
            {
                _subscriberDateOfBirth = new HealthServiceDateTime();
                _subscriberDateOfBirth.ParseXml(subscriberDobNav);
            }

            XPathNavigator isPrimaryNav =
                payerNav.SelectSingleNode("is-primary");
            if (isPrimaryNav != null)
            {
                _isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            XPathNavigator expirationDateNav =
                payerNav.SelectSingleNode("expiration-date");
            if (expirationDateNav != null)
            {
                _expirationDate = new HealthServiceDateTime();
                _expirationDate.ParseXml(expirationDateNav);
            }

            XPathNavigator contactNav =
                payerNav.SelectSingleNode("contact");
            if (contactNav != null)
            {
                _contact = new ContactInfo();
                _contact.ParseXml(contactNav);
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
                _planName, 
                "PayerPlanNameNotSet");
            
            // <payer>
            writer.WriteStartElement("payer");

            writer.WriteElementString("plan-name", _planName);

            // <coverage-type>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "coverage-type",
                _coverageType);

            if (!String.IsNullOrEmpty(_carrierId))
            {
                writer.WriteElementString("carrier-id", _carrierId);
            }

            if (!String.IsNullOrEmpty(_groupNumber))
            {
                writer.WriteElementString("group-num", _groupNumber);
            }

            if (!String.IsNullOrEmpty(_planCode))
            {
                writer.WriteElementString("plan-code", _planCode);
            }

            if (!String.IsNullOrEmpty(_subscriberId))
            {
                writer.WriteElementString("subscriber-id", _subscriberId);
            }

            if (!String.IsNullOrEmpty(_personCode))
            {
                writer.WriteElementString("person-code", _personCode);
            }

            if (!String.IsNullOrEmpty(_subscriberName))
            {
                writer.WriteElementString("subscriber-name", _subscriberName);
            }

            if (_subscriberDateOfBirth != null)
            {
                _subscriberDateOfBirth.WriteXml("subscriber-dob", writer);
            }

            if (_isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary",
                    SDKHelper.XmlFromBool((bool)_isPrimary));
            }

            if (_expirationDate != null)
            {
                _expirationDate.WriteXml("expiration-date", writer);
            }

            if (_contact != null)
            {
                _contact.WriteXml("contact", writer);
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
            get { return _planName; }
            set 
            {
                Validator.ThrowIfStringNullOrEmpty(value, "PlanName");
                Validator.ThrowIfStringIsWhitespace(value, "PlanName");
                _planName = value; 
            }
        }
        private string _planName;

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
            get { return _coverageType; }
            set { _coverageType = value; }
        }
        private CodableValue _coverageType;

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
            get { return _carrierId; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "CarrierId");
                _carrierId = value;
            }
        }
        private string _carrierId;

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
            get { return _groupNumber; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "GroupNumber");
                _groupNumber = value;
            }
        }
        private string _groupNumber;

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
            get { return _planCode; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "PlanCode");
                _planCode = value;
            }
        }
        private string _planCode;

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
            get { return _subscriberId; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "SubscriberId");
                _subscriberId = value;
            }
        }
        private string _subscriberId;

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
            get { return _personCode; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "PersonCode");
                _personCode = value;
            }
        }
        private string _personCode;

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
            get { return _subscriberName; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "SubscriberName");
                _subscriberName = value;
            }
        }
        private string _subscriberName;

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
            get { return _subscriberDateOfBirth; }
            set { _subscriberDateOfBirth  = value; }
        }
        private HealthServiceDateTime _subscriberDateOfBirth;

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
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }
        private bool? _isPrimary;

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
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }
        private HealthServiceDateTime _expirationDate;

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
            get { return _contact; }
            set { _contact = value; }
        }
        private ContactInfo _contact;

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
            string result = PlanName;

            if (CoverageType != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "PayerToStringFormat"),
                        PlanName,
                        CoverageType.Text);
            }
            return result;
        }
    }

}
