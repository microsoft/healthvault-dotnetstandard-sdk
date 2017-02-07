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
    /// A single assessment of menstrual flow.
    /// </summary>
    public class Menstruation : HealthRecordItem
    {                        
        /// <summary>
        /// Creates a new instance of the <see cref="Menstruation"/> class with default values. 
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Menstruation()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Menstruation"/> class. 
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time of the menstrual flow.
        /// </param>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        ///
        public Menstruation(HealthServiceDateTime when)
            : base(TypeId)
        {
            Validator.ThrowIfArgumentNull(when, "When", "WhenNullValue");
            _when = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        /// <value>
        /// A GUID.
        /// </value>
        /// 
        public static new readonly Guid TypeId =
            new Guid("caff3ff3-812f-44b1-9c9f-c1af13167705");

        /// <summary>
        /// Gets or sets the date/time of the menstrual flow.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance. 
        /// The value defaults to the current year, month, and day.
        /// </value>
        /// 
        /// <remarks>
        /// Menstrual flow is generally recorded once per day.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        public HealthServiceDateTime When
        {
            get 
            { 
                return _when; 
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the amount of discharged fluid (e.g., light, medium, heavy or spotting).
        /// </summary>
        /// <remarks>
        /// The preferred vocabulary for route is "menstrual-flow-amount".
        /// </remarks>
        public CodableValue Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        private CodableValue _amount;

        /// <summary>
        /// Gets or sets the bool which indicates whether this instance represents the start of 
        /// a new menstrual cycle, e.g., the first day of a period.
        /// </summary>        
        public bool? IsNewCycle
        {
            get { return _isNewCycle; }
            set { _isNewCycle = value; }
        }

        private bool? _isNewCycle;

        /// <summary>
        /// Populates this <see cref="Menstruation"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the menstrual flow data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// an "menstrual-flow" node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("menstruation");

            Validator.ThrowInvalidIfNull(itemNav, "MenstruationUnexpectedNode");

            // when
            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // isNewCycle
            _isNewCycle = XPathHelper.GetOptNavValueAsBool(itemNav, "is-new-cycle");            

            // amount
            _amount = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "amount");
        }

        /// <summary>
        /// Writes the mensrual flow data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the menstrual flow data to. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "WhenNullValue");

            // <menstrual-flow>
            writer.WriteStartElement("menstruation");

            // <when>
            _when.WriteXml("when", writer);

            // <is-new-cycle>
            XmlWriterHelper.WriteOptBool(writer, "is-new-cycle", _isNewCycle);

            // <amount>
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "amount", _amount);

            // </menstrual-flow>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of the Menstruation item.
        /// </summary> 
        ///
        /// <returns>
        /// A string representation of the Menstruation item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (_amount == null)
            {
                return null;
            }

            return _amount.Text;
        }
    }
}