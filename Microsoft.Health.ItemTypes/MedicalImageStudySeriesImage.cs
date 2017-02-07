// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents the details of a single image in a series.
    /// </summary>
    public class MedicalImageStudySeriesImage : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeriesImage"/> class with default values.
        /// </summary>
        public MedicalImageStudySeriesImage()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeriesImage"/> class
        /// specifying mandatory values.
        /// </summary>
        /// 
        /// <param name="imageBlobName">
        /// The name of the BLOB holding the image.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="imageBlobName"/> parameter is <b>null</b> or empty or contains only whitespace.
        /// </exception>
        /// 
        public MedicalImageStudySeriesImage(string imageBlobName)
        {
            ImageBlobName = imageBlobName;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the medical image study series image.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _imageBlobName = navigator.SelectSingleNode("image-blob-name").Value;
            _imagePreviewBlobName = XPathHelper.GetOptNavValue(navigator, "image-preview-blob-name");
        }

        /// <summary>
        /// Writes the XML representation of the medical image study series image into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series image.
        /// </param>
        /// Name of the BLOB holding the image.
        /// <param name="writer">
        /// The XML writer into which the medical image study series image should be 
        /// written.
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
        /// If <see cref="ImageBlobName"/> is <b>null</b> or empty or contains only whitespace.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIf(
                string.IsNullOrEmpty(_imageBlobName) ||
                string.IsNullOrEmpty(_imageBlobName.Trim()),
                "ImageBlobNameMandatory");

            writer.WriteStartElement(nodeName);

            writer.WriteElementString("image-blob-name", _imageBlobName);
            XmlWriterHelper.WriteOptString(writer, "image-preview-blob-name", _imagePreviewBlobName);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the BLOB holding the image.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0008,0018).
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        /// 
        public string ImageBlobName
        {
            get { return _imageBlobName; }
            set 
            {
                Validator.ThrowIfStringNullOrEmpty(value, "ImageBlobName");
                Validator.ThrowIfStringIsWhitespace(value, "ImageBlobName");
                _imageBlobName = value; 
            }
        }
        private string _imageBlobName;

        /// <summary>
        /// Gets or sets the name of the BLOB holding a smaller version of the image
        /// suitable for web viewing or email.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no image preview BLOB name the value should be set to <b>null</b>.
        /// Previews should be stored using the jpeg or png format.
        /// It is recommended that the shorter dimension of the image be no less than 500 pixels in size.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string ImagePreviewBlobName
        {
            get { return _imagePreviewBlobName; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "ImagePreviewBlobName");
                _imagePreviewBlobName = value; 
            }
        }
        private string _imagePreviewBlobName;

        /// <summary>
        /// Gets a string representation of the medical image study series image.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the medical image study series image.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (!string.IsNullOrEmpty(ImageBlobName) && !string.IsNullOrEmpty(ImageBlobName.Trim()))
            {
                result.Append(ImageBlobName);
            }
            
            return result.ToString();
        }
    }
}
