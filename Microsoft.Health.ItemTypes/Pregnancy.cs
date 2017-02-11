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
    /// Record of a pregnancy.
    /// </summary>
    /// 
    public class Pregnancy : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Pregnancy"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Pregnancy()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        /// <value>
        /// A GUID.
        /// </value>
        /// 
        public new static readonly Guid TypeId =
            new Guid("46d485cf-2b84-429d-9159-83152ba801f4");

        /// <summary>
        /// Information related to a pregnancy.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the pregnancy data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a pregnancy node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("pregnancy");

            Validator.ThrowInvalidIfNull(itemNav, "PregnancyUnexpectedNode");

            _dueDate = XPathHelper.GetOptNavValue<ApproximateDate>(itemNav, "due-date");
            _lastMenstrualPeriod = XPathHelper.GetOptNavValue<HealthServiceDate>(itemNav, "last-menstrual-period");
            _conceptionMethod = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "conception-method");
            _fetusCount = XPathHelper.GetOptNavValueAsInt(itemNav, "fetus-count");
            _gestationalAge = XPathHelper.GetOptNavValueAsInt(itemNav, "gestational-age");

            _delivery.Clear();
            foreach (XPathNavigator deliveryNav in itemNav.Select("delivery"))
            {
                Delivery delivery = new Delivery();
                delivery.ParseXml(deliveryNav);
                _delivery.Add(delivery);
            }
        }

        /// <summary>
        /// Writes the pregnancy data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the pregnancy data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <pregnancy>
            writer.WriteStartElement("pregnancy");

            XmlWriterHelper.WriteOpt<ApproximateDate>(writer, "due-date", _dueDate);
            XmlWriterHelper.WriteOpt<HealthServiceDate>(writer, "last-menstrual-period", _lastMenstrualPeriod);
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "conception-method", _conceptionMethod);
            XmlWriterHelper.WriteOptInt(writer, "fetus-count", _fetusCount);
            XmlWriterHelper.WriteOptInt(writer, "gestational-age", _gestationalAge);

            for (int index = 0; index < _delivery.Count; ++index)
            {
                _delivery[index].WriteXml("delivery", writer);
            }

            // </pregnancy>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the approximate due date.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no due date the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public ApproximateDate DueDate
        {
            get { return _dueDate; }
            set { _dueDate = value; }
        }
        private ApproximateDate _dueDate;

        /// <summary>
        /// Gets or sets the first day of the last menstrual cycle.
        /// </summary>
        /// 
        /// <remarks>
        /// If the last menstrual period is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public HealthServiceDate LastMenstrualPeriod
        {
            get { return _lastMenstrualPeriod; }
            set { _lastMenstrualPeriod = value; }
        }
        private HealthServiceDate _lastMenstrualPeriod;

        /// <summary>
        /// Gets or sets the method of conception.
        /// </summary>
        /// 
        /// <remarks>
        /// If the conception method is not known the value should be set to <b>null</b>.
        /// The preferred vocabulary for this value is "conception-methods". 
        /// </remarks>
        /// 
        public CodableValue ConceptionMethod
        {
            get { return _conceptionMethod; }
            set { _conceptionMethod = value; }
        }
        private CodableValue _conceptionMethod;

        /// <summary>
        /// Gets or sets the number of fetuses.
        /// </summary>
        /// 
        /// <remarks>
        /// If the fetus count is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public int? FetusCount
        {
            get { return _fetusCount; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0,
                    "FetusCount",
                    "PregnancyFetusCountMustBeNonNegative");
                _fetusCount = value;
            }
        }
        private int? _fetusCount;

        /// <summary>
        /// Gets or sets the number of weeks of pregnancy at the time of delivery.
        /// </summary>
        /// 
        /// <remarks>
        /// If the gestational age is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// 
        public int? GestationalAge
        {
            get { return _gestationalAge; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0,
                    "GestationalAge",
                    "PregnancyGestationalAgeMustBePositive");
                _gestationalAge = value; 
            }
        }
        private int? _gestationalAge;

        /// <summary>
        /// Gets a collection of the details of the resolution of each fetus.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no delivery information the collection should be empty.
        /// </remarks>
        /// 
        public Collection<Delivery> Delivery
        {
            get { return _delivery; }
        }
        private Collection<Delivery> _delivery = new Collection<Delivery>();

                /// <summary>
        /// Gets a string representation of the pregnancy item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the pregnancy item.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            foreach (Delivery delivery in Delivery)
            {
                if (result.Length > 0 && delivery.Baby != null && delivery.Baby.Name != null)
                {
                    result.Append(ResourceRetriever.GetResourceString("ListSeparator"));
                }

                if (delivery.Baby != null && delivery.Baby.Name != null)
                {
                    result.Append(delivery.Baby.Name.ToString());
                }
            }

            if (result.Length == 0 && DueDate != null)
            {
                result.Append(DueDate.ToString());
            }
            return result.ToString();
        }
    }
}
