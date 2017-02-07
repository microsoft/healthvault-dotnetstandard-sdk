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
    /// Represents a health record item type that an activity related to a group membership.
    /// </summary>
    /// 
    public class GroupMembershipActivity : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GroupMembershipActivity"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public GroupMembershipActivity()
            : base(TypeId)
        {
        }


        /// <summary>
        /// Creates a new instance of the <see cref="GroupMembershipActivity"/> class with the 
        /// specified date/time and activity.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date/time when the activity occurred.
        /// </param>
        /// 
        /// <param name="activity">
        /// The activity that occurred.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> or <paramref name="activity"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public GroupMembershipActivity(HealthServiceDateTime when, CodedValue activity)
            : base(TypeId)
        {
            this.When = when;
            this.Activity = activity;
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
            new Guid("e75fa095-31ed-4b30-b5f7-463963b5e734");

        /// <summary>
        /// Populates this <see cref="GroupMembershipActivity"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the group membership activity data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a group-membership-activity node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("group-membership-activity");

            Validator.ThrowInvalidIfNull(itemNav, "GroupMembershipActivityUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _activity = new CodedValue();
            _activity.ParseXml(itemNav.SelectSingleNode("activity"));

            _activityInfo = XPathHelper.GetOptNavValue(itemNav, "activity-info");
        }

        /// <summary>
        /// Writes the group membership activity data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the group membership activity data to.
        /// </param>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> or <see cref="Activity"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "GroupMembershipActivityWhenNotSet");
            Validator.ThrowSerializationIfNull(_activity, "GroupMembershipActivityActivityNotSet");

            // <group-membership-activity>
            writer.WriteStartElement("group-membership-activity");

            // <when>
            _when.WriteXml("when", writer);

            _activity.WriteXml("activity", writer);

            XmlWriterHelper.WriteOptString(writer, "activity-info", _activityInfo);

            // </group-membership-activity>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of the object.
        /// </summary>
        /// 
        /// <returns>
        /// <see cref="Activity"/> if set; otherwise String.Empty.
        /// </returns>
        /// 
        public override string ToString()
        {
            string result = String.Empty;

            if (Activity != null)
            {
                result = Activity.Value.ToString();
            }

            if (ActivityInfo != null)
            {
                result += ResourceRetriever.GetResourceString("ListSeparator");
                result += ActivityInfo;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the date/time when the group membership activity
        /// occurred.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> for the group membership activity. 
        /// The default value is the current year, month, and day.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> parameter is <b>null</b>.
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
        /// Gets or sets the activity that occurred.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodedValue"/>.
        /// </value>
        /// 
        /// <remarks>
        /// For example: RecordAuthorizedForApplication.
        /// The preferred vocabulary for the activity is "group-membership-activities".
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        /// 
        public CodedValue Activity
        {
            get { return _activity; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Activity", "GroupMembershipActivityActivityMandatory");
                _activity = value;
            }
        }
        private CodedValue _activity;

        /// <summary>
        /// Gets or sets additional information about the activity.
        /// </summary>
        /// 
        /// <remarks>
        /// If the value is not known, it will be set to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string ActivityInfo
        {
            get { return _activityInfo; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "ActivityInfo");
                _activityInfo = value;
            }
        }
        private string _activityInfo;
    }
}
