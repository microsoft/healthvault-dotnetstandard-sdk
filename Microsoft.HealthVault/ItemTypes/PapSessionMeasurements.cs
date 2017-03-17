// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Encapsulates the the details of PAP session measurements.
    /// </summary>
    /// <typeparam name="T">The type of the measurement.</typeparam>
    ///
    public class PapSessionMeasurements<T> : ItemBase
        where T : ItemBase, new()
    {
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

            this.mean = XPathHelper.GetOptNavValue<T>(navigator, "mean");
            this.median = XPathHelper.GetOptNavValue<T>(navigator, "median");
            this.maximum = XPathHelper.GetOptNavValue<T>(navigator, "maximum");
            this.percentile95th = XPathHelper.GetOptNavValue<T>(navigator, "percentile-95th");
            this.percentile90th = XPathHelper.GetOptNavValue<T>(navigator, "percentile-90th");
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

            XmlWriterHelper.WriteOpt(writer, "mean", this.mean);
            XmlWriterHelper.WriteOpt(writer, "median", this.median);
            XmlWriterHelper.WriteOpt(writer, "maximum", this.maximum);
            XmlWriterHelper.WriteOpt(writer, "percentile-95th", this.percentile95th);
            XmlWriterHelper.WriteOpt(writer, "percentile-90th", this.percentile90th);

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
            get { return this.mean; }
            set { this.mean = value; }
        }

        private T mean;

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
            get { return this.median; }
            set { this.median = value; }
        }

        private T median;

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
            get { return this.maximum; }
            set { this.maximum = value; }
        }

        private T maximum;

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
            get { return this.percentile95th; }
            set { this.percentile95th = value; }
        }

        private T percentile95th;

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
            get { return this.percentile90th; }
            set { this.percentile90th = value; }
        }

        private T percentile90th;

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

            AddStringRepresentationOfMeasurement(result, this.Mean, Resources.MeanToStringFormat);
            AddStringRepresentationOfMeasurement(result, this.Median, Resources.MedianToStringFormat);
            AddStringRepresentationOfMeasurement(result, this.Maximum, Resources.MaximumToStringFormat);
            AddStringRepresentationOfMeasurement(result, this.Percentile95th, Resources.Percentile95thToStringFormat);
            AddStringRepresentationOfMeasurement(result, this.Percentile90th, Resources.Percentile90thToStringFormat);

            return result.ToString();
        }

        private static void AddStringRepresentationOfMeasurement(StringBuilder sb, T measurement, string format)
        {
            if (measurement != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Resources.ListSeparator);
                }

                sb.AppendFormat(format, measurement.ToString());
            }
        }
    }
}
