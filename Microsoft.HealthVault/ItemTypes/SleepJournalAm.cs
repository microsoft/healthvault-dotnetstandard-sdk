// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a sleep journal
    /// morning entry.
    /// </summary>
    ///
    public class SleepJournalAM : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SleepJournalAM"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public SleepJournalAM()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SleepJournalAM"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time when the AM sleep journal entry was taken.
        /// </param>
        ///
        /// <param name="bedtime">
        /// The approximate time the person went to bed.
        /// </param>
        ///
        /// <param name="wakeTime">
        /// The approximate time the person woke up.
        /// </param>
        ///
        /// <param name="sleepMinutes">
        /// The number of minutes the person slept.
        /// </param>
        ///
        /// <param name="settlingMinutes">
        /// The number of minutes it took the person to fall asleep after
        /// going to bed.
        /// </param>
        ///
        /// <param name="wakeState">
        /// The state of the person when they awoke.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="sleepMinutes"/> parameter or
        /// <paramref name="settlingMinutes"/> parameter is negative.
        /// </exception>
        ///
        public SleepJournalAM(
            HealthServiceDateTime when,
            ApproximateTime bedtime,
            ApproximateTime wakeTime,
            int sleepMinutes,
            int settlingMinutes,
            WakeState wakeState)
            : base(TypeId)
        {
            this.When = when;
            this.Bedtime = bedtime;
            this.WakeTime = wakeTime;
            this.SleepMinutes = sleepMinutes;
            this.SettlingMinutes = settlingMinutes;
            this.WakeState = wakeState;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("11c52484-7f1a-11db-aeac-87d355d89593");

        /// <summary>
        /// Populates this SleepJournalAM instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the morning sleep journal data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in the <paramref name="typeSpecificXml"/> parameter
        /// is not a sleep-am node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator sleepNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("sleep-am");

            Validator.ThrowInvalidIfNull(sleepNav, Resources.SleepJournalAMUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(sleepNav.SelectSingleNode("when"));

            this.bedtime = new ApproximateTime();
            this.bedtime.ParseXml(sleepNav.SelectSingleNode("bed-time"));

            this.wakeTime = new ApproximateTime();
            this.wakeTime.ParseXml(sleepNav.SelectSingleNode("wake-time"));

            this.sleepMinutes =
                sleepNav.SelectSingleNode("sleep-minutes").ValueAsInt;

            this.settlingMinutes =
                sleepNav.SelectSingleNode("settling-minutes").ValueAsInt;

            XPathNodeIterator awakeningsIterator =
                sleepNav.Select("awakening");

            foreach (XPathNavigator awakeningNav in awakeningsIterator)
            {
                Occurrence awakening = new Occurrence();
                awakening.ParseXml(awakeningNav);
                this.awakenings.Add(awakening);
            }

            XPathNavigator medNav = sleepNav.SelectSingleNode("medications");

            if (medNav != null)
            {
                this.medications = new CodableValue();
                this.medications.ParseXml(medNav);
            }

            this.wakeState =
                (WakeState)sleepNav.SelectSingleNode("wake-state").ValueAsInt;
        }

        /// <summary>
        /// Writes the morning sleep journal data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the morning sleep journal data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="When"/>, <see cref="Bedtime"/>,
        /// <see cref="WakeTime"/>, <see cref="SleepMinutes"/>,
        /// <see cref="SettlingMinutes"/>, or <see cref="WakeState"/> parameter
        /// has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIfNull(this.when, Resources.SleepJournalAMWhenNotSet);
            Validator.ThrowSerializationIfNull(this.bedtime, Resources.SleepJournalAMBedTimeNotSet);
            Validator.ThrowSerializationIfNull(this.wakeTime, Resources.SleepJournalAMWakeTimeNotSet);
            Validator.ThrowSerializationIfNull(this.sleepMinutes, Resources.SleepJournalAMSleepMinutesNotSet);
            Validator.ThrowSerializationIfNull(this.settlingMinutes, Resources.SleepJournalAMSettlingMinutesNotSet);

            if (this.wakeState == WakeState.Unknown)
            {
                throw new ThingSerializationException(Resources.SleepJournalAMWakeStateNotSet);
            }

            // <sleep-am>
            writer.WriteStartElement("sleep-am");

            this.when.WriteXml("when", writer);
            this.bedtime.WriteXml("bed-time", writer);
            this.wakeTime.WriteXml("wake-time", writer);

            writer.WriteElementString(
                "sleep-minutes",
                this.sleepMinutes.ToString());

            writer.WriteElementString(
                "settling-minutes",
                this.settlingMinutes.ToString());

            foreach (Occurrence awakening in this.awakenings)
            {
                awakening.WriteXml("awakening", writer);
            }

            this.medications?.WriteXml("medications", writer);

            writer.WriteElementString(
                "wake-state",
                ((int)this.wakeState).ToString(CultureInfo.InvariantCulture));

            // </sleep-am>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the when the journal entry is made.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthServiceDateTime"/> instance representing the
        /// date and time.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> when set.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.SleepJournalAMWhenMandatory);
                this.when = value;
            }
        }

        private HealthServiceDateTime when;

        /// <summary>
        /// Gets or sets the when the person went to bed.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ApproximateTime"/> instance representing the time.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> when set.
        /// </exception>
        ///
        public ApproximateTime Bedtime
        {
            get { return this.bedtime; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Bedtime), Resources.SleepJournalAMBedTimeMandatory);
                this.bedtime = value;
            }
        }

        private ApproximateTime bedtime;

        /// <summary>
        /// Gets or sets the when the person woke up.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="ApproximateTime"/> instance representing the time.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> when set.
        /// </exception>
        ///
        public ApproximateTime WakeTime
        {
            get { return this.wakeTime; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(WakeTime), Resources.SleepJournalAMWakeTimeMandatory);
                this.wakeTime = value;
            }
        }

        private ApproximateTime wakeTime;

        /// <summary>
        /// Gets or sets the number of minutes slept.
        /// </summary>
        ///
        /// <returns>
        /// An integer representing the number of minutes.
        /// </returns>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public int SleepMinutes
        {
            get { return this.sleepMinutes ?? 0; }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.SleepMinutes), Resources.SleepJournalAMSleepMinutesNotPositive);
                }

                this.sleepMinutes = value;
            }
        }

        private int? sleepMinutes;

        /// <summary>
        /// Gets or sets the number of minutes spent settling into sleep.
        /// </summary>
        ///
        /// <returns>
        /// An integer representing the number of minutes.
        /// </returns>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public int SettlingMinutes
        {
            get { return this.settlingMinutes ?? 0; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.SettlingMinutes), Resources.SleepJournalAMSettlingMinutesMandatory);
                }

                this.settlingMinutes = value;
            }
        }

        private int? settlingMinutes;

        /// <summary>
        /// Gets the occurrences of awakenings that occurred during
        /// sleeping period.
        /// </summary>
        ///
        /// <returns>
        /// A collection representing the number of awakenings.
        /// </returns>
        ///
        /// <remarks>
        /// To add awakenings, add new <see cref="Occurrence"/> instances to the returned
        /// collection.
        /// </remarks>
        ///
        public Collection<Occurrence> Awakenings => this.awakenings;

        private readonly Collection<Occurrence> awakenings =
            new Collection<Occurrence>();

        /// <summary>
        /// Gets or sets a description of the medications taken before bed.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="CodableValue"/> instance representing the description.
        /// </returns>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the medications should not be stored.
        /// </remarks>
        ///
        public CodableValue Medications
        {
            get { return this.medications; }
            set { this.medications = value; }
        }

        private CodableValue medications;

        /// <summary>
        /// Gets or sets the state of the person when they awoke.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="WakeState"/> value representing the state.
        /// </returns>
        ///
        public WakeState WakeState
        {
            get { return this.wakeState; }
            set { this.wakeState = value; }
        }

        private WakeState wakeState = WakeState.Unknown;

        /// <summary>
        /// Gets a string representation of the sleep journal entry.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the sleep journal entry.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = this.When.ToString();

            if (this.SleepMinutes > 0)
            {
                result =
                    string.Format(
                        Resources.SleepJournalAmToStringFormat,
                    this.When.ToString(),
                    this.SleepMinutes);
            }

            return result;
        }
    }
}
