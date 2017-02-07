// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a duration of 
    /// time with a starting and optional ending date.
    /// </summary>
    /// 
    public class DurationValue : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DurationValue"/> class with default 
        /// values.
        /// </summary>
        /// 
        public DurationValue()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DurationValue"/> class with 
        /// the specified starting date.
        /// </summary>
        /// 
        /// <param name="startDate">
        /// The starting date/time of the duration value.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="startDate"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public DurationValue(ApproximateDateTime startDate)
        {
            this.StartDate = startDate;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="DurationValue"/> class with 
        /// the specified starting and ending date.
        /// </summary>
        /// 
        /// <param name="startDate">
        /// The start date/time of the duration value.
        /// </param>
        /// 
        /// <param name="endDate">
        /// The end date/time of the duration value.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="startDate"/> or <paramref name="endDate"/> 
        /// parameter is <b>null</b>.
        /// </exception>
        /// 
        public DurationValue(
            ApproximateDateTime startDate, 
            ApproximateDateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        /// <summary>
        /// Populates this Person instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the duration information.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="navigator"/> is not
        /// a person node.
        /// </exception>
        /// 
        public override void  ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // <start-date>
            _startDate = new ApproximateDateTime();
            _startDate.ParseXml(navigator.SelectSingleNode("start-date"));

            // <end-date>
            _endDate =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    navigator,
                    "end-date");
        }

        /// <summary>
        /// Writes the duration value to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the duration value.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the duration value to.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="StartDate"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_startDate, "DurationValueStartDateNotSet");
            
            // <duration-value>
            writer.WriteStartElement(nodeName);

            _startDate.WriteXml("start-date", writer);

            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "end-date",
                _endDate);

            // </duration-value>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the approximate date of the start of the duration.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="ApproximateDateTime"/> representing the start of the duration.
        /// </value>
        /// 
        /// <remarks>
        /// An approximate date must have a year and can also have the month, 
        /// day, or both.
        /// </remarks>
        /// 
        public ApproximateDateTime StartDate
        {
            get { return _startDate; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "StartDate", "DurationValueStartDateMandatory");
                _startDate = value;
            }
        }
        private ApproximateDateTime _startDate = new ApproximateDateTime();

        /// <summary>
        /// Gets or sets the approximate date of the end of the duration.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="ApproximateDateTime"/> representing the end of the duration.
        /// </value>
        /// 
        /// <remarks>
        /// An approximate date must have a year and can also have the month, 
        /// day, or both.
        /// </remarks>
        /// 
        public ApproximateDateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        private ApproximateDateTime _endDate;

        /// <summary>
        /// Gets a string representation of the duration value.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the duration value.
        /// </returns>
        /// 
        public override string ToString()
        {
            String result = String.Empty;

            if (EndDate != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "DateRange"),
                        StartDate.ToString(),
                        EndDate.ToString());
            }
            else
            {
                result = StartDate.ToString();
            }
            return result;
        }
     }

}
