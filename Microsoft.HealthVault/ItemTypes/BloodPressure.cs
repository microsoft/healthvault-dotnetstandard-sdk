// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
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
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
            When = when;
            Systolic = systolic;
            Diastolic = diastolic;
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

            _when = new HealthServiceDateTime();
            _when.ParseXml(bpNav.SelectSingleNode("when"));

            _systolic = bpNav.SelectSingleNode("systolic").ValueAsInt;
            _diastolic = bpNav.SelectSingleNode("diastolic").ValueAsInt;

            XPathNavigator pulseNav = bpNav.SelectSingleNode("pulse");

            if (pulseNav != null)
            {
                _pulse = pulseNav.ValueAsInt;
            }

            XPathNavigator irregularHeartbeatNav =
                bpNav.SelectSingleNode("irregular-heartbeat");

            if (irregularHeartbeatNav != null)
            {
                _irregularHeartbeatDetected =
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Systolic"/> or <see cref="Diastolic"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_systolic, Resources.BPSystolicNotSet);
            Validator.ThrowSerializationIfNull(_diastolic, Resources.BPDiastolicNotSet);

            // <blood-pressure>
            writer.WriteStartElement("blood-pressure");

            // <when>
            _when.WriteXml("when", writer);

            writer.WriteElementString("systolic", _systolic.ToString());

            writer.WriteElementString("diastolic", _diastolic.ToString());

            if (_pulse != null)
            {
                writer.WriteElementString("pulse", _pulse.ToString());
            }

            if (_irregularHeartbeatDetected != null)
            {
                writer.WriteElementString(
                    "irregular-heartbeat",
                    SDKHelper.XmlFromBool((bool)_irregularHeartbeatDetected));
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
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

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
                return _systolic ?? 0;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Systolic), Resources.BPSystolicNegative);
                }

                _systolic = value;
            }
        }

        private int? _systolic;

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
                return _diastolic ?? 0;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Diastolic), Resources.BPDiastolicNegative);
                }

                _diastolic = value;
            }
        }

        private int? _diastolic;

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
            get { return _pulse; }

            set
            {
                if (value != null && value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Pulse), Resources.BPPulseNegative);
                }

                _pulse = value;
            }
        }

        private int? _pulse;

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
            get { return _irregularHeartbeatDetected; }
            set { _irregularHeartbeatDetected = value; }
        }

        private bool? _irregularHeartbeatDetected;

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
                    Systolic,
                    Diastolic);
        }
    }
}
