// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.ItemTypes
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
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
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
        public static new readonly Guid TypeId =
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

            this.dueDate = XPathHelper.GetOptNavValue<ApproximateDate>(itemNav, "due-date");
            this.lastMenstrualPeriod = XPathHelper.GetOptNavValue<HealthServiceDate>(itemNav, "last-menstrual-period");
            this.conceptionMethod = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "conception-method");
            this.fetusCount = XPathHelper.GetOptNavValueAsInt(itemNav, "fetus-count");
            this.gestationalAge = XPathHelper.GetOptNavValueAsInt(itemNav, "gestational-age");

            this.delivery.Clear();
            foreach (XPathNavigator deliveryNav in itemNav.Select("delivery"))
            {
                Delivery delivery = new Delivery();
                delivery.ParseXml(deliveryNav);
                this.delivery.Add(delivery);
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

            XmlWriterHelper.WriteOpt(writer, "due-date", this.dueDate);
            XmlWriterHelper.WriteOpt(writer, "last-menstrual-period", this.lastMenstrualPeriod);
            XmlWriterHelper.WriteOpt(writer, "conception-method", this.conceptionMethod);
            XmlWriterHelper.WriteOptInt(writer, "fetus-count", this.fetusCount);
            XmlWriterHelper.WriteOptInt(writer, "gestational-age", this.gestationalAge);

            for (int index = 0; index < this.delivery.Count; ++index)
            {
                this.delivery[index].WriteXml("delivery", writer);
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
            get { return this.dueDate; }
            set { this.dueDate = value; }
        }

        private ApproximateDate dueDate;

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
            get { return this.lastMenstrualPeriod; }
            set { this.lastMenstrualPeriod = value; }
        }

        private HealthServiceDate lastMenstrualPeriod;

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
            get { return this.conceptionMethod; }
            set { this.conceptionMethod = value; }
        }

        private CodableValue conceptionMethod;

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
            get { return this.fetusCount; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0,
                    "FetusCount",
                    "PregnancyFetusCountMustBeNonNegative");
                this.fetusCount = value;
            }
        }

        private int? fetusCount;

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
            get { return this.gestationalAge; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0,
                    "GestationalAge",
                    "PregnancyGestationalAgeMustBePositive");
                this.gestationalAge = value;
            }
        }

        private int? gestationalAge;

        /// <summary>
        /// Gets a collection of the details of the resolution of each fetus.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no delivery information the collection should be empty.
        /// </remarks>
        ///
        public Collection<Delivery> Delivery => this.delivery;

        private readonly Collection<Delivery> delivery = new Collection<Delivery>();

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

            foreach (Delivery delivery in this.Delivery)
            {
                if (result.Length > 0 && delivery.Baby != null && delivery.Baby.Name != null)
                {
                    result.Append(ResourceRetriever.GetResourceString("ListSeparator"));
                }

                if (delivery.Baby != null && delivery.Baby.Name != null)
                {
                    result.Append(delivery.Baby.Name);
                }
            }

            if (result.Length == 0 && this.DueDate != null)
            {
                result.Append(this.DueDate);
            }

            return result.ToString();
        }
    }
}
