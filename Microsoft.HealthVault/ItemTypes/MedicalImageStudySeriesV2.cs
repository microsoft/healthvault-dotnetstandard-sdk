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
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the details of a specific series of images in a medical image study.
    /// </summary>
    public class MedicalImageStudySeriesV2 : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeriesV2"/> class with default values.
        /// </summary>
        ///
        public MedicalImageStudySeriesV2()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudySeriesV2"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="images">
        /// Medical image study series images.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="images"/> parameter is <b>null</b> or doesn't contain any images.
        /// </exception>
        ///
        public MedicalImageStudySeriesV2(ICollection<MedicalImageStudySeriesImage> images)
        {
            Validator.ThrowIfArgumentNull(images, nameof(images), Resources.ImagesMandatory);

            if (images.Count == 0)
            {
                throw new ArgumentException(Resources.ImagesMandatory, nameof(images));
            }

            foreach (MedicalImageStudySeriesImage image in images)
            {
                this.images.Add(image);
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

            this.acquisitionDateTime = XPathHelper.GetOptNavValue<HealthServiceDateTime>(navigator, "acquisition-datetime");
            this.description = XPathHelper.GetOptNavValue(navigator, "description");

            this.images.Clear();
            XPathNodeIterator imageIterator = navigator.Select("images");
            foreach (XPathNavigator imageNav in imageIterator)
            {
                MedicalImageStudySeriesImage image = new MedicalImageStudySeriesImage();
                image.ParseXml(imageNav);
                this.images.Add(image);
            }

            this.institutionName = XPathHelper.GetOptNavValue<Organization>(navigator, "institution-name");
            this.modality = XPathHelper.GetOptNavValue<CodableValue>(navigator, "modality");
            this.bodyPart = XPathHelper.GetOptNavValue<CodableValue>(navigator, "body-part");
            this.previewBlobName = XPathHelper.GetOptNavValue(navigator, "preview-blob-name");
            this.seriesInstanceUID = XPathHelper.GetOptNavValue(navigator, "series-instance-uid");
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="AcquisitionDateTime"/> is <b>null</b>.
        /// If <see cref="Images"/> is <b>null</b> or doesn't contain any image.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            if (this.images == null || this.images.Count == 0)
            {
                throw new ThingSerializationException(Resources.ImagesMandatory);
            }

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteOpt(writer, "acquisition-datetime", this.acquisitionDateTime);
            XmlWriterHelper.WriteOptString(writer, "description", this.description);

            foreach (MedicalImageStudySeriesImage image in this.images)
            {
                XmlWriterHelper.WriteOpt(writer, "images", image);
            }

            XmlWriterHelper.WriteOpt(writer, "institution-name", this.institutionName);
            XmlWriterHelper.WriteOpt(writer, "modality", this.modality);
            XmlWriterHelper.WriteOpt(writer, "body-part", this.bodyPart);
            XmlWriterHelper.WriteOptString(writer, "preview-blob-name", this.previewBlobName);
            XmlWriterHelper.WriteOptString(writer, "series-instance-uid", this.seriesInstanceUID);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time that the image was acquired.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tags (0008, 0022) and (0008, 0032).
        ///
        /// If there is no information about the acquisition date time the value should be set
        /// to <b>null</b>.
        /// </remarks>
        ///
        public HealthServiceDateTime AcquisitionDateTime
        {
            get { return this.acquisitionDateTime; }
            set { this.acquisitionDateTime = value; }
        }

        private HealthServiceDateTime acquisitionDateTime;

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
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets medical images.
        /// </summary>
        ///
        public Collection<MedicalImageStudySeriesImage> Images => this.images;

        private readonly Collection<MedicalImageStudySeriesImage> images = new Collection<MedicalImageStudySeriesImage>();

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
            get { return this.institutionName; }
            set { this.institutionName = value; }
        }

        private Organization institutionName;

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
            get { return this.modality; }
            set { this.modality = value; }
        }

        private CodableValue modality;

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
            get { return this.bodyPart; }
            set { this.bodyPart = value; }
        }

        private CodableValue bodyPart;

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
        public string PreviewBlobName
        {
            get
            {
                return this.previewBlobName;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PreviewBlobName");
                this.previewBlobName = value;
            }
        }

        private string previewBlobName;

        /// <summary>
        /// Gets or sets the series instance UID.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0020,000E)
        ///
        /// If there is no series instance UID, the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string SeriesInstanceUID
        {
            get
            {
                return this.seriesInstanceUID;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "SeriesInstanceUID");
                this.seriesInstanceUID = value;
            }
        }

        private string seriesInstanceUID;

        /// <summary>
        /// Gets a string representation of the medical image study series.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the medical image study series.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (!string.IsNullOrEmpty(this.Description) && !string.IsNullOrEmpty(this.Description.Trim()))
            {
                result.Append(this.Description);

                if (result.Length > 0)
                {
                    result.Append(" ");
                }

                result.Append(Resources.OpenParen);

                if (this.BodyPart != null)
                {
                    result.Append(this.BodyPart.Text);
                }

                if (this.AcquisitionDateTime != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Resources.ListSeparator);
                    }

                    result.Append(this.AcquisitionDateTime);
                }

                if (this.InstitutionName != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Resources.ListSeparator);
                    }

                    result.Append(this.InstitutionName.Name);
                }

                result.Append(Resources.CloseParen);
            }

            return result.ToString();
        }
    }
}
