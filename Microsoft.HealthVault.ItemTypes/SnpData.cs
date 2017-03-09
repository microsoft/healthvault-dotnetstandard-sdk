// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// The <see cref="SnpData"/> class is used to store and retrieve SNP result data.
    /// </summary>
    ///
    /// <remarks>
    /// SNP data are stored in a dictionary. It handles converting from a SNP result
    /// format to the comma-separated format.
    /// </remarks>
    ///
    public class SnpData : OtherItemDataCsv
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SnpData"/> class.
        /// </summary>
        ///
        public SnpData()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SnpData"/> class
        /// with the specified data, encoding, and content type.
        /// </summary>
        ///
        /// <param name="data">
        /// The data to store in the other data section of the health record
        /// item.
        /// </param>
        ///
        /// <param name="contentEncoding">
        /// The type of encoding that was done on the data. Usually this will
        /// be "base64" but other encodings are acceptable.
        /// </param>
        ///
        /// <param name="contentType">
        /// The MIME-content type of the data.
        /// </param>
        ///
        public SnpData(
            string data,
            string contentEncoding,
            string contentType)
            : base(data, contentEncoding, contentType)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="SnpData"/> class with
        /// specified other item data.
        /// </summary>
        ///
        /// <param name="otherItemData">The other item data.</param>
        ///
        internal SnpData(OtherItemData otherItemData)
        {
            // Convert an OtherData instance to one of this type.
            // This is used to convert the OtherItemData instance that was deserialized when an SnpData
            // instance was created to an instance of this type.
            this.Data = otherItemData.Data;
            this.ContentEncoding = otherItemData.ContentEncoding;
            this.ContentType = otherItemData.ContentType;
        }

        private Dictionary<string, SnpItem> snpItems;

        /// <summary>
        /// Gets the SNP data as a dictionary of SNP items.
        /// </summary>
        ///
        /// <remarks>
        /// SNP items in a dictionary can be easily accessed by the reference SNP Id.
        /// </remarks>
        ///
        public Dictionary<string, SnpItem> SnpItems
        {
            get
            {
                if (this.snpItems == null)
                {
                    // If there is no OtherData here, we're creating a new set of samples...
                    if (this.Data == null)
                    {
                        this.snpItems = new Dictionary<string, SnpItem>();
                    }
                    else
                    {
                        this.CreateSnpItems();
                    }
                }

                return this.snpItems;
            }
        }

        /// <summary>
        /// Reloads SNP items from other data section of the health record item.
        /// </summary>
        ///
        public void RefreshSnpItems()
        {
            if (this.Data == null)
            {
                this.snpItems = null;
            }
            else
            {
                this.CreateSnpItems();
            }
        }

        /// <summary>
        /// Gets all the genetic snp results as strings and store them in a dictionary.
        /// </summary>
        ///
        /// <exception cref="FormatException">
        /// If the data cannot be completely parsed into SNP item.
        /// </exception>
        ///
        private void CreateSnpItems()
        {
            this.snpItems = new Dictionary<string, SnpItem>();
            Collection<OtherItemDataCsvItem> rawSamples = this.GetAsString();

            // csvStringValueBuffer is used to store up to three valid string item.
            // For every three valid string items, a SNP item will be created.
            OtherItemDataCsvString[] csvStringValueBuffer = new OtherItemDataCsvString[4];
            string referenceSnpId, strandOrientation, result, assayId;
            int startPosition, endPosition;

            for (int sampleIndex = 0; sampleIndex < rawSamples.Count; sampleIndex += 6)
            {
                if (sampleIndex + 5 >= rawSamples.Count)
                {
                    throw new FormatException(
                        ResourceRetriever.GetResourceString("ExtraCsvItemToParse"));
                }

                referenceSnpId = ((OtherItemDataCsvString)rawSamples[sampleIndex]).Value;
                strandOrientation = ((OtherItemDataCsvString)rawSamples[sampleIndex + 1]).Value;
                result = ((OtherItemDataCsvString)rawSamples[sampleIndex + 2]).Value;
                assayId = ((OtherItemDataCsvString)rawSamples[sampleIndex + 3]).Value;
                startPosition =
                    int.Parse(((OtherItemDataCsvString)rawSamples[sampleIndex + 4]).Value);
                endPosition =
                    int.Parse(((OtherItemDataCsvString)rawSamples[sampleIndex + 5]).Value);
                SnpItem snpItem = new SnpItem(
                    referenceSnpId,
                    strandOrientation,
                    result,
                    assayId,
                    startPosition,
                    endPosition);

                this.snpItems.Add(referenceSnpId, snpItem);

                // ignore the escape case since it is not applicable for
                // snp data.
            }
        }

        /// <summary>
        /// Writes the SNP Items to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the SNP item data.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If <see cref="SnpItems"/> has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowInvalidIfNull(this.snpItems, "SnpItemsNotSet");

            this.StoreSnpItems();

            // The base class takes the other data string and puts it in the proper xml format.
            base.WriteXml(writer);
        }

        /// <summary>
        /// Take the dictionary SNP items, and convert them into the underlying
        /// format.
        /// </summary>
        ///
        internal void StoreSnpItems()
        {
            List<OtherItemDataCsvItem> rawSamples = new List<OtherItemDataCsvItem>();
            foreach (KeyValuePair<string, SnpItem> stringSnpItemPair in this.snpItems)
            {
                rawSamples.Add(new OtherItemDataCsvString(stringSnpItemPair.Value.ReferenceSnpId));
                rawSamples.Add(new OtherItemDataCsvString(stringSnpItemPair.Value.StrandOrientation));
                rawSamples.Add(new OtherItemDataCsvString(stringSnpItemPair.Value.Result));
                rawSamples.Add(new OtherItemDataCsvString(stringSnpItemPair.Value.AssayId));
                rawSamples.Add(new OtherItemDataCsvString(stringSnpItemPair.Value.StartPosition.ToString()));
                rawSamples.Add(new OtherItemDataCsvString(stringSnpItemPair.Value.EndPosition.ToString()));
            }

            this.SetOtherData(rawSamples);
        }
    }
}
