// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a sleep journal 
    /// evening entry.
    /// </summary>
    /// 
    public class SleepJournalPM : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SleepJournalPM"/> class 
        /// with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public SleepJournalPM()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SleepJournalPM"/> class 
        /// specifying the mandatory values.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time when the PM sleep journal entry was taken.
        /// </param>
        /// 
        /// <param name="sleepiness">
        /// The state of sleepiness the person was in throughout the day.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="sleepiness"/> parameter is 
        /// <see cref="Microsoft.Health.ItemTypes.Sleepiness.Unknown"/>.
        /// </exception>
        /// 
        public SleepJournalPM(
            HealthServiceDateTime when,
            Sleepiness sleepiness)
            : base(TypeId)
        {
            this.When = when;
            this.Sleepiness = sleepiness;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        /// <value>
        /// A GUID.
        /// </value>
        /// 
        public new static readonly Guid TypeId =
            new Guid("031f5706-7f1a-11db-ad56-7bd355d89593");

        /// <summary>
        /// Populates this <see cref="SleepJournalPM"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the evening sleep journal data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a sleep-pm node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator sleepNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("sleep-pm");

            Validator.ThrowInvalidIfNull(sleepNav, "SleepJournalPMUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(sleepNav.SelectSingleNode("when"));

            XPathNodeIterator caffeineNodes = sleepNav.Select("caffeine");
            foreach (XPathNavigator caffeineNav in caffeineNodes)
            {
                ApproximateTime caffeineTaken = new ApproximateTime();
                caffeineTaken.ParseXml(caffeineNav);
                _caffeine.Add(caffeineTaken);
            }

            XPathNodeIterator alcoholNodes = sleepNav.Select("alcohol");
            foreach (XPathNavigator alcoholNav in alcoholNodes)
            {
                ApproximateTime alcoholTaken = new ApproximateTime();
                alcoholTaken.ParseXml(alcoholNav);
                _alcohol.Add(alcoholTaken);
            }

            XPathNodeIterator napNodes = sleepNav.Select("nap");
            foreach (XPathNavigator napNav in napNodes)
            {
                Occurrence napTaken = new Occurrence();
                napTaken.ParseXml(napNav);
                _naps.Add(napTaken);
            }

            XPathNodeIterator exerciseNodes = sleepNav.Select("exercise");
            foreach (XPathNavigator exerciseNav in exerciseNodes)
            {
                Occurrence exerciseTaken = new Occurrence();
                exerciseTaken.ParseXml(exerciseNav);
                _exercise.Add(exerciseTaken);
            } 
            
            _sleepiness = 
                (Sleepiness)
                sleepNav.SelectSingleNode("sleepiness").ValueAsInt;
        }

        /// <summary>
        /// Writes the evening sleep journal data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the evening sleep journal data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/> or <see cref="Sleepiness"/> properties 
        /// have not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIfNull(_when, "SleepJournalPMWhenNotSet");
            Validator.ThrowSerializationIf(_sleepiness == Sleepiness.Unknown, "SleepJournalPMSleepinessNotSet");

            // <sleep-pm>
            writer.WriteStartElement("sleep-pm");

            _when.WriteXml("when", writer);

            WriteApproximateTimeCollection(_caffeine, "caffeine", writer);
            WriteApproximateTimeCollection(_alcohol, "alcohol", writer);
            WriteOccurrenceCollection(_naps, "nap", writer);
            WriteOccurrenceCollection(_exercise, "exercise", writer);

            writer.WriteElementString(
                "sleepiness",
                ((int)_sleepiness).ToString(CultureInfo.InvariantCulture));

            // </sleep-pm>
            writer.WriteEndElement();
        }

        private static void WriteApproximateTimeCollection(
            IEnumerable<ApproximateTime> collection,
            string nodeName,
            XmlWriter writer)
        {
            foreach (ApproximateTime time in collection)
            {
                time.WriteXml(nodeName, writer);
            }
        }

        private static void WriteOccurrenceCollection(
            IEnumerable<Occurrence> collection,
            string nodeName,
            XmlWriter writer)
        {
            foreach (Occurrence occurrence in collection)
            {
                occurrence.WriteXml(nodeName, writer);
            }
        }

        /// <summary>
        /// Gets or sets the date and time when the journal entry is made.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="HealthServiceDateTime"/> representing 
        /// the date and time.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        /// 
        public HealthServiceDateTime When
        {
            get { return _when; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "When", "SleepJournalPMWhenMandatory");
                _when = value; 
            }
        }
        private HealthServiceDateTime _when;


        /// <summary>
        /// Gets the time(s) in which caffeine was consumed
        /// during the day.
        /// </summary>
        /// 
        /// <value>
        /// A collection of times.
        /// </value>
        /// 
        /// <remarks>
        /// To add occurrences of caffeine intake, add new <see cref="ApproximateTime"/> 
        /// instances to the returned collection.
        /// </remarks>
        /// 
        public Collection<ApproximateTime> Caffeine
        {
            get { return _caffeine; }
        }
        private Collection<ApproximateTime> _caffeine =
            new Collection<ApproximateTime>();

        /// <summary>
        /// Gets or sets the time(s) in which alcohol was consumed
        /// during the day.
        /// </summary>
        /// 
        /// <value>
        /// A collection of times.
        /// </value> 
        /// 
        /// <remarks>
        /// To add occurrences of alcohol intake, add new <see cref="ApproximateTime"/> 
        /// instances to the returned collection.
        /// </remarks>
        /// 
        public Collection<ApproximateTime> Alcohol
        {
            get { return _alcohol; }
        }
        private Collection<ApproximateTime> _alcohol =
            new Collection<ApproximateTime>();

        /// <summary>
        /// Gets the occurrences of any naps during the day.
        /// </summary>
        /// 
        /// <value>
        /// A collection of occurrences.
        /// </value>
        /// 
        /// <remarks>
        /// To add an occurrence of a nap, add new <see cref="Occurrence"/> instances 
        /// to the returned collection.
        /// </remarks>
        /// 
        public Collection<Occurrence> Naps
        {
            get { return _naps; }
        }
        private Collection<Occurrence> _naps =
            new Collection<Occurrence>();

        /// <summary>
        /// Gets the occurrences of any exercise that occurred 
        /// during the day.
        /// </summary>
        /// 
        /// <value>
        /// A collection of occurrences.
        /// </value> 
        /// 
        /// <remarks>
        /// To add an occurrence of an exercise, add new <see cref="Occurrence"/> instances 
        /// to the returned collection.
        /// </remarks>
        /// 
        public Collection<Occurrence> Exercise
        {
            get { return _exercise; }
        }
        private Collection<Occurrence> _exercise =
            new Collection<Occurrence>();

        /// <summary>
        /// Gets or sets the state of sleepiness the person was in throughout
        /// the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="Sleepiness"/> representing the state.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is 
        /// <see cref="Microsoft.Health.ItemTypes.Sleepiness.Unknown"/>.
        /// </exception>
        /// 
        public Sleepiness Sleepiness
        {
            get { return _sleepiness; }
            set 
            {
                Validator.ThrowArgumentExceptionIf(
                    value == Sleepiness.Unknown,
                    "Sleepiness", 
                    "SleepJournalPMSleepinessNotSet");
                _sleepiness = value;
            }
        }
        private Sleepiness _sleepiness = Sleepiness.Unknown;

        /// <summary>
        /// Gets the string representation of the sleep journal entry.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the sleep journal entry.
        /// </returns>
        /// 
        public override string ToString()
        {
            return When.ToString();
        }
    }
}
