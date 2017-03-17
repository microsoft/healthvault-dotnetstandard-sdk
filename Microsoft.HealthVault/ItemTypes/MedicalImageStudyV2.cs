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
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a study containing medical images.
    /// </summary>
    ///
    /// <remarks>
    /// DICOM data is stored in the named BLOB portion of the data type, and it is recommended
    /// that normal DICOM conventions are used for naming.
    ///
    /// An application may store XDS-i manifest information in XML format in the xds-i element.
    /// </remarks>
    ///
    public class MedicalImageStudyV2 : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudyV2"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public MedicalImageStudyV2()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MedicalImageStudyV2"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        /// <param name="series">
        /// Medical image study series
        /// </param>
        /// <param name="when">
        /// Time that the study was created.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="series"/> parameter is <b>null</b> or doesn't contain any series.
        /// </exception>
        ///
        public MedicalImageStudyV2(
            HealthServiceDateTime when,
            IEnumerable<MedicalImageStudySeriesV2> series)
            : base(TypeId)
        {
            Validator.ThrowIfArgumentNull(series, nameof(series), Resources.MedicalImageStudySeriesMandatory);

            this.When = when;
            foreach (MedicalImageStudySeriesV2 medicalImageStudySeries in series)
            {
                this.Series.Add(medicalImageStudySeries);
            }

            if (this.Series.Count == 0)
            {
                throw new ArgumentException(Resources.MedicalImageStudySeriesMandatory, nameof(series));
            }
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("cdfc0a9b-6d3b-4d16-afa8-02b86d621a8d");

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the medical image study data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a medical image study node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("medical-image-study");

            Validator.ThrowInvalidIfNull(itemNav, Resources.MedicalImageStudyUnexpectedNode);

            if (this.when == null)
            {
                this.when = new HealthServiceDateTime();
            }

            this.when.ParseXml(itemNav.SelectSingleNode("when"));
            this.patientName = XPathHelper.GetOptNavValue(itemNav, "patient-name");
            this.description = XPathHelper.GetOptNavValue(itemNav, "description");

            this.series.Clear();
            foreach (XPathNavigator imageStudySeriesNav in itemNav.Select("series"))
            {
                MedicalImageStudySeriesV2 imageStudySeries = new MedicalImageStudySeriesV2();
                imageStudySeries.ParseXml(imageStudySeriesNav);
                this.series.Add(imageStudySeries);
            }

            this.reason = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "reason");
            this.previewBlobName = XPathHelper.GetOptNavValue(itemNav, "preview-blob-name");

            this.keyImages.Clear();
            foreach (XPathNavigator imageNav in itemNav.Select("key-images"))
            {
                MedicalImageStudySeriesImage image = new MedicalImageStudySeriesImage();
                image.ParseXml(imageNav);
                this.keyImages.Add(image);
            }

            this.studyInstanceUID = XPathHelper.GetOptNavValue(itemNav, "study-instance-uid");
            this.referringPhysician = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "referring-physician");
            this.accessionNumber = XPathHelper.GetOptNavValue(itemNav, "accession-number");
        }

        /// <summary>
        /// Writes the XML representation of the medical image study into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the medical image study series should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// If <see cref="Series"/> collection is <b>null</b> or empty.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNullValue);

            if (this.series == null || this.series.Count == 0)
            {
                throw new ThingSerializationException(Resources.MedicalImageStudySeriesMandatory);
            }

            writer.WriteStartElement("medical-image-study");

            this.when.WriteXml("when", writer);
            XmlWriterHelper.WriteOptString(writer, "patient-name", this.patientName);
            XmlWriterHelper.WriteOptString(writer, "description", this.description);

            foreach (MedicalImageStudySeriesV2 medicalImageStudySeries in this.series)
            {
                medicalImageStudySeries.WriteXml("series", writer);
            }

            XmlWriterHelper.WriteOpt(writer, "reason", this.reason);
            XmlWriterHelper.WriteOptString(writer, "preview-blob-name", this.previewBlobName);

            foreach (MedicalImageStudySeriesImage image in this.keyImages)
            {
                image.WriteXml("key-images", writer);
            }

            XmlWriterHelper.WriteOptString(writer, "study-instance-uid", this.studyInstanceUID);
            XmlWriterHelper.WriteOpt(writer, "referring-physician", this.referringPhysician);
            XmlWriterHelper.WriteOptString(writer, "accession-number", this.accessionNumber);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the time that the study was created.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tags (0008, 0020) and (0008, 0030).
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get
            {
                return this.when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when;

        /// <summary>
        /// Gets or sets the name of the patient as contained in the medical image.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0010, 0010).
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PatientName
        {
            get
            {
                return this.patientName;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PatientName");
                this.patientName = value;
            }
        }

        private string patientName;

        /// <summary>
        /// Gets or sets a description of the study.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0008, 1030).
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
        /// Gets the collection of image series.
        /// </summary>
        ///
        public Collection<MedicalImageStudySeriesV2> Series => this.series;

        private readonly Collection<MedicalImageStudySeriesV2> series = new Collection<MedicalImageStudySeriesV2>();

        /// <summary>
        /// Gets or sets the reason for the study.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0032, 1030).
        ///
        /// If there is no information about the reason the value should be set
        /// to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue Reason
        {
            get { return this.reason; }
            set { this.reason = value; }
        }

        private CodableValue reason;

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
        /// Gets the important images in the study.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the key images the collection should be empty.
        /// </remarks>
        ///
        public Collection<MedicalImageStudySeriesImage> KeyImages => this.keyImages;

        private readonly Collection<MedicalImageStudySeriesImage> keyImages = new Collection<MedicalImageStudySeriesImage>();

        /// <summary>
        /// Gets or sets the study instance UID.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0020,000D).
        /// If there is no study instance UID the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string StudyInstanceUID
        {
            get
            {
                return this.studyInstanceUID;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "StudyInstanceUID");
                this.studyInstanceUID = value;
            }
        }

        private string studyInstanceUID;

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
            get { return this.referringPhysician; }
            set { this.referringPhysician = value; }
        }

        private PersonItem referringPhysician;

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        ///
        /// <remarks>
        /// This value corresponds to DICOM tag (0008,0050).
        /// If there is no Accession Number the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string AccessionNumber
        {
            get
            {
                return this.accessionNumber;
            }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "AccessionNumber");
                this.accessionNumber = value;
            }
        }

        private string accessionNumber;

        /// <summary>
        /// Gets a string representation of the medical image study.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the medical image study.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (!string.IsNullOrEmpty(this.Description) && !string.IsNullOrEmpty(this.Description.Trim()))
            {
                result.Append(this.Description);
            }

            if (this.Reason != null)
            {
                if (result.Length > 0)
                {
                    result.Append(Resources.ListSeparator);
                }

                result.Append(this.Reason);
            }

            return result.ToString();
        }
    }
}
