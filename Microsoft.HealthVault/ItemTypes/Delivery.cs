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

            location = XPathHelper.GetOptNavValue<Organization>(navigator, "location");
            _timeOfDelivery = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "time-of-delivery");
            _laborDuration = XPathHelper.GetOptNavValueAsDouble(navigator, "labor-duration");

            _complications.Clear();
            foreach (XPathNavigator complicationNav in navigator.Select("complications"))
            {
                CodableValue complication = new CodableValue();
                complication.ParseXml(complicationNav);
                _complications.Add(complication);
            }

            _anesthesia.Clear();
            foreach (XPathNavigator anesthesiaNav in navigator.Select("anesthesia"))
            {
                CodableValue anesthesia = new CodableValue();
                anesthesia.ParseXml(anesthesiaNav);
                _anesthesia.Add(anesthesia);
            }

            _deliveryMethod = XPathHelper.GetOptNavValue<CodableValue>(navigator, "delivery-method");
            _outcome = XPathHelper.GetOptNavValue<CodableValue>(navigator, "outcome");
            _baby = XPathHelper.GetOptNavValue<Baby>(navigator, "baby");
            _note = XPathHelper.GetOptNavValue(navigator, "note");
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

            XmlWriterHelper.WriteOpt(writer, "location", location);
            XmlWriterHelper.WriteOpt(writer, "time-of-delivery", _timeOfDelivery);
            XmlWriterHelper.WriteOptDouble(writer, "labor-duration", _laborDuration);

            foreach (CodableValue complication in _complications)
            {
                complication.WriteXml("complications", writer);
            }

            foreach (CodableValue anesthesia in _anesthesia)
            {
                anesthesia.WriteXml("anesthesia", writer);
            }

            XmlWriterHelper.WriteOpt(writer, "delivery-method", _deliveryMethod);
            XmlWriterHelper.WriteOpt(writer, "outcome", _outcome);
            XmlWriterHelper.WriteOpt(writer, "baby", _baby);
            XmlWriterHelper.WriteOptString(writer, "note", _note);

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
            get { return location; }
            set { location = value; }
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
            get { return _timeOfDelivery; }
            set { _timeOfDelivery = value; }
        }

        private ApproximateDateTime _timeOfDelivery;

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
            get { return _laborDuration; }

            set
            {
                if (value != null && value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(LaborDuration), Resources.DeliveryLaborDurationMustBePositive);
                }

                _laborDuration = value;
            }
        }

        private double? _laborDuration;

        /// <summary>
        /// Gets a collection containing any complications that occurred during labor and delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the complications the collection should be empty.
        /// The preferred vocabulary for this value is "delivery-complications".
        /// </remarks>
        ///
        public Collection<CodableValue> Complications => _complications;

        private readonly Collection<CodableValue> _complications = new Collection<CodableValue>();

        /// <summary>
        /// Gets a collection containing any anesthesia used during labor and delivery.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the anesthesia the collection should be empty.
        /// The preferred vocabulary for this value is "anesthesia-methods".
        /// </remarks>
        ///
        public Collection<CodableValue> Anesthesia => _anesthesia;

        private readonly Collection<CodableValue> _anesthesia = new Collection<CodableValue>();

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
            get { return _deliveryMethod; }
            set { _deliveryMethod = value; }
        }

        private CodableValue _deliveryMethod;

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
            get { return _outcome; }
            set { _outcome = value; }
        }

        private CodableValue _outcome;

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
            get { return _baby; }
            set { _baby = value; }
        }

        private Baby _baby;

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
            get { return _note; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Note");
                _note = value;
            }
        }

        private string _note;

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

            if (Baby?.Name != null)
            {
                result.Append(Baby.Name);
            }

            if (TimeOfDelivery != null ||
                (Baby != null && (Baby.Weight != null || Baby.Length != null)))
            {
                if (result.Length > 0)
                {
                    result.Append(" ");
                }

                result.Append(
                    Resources.OpenParen);

                if (TimeOfDelivery != null)
                {
                    result.Append(TimeOfDelivery);
                }

                if (Baby?.Weight != null)
                {
                    if (TimeOfDelivery != null)
                    {
                        result.Append(
                            Resources.ListSeparator);
                    }

                    result.Append(Baby.Weight);
                }

                if (Baby?.Length != null)
                {
                    if (TimeOfDelivery != null || (Baby?.Weight != null))
                    {
                        result.Append(
                            Resources.ListSeparator);
                    }

                    result.Append(Baby.Length);
                }

                result.Append(
                    Resources.CloseParen);
            }

            return result.ToString();
        }
    }
}
