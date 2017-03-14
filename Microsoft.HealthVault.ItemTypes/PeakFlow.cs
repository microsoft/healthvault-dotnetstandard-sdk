// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing that encapsulates a single peak flow reading.
    /// </summary>
    ///
    /// <remarks>
    /// Peak flow measures are typically collected on a daily basis by patients to track their
    /// lung function.
    /// </remarks>
    ///
    public class PeakFlow : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PeakFlow"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public PeakFlow()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PeakFlow"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time when the peak flow reading occurred.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public PeakFlow(ApproximateDateTime when)
            : base(TypeId)
        {
            this.When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("5d8419af-90f0-4875-a370-0f881c18f6b3");

        /// <summary>
        /// Populates this <see cref="PeakFlow"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the peack flow data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in the <paramref name="typeSpecificXml"/> parameter
        /// is not a peak-flow node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "peak-flow");

            Validator.ThrowInvalidIfNull(itemNav, "PeakFlowUnexpectedNode");

            this.when = new ApproximateDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.peakExpiratoryFlow =
                XPathHelper.GetOptNavValue<FlowMeasurement>(
                    itemNav,
                    "pef");

            this.fev1 =
                XPathHelper.GetOptNavValue<VolumeMeasurement>(
                    itemNav,
                    "fev1");

            this.fev6 =
                XPathHelper.GetOptNavValue<VolumeMeasurement>(
                    itemNav,
                    "fev6");

            this.measurementFlags.Clear();
            XPathNodeIterator measurementFlagsIterator = itemNav.Select("measurement-flags");
            foreach (XPathNavigator flagNav in measurementFlagsIterator)
            {
                CodableValue flag = new CodableValue();
                flag.ParseXml(flagNav);

                this.measurementFlags.Add(flag);
            }
        }

        /// <summary>
        /// Writes the peak flow data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the peak flow data to.
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
            Validator.ThrowSerializationIfNull(this.when, "PeakFlowWhenNotSet");

            // <peak-flow>
            writer.WriteStartElement("peak-flow");

            // <when>
            this.when.WriteXml("when", writer);

            XmlWriterHelper.WriteOpt(
                writer,
                "pef",
                this.peakExpiratoryFlow);

            XmlWriterHelper.WriteOpt(
                writer,
                "fev1",
                this.fev1);

            XmlWriterHelper.WriteOpt(
                writer,
                "fev6",
                this.fev6);

            foreach (CodableValue flag in this.measurementFlags)
            {
                flag.WriteXml("measurement-flags", writer);
            }

            // </peak-flow>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time when the peak flow reading occurred.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="ApproximateDateTime"/> instance representing the date
        /// and time.
        /// </returns>
        ///
        /// <remarks>
        /// The value defaults to the current year only.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private ApproximateDateTime when = new ApproximateDateTime();

        /// <summary>
        /// Gets or sets the peak expiratory flow measured in liters per
        /// second (L/s).
        /// </summary>
        ///
        /// <returns>
        /// A number representing the peak flow.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the peak expiratory flow should not
        /// be stored.
        /// </remarks>
        ///
        public FlowMeasurement Pef
        {
            get { return this.peakExpiratoryFlow; }
            set { this.peakExpiratoryFlow = value; }
        }

        private FlowMeasurement peakExpiratoryFlow;

        /// <summary>
        /// Gets or sets the forced expiratory volume in one second, measured in
        /// liters (L).
        /// </summary>
        ///
        /// <returns>
        /// A number representing the volume.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the forced expiratory volume should not
        /// be stored.
        /// </remarks>
        ///
        public VolumeMeasurement Fev1
        {
            get { return this.fev1; }
            set { this.fev1 = value; }
        }

        private VolumeMeasurement fev1;

        /// <summary>
        /// Gets or sets the forced expiratory volume in six seconds, measured in
        /// liters (L).
        /// </summary>
        ///
        /// <returns>
        /// A number representing the volume.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the forced expiratory volume should not
        /// be stored.
        /// </remarks>
        ///
        public VolumeMeasurement Fev6
        {
            get { return this.fev6; }
            set { this.fev6 = value; }
        }

        private VolumeMeasurement fev6;

        /// <summary>
        /// Gets a collection of additional information about the measurement.
        /// </summary>
        ///
        /// <returns>
        /// A collection of <see cref="CodableValue"/> representing the flags.
        /// </returns>
        ///
        /// <remarks>
        /// Examples: incomplete measurement.
        /// </remarks>
        ///
        public Collection<CodableValue> MeasurementFlags => this.measurementFlags;

        private readonly Collection<CodableValue> measurementFlags = new Collection<CodableValue>();

        /// <summary>
        /// Gets a string representation of the peak flow reading.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the peak flow reading.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Pef != null && this.Fev1 != null && this.Fev6 != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatPefFev1Fev6"),
                        this.Pef.ToString(),
                        this.Fev1.ToString(),
                        this.Fev6.ToString());
            }
            else if (this.Pef != null && this.Fev1 != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatPefFev1"),
                        this.Pef.ToString(),
                        this.Fev1.ToString());
            }
            else if (this.Pef != null && this.Fev6 != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatPefFev6"),
                        this.Pef.ToString(),
                        this.Fev6.ToString());
            }
            else if (this.Fev1 != null && this.Fev6 != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatFev1Fev6"),
                        this.Fev1.ToString(),
                        this.Fev6.ToString());
            }
            else if (this.Pef != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatPef"),
                        this.Pef.ToString());
            }
            else if (this.Fev1 != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatFev1"),
                        this.Fev1.ToString());
            }
            else if (this.Fev6 != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "PeakFlowToStringFormatFev6"),
                        this.Fev6.ToString());
            }

            return result;
        }
    }
}
