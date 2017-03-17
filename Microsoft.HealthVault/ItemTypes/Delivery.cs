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

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the details of the delivery of a baby.
    /// </summary>
    ///
    public class Delivery : ItemBase
    {
        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the delivery.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.location = XPathHelper.GetOptNavValue<Organization>(navigator, "location");
            this.timeOfDelivery = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "time-of-delivery");
            this.laborDuration = XPathHelper.GetOptNavValueAsDouble(navigator, "labor-duration");

            this.complications.Clear();
            foreach (XPathNavigator complicationNav in navigator.Select("complications"))
            {
                CodableValue complication = new CodableValue();
                complication.ParseXml(complicationNav);
                this.complications.Add(complication);
            }

            this.anesthesia.Clear();
            foreach (XPathNavigator anesthesiaNav in navigator.Select("anesthesia"))
            {
                CodableValue anesthesia = new CodableValue();
                anesthesia.ParseXml(anesthesiaNav);
                this.anesthesia.Add(anesthesia);
            }

            this.deliveryMethod = XPathHelper.GetOptNavValue<CodableValue>(navigator, "delivery-method");
            this.outcome = XPathHelper.GetOptNavValue<CodableValue>(navigator, "outcome");
            this.baby = XPathHelper.GetOptNavValue<Baby>(navigator, "baby");
            this.note = XPathHelper.GetOptNavValue(navigator, "note");
        }

        /// <summary>
        /// Writes the XML representation of the delivery into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the delivery.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the delivery should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOpt(writer, "location", this.location);
            XmlWriterHelper.WriteOpt(writer, "time-of-delivery", this.timeOfDelivery);
            XmlWriterHelper.WriteOptDouble(writer, "labor-duration", this.laborDuration);

            foreach (CodableValue complication in this.complications)
            {
                complication.WriteXml("complications", writer);
            }

            foreach (CodableValue anesthesia in this.anesthesia)
            {
                anesthesia.WriteXml("anesthesia", writer);
            }

            XmlWriterHelper.WriteOpt(writer, "delivery-method", this.deliveryMethod);
            XmlWriterHelper.WriteOpt(writer, "outcome", this.outcome);
            XmlWriterHelper.WriteOpt(writer, "baby", this.baby);
            XmlWriterHelper.WriteOptString(writer, "note", this.note);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the place where the delivery occurred.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the location of the delivery the value should be set
        /// to <b>null</b>.
        /// </remarks>
        ///
        public Organization Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        private Organization location;

        /// <summary>
        /// Gets or sets the date/time of the delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the time of the delivery the value should be set
        /// to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime TimeOfDelivery
        {
            get { return this.timeOfDelivery; }
            set { this.timeOfDelivery = value; }
        }

        private ApproximateDateTime timeOfDelivery;

        /// <summary>
        /// Gets or sets the duration of labor in minutes.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the labor duration the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/>is less than or equal to zero.
        /// </exception>
        ///
        public double? LaborDuration
        {
            get { return this.laborDuration; }

            set
            {
                if (value != null && value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.LaborDuration), Resources.DeliveryLaborDurationMustBePositive);
                }

                this.laborDuration = value;
            }
        }

        private double? laborDuration;

        /// <summary>
        /// Gets a collection containing any complications that occurred during labor and delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the complications the collection should be empty.
        /// The preferred vocabulary for this value is "delivery-complications".
        /// </remarks>
        ///
        public Collection<CodableValue> Complications => this.complications;

        private readonly Collection<CodableValue> complications = new Collection<CodableValue>();

        /// <summary>
        /// Gets a collection containing any anesthesia used during labor and delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the anesthesia the collection should be empty.
        /// The preferred vocabulary for this value is "anesthesia-methods".
        /// </remarks>
        ///
        public Collection<CodableValue> Anesthesia => this.anesthesia;

        private readonly Collection<CodableValue> anesthesia = new Collection<CodableValue>();

        /// <summary>
        /// Gets or sets the method of delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the delivery method the value should be set to <b>null</b>.
        /// The preferred vocabulary for this value is "delivery-methods".
        /// </remarks>
        ///
        public CodableValue DeliveryMethod
        {
            get { return this.deliveryMethod; }
            set { this.deliveryMethod = value; }
        }

        private CodableValue deliveryMethod;

        /// <summary>
        /// Gets or sets the outcome for a fetus.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the outcome the value should be set to <b>null</b>.
        /// The preferred vocabulary for this value is "pregnancy-outcomes".
        /// </remarks>
        ///
        public CodableValue Outcome
        {
            get { return this.outcome; }
            set { this.outcome = value; }
        }

        private CodableValue outcome;

        /// <summary>
        /// Gets or sets details about a baby.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the baby the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public Baby Baby
        {
            get { return this.baby; }
            set { this.baby = value; }
        }

        private Baby baby;

        /// <summary>
        /// Gets or sets additional information about the delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there are no additional notes the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Note
        {
            get { return this.note; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                this.note = value;
            }
        }

        private string note;

        /// <summary>
        /// Gets a string representation of the delivery.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the delivery.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (this.Baby?.Name != null)
            {
                result.Append(this.Baby.Name);
            }

            if (this.TimeOfDelivery != null ||
                (this.Baby != null && (this.Baby.Weight != null || this.Baby.Length != null)))
            {
                if (result.Length > 0)
                {
                    result.Append(" ");
                }

                result.Append(
                    Resources.OpenParen);

                if (this.TimeOfDelivery != null)
                {
                    result.Append(this.TimeOfDelivery);
                }

                if (this.Baby?.Weight != null)
                {
                    if (this.TimeOfDelivery != null)
                    {
                        result.Append(
                            Resources.ListSeparator);
                    }

                    result.Append(this.Baby.Weight);
                }

                if (this.Baby?.Length != null)
                {
                    if (this.TimeOfDelivery != null || (this.Baby?.Weight != null))
                    {
                        result.Append(
                            Resources.ListSeparator);
                    }

                    result.Append(this.Baby.Length);
                }

                result.Append(
                    Resources.CloseParen);
            }

            return result.ToString();
        }
    }
}
