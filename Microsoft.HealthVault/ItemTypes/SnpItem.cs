// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Text;

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
            ReferenceSnpId = referenceSnpId;
            StrandOrientation = strandOrientation;
            Result = result;
            AssayId = assayId;
            StartPosition = startPosition;
            EndPosition = endPosition;
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
            get { return _referenceSnpId; }
            set { _referenceSnpId = value; }
        }

        private string _referenceSnpId;

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
            get { return _strandOrientation; }
            set { _strandOrientation = value; }
        }

        private string _strandOrientation;

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
            get { return _result; }
            set { _result = value; }
        }

        private string _result;

        /// <summary>
        /// Gets or sets the assay ID.
        /// </summary>
        ///
        public string AssayId
        {
            get { return _assayId; }
            set { _assayId = value; }
        }

        private string _assayId;

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        ///
        public int StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        private int _startPosition;

        /// <summary>
        /// Gets or sets the end Position.
        /// </summary>
        ///
        public int EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }

        private int _endPosition;

        /// <summary>
        /// Gets a string representation of the snp result item.
        /// </summary>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);
            if (string.IsNullOrEmpty(ReferenceSnpId))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatReferenceSnpId,
                        ReferenceSnpId);
            }

            if (string.IsNullOrEmpty(StrandOrientation))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatStrandOrientation,
                        StrandOrientation);
            }

            if (string.IsNullOrEmpty(Result))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatResult,
                        Result);
            }

            if (string.IsNullOrEmpty(AssayId))
            {
                result.AppendFormat(
                    Resources.SnpItemToStringFormatAssayId,
                        AssayId);
            }

            result.AppendFormat(
                Resources.SnpItemToStringFormatStartAndEnd,
                StartPosition,
                EndPosition);

            return result.ToString();
        }
    }
}
