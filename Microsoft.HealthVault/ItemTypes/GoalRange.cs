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
    /// This represents a goal range associated with a goal.
    /// </summary>
    ///
    public class GoalRange : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GoalRange"/> class with default values.
        /// </summary>
        ///
        public GoalRange()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GoalRange"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the goal range.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public GoalRange(CodableValue name)
        {
            Name = name;
        }

        /// <summary>
        /// Populates this <see cref="GoalRange"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the GoalRange data from.
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
                    nameof(navigator),
                    Resources.ParseXmlNavNull);
            }

            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("name"));
            _description = XPathHelper.GetOptNavValue(navigator, "description");
            _minimum = XPathHelper.GetOptNavValue<GeneralMeasurement>(navigator, "minimum");
            _maximum = XPathHelper.GetOptNavValue<GeneralMeasurement>(navigator, "maximum");
        }

        /// <summary>
        /// Writes the XML representation of the GoalRange into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the GoalRange should be
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
                    nameof(nodeName));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(
                    nameof(writer),
                    Resources.WriteXmlNullWriter);
            }

            if (_name == null)
            {
                throw new ThingSerializationException(
                    Resources.GoalRangeNameNullValue);
            }

            writer.WriteStartElement(nodeName);

            _name.WriteXml("name", writer);
            XmlWriterHelper.WriteOptString(writer, "description", _description);
            XmlWriterHelper.WriteOpt(writer, "minimum", _minimum);
            XmlWriterHelper.WriteOpt(writer, "maximum", _maximum);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the goal range.
        /// </summary>
        ///
        /// <remarks>
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
                        nameof(value),
                        Resources.GoalRangeNameNullValue);
                }

                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Gets or sets a description for the goal range allows more detailed information about the range.
        /// </summary>
        ///
        /// <remarks>
        /// This information could for instance be included in tooltips when hovering over a graph.
        /// If there is no information about description the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(Resources.WhitespaceOnlyValue, nameof(value));
                }

                _description = value;
            }
        }

        private string _description;

        /// <summary>
        /// Gets or sets minimum value of the range.
        /// </summary>
        ///
        /// <remarks>
        /// For ranges greater than a specified value with no maximum, specify a minimum but no maximum.
        /// </remarks>
        ///
        public GeneralMeasurement Minimum
        {
            get
            {
                return _minimum;
            }

            set
            {
                _minimum = value;
            }
        }

        private GeneralMeasurement _minimum;

        /// <summary>
        /// Gets or sets maximum value of the range.
        /// </summary>
        ///
        /// <remarks>
        /// For ranges less than a specified value with no minimum, specify a maximum but no minimum.
        /// </remarks>
        ///
        public GeneralMeasurement Maximum
        {
            get
            {
                return _maximum;
            }

            set
            {
                _maximum = value;
            }
        }

        private GeneralMeasurement _maximum;

        /// <summary>
        /// Gets a string representation of the GoalRange.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the GoalRange.
        /// </returns>
        ///
        public override string ToString()
        {
            if (Minimum != null && Maximum != null)
            {
                return
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.GoalRangeWithMinAndMaxFormat,
                        Minimum.ToString(),
                        Maximum.ToString());
            }

            if (Minimum != null)
            {
                return
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.GoalRangeWithMinFormat,
                        Minimum.ToString());
            }

            if (Maximum != null)
            {
                return
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.GoalRangeWithMaxFormat,
                        Maximum.ToString());
            }

            return Name.Text;
        }
    }
}
