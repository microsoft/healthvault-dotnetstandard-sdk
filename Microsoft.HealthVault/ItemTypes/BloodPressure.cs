// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a blood
    /// pressure measurement.
    /// </summary>
    ///
    public class BloodPressure : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BloodPressure"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public BloodPressure()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BloodPressure"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the blood pressure reading was take.
        /// </param>
        ///
        /// <param name="systolic">
        /// The systolic pressure of the reading.
        /// </param>
        ///
        /// <param name="diastolic">
        /// The diastolic pressure of the reading.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="systolic"/> or <paramref name="diastolic"/>
        /// parameter is negative.
        /// </exception>
        ///
        public BloodPressure(
            HealthServiceDateTime when,
            int systolic,
            int diastolic)
            : base(TypeId)
        {
            this.When = when;
            this.Systolic = systolic;
            this.Diastolic = diastolic;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value
        /// >A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("ca3c57f4-f4c1-4e15-be67-0a3caf5414ed");

        /// <summary>
        /// Populates this <see cref="BloodPressure"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the blood pressure data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a bp node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator bpNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                "blood-pressure");

            Validator.ThrowInvalidIfNull(bpNav, Resources.BPUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(bpNav.SelectSingleNode("when"));

            this.systolic = bpNav.SelectSingleNode("systolic").ValueAsInt;
            this.diastolic = bpNav.SelectSingleNode("diastolic").ValueAsInt;

            XPathNavigator pulseNav = bpNav.SelectSingleNode("pulse");

            if (pulseNav != null)
            {
                this.pulse = pulseNav.ValueAsInt;
            }

            XPathNavigator irregularHeartbeatNav =
                bpNav.SelectSingleNode("irregular-heartbeat");

            if (irregularHeartbeatNav != null)
            {
                this.irregularHeartbeatDetected =
                    irregularHeartbeatNav.ValueAsBoolean;
            }
        }

        /// <summary>
        /// Writes the blood pressure data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the blood pressure data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Systolic"/> or <see cref="Diastolic"/> property
        /// has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.systolic, Resources.BPSystolicNotSet);
            Validator.ThrowSerializationIfNull(this.diastolic, Resources.BPDiastolicNotSet);

            // <blood-pressure>
            writer.WriteStartElement("blood-pressure");

            // <when>
            this.when.WriteXml("when", writer);

            writer.WriteElementString("systolic", this.systolic.ToString());

            writer.WriteElementString("diastolic", this.diastolic.ToString());

            if (this.pulse != null)
            {
                writer.WriteElementString("pulse", this.pulse.ToString());
            }

            if (this.irregularHeartbeatDetected != null)
            {
                writer.WriteElementString(
                    "irregular-heartbeat",
                    SDKHelper.XmlFromBool((bool)this.irregularHeartbeatDetected));
            }

            // </bp>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the blood pressure measurement
        /// was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the systolic pressure of the reading.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the pressure.
        /// </value>
        ///
        /// <remarks>
        /// This property must be set before the item is created or updated.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than zero.
        /// </exception>
        ///
        public int Systolic
        {
            get
            {
                return this.systolic ?? 0;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Systolic), Resources.BPSystolicNegative);
                }

                this.systolic = value;
            }
        }

        private int? systolic;

        /// <summary>
        /// Gets or sets the diastolic pressure of the reading.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the pressure.
        /// </value>
        ///
        /// <remarks>
        /// This property must be set before the item is created or updated.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than zero.
        /// </exception>
        ///
        public int Diastolic
        {
            get
            {
                return this.diastolic ?? 0;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Diastolic), Resources.BPDiastolicNegative);
                }

                this.diastolic = value;
            }
        }

        private int? diastolic;

        /// <summary>
        /// Gets or sets the pulse during the reading.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the pulse.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the pulse should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than zero.
        /// </exception>
        ///
        public int? Pulse
        {
            get { return this.pulse; }

            set
            {
                if (value != null && value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.Pulse), Resources.BPPulseNegative);
                }

                this.pulse = value;
            }
        }

        private int? pulse;

        /// <summary>
        /// Gets or sets a value indicating whether an irregular heartbeat was
        /// detected.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if an irregular heartbeat was detected; otherwise, <b>false</b>.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the reading does not support detecting an
        /// irregular heartbeat.
        /// </remarks>
        ///
        public bool? IrregularHeartbeatDetected
        {
            get { return this.irregularHeartbeatDetected; }
            set { this.irregularHeartbeatDetected = value; }
        }

        private bool? irregularHeartbeatDetected;

        /// <summary>
        /// Gets a string representation of the blood pressure item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the blood pressure item.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    Resources.BPToStringFormat,
                    this.Systolic,
                    this.Diastolic);
        }
    }
}
