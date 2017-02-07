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
    /// Represents a health record item type that encapsulates a person's 
    /// diabetic profile at a single point in time.
    /// </summary>
    /// 
    public class DiabeticProfile : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DiabeticProfile"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> 
        /// method is called.
        /// </remarks>
        /// 
        public DiabeticProfile()
            : base(TypeId)
        {
        }
 
        /// <summary>
        /// Creates a new instance of the <see cref="DiabeticProfile"/> class 
        /// with the specified date and time.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time when the diabetic profile was take.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public DiabeticProfile(HealthServiceDateTime when)
            : base(TypeId)
        {
            this.When = when;
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
            new Guid("80CF4080-AD3F-4BB5-A0B5-907C22F73017");

        /// <summary>
        /// Populates this <see cref="DiabeticProfile"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the diabetic profile data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a diabetic-profile node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "diabetic-profile");

            Validator.ThrowInvalidIfNull(itemNav, "DiabeticProfileUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            XPathNavigator maxHbA1CNav = 
                itemNav.SelectSingleNode("max-HbA1C");

            if (maxHbA1CNav != null)
            {
                _maxHbA1C = maxHbA1CNav.ValueAsDouble;
            }

            XPathNodeIterator zoneGroupIterator =
                itemNav.Select("target-glucose-zone-group");

            foreach (XPathNavigator groupNav in zoneGroupIterator)
            {
                TargetGlucoseZoneGroup zoneGroup = new TargetGlucoseZoneGroup();
                zoneGroup.ParseXml(groupNav);
                _zoneGroups.Add(zoneGroup);
            }
        }

        /// <summary>
        /// Writes the diabetic profile data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the diabetic profile data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <diabetic-profile>
            writer.WriteStartElement("diabetic-profile");

            // <when>
            _when.WriteXml("when", writer);

            // <max-HbA1C>
            if (_maxHbA1C != null)
            {
                writer.WriteElementString(
                    "max-HbA1C", 
                    _maxHbA1C.Value.ToString(CultureInfo.InvariantCulture));
            }


            foreach (TargetGlucoseZoneGroup group in _zoneGroups)
            {
                group.WriteXml("target-glucose-zone-group", writer);
            }

            // </diabetic-profile>
            writer.WriteEndElement();
        }


        /// <summary>
        /// Gets or sets the date/time when the diabetic profile was taken.
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
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the person's maximum HbA1C.
        /// </summary>
        /// 
        /// <value>
        /// A number representing the maximum HbA1C.
        /// </value>
        /// 
        /// <remarks>
        /// If the maximum HbA1C is unknown, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0 or greater than 1.0.
        /// </exception>
        /// 
        public double? MaximumHbA1C
        {
            get { return _maxHbA1C; }
            set 
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && value < 0.0 || value > 1.0,
                    "MaximumHbA1C",
                    "DiabeticProfileMaxHbA1CRange");
                _maxHbA1C = value; }
        }
        private double? _maxHbA1C;

        /// <summary>
        /// Gets the target glucose zone groups.
        /// </summary>
        /// 
        /// <value>
        /// A collection of zone groups.
        /// </value>
        /// 
        /// <remarks>
        /// Target glucose zones are grouped to allow different sets of
        /// target zones based on a person's needs.
        /// To add a group of target glucose zones, call the Add method on the
        /// returned collection.
        /// </remarks>
        /// 
        public Collection<TargetGlucoseZoneGroup> TargetGlucoseZoneGroups
        {
            get { return _zoneGroups; }
        }
        private Collection<TargetGlucoseZoneGroup> _zoneGroups =
            new Collection<TargetGlucoseZoneGroup>();

        /// <summary>
        /// Gets a string representation of the diabetic profile item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the diabetic profile item.
        /// </returns>
        /// 
        public override string ToString()
        {
            string result = String.Empty;
            if (MaximumHbA1C != null)
            {
                result = 
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "DiabeticProfileToStringFormatPercent"),
                        (MaximumHbA1C.Value * 100.0).ToString(CultureInfo.CurrentCulture));
            }
            return result;
        }
    }

}
