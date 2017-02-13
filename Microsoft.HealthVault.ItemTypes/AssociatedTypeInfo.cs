// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// The HealthVault thing type associated with a goal or task.
    /// </summary>
    ///
    public class AssociatedTypeInfo : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AssociatedTypeInfo"/> class with default values.
        /// </summary>
        ///
        public AssociatedTypeInfo()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="AssociatedTypeInfo"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="thingTypeVersionId">
        /// The version ID of the HealthVault type associated with this goal or task.
        /// </param>
        ///
        public AssociatedTypeInfo(Guid thingTypeVersionId)
        {
            ThingTypeVersionId = thingTypeVersionId;
        }
        
        /// <summary>
        /// Populates this <see cref="AssociatedTypeInfo"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the AssociatedTypeInfo data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException(
                    nameof(navigator),
                    ResourceRetriever.GetResourceString(
                        "errors", "ParseXmlNavNull"));
            }

            Guid? thingTypeVersionId = XPathHelper.GetOptNavValueAsGuid(navigator, "thing-type-version-id");

            Validator.ThrowInvalidIfNull(thingTypeVersionId, "AssociatedThingTypeVersionIdNullorEmpty");
            Validator.ThrowInvalidIf(thingTypeVersionId.Equals(Guid.Empty), "AssociatedThingTypeVersionIdNullorEmpty");

            _thingTypeVersionId = thingTypeVersionId.Value;
            _thingTypeValueXPath = XPathHelper.GetOptNavValue(navigator, "thing-type-value-xpath");
            _thingTypeDisplayXPath = XPathHelper.GetOptNavValue(navigator, "thing-type-display-xpath");
        }
        
        /// <summary>
        /// Writes the XML representation of the AssociatedTypeInfo into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the associated type info.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the AssociatedTypeInfo should be
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentException(
                    ResourceRetriever.GetResourceString(
                        "errors", "WriteXmlEmptyNodeName"), 
                    nameof(nodeName));
            }
            
            if (writer == null)
            {
                throw new ArgumentNullException(
                    nameof(writer),
                    ResourceRetriever.GetResourceString(
                        "errors", "WriteXmlNullWriter"));
            }

            writer.WriteStartElement(nodeName);

            Validator.ThrowInvalidIf(_thingTypeVersionId.Equals(Guid.Empty), "AssociatedThingTypeVersionIdNullorEmpty");

            XmlWriterHelper.WriteOptGuid(writer, "thing-type-version-id", _thingTypeVersionId);
            XmlWriterHelper.WriteOptString(writer, "thing-type-value-xpath", _thingTypeValueXPath);
            XmlWriterHelper.WriteOptString(writer, "thing-type-display-xpath", _thingTypeDisplayXPath);
            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Gets or sets the version ID of the HealthVault type associated with this goal or task.
        /// </summary>
        /// 
        /// <remarks>
        /// Thing type version ID is used to specify measurements relevant for this goal or task.
        /// </remarks>
        ///
        public Guid ThingTypeVersionId
        {
            get
            {
                return _thingTypeVersionId;
            }
            
            set
            {
                Validator.ThrowArgumentExceptionIf(value.Equals(Guid.Empty), "thingTypeVersionId", "AssociatedThingTypeVersionIdNullorEmpty");
                _thingTypeVersionId = value;
            }
        }
        
        private Guid _thingTypeVersionId;
        
        /// <summary>
        /// Gets or sets xPath expression for the value field associated with this goal or task in the thing type.
        /// </summary>
        /// 
        /// <remarks>
        /// Thing type value XPath could be used to specify which element in a thing type defined by the thing-type-version-id can be used to find the measurements. The XPath can also include a condition such as steps greater than 1000.
        /// If there is no information about thingTypeValueXpath the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string ThingTypeValueXPath
        {
            get
            {
                return _thingTypeValueXPath;
            }
            
            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(
                        ResourceRetriever.GetResourceString("errors", "WhitespaceOnlyValue"), nameof(value));
                }
                
                _thingTypeValueXPath = value;
            }
        }
        
        private string _thingTypeValueXPath;
        
        /// <summary>
        /// Gets or sets xPath expression for the display field associated with this goal or task in the thing type.
        /// </summary>
        /// 
        /// <remarks>
        /// Thing type display XPath should point to a "display-value" element in the thing XML for the type defined by the thing-type-version-id.
        /// If there is no information about thingTypeDisplayXpath the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string ThingTypeDisplayXPath
        {
            get
            {
                return _thingTypeDisplayXPath;
            }
            
            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(
                        ResourceRetriever.GetResourceString("errors", "WhitespaceOnlyValue"), nameof(value));
                }
                
                _thingTypeDisplayXPath = value;
            }
        }
        
        private string _thingTypeDisplayXPath;
    }
}
