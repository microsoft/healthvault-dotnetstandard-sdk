// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a medication thing.
    /// </summary>
    ///
    public class Medication : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Medication"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Medication()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Medication"/> class with the specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the medication.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public Medication(CodableValue name)
            : base(TypeId)
        {
            Name = name;
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("30cafccc-047d-4288-94ef-643571f7919d");

        /// <summary>
        /// Populates this medication instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the medication data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a medication node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("medication");

            Validator.ThrowInvalidIfNull(itemNav, Resources.MedicationUnexpectedNode);

            _name = new CodableValue();
            _name.ParseXml(itemNav.SelectSingleNode("name"));

            _genericName =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "generic-name");

            _dose =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "dose");

            _strength =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "strength");

            _frequency =
                XPathHelper.GetOptNavValue<GeneralMeasurement>(itemNav, "frequency");

            _route =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "route");

            _indication =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "indication");

            _dateStarted =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-started");

            _dateDiscontinued =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "date-discontinued");

            _prescribed =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "prescribed");

            _prescription =
                XPathHelper.GetOptNavValue<Prescription>(itemNav, "prescription");
        }

        /// <summary>
        /// Writes the medication data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the medication data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Name"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, Resources.MedicationNameNotSet);

            // <medication>
            writer.WriteStartElement("medication");

            // <name>
            _name.WriteXml("name", writer);

            // <generic-name>
            XmlWriterHelper.WriteOpt(
                writer,
                "generic-name",
                _genericName);

            // <dose>
            XmlWriterHelper.WriteOpt(
                writer,
                "dose",
                _dose);

            // <strength>
            XmlWriterHelper.WriteOpt(
                writer,
                "strength",
                _strength);

            // <frequency>
            XmlWriterHelper.WriteOpt(
                writer,
                "frequency",
                _frequency);

            // <route>
            XmlWriterHelper.WriteOpt(
                writer,
                "route",
                _route);

            // <indication>
            XmlWriterHelper.WriteOpt(
                writer,
                "indication",
                _indication);

            // <date-started>
            XmlWriterHelper.WriteOpt(
                writer,
                "date-started",
                _dateStarted);

            // <date-discontinued>
            XmlWriterHelper.WriteOpt(
                writer,
                "date-discontinued",
                _dateDiscontinued);

            // <prescribed>
            XmlWriterHelper.WriteOpt(
                writer,
                "prescribed",
                _prescribed);

            // <prescription>
            XmlWriterHelper.WriteOpt(
                writer,
                "prescription",
                _prescription);

            // </medication>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the medication name and clinical code.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b> on set.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.MedicationNameMandatory);
                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Gets or sets the generic name of the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue GenericName
        {
            get { return _genericName; }
            set { _genericName = value; }
        }

        private CodableValue _genericName;

        /// <summary>
        /// Gets or sets the dose of the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// Examples: 1 tablet, 50 ml.
        /// </remarks>
        ///
        public GeneralMeasurement Dose
        {
            get { return _dose; }
            set { _dose = value; }
        }

        private GeneralMeasurement _dose;

        /// <summary>
        /// Gets or sets the strength of the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// Examples: 500 mg, 10 mg/ml.
        /// </remarks>
        ///
        public GeneralMeasurement Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }

        private GeneralMeasurement _strength;

        /// <summary>
        /// Gets or sets how often the medication is taken.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// Examples: 1 tablet per day, 2 every 6 hours, as needed.
        /// </remarks>
        ///
        public GeneralMeasurement Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        private GeneralMeasurement _frequency;

        /// <summary>
        /// Gets or sets the route by which the medication is administered.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// The preferred vocabulary for route is "medication-routes".
        /// </remarks>
        ///
        public CodableValue Route
        {
            get { return _route; }
            set { _route = value; }
        }

        private CodableValue _route;

        /// <summary>
        /// Gets or sets the indication for the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue Indication
        {
            get { return _indication; }
            set { _indication = value; }
        }

        private CodableValue _indication;

        /// <summary>
        /// Gets or sets the date on which the person started taken the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime DateStarted
        {
            get { return _dateStarted; }
            set { _dateStarted = value; }
        }

        private ApproximateDateTime _dateStarted;

        /// <summary>
        /// Gets or sets the date on which the medication was discontinued.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime DateDiscontinued
        {
            get { return _dateDiscontinued; }
            set { _dateDiscontinued = value; }
        }

        private ApproximateDateTime _dateDiscontinued;

        /// <summary>
        /// Gets or sets the source of the prescription.
        /// </summary>
        ///
        /// <remarks>
        /// A medication that is prescribed by a physician should code "prescribed"
        /// into this element.
        /// If the value is not known, it will be set to <b>null</b>.
        /// The preferred vocabulary for prescribed is "medication-prescribed".
        /// </remarks>
        ///
        public CodableValue Prescribed
        {
            get { return _prescribed; }
            set { _prescribed = value; }
        }

        private CodableValue _prescribed;

        /// <summary>
        /// Gets or sets the prescription for the medication.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        ///
        public Prescription Prescription
        {
            get { return _prescription; }
            set { _prescription = value; }
        }

        private Prescription _prescription;

        /// <summary>
        /// Gets a string representation of the medication.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the medication.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(Name);

            if (GenericName != null)
            {
                result.Append(" ");
                result.AppendFormat(
                    Resources.MedicationToStringFormatGenericName,
                    GenericName.ToString());
            }

            if (Strength != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    Strength.ToString());
            }

            if (Dose != null)
            {
                result.AppendFormat(
                    Resources.ListFormat,
                    Dose.ToString());
            }

            if (Frequency != null)
            {
                result.Append(" ");
                result.Append(Frequency);
            }

            return result.ToString();
        }
    }
}
