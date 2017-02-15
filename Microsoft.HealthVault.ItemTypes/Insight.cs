// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates an insight
    /// related to the record. This insight is generated by the system.
    /// </summary>
    public class Insight : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Insight"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/>
        /// method is called.
        /// </remarks>
        public Insight() : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Insight"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="raisedInsightId">
        /// Unique Id of this insight instance.
        /// </param>
        ///
        /// <param name="catalogId">
        /// Unique Id of the catalog item used to generate this insight.
        /// </param>
        ///
        /// <param name="when">
        /// The date when the insight was generated.
        /// </param>
        ///
        /// <param name="expirationDate">
        /// The date when the insight will expire.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="expirationDate"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Insight(
            string raisedInsightId,
            string catalogId,
            HealthServiceDateTime when,
            HealthServiceDateTime expirationDate)
            : base(TypeId)
        {
            RaisedInsightId = raisedInsightId;
            CatalogId = catalogId;
            When = when;
            ExpirationDate = expirationDate;
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
            new Guid("5D15B7BC-0499-4DC4-8DF7-EF1A2332CFB5");

        /// <summary>
        /// Populates this <see cref="Insight"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the insight data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an insight node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "insight");

            Validator.ThrowInvalidIfNull(itemNav, "InsightUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _raisedInsightId = itemNav.SelectSingleNode("raised-insight-id").Value;
            _catalogId = itemNav.SelectSingleNode("catalog-id").Value;

            _expirationDate = new HealthServiceDateTime();
            _expirationDate.ParseXml(itemNav.SelectSingleNode("expiration-date"));

            _channel = XPathHelper.GetOptNavValue(itemNav, "channel");
            _algoClass = XPathHelper.GetOptNavValue(itemNav, "algo-class");
            _directionality = XPathHelper.GetOptNavValue(itemNav, "directionality");
            _timespanPivot = XPathHelper.GetOptNavValue(itemNav, "time-span-pivot");
            _comparisonPivot = XPathHelper.GetOptNavValue(itemNav, "comparison-pivot");
            _tonePivot = XPathHelper.GetOptNavValue(itemNav, "tone-pivot");
            _scopePivot = XPathHelper.GetOptNavValue(itemNav, "scope-pivot");

            XPathNavigator dataUsedNav = itemNav.SelectSingleNode("data-used-pivot");
            _dataUsedPivot = GetStringList(dataUsedNav, "data-used");

            _annotation = XPathHelper.GetOptNavValue(itemNav, "annotation");
            _strength = XPathHelper.GetOptNavValueAsDouble(itemNav, "strength");
            _confidence = XPathHelper.GetOptNavValueAsDouble(itemNav, "confidence");
            _origin = XPathHelper.GetOptNavValue(itemNav, "origin");

            XPathNavigator tagsNav = itemNav.SelectSingleNode("tags");
            _insightTags = GetStringList(tagsNav, "tag");

            XPathNavigator valueNav = itemNav.SelectSingleNode("values");
            _values = GetDictionary(valueNav, "value");

            XPathNavigator callToActionNav = itemNav.SelectSingleNode("links");
            _links = GetDictionary(callToActionNav, "value");
        }

        /// <summary>
        /// Writes the insight data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the insight data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_raisedInsightId, "InsightIdNullValue");
            Validator.ThrowSerializationIfNull(_catalogId, "InsightCatalogIdNullValue");
            Validator.ThrowSerializationIfNull(_when, "WhenNullValue");
            Validator.ThrowSerializationIfNull(_expirationDate, "InsightExpirationDateNullValue");

            // <insight>
            writer.WriteStartElement("insight");

            // <raised-insight-id>
            writer.WriteElementString("raised-insight-id", _raisedInsightId);

            // <catalog-id>
            writer.WriteElementString("catalog-id", _catalogId);

            // <when>
            _when.WriteXml("when", writer);

            // <expiration-date>
            _expirationDate.WriteXml("expiration-date", writer);

            // <channel>
            XmlWriterHelper.WriteOptString(writer, "channel", _channel);

            // <algo-class>
            XmlWriterHelper.WriteOptString(writer, "algo-class", _algoClass);

            // <directionality>
            XmlWriterHelper.WriteOptString(writer, "directionality", _directionality);

            // <time-span-pivot>
            XmlWriterHelper.WriteOptString(writer, "time-span-pivot", _timespanPivot);

            // <comparison-pivot>
            XmlWriterHelper.WriteOptString(writer, "comparison-pivot", _comparisonPivot);

            // <tone-pivot>
            XmlWriterHelper.WriteOptString(writer, "tone-pivot", _tonePivot);

            // <scope-pivot>
            XmlWriterHelper.WriteOptString(writer, "scope-pivot", _scopePivot);

            // <data-used-pivot>
            WriteStringList(_dataUsedPivot, writer, "data-used-pivot", "data-used");

            // <annotation>
            XmlWriterHelper.WriteOptString(writer, "annotation", _annotation);

            // <strength>
            XmlWriterHelper.WriteOptDouble(writer, "strength", _strength);

            // <confidence>
            XmlWriterHelper.WriteOptDouble(writer, "confidence", _confidence);

            // <origin>
            XmlWriterHelper.WriteOptString(writer, "origin", _origin);

            // <tags>
            WriteStringList(_insightTags, writer, "tags", "tag");

            // <values>
            WriteDictionary(_values, writer, "values", "value");

            // <links>
            WriteDictionary(_links, writer, "links", "value");

            // </insight>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of the insight.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the insight.
        /// </returns>
        ///
        public override string ToString()
        {
            string value =
                string.Format(
                CultureInfo.CurrentCulture,
                ResourceRetriever.GetResourceString("InsightSummaryText"),
                _raisedInsightId,
                _catalogId);

            return value;
        }

        /// <summary>
        /// Uniquely identifies an instance of Insight generated for a user.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        public string RaisedInsightId
        {
            get
            {
                return _raisedInsightId;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "RaisedInsightId", "InsightIdNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "RaisedInsightId");
                _raisedInsightId = value;
            }
        }

        private string _raisedInsightId;

        /// <summary>
        /// Unique identity of the catalog item used to create this Insight.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        public string CatalogId
        {
            get
            {
                return _catalogId;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "CatalogId", "InsightCatalogIdNullValue");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "CatalogId");
                _catalogId = value;
            }
        }

        private string _catalogId;

        /// <summary>
        /// Gets or sets the date when the insight was created.
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
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }

        private HealthServiceDateTime _when;

        /// <summary>
        /// Date and time when this Insight instance expires.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        public HealthServiceDateTime ExpirationDate
        {
            get
            {
                return _expirationDate;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "ExpirationDate", "InsightExpirationDateNullValue");
                _expirationDate = value;
            }
        }

        private HealthServiceDateTime _expirationDate;

        /// <summary>
        /// Shows what does this Insight impact. For example sleep or activity etc.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's channel enum or <b>null</b> if unknown.
        /// </value>
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        private string _channel;

        /// <summary>
        /// Represents the algorithm class used to create this Insight.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's algo class enum or <b>null</b> if unknown.
        /// </value>
        public string AlgoClass
        {
            get { return _algoClass; }
            set { _algoClass = value; }
        }

        private string _algoClass;

        /// <summary>
        /// Represents which way the Insight is trending. For example positive, negative or neutral.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's directionality enum or <b>null</b> if unknown.
        /// </value>
        public string Directionality
        {
            get { return _directionality; }
            set { _directionality = value; }
        }

        private string _directionality;

        /// <summary>
        /// Represents the aggregation time span of the data. Example, data is aggregated weekly or daily.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's timespan pivot enum or <b>null</b> if unknown.
        /// </value>
        public string TimespanPivot
        {
            get { return _timespanPivot; }
            set { _timespanPivot = value; }
        }

        private string _timespanPivot;

        /// <summary>
        /// Represents how the user was compared for deriving this Insight. Example with themselves or people similar to the user.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's comparison pivot enum or <b>null</b> if unknown.
        /// </value>
        public string ComparisonPivot
        {
            get { return _comparisonPivot; }
            set { _comparisonPivot = value; }
        }

        private string _comparisonPivot;

        /// <summary>
        /// Represents the tone of the Insight, like better or worse.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's tone pivot enum or <b>null</b> if unknown.
        /// </value>
        public string TonePivot
        {
            get { return _tonePivot; }
            set { _tonePivot = value; }
        }

        private string _tonePivot;

        /// <summary>
        /// Represents the scope of the Insight like for a specific event or event types.
        /// </summary>
        ///
        /// <value>
        /// The value of insight's scope pivot enum or <b>null</b> if unknown.
        /// </value>
        public string ScopePivot
        {
            get { return _scopePivot; }
            set { _scopePivot = value; }
        }

        private string _scopePivot;

        /// <summary>
        /// Represents a list of data types used as input to the insight calculation.
        /// </summary>
        ///
        /// <value>
        /// List of datatype enum values or <b>null</b> if unknown.
        /// </value>
        public List<string> DataUsedPivot
        {
            get { return _dataUsedPivot; }
            set { _dataUsedPivot = value; }
        }

        private List<string> _dataUsedPivot;

        /// <summary>
        /// Describes how we got to this conclusion and why this Insight is relevant to the user.
        /// </summary>
        ///
        /// <value>
        /// The value of annotation text or <b>null</b> if unknown.
        /// </value>
        public string Annotation
        {
            get { return _annotation; }
            set { _annotation = value; }
        }

        private string _annotation;

        /// <summary>
        /// Represents the strength of the data used for calculating the Insights.
        /// </summary>
        ///
        /// <value>
        /// The value representing strength of this insight or <b>null</b> if unknown.
        /// </value>
        public double? Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }

        private double? _strength;

        /// <summary>
        /// Confidence level of the insight process that generated the insight.
        /// </summary>
        ///
        /// <value>
        /// The value representing the confidence or <b>null</b> if unknown.
        /// </value>
        public double? Confidence
        {
            get { return _confidence; }
            set { _confidence = value; }
        }

        private double? _confidence;

        /// <summary>
        /// Where was this insight generated.
        /// </summary>
        ///
        /// <value>
        /// The value of the insight's origin or <b>null</b> if unknown.
        /// </value>
        public string Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        private string _origin;

        /// <summary>
        /// Tags associated with this insight. Can be used by clients for grouping, filtering etc.
        /// </summary>
        ///
        /// <value>
        /// List of tags associated with this insight or <b>null</b> if unknown.
        /// </value>
        public List<string> InsightTags
        {
            get { return _insightTags; }
            set { _insightTags = value; }
        }

        private List<string> _insightTags;

        /// <summary>
        /// Contains the key-value collection associated with the Insight. Keys and their description is included
        /// in Insights Catalog.
        /// </summary>
        ///
        /// <value>
        /// List of key-values containing the insight values or <b>null</b> if unknown.
        /// </value>
        public Dictionary<string, object> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        private Dictionary<string, object> _values;

        /// <summary>
        /// Gets or sets links for Insights.
        /// </summary>
        ///
        /// <value>
        /// List of key-values containing links for this insight or <b>null</b> if unknown.
        /// </value>
        public Dictionary<string, object> Links
        {
            get { return _links; }
            set { _links = value; }
        }

        private Dictionary<string, object> _links;

        /// <summary>
        /// Gets or sets insight messages.
        /// </summary>
        ///
        /// <value>
        /// Collection of message strings associated with this insight.
        /// </value>
        public InsightMessages Messages
        {
            get { return _insightMessages; }
            set { _insightMessages = value; }
        }

        private InsightMessages _insightMessages;

        /// <summary>
        /// Gets or sets insight attribution
        /// </summary>
        ///
        /// <value>
        /// Attribution information for this insight.
        /// </value>
        public InsightAttribution Attribution
        {
            get { return _insightAttribution; }
            set { _insightAttribution = value; }
        }

        private InsightAttribution _insightAttribution;

        private static List<string> GetStringList(XPathNavigator subItemNav, string subItemName)
        {
            if (subItemNav == null || string.IsNullOrEmpty(subItemName))
            {
                return null;
            }

            List<string> listOfItems = new List<string>();

            foreach (XPathNavigator servicesNav in subItemNav.Select(subItemName))
            {
                string item = servicesNav.Value;
                listOfItems.Add(item);
            }

            return listOfItems;
        }

        private static Dictionary<string, object> GetDictionary(XPathNavigator subItemNav, string subItemName)
        {
            if (subItemNav == null || string.IsNullOrEmpty(subItemName))
            {
                return null;
            }

            Dictionary<string, object> itemDictionary = new Dictionary<string, object>();

            foreach (XPathNavigator nav in subItemNav.Select(subItemName))
            {
                string key = nav.SelectSingleNode("key").Value;
                object value = nav.SelectSingleNode("value").TypedValue;

                if (!itemDictionary.ContainsKey(key))
                {
                    itemDictionary.Add(key, value);
                }
            }

            return itemDictionary;
        }

        private static void WriteStringList(List<string> items, XmlWriter writer, string itemName, string subItemName)
        {
            if (items == null || items.Count < 1)
            {
                return;
            }

            // <itemName>
            writer.WriteStartElement(itemName);

            foreach (string item in items)
            {
                writer.WriteElementString(subItemName, item);
            }

            // </itemName>
            writer.WriteEndElement();
        }

        private static void WriteDictionary(Dictionary<string, object> dictionary, XmlWriter writer, string itemName, string subItemName)
        {
            if (dictionary == null || dictionary.Count < 1)
            {
                return;
            }

            writer.WriteStartElement(itemName);

            foreach (KeyValuePair<string, object> item in dictionary)
            {
                // <value> - parent element of key-value pair.
                writer.WriteStartElement(subItemName);

                writer.WriteElementString("key", item.Key);

                // <value> - value of the key-value pair element.
                writer.WriteStartElement("value");
                writer.WriteValue(item.Value);

                // </value>
                writer.WriteEndElement();

                // </value>
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}