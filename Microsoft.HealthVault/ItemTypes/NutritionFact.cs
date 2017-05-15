// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information related to a nutrient consumed.
    /// </summary>
    ///
    public class NutritionFact : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NutritionFact"/> class with default values.
        /// </summary>
        ///
        public NutritionFact()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NutritionFact"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the nutrient consumed.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public NutritionFact(CodableValue name)
        {
            Name = name;
        }

        /// <summary>
        /// Populates this <see cref="NutritionFact"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the NutritionFact data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException(
                    "navigator",
                    Resources.ParseXmlNavNull);
            }

            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("name"));
            _fact = XPathHelper.GetOptNavValue<GeneralMeasurement>(navigator, "fact");
        }

        /// <summary>
        /// Writes the XML representation of the NutritionFact into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the NutritionFact should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentException(
                    Resources.WriteXmlEmptyNodeName,
                    "nodeName");
            }

            if (writer == null)
            {
                throw new ArgumentNullException(
                    "writer",
                    Resources.WriteXmlNullWriter);
            }

            if (_name == null)
            {
                throw new ThingSerializationException(
                    Resources.NutrientNameNullValue);
            }

            writer.WriteStartElement(nodeName);

            _name.WriteXml("name", writer);
            XmlWriterHelper.WriteOpt(writer, "fact", _fact);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the nutrient consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Example: calcium.
        /// If there is no information about name the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value",
                        Resources.NutrientNameNullValue);
                }

                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Gets or sets the amount of nutrient consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Examples include 30 cc, 500 mg, 15 liters, 30 inches, etc.
        /// If there is no information about fact the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public GeneralMeasurement Fact
        {
            get
            {
                return _fact;
            }

            set
            {
                _fact = value;
            }
        }

        private GeneralMeasurement _fact;

        /// <summary>
        /// Gets a string representation of the NutritionFact.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the NutritionFact.
        /// </returns>
        ///
        public override string ToString()
        {
            if (Fact != null)
            {
                return string.Format(
                            CultureInfo.CurrentUICulture,
                            Resources.NutritionValueFormat,
                            Name.Text,
                            Fact.ToString());
            }

            return Name.Text;
        }
    }
}
