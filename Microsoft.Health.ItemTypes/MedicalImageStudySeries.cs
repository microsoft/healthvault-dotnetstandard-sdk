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
    /// Represents the details of a specific series of images in a medical image study.
    /// </summary>
    public class MedicalImageStudySeries : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeries"/> class with default values.
        /// </summary>
        /// 
        public MedicalImageStudySeries()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeries"/> class
        /// specifying mandatory values.
        /// </summary>
        /// 
        /// <param name="acquisitionDateTime">
        /// The date and time that the image was acquired.
        /// </param>
        /// <param name="images">
        /// Medical image study series images. 
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="images"/> parameter is <b>null</b> or doesn't contain any images.
        /// </exception>
        /// 
        public MedicalImageStudySeries(
            HealthServiceDateTime acquisitionDateTime,
            ICollection<MedicalImageStudySeriesImage> images)
        {
            Validator.ThrowIfArgumentNull(images, "images", "ImagesMandatory");

            Validator.ThrowArgumentExceptionIf(
                images.Count == 0,
                "images",
                "ImagesMandatory");

            AcquisitionDateTime = acquisitionDateTime;
            foreach (MedicalImageStudySeriesImage image in images)
            {
                _images.Add(image);
            }
        }
        
        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the medical image study series.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            if (_acquisitionDateTime == null)
            {
                _acquisitionDateTime = new HealthServiceDateTime();
            }
            _acquisitionDateTime.ParseXml(navigator.SelectSingleNode("acquisition-datetime"));

            _description = XPathHelper.GetOptNavValue(navigator, "description");

            _images.Clear();

            XPathNodeIterator imageIterator = navigator.Select("images");
            foreach (XPathNavigator imageNav in imageIterator)
            {
                MedicalImageStudySeriesImage image = new MedicalImageStudySeriesImage();
                image.ParseXml(imageNav);
                _images.Add(image);
            }
            _institutionName = XPathHelper.GetOptNavValue<Organization>(navigator, "institution-name");
            _referringPhysician = XPathHelper.GetOptNavValue<PersonItem>(navigator, "referring-physician");
            _modality = XPathHelper.GetOptNavValue<CodableValue>(navigator, "modality");
            _bodyPart = XPathHelper.GetOptNavValue<CodableValue>(navigator, "body-part");
            _previewBlobName = XPathHelper.GetOptNavValue(navigator, "preview-blob-name");
            _seriesInstanceUID = XPathHelper.GetOptNavValue(navigator, "series-instance-uid");
        }

        /// <summary>
        /// Writes the XML representation of the medical image study series into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the medical image study series should be 
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
        /// If <see cref="AcquisitionDateTime"/> is <b>null</b>.
        /// If <see cref="Images"/> is <b>null</b> or doesn't contain any image. 
        /// </exception>
        /// 
        public override void WriteXml(String nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_acquisitionDateTime, "AcquisitionDateTimeMandatory");
            Validator.ThrowSerializationIf(
                _images == null || _images.Count == 0,
                "ImagesMandatory");

            writer.WriteStartElement(nodeName);
            
            // acquisition-datetime
            _acquisitionDateTime.WriteXml("acquisition-datetime", writer);
            // description 
            XmlWriterHelper.WriteOptString(writer, "description", _description);
            // images
            foreach (MedicalImageStudySeriesImage image in _images)
            {
                XmlWriterHelper.WriteOpt<MedicalImageStudySeriesImage>(writer, "images", image);
            }
            // institution-name
            XmlWriterHelper.WriteOpt<Organization>(writer, "institution-name", _institutionName);
            // referring-physician
            XmlWriterHelper.WriteOpt<PersonItem>(writer, "referring-physician", _referringPhysician);
            // modality
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "modality", _modality);
            // body-part
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "body-part", _bodyPart);
            // preview-blob-name
            XmlWriterHelper.WriteOptString(writer, "preview-blob-name", _previewBlobName);
            // series-instance-uid
            XmlWriterHelper.WriteOptString(writer, "series-instance-uid", _seriesInstanceUID);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time that the image was acquired.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tags (0008, 0022) and (0008, 0032).
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public HealthServiceDateTime AcquisitionDateTime
        {
            get { return _acquisitionDateTime; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "AcquisitionDateTime", "AcquisitionDateTimeMandatory");
                _acquisitionDateTime = value; 
            }
        }
        private HealthServiceDateTime _acquisitionDateTime;

        /// <summary>
        /// Gets or sets a description of the series.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 103E).
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public String Description
        {
            get { return _description; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value; 
            }
        }
        private String _description;

        /// <summary>
        /// Gets and sets medical images. 
        /// </summary>
        /// 
        public Collection<MedicalImageStudySeriesImage> Images
        {
            get { return _images; }
        }
        private Collection<MedicalImageStudySeriesImage> _images = new Collection<MedicalImageStudySeriesImage>();

        /// <summary>
        /// Gets or sets the name of the institution where the images were acquired.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 0080).
        /// 
        /// If there is no information about the institution the value should be set 
        /// to <b>null</b>.
        /// </remarks>
        /// 
        public Organization InstitutionName
        {
            get { return _institutionName; }
            set { _institutionName = value; }
        }
        private Organization _institutionName;

        /// <summary>
        /// Gets or sets the physician who ordered the study.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 0090).
        /// 
        /// If there is no information about the physician the value should be set 
        /// to <b>null</b>.
        /// </remarks>
        /// 
        public PersonItem ReferringPhysician
        {
            get { return _referringPhysician; }
            set { _referringPhysician = value; }
        }
        private PersonItem _referringPhysician;

        /// <summary>
        /// Gets or sets the method (or modality) in which the images were acquired.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 0060).
        /// 
        /// If there is no information about the modality the value should be set to <b>null</b>.
        ///
        /// The preferred vocabulary is dicom.modality.
        /// </remarks>
        /// 
        public CodableValue Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }
        private CodableValue _modality;

        /// <summary>
        /// Gets or sets the body part that was imaged.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0018, 0015).
        /// 
        /// If there is no information about the body part the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public CodableValue BodyPart
        {
            get { return _bodyPart; }
            set { _bodyPart = value; }
        }
        private CodableValue _bodyPart;

        /// <summary>
        /// Gets or sets the name of the BLOB holding a smaller version of the image
        /// suitable for web viewing or email.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no preview BLOB name the value should be set to <b>null</b>.
        /// Previews should be stored using the jpeg or png format.
        /// It is recommended that the shorter dimension of the image be no less than 500 pixels in size.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public String PreviewBlobName
        {
            get { return _previewBlobName; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "PreviewBlobName");
                _previewBlobName = value; 
            }
        }
        private String _previewBlobName;

        /// <summary>
        /// Gets or sets the series instance UID.
        /// </summary>
        /// 
        /// <remarks>
        /// This value corresponds to DICOM tag (0020,000E)
        /// 
        /// If there is no series instnace UID, the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public String SeriesInstanceUID
        {
            get { return _seriesInstanceUID; }
            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SeriesInstanceUID");
                _seriesInstanceUID = value;
            }
        }
        private String _seriesInstanceUID;

        
        /// <summary>
        /// Gets a string representation of the medical image study series.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the medical image study series.
        /// </returns>
        /// 
        public override String ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (!String.IsNullOrEmpty(Description) && !String.IsNullOrEmpty(Description.Trim()))
            {
                result.Append(Description);

                if (result.Length > 0)
                {
                    result.Append(ResourceRetriever.GetSpace("errors"));
                }

                result.Append(ResourceRetriever.GetResourceString("errors", "OpenParen"));

                if (BodyPart != null)
                {
                    result.Append(BodyPart.Text);
                }

                if (AcquisitionDateTime != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(ResourceRetriever.GetResourceString("errors", "ListSeparator")); 
                    }
                    result.Append(AcquisitionDateTime.ToString());
                }

                if (InstitutionName != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(ResourceRetriever.GetResourceString("errors", "ListSeparator"));
                    }
                    result.Append(InstitutionName.Name.ToString());
                }

                if (ReferringPhysician != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(ResourceRetriever.GetResourceString("errors", "ListSeparator"));
                    }
                    result.Append(ReferringPhysician.Name.ToString());
                }

                result.Append(ResourceRetriever.GetResourceString("errors", "CloseParen"));
            }

            return result.ToString();
        }
    }
}
