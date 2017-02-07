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
    /// An attachment to the message.
    /// </summary>
    ///
    public class MessageAttachment : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MessageAttachment"/> class with default values.
        /// </summary>
        ///
        public MessageAttachment()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="MessageAttachment"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the attachment.
        /// </param>
        /// <param name="blobName">
        /// The name of the blob storing the attachment
        /// </param>
        /// <param name="inlineDisplay">
        /// If true the attachment is intended to be displayed inline with the text.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b>, empty or contains only whitespace.
        /// If <paramref name="blobName"/> is <b>null</b>, empty or contains only whitespace.
        /// </exception>
        ///
        public MessageAttachment(
            string name,
            string blobName,
            bool inlineDisplay)
        {
            Name = name;
            BlobName = blobName;
            InlineDisplay = inlineDisplay;
        }
        
        /// <summary>
        /// Populates this <see cref="MessageAttachment"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the MessageAttachment data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(navigator, "navigator", "ParseXmlNavNull");
            
            _name = navigator.SelectSingleNode("name").Value;
            _blobName = navigator.SelectSingleNode("blob-name").Value;
            _inlineDisplay = navigator.SelectSingleNode("inline-display").ValueAsBoolean;
            _contentId = XPathHelper.GetOptNavValue(navigator, "content-id");
        }
        
        /// <summary>
        /// Writes the XML representation of the MessageAttachment into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the MessageAttachment should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b>null</b> or empty or contains only whitespace.
        /// If <see cref="BlobName"/> is <b>null</b> or empty or contains only whitespace.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIf(
                String.IsNullOrEmpty(_name) || String.IsNullOrEmpty(_name.Trim()),
                "MessageNameMandatory");

            Validator.ThrowSerializationIf(
                String.IsNullOrEmpty(_blobName) || String.IsNullOrEmpty(_blobName.Trim()),
                "BlobNameMandatory");
            
            writer.WriteStartElement("attachments");
            
            writer.WriteElementString("name", _name);
            writer.WriteElementString("blob-name", _blobName);
            writer.WriteElementString("inline-display", SDKHelper.XmlFromBool(_inlineDisplay));
            XmlWriterHelper.WriteOptString(writer, "content-id", _contentId);
            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Gets or sets the name of the attachment.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no information about name the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Name
        {
            get
            {
                return _name;
            }
            
            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                
                _name = value;
            }
        }
        
        private string _name;
        
        /// <summary>
        /// Gets or sets the name of the blob storing the attachment.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no information about blobName the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string BlobName
        {
            get
            {
                return _blobName;
            }
            
            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "BlobName");
                Validator.ThrowIfStringNullOrEmpty(value, "BlobName");
                
                _blobName = value;
            }
        }
        
        private string _blobName;
        
        /// <summary>
        /// Gets or sets whether the attachment is intended to be displayed inline with the text.
        /// </summary>
        /// 
        /// <remarks>
        /// If true, the location of the attachment in the message can be found using the 
        /// <see cref="ContentId"/>.
        /// </remarks>
        /// 
        public bool InlineDisplay
        {
            get
            {
                return _inlineDisplay;
            }
            
            set
            {
                _inlineDisplay = value;
            }
        }
        
        private bool _inlineDisplay;
        
        /// <summary>
        /// Gets or sets the content identifier for attachments that will be displayed inline with the text.
        /// </summary>
        /// 
        /// <remarks>
        /// If this attachment is inline and the message text is available as HTML, the HTML contains 
        /// an img tag reference to this content id in the location where the attachment should be displayed.
        /// If there is no information about contentId the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string ContentId
        {
            get
            {
                return _contentId;
            }
            
            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "ContentId");

                _contentId = value;
            }
        }
        
        private string _contentId;
        
        /// <summary>
        /// Gets a string representation of the MessageAttachment.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the MessageAttachment.
        /// </returns>
        ///
        public override string ToString()
        {
            return _name;
        }
    }
}
