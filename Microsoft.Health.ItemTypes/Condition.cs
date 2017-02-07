// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a single 
    /// condition, issue, or problem.
    /// </summary>
    /// 
    public class Condition : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Condition"/> class with 
        /// default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Condition()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Condition"/> class with the 
        /// specified name.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the condition.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public Condition(CodableValue name)
            : base(TypeId)
        {
            this.Name = name;
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
            new Guid("7ea7a1f9-880b-4bd4-b593-f5660f20eda8");

        /// <summary>
        /// Populates this <see cref="Condition"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the condition data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a condition node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            _name.Clear();
            XPathNavigator conditionNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("condition");

            Validator.ThrowInvalidIfNull(conditionNav, "ConditionUnexpectedNode");

            _name.ParseXml(conditionNav.SelectSingleNode("name"));

            XPathNavigator onsetNav =
                conditionNav.SelectSingleNode("onset-date");
            if (onsetNav != null)
            {
                _onsetDate = new ApproximateDateTime();
                _onsetDate.ParseXml(onsetNav);
            }

            XPathNavigator statusNav =
                conditionNav.SelectSingleNode("status");
            if (statusNav != null)
            {
                _status = new CodableValue();
                _status.ParseXml(statusNav);
            }

            XPathNavigator stopDateNav =
                conditionNav.SelectSingleNode("stop-date");
            if (stopDateNav != null)
            {
                _stopDate = new ApproximateDateTime();
                _stopDate.ParseXml(stopDateNav);
            }

            XPathNavigator stopReasonNav =
                conditionNav.SelectSingleNode("stop-reason");
            if (stopReasonNav != null)
            {
                _stopReason = stopReasonNav.Value;
            }


        }

        /// <summary>
        /// Writes the condition data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the condition data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name.Text, "ConditionNameNotSet");

            // <condition>
            writer.WriteStartElement("condition");

            _name.WriteXml("name", writer);

            if (_onsetDate != null)
            {
                _onsetDate.WriteXml("onset-date", writer);
            }

            if (_status != null)
            {
                _status.WriteXml("status", writer);
            }

            if (_stopDate != null)
            {
                _stopDate.WriteXml("stop-date", writer);
            }

            if (!String.IsNullOrEmpty(_stopReason))
            {
                writer.WriteElementString("stop-reason", _stopReason);
            }

            // </condition>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the condition.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> on set.
        /// </exception>
        /// 
        public CodableValue Name
        {
            get { return _name; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Name", "ConditionNameMandatory");
                _name = value; 
            }
        }
        private CodableValue _name = new CodableValue();

        /// <summary>
        /// Gets or sets the approximate date of the first occurrence of the
        /// condition.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="ApproximateDateTime"/> representing the 
        /// date of the first occurrence.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the onset date should not be stored.
        /// </remarks>
        /// 
        public ApproximateDateTime OnsetDate
        {
            get { return _onsetDate; }
            set { _onsetDate = value; }
        }
        private ApproximateDateTime _onsetDate;
        
        /// <summary>
        /// Gets or sets the status of the condition.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> representing the status.
        /// </value>
        /// 
        /// <remarks>
        /// Examples of the status include values such as acute or chronic.
        /// <br/><br/>
        /// Set the value to <b>null</b> if the status should not be stored.
        /// </remarks>
        /// 
        public CodableValue Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private CodableValue _status;

        /// <summary>
        /// Gets or sets the approximate date the condition resolved.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="ApproximateDateTime"/> representing the date.
        /// </value>
        /// 
        /// <remarks>
        /// For multiple acute episodes, this is the last date the condition
        /// resolved.
        /// <br/><br/>
        /// Set the value to <b>null</b> if the stop date should not be stored.
        /// </remarks>
        /// 
        public ApproximateDateTime StopDate
        {
            get { return _stopDate; }
            set { _stopDate = value; }
        }
        private ApproximateDateTime _stopDate;


        /// <summary>
        /// Gets or sets how the condition was resolved.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the condition resolution.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the reason should not be stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string StopReason
        {
            get { return _stopReason; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "StopReason");
                _stopReason = value;
            }
        }
        private string _stopReason;

        /// <summary>
        /// Gets a string representation of the condition item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the condition item.
        /// </returns>
        /// 
        public override string ToString()
        {
            return Name.Text;
        }
    }
}
