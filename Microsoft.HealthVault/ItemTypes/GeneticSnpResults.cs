// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.ItemTypes;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores a set of SNP genetic test results.
    /// </summary>
    ///
    /// <remarks>
    /// Examples:
    ///
    /// The SNP data is stored in the blob store of the object, and must
    /// be fetched by specifying HealthItemRecordSections.BlobPayload.
    ///
    /// The format of the other-data section is the HealthVault comma-separated format.
    ///
    /// Within the comma-separated data, each SNP is encoded as follows:
    ///
    /// [refSNP id],[strand orientation],[result],[assay id],[start position],[end position]
    ///
    /// Where:
    /// refSNP id: Reference SNP identifier from NCBI dbSNP database.
    ///  strand orientation: "+" encodes top, "-" encodes bottom.
    ///  result: the result of the test.
    ///  assay id: platform dependent probe set id.
    ///  start position: start position on the chromosome.
    ///  end position: end position on the chromosome.
    ///
    /// Example: rs1891906,-,GT, SNP_C-315533, 940106, 940107
    ///
    /// </remarks>
    ///
    public class GeneticSnpResults : ThingBase
    {
        /// <summary>
        /// Creates an instance of <see cref="GeneticSnpResults"/> with default values.
        /// </summary>
        ///
        public GeneticSnpResults()
            : base(TypeId)
        {
            Sections |= ThingSections.BlobPayload;
        }

        /// <summary>
        /// Creates an instance of <see cref="GeneticSnpResults"/> with specified
        /// parameters.
        /// </summary>
        ///
        /// <param name="when">The date and time of the SNP test.</param>
        ///
        /// <param name="genomeBuild">The genome build that defines the SNPs.</param>
        ///
        /// <param name="chromosome">The chromosome on which the SNPs are located.</param>
        ///
        /// <param name="numberingScheme">The numbering scheme used for positions.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b> null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="genomeBuild"/> or <paramref name="chromosome" />
        /// is <b>null</b> or empty, or <paramref name="numberingScheme"/> is not 0 or 1.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException" >
        /// The <paramref name="numberingScheme" /> is neither zero based scheme nor one
        /// based scheme.
        /// </exception>
        ///
        public GeneticSnpResults(
            ApproximateDateTime when,
            string genomeBuild,
            string chromosome,
            GenomeNumberingScheme numberingScheme)
            : base(TypeId)
        {
            When = when;
            GenomeBuild = genomeBuild;
            Chromosome = chromosome;
            NumberingScheme = numberingScheme;
            Sections |= ThingSections.BlobPayload;
        }

        /// <summary>
        /// Retrieves the unique identifier for the genetic snp result type.
        /// </summary>
        public static new readonly Guid TypeId =
            new Guid("9d006053-116c-43cc-9554-e0cda43558cb");

        /// <summary>
        /// Populates this <see cref="GeneticSnpResults"/> instance from the data
        /// in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the genetic snp result data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// an "genetic-snp-results" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("genetic-snp-results");

            Validator.ThrowInvalidIfNull(itemNav, Resources.GeneticSnpResultsUnexpectedNode);

            // <when> mandatory
            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <genome-build> mandatory
            _genomeBuild = itemNav.SelectSingleNode("genome-build").Value;

            // <chromosome> mandatory
            _chromosome = itemNav.SelectSingleNode("chromosome").Value;

            // <numbering-scheme> mandatory
            int numberingScheme = itemNav.SelectSingleNode("numbering-scheme").ValueAsInt;
            if ((numberingScheme == 0) || (numberingScheme == 1))
            {
                _numberingScheme = (GenomeNumberingScheme)numberingScheme;
            }
            else
            {
                _numberingScheme = GenomeNumberingScheme.Unknown;
            }

            // ordered-by
            _orderedBy =
                XPathHelper.GetOptNavValue<Organization>(itemNav, "ordered-by");

            // test-provider
            _testProvider =
                XPathHelper.GetOptNavValue<Organization>(itemNav, "test-provider");

            // laboratory-name
            _laboratoryName =
                XPathHelper.GetOptNavValue<Organization>(itemNav, "laboratory-name");

            // annotation-version
            _annotationVersion =
                XPathHelper.GetOptNavValue(itemNav, "annotation-version");

            // dbSNP-build
            _dbSnpBuild =
                XPathHelper.GetOptNavValue(itemNav, "dbSNP-build");

            // platform
            _platform =
                XPathHelper.GetOptNavValue(itemNav, "platform");
        }

        /// <summary>
        /// Writes the SNP test results to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the SNP test result data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> or <see cref="GenomeBuild"/> or <see cref="Chromosome" />
        /// is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);

            Validator.ThrowSerializationIfNull(_genomeBuild, Resources.GenomeBuildNotSet);

            Validator.ThrowSerializationIfNull(_chromosome, Resources.ChromosomeNotSet);

            if (_snpData != null)
            {
                _snpData.StoreSnpItems();

                if (!string.IsNullOrEmpty(_snpData.Data))
                {
                    Blob blob =
                        GetBlobStore(default(HealthRecordAccessor)).NewBlob(
                            string.Empty,
                            _snpData.ContentType);
                    blob.WriteInline(_snpData.Data);
                }
            }

            // <genetic-snp-results>
            writer.WriteStartElement("genetic-snp-results");

            // <when> mandatory
            _when.WriteXml("when", writer);

            // <genome-build> mandatory
            writer.WriteElementString("genome-build", _genomeBuild);

            // <chromosome>
            writer.WriteElementString("chromosome", _chromosome);

            // <numbering-scheme>
            writer.WriteElementString("numbering-scheme", ((int)_numberingScheme).ToString());

            // <ordered-by>
            XmlWriterHelper.WriteOpt(
                writer,
                "ordered-by",
                _orderedBy);

            // <test-provider>
            XmlWriterHelper.WriteOpt(
                writer,
                "test-provider",
                _testProvider);

            // <laboratory-name>
            XmlWriterHelper.WriteOpt(
                writer,
                "laboratory-name",
                _laboratoryName);

            // <annotation-version>
            XmlWriterHelper.WriteOptString(
                writer,
                "annotation-version",
                _annotationVersion);

            // <dbSNP-build>
            XmlWriterHelper.WriteOptString(
                writer,
                "dbSNP-build",
                _dbSnpBuild);

            // <platform>
            XmlWriterHelper.WriteOptString(
                writer,
                "platform",
                _platform);

            // </genetic-snp-results>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time when the samples were collected.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="ApproximateDateTime"/> instance representing the date
        /// and time.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private ApproximateDateTime _when;

        /// <summary>
        /// Gets or sets the genome build that defines the SNPs.
        /// </summary>
        ///
        /// <remarks>
        /// Example: NCBI Build 36.3.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException" >
        /// If the <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace.
        /// </exception>
        ///
        public string GenomeBuild
        {
            get { return _genomeBuild; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "GenomeBuild");
                Validator.ThrowIfStringIsWhitespace(value, "GenomeBuild");
                _genomeBuild = value;
            }
        }

        private string _genomeBuild;

        /// <summary>
        /// Gets or sets the chromosome on which the SNPs are located.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: 1, 22, X, MT
        /// </remarks>
        ///
        /// <exception cref="ArgumentException" >
        /// If the <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace.
        /// </exception>
        ///
        public string Chromosome
        {
            get { return _chromosome; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Chromosome");
                Validator.ThrowIfStringIsWhitespace(value, "Chromosome");
                _chromosome = value;
            }
        }

        private string _chromosome;

        /// <summary>
        /// The numbering scheme used for positions.
        /// </summary>
        ///
        /// <remarks>
        /// 0 = 0 based numbering scheme.
        /// 1 = 1 based numbering scheme.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException" >
        /// The <paramref name="value" /> parameter is neither zero based scheme nor one
        /// based scheme.
        /// </exception>
        ///
        public GenomeNumberingScheme NumberingScheme
        {
            get { return _numberingScheme; }

            set
            {
                if (value != GenomeNumberingScheme.ZeroBased && value != GenomeNumberingScheme.OneBased)
                {
                    throw new ArgumentOutOfRangeException(nameof(NumberingScheme), Resources.InvalidNumberingScheme);
                }

                _numberingScheme = value;
            }
        }

        private GenomeNumberingScheme _numberingScheme;

        /// <summary>
        /// Gets or sets the person or organization that ordered the SNP test.
        /// </summary>
        ///
        public Organization OrderedBy
        {
            get { return _orderedBy; }
            set { _orderedBy = value; }
        }

        private Organization _orderedBy;

        /// <summary>
        /// The organization that provides the SNP test service.
        /// </summary>
        ///
        /// <remarks>
        /// This organization typically also provides analysis of the results.
        /// </remarks>
        ///
        public Organization TestProvider
        {
            get { return _testProvider; }
            set { _testProvider = value; }
        }

        private Organization _testProvider;

        /// <summary>
        /// The name of the laboratory that performed the test.
        /// </summary>
        ///
        public Organization LaboratoryName
        {
            get { return _laboratoryName; }
            set { _laboratoryName = value; }
        }

        private Organization _laboratoryName;

        /// <summary>
        /// Gets or sets the annotation version.
        /// </summary>
        ///
        /// <value>
        /// A string representing the annotation version.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the annotation version should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string AnnotationVersion
        {
            get { return _annotationVersion; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "AnnotationVersion");
                _annotationVersion = value;
            }
        }

        private string _annotationVersion;

        /// <summary>
        /// Gets or sets the dbSNP build version.
        /// </summary>
        ///
        /// <value>
        /// A string representing the dbSNP build version.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the dbSNP version should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string DbSnpBuild
        {
            get { return _dbSnpBuild; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "dbSnpBuild");
                _dbSnpBuild = value;
            }
        }

        private string _dbSnpBuild;

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        ///
        /// <value>
        /// A string representing the platform.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the platform should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Platform
        {
            get { return _platform; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Platform");
                _platform = value;
            }
        }

        private string _platform;

        /// <summary>
        /// Gets the description of a SNP result instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the SNP result instance.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    Resources.GeneticSnpResultsToStringFormat,
                    _chromosome,
                    _genomeBuild);
        }

        /// <summary>
        /// The SNP test results data.
        /// </summary>
        ///
        /// <remarks>
        /// The SNP test result data is exposed as a <see cref="SnpData"/> instance.
        ///
        /// To get the SNP test result data when fetching an instance of the
        /// GeneticSnpResults thing type, you must specify that
        /// the other-data section to be returned to access the SnpData.
        /// </remarks>
        ///
        public SnpData SnpData
        {
            // This property auto-creates when the user first accesses it.
            // If there is no Blob, we will create a new one. If the Blob exists
            // (ie it was fetched with the instance), we convert the other data
            // to a SnpData instance.
            get
            {
                BlobStore store = GetBlobStore(default(HealthRecordAccessor));
                Blob blob = store[string.Empty];

                // no data, create an instance for the user to use.
                if (blob == null)
                {
                    if (_snpData == null)
                    {
                        _snpData = new SnpData(null, string.Empty, "text/csv");
                    }
                }

                // data is of the wrong type. The data has been fetched, but not yet encapsulated in the
                // SnpData instance.
                else
                {
                    // Validate that it's text/csv before we change its type.
                    if (blob.ContentType == "text/csv")
                    {
                        _snpData =
                            new SnpData(
                                blob.ReadAsString(),
                                null,
                                "text/csv");
                    }
                    else
                    {
                        return null;
                    }
                }

                return _snpData;
            }
        }

        private SnpData _snpData;
    }
}
