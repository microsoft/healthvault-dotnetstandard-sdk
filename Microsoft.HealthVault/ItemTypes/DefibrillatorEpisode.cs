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
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information containing a Defibrillator Episode.
    /// </summary>
    /// <remarks>
    /// This information will usually be obtained from a
    /// defibrillator device.
    /// </remarks>
    public class DefibrillatorEpisode : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DefibrillatorEpisode"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public DefibrillatorEpisode()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DefibrillatorEpisode"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date when defibrillator episode was recorded.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public DefibrillatorEpisode(HealthServiceDateTime when)
            : base(TypeId)
        {
            When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("a3d38add-b7b2-4ccd-856b-9b14bbc4e075");

        /// <summary>
        /// Populates this <see cref="DefibrillatorEpisode"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the DefibrillatorEpisode data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a DefibrillatorEpisode node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                "defibrillator-episode");

            Validator.ThrowInvalidIfNull(itemNav, Resources.DefibrillatorEpisodeFieldUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _episodeTypeGroup = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "episode-type-group");
            _episodeType = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "episode-type");
            _dataSource = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "data-source");
            _durationInSeconds = XPathHelper.GetOptNavValueAsUInt(itemNav, "duration-in-seconds");
            _episodeFields = XPathHelper.ParseXmlCollection<DefibrillatorEpisodeField>(itemNav, "episode-fields/episode-field");
        }

        /// <summary>
        /// Writes the defibrillator episode data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the defibrillator data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.DefibrillatorEpisodeWhenNullValue);

            writer.WriteStartElement("defibrillator-episode");
            _when.WriteXml("when", writer);
            XmlWriterHelper.WriteOpt(writer, "episode-type-group", _episodeTypeGroup);
            XmlWriterHelper.WriteOpt(writer, "episode-type", _episodeType);
            XmlWriterHelper.WriteOpt(writer, "data-source", _dataSource);
            XmlWriterHelper.WriteOptUInt(writer, "duration-in-seconds", _durationInSeconds);
            writer.WriteStartElement("episode-fields");
            XmlWriterHelper.WriteXmlCollection(writer, _episodeFields, "episode-field");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of the defibrillator episode.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the defibrillator episode.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (EpisodeTypeGroup != null)
            {
                stringBuilder.Append(EpisodeTypeGroup);
            }

            if (EpisodeType != null)
            {
                AddListSeparator(stringBuilder);
                stringBuilder.Append(EpisodeType);
            }

            if (DataSource != null)
            {
                AddListSeparator(stringBuilder);
                stringBuilder.Append(DataSource);
            }

            if (DurationInSeconds != null)
            {
                AddListSeparator(stringBuilder);
                stringBuilder.Append(DurationInSeconds);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets or sets the date when the defibrillator episode was recorded.
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
            get
            {
                return _when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.DefibrillatorEpisodeWhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets and sets the episode type group.
        /// </summary>
        /// <remarks>
        /// Specifies the high-level grouping of the defibrillator episode. For example
        /// VT/VF represent episode type group for Ventricular Tachycardia / Ventricular Fibrillation.
        /// </remarks>
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue EpisodeTypeGroup
        {
            get
            {
                return _episodeTypeGroup;
            }

            set
            {
                _episodeTypeGroup = value;
            }
        }

        private CodableValue _episodeTypeGroup;

        /// <summary>
        /// Gets and sets the episode type.
        /// </summary>
        /// <remarks>
        /// Specifies the specific type of defibrillator episode. For example VT represents
        /// the episode type for Ventricular Tachycardia.
        /// </remarks>
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue EpisodeType
        {
            get
            {
                return _episodeType;
            }

            set
            {
                _episodeType = value;
            }
        }

        private CodableValue _episodeType;

        /// <summary>
        /// Gets and sets the data source of the defibrillator episode.
        /// </summary>
        /// <remarks>
        /// Data can come from different sources, for example Paceart device.
        /// </remarks>
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue DataSource
        {
            get
            {
                return _dataSource;
            }

            set
            {
                _dataSource = value;
            }
        }

        private CodableValue _dataSource;

        /// <summary>
        /// Gets and sets the duration of the episode in seconds.
        /// </summary>
        public uint? DurationInSeconds
        {
            get
            {
                return _durationInSeconds;
            }

            set
            {
                _durationInSeconds = value;
            }
        }

        private uint? _durationInSeconds;

        /// <summary>
        /// Gets the collection of defibrillator episode fields.
        /// </summary>
        /// <remarks>
        /// Each episode field stores an episode property as a name/value pair.
        /// </remarks>
        public Collection<DefibrillatorEpisodeField> EpisodeFields => _episodeFields;

        private Collection<DefibrillatorEpisodeField> _episodeFields =
            new Collection<DefibrillatorEpisodeField>();

        private static void AddListSeparator(StringBuilder stringBuilder)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(Resources.ListSeparator);
            }
        }
    }
}