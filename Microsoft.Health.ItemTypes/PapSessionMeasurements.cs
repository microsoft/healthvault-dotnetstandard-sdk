// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Encapsulates the the details of PAP session measurements.
    /// </summary>
    /// <typeparam name="T">The type of the measurement.</typeparam>
    ///
    public class PapSessionMeasurements<T> : HealthRecordItemData
        where T : HealthRecordItemData, new()
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PapSessionMeasurements{T}"/> class with default values.
        /// </summary>
        /// <remarks>
        /// Specialized types of the <see cref="PapSessionMeasurements{T}"/> class are used in PAP session.
        /// </remarks>
        public PapSessionMeasurements()
        {
        }

        /// <summary>
        /// Populates this <see cref="PapSessionMeasurements{T}"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the PAP session measurements data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _mean = XPathHelper.GetOptNavValue<T>(navigator, "mean");
            _median = XPathHelper.GetOptNavValue<T>(navigator, "median");
            _maximum = XPathHelper.GetOptNavValue<T>(navigator, "maximum");
            _percentile95th = XPathHelper.GetOptNavValue<T>(navigator, "percentile-95th");
            _percentile90th = XPathHelper.GetOptNavValue<T>(navigator, "percentile-90th");
        }

        /// <summary>
        /// Writes the XML representation of the PAP session measurments into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the PAP session measurements.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the PAP session measurements should be
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOpt<T>(writer, "mean", _mean);
            XmlWriterHelper.WriteOpt<T>(writer, "median", _median);
            XmlWriterHelper.WriteOpt<T>(writer, "maximum", _maximum);
            XmlWriterHelper.WriteOpt<T>(writer, "percentile-95th", _percentile95th);
            XmlWriterHelper.WriteOpt<T>(writer, "percentile-90th", _percentile90th);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the mean value that occurred during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the mean the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public T Mean
        {
            get { return _mean; }
            set { _mean = value; }
        }

        private T _mean;

        /// <summary>
        /// Gets or sets the median value that occurred during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the median the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public T Median
        {
            get { return _median; }
            set { _median = value; }
        }

        private T _median;

        /// <summary>
        /// Gets or sets the greatest value that occured during the session.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the maximum the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public T Maximum
        {
            get { return _maximum; }
            set { _maximum = value; }
        }

        private T _maximum;

        /// <summary>
        /// Gets or sets the value that was at or below this value 95% of the time.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the percentile95th the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "A valid element name in PAP session.")]
        public T Percentile95th
        {
            get { return _percentile95th; }
            set { _percentile95th = value; }
        }

        private T _percentile95th;

        /// <summary>
        /// Gets or sets the value that was at or below this value 90% of the time.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the percentile90th the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "A valid element name in PAP session.")]
        public T Percentile90th
        {
            get { return _percentile90th; }
            set { _percentile90th = value; }
        }

        private T _percentile90th;

        /// <summary>
        /// Gets a string representation of the PAP session measurements.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the PAP session measurements.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            AddStringRepresentationOfMeasurement(result, Mean, ResourceRetriever.GetResourceString("MeanToStringFormat"));
            AddStringRepresentationOfMeasurement(result, Median, ResourceRetriever.GetResourceString("MedianToStringFormat"));
            AddStringRepresentationOfMeasurement(result, Maximum, ResourceRetriever.GetResourceString("MaximumToStringFormat"));
            AddStringRepresentationOfMeasurement(result, Percentile95th, ResourceRetriever.GetResourceString("Percentile95thToStringFormat"));
            AddStringRepresentationOfMeasurement(result, Percentile90th, ResourceRetriever.GetResourceString("Percentile90thToStringFormat"));

            return result.ToString();
        }

        private static void AddStringRepresentationOfMeasurement(StringBuilder sb, T measurement, string format)
        {
            if (measurement != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(ResourceRetriever.GetResourceString("ListSeparator"));
                }

                sb.AppendFormat(format, measurement.ToString());
            }
        }
    }
}
