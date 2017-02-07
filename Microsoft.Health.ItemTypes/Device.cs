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
    /// Represents a health record item type that encapsulates a medical device.
    /// </summary>
    /// 
    public class Device : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Device"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Device()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Device"/> class with the 
        /// specified date and time.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date and  time relevant for the medical device.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public Device(HealthServiceDateTime when)
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
            new Guid("EF9CF8D5-6C0B-4292-997F-4047240BC7BE");

        /// <summary>
        /// Populates this medical device instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the medical device data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a medical device node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("device");

            Validator.ThrowInvalidIfNull(itemNav, "DeviceUnexpectedNode");

            // <when>
            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <device-name>
            _deviceName = XPathHelper.GetOptNavValue(itemNav, "device-name");

            // <vendor>
            _vendor = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "vendor");

            // <model>
            _model = XPathHelper.GetOptNavValue(itemNav, "model");

            // <serial-number>
            _serialNumber = 
                XPathHelper.GetOptNavValue(itemNav, "serial-number");

            // <anatomic-site>
            _anatomicSite =
                XPathHelper.GetOptNavValue(itemNav, "anatomic-site");

            // <description>
            _description =
                XPathHelper.GetOptNavValue(itemNav, "description");
        }

        /// <summary>
        /// Writes the medical device data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the medical device data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "DeviceWhenNotSet");

            // <device>
            writer.WriteStartElement("device");

            // <when>
            _when.WriteXml("when", writer);

            // <device-name>
            XmlWriterHelper.WriteOptString(
                writer,
                "device-name",
                _deviceName);

            // <vendor>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "vendor", 
                _vendor);

            // <model>
            XmlWriterHelper.WriteOptString(
                writer,
                "model", 
                _model);
            
            // <serial-number>
            XmlWriterHelper.WriteOptString(
                writer,
                "serial-number", 
                _serialNumber);

            // <anatomic-site>
            XmlWriterHelper.WriteOptString(
                writer,
                "anatomic-site", 
                _anatomicSite);
            
            // <description>
            XmlWriterHelper.WriteOptString(
                writer,
                "description", 
                _description);

            // </device>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time relevant for the medical device.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing 
        /// the date. The default value is the current year, month, and day.
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
        /// Gets or sets the device name of the medical device.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the device name.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the device name should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string DeviceName
        {
            get { return _deviceName; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "DeviceName");
                _deviceName = value;
            }
        }
        private string _deviceName;

        /// <summary>
        /// Gets or sets the vendor contact information.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the vendor contact information
        /// should not be stored.
        /// </remarks>
        /// 
        public PersonItem Vendor
        {
            get { return _vendor; }
            set { _vendor = value; }
        }
        private PersonItem _vendor;

        /// <summary>
        /// Gets or sets the model of the medical device.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the device model.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the model should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Model
        {
            get { return _model; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Model");
                _model = value;
            }
        }
        private string _model;

        /// <summary>
        /// Gets or sets the serial number of the medical device.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the device serial number.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the serial number should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string SerialNumber
        {
            get { return _serialNumber; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "SerialNumber");
                _serialNumber = value;
            }
        }
        private string _serialNumber;

        /// <summary>
        /// Gets or sets the position on the body from which the device 
        /// takes readings.
        /// </summary>
        /// 
        ///<value>
        /// A string representing the position.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the anatomic site should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string AnatomicSite
        {
            get { return _anatomicSite; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "AnatomicSite");
                _anatomicSite = value;
            }
        }
        private string _anatomicSite;
        
        /// <summary>
        /// Gets or sets the description of the medical device.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the device description.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the description should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Description
        {
            get { return _description; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }
        private string _description;

        /// <summary>
        /// Gets a string representation of the device item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the device item.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (DeviceName != null)
            {
                result.Append(DeviceName);
            }

            if (Model != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Model);
            }

            if (Vendor != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Vendor.ToString());
            }

            if (AnatomicSite != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    AnatomicSite);
            }

            if (Description != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Description);
            }

            return result.ToString();
        }
    }
}
