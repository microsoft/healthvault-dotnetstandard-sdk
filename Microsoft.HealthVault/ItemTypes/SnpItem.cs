// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Text;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A SNP test result item.
    /// </summary>
    ///
    /// <remarks>
    /// Within the comma-separated data, each SNP is encoded as follows:
    ///
    /// [refSNP id],[strand orientation],[result],[assay id],[start position],[end position]
    ///
    /// Where:
    /// refSNP id: Reference SNP identifier from NCBI dbSNP database.
    ///  strand orientation: "+" encodes top, "-" encodes bottom.
    ///  result: the result of the test.
    ///  assay id: Platform dependent probe set id.
    ///  start position: Start position on the chromosome.
    ///  end position: End position on the chromosome.
    ///
    /// Example: rs1891906,-,GT, SNP_C-315533, 940106, 940107
    /// </remarks>
    ///
    public class SnpItem
    {
        /// <summary>
        /// Creates an instance of the <see cref="SnpItem"/> type with default values.
        /// </summary>
        ///
        public SnpItem()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="SnpItem"/> type with the specified values.
        /// </summary>
        ///
        /// <param name="referenceSnpId">Reference SNP Id (rs).</param>
        /// <param name="strandOrientation">The orientation of the strand. </param>
        /// <param name="result">The result of the SNP test. </param>
        /// <param name="assayId">The platform dependent probe set id. </param>
        /// <param name="startPosition">The start position on the chromosome. </param>
        /// <param name="endPosition">The end position on the chromosome. </param>
        ///
        public SnpItem(
            string referenceSnpId,
            string strandOrientation,
            string result,
            string assayId,
            int startPosition,
            int endPosition)
        {
            this.ReferenceSnpId = referenceSnpId;
            this.StrandOrientation = strandOrientation;
            this.Result = result;
            this.AssayId = assayId;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        /// <summary>
        /// Gets or sets the Reference SNP ID of the result.
        /// </summary>
        ///
        /// <remarks>
        /// For example, rs132342.
        /// </remarks>
        ///
        public string ReferenceSnpId
        {
            get { return this.referenceSnpId; }
            set { this.referenceSnpId = value; }
        }

        private string referenceSnpId;

        /// <summary>
        /// Gets or sets the orientation of the strand.
        /// </summary>
        ///
        /// <remarks>
        /// The value can be either + or -.
        /// </remarks>
        ///
        public string StrandOrientation
        {
            get { return this.strandOrientation; }
            set { this.strandOrientation = value; }
        }

        private string strandOrientation;

        /// <summary>
        /// Gets or sets the result of the SNP test.
        /// </summary>
        ///
        /// <remarks>
        /// For example, AA, AT. Each letter correspond to one copy of
        /// the specified SNP is the customer's genome, since each of us has two
        /// copies of each gene under normal circumstance.
        /// </remarks>
        ///
        public string Result
        {
            get { return this.result; }
            set { this.result = value; }
        }

        private string result;

        /// <summary>
        /// Gets or sets the assay ID.
        /// </summary>
        ///
        public string AssayId
        {
            get { return this.assayId; }
            set { this.assayId = value; }
        }

        private string assayId;

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        ///
        public int StartPosition
        {
            get { return this.startPosition; }
            set { this.startPosition = value; }
        }

        private int startPosition;

        /// <summary>
        /// Gets or sets the end Position.
        /// </summary>
        ///
        public int EndPosition
        {
            get { return this.endPosition; }
            set { this.endPosition = value; }
        }

        private int endPosition;

        /// <summary>
        /// Gets a string representation of the snp result item.
        /// </summary>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            if (string.IsNullOrEmpty(this.ReferenceSnpId))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatReferenceSnpId,
                        this.ReferenceSnpId);
            }

            if (string.IsNullOrEmpty(this.StrandOrientation))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatStrandOrientation,
                        this.StrandOrientation);
            }

            if (string.IsNullOrEmpty(this.Result))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatResult,
                        this.Result);
            }

            if (string.IsNullOrEmpty(this.AssayId))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatAssayId,
                        this.AssayId);
            }

            result.AppendFormat(
                Resources.SnpItemToStringFormatStartAndEnd,
                this.StartPosition,
                this.EndPosition);

            return result.ToString();
        }
    }
}
