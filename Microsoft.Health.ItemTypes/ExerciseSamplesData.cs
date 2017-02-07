// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.Health;
using System.Xml;
using Microsoft.Health.ItemTypes.Csv;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// The ExerciseSamplesData class is used to store and retrieve samples data for the 
    /// <see cref="ExerciseSamples"/> class. It handles converting from a sample data format to the
    /// comma-separated format.
    /// </summary>
    /// <remarks>
    /// Data is represented as either an array of samples with a single value (such as heart rate), or an array
    /// of samples with two values (such as a GPS position). The <see cref="SingleValuedSamples"/> or <see cref="TwoValuedSamples"/> properties
    /// should be used as appropriate based on the stored data type. 
    /// 
    /// The underlying format allows the sampling interval to be changed in the middle of a sample set. This class
    /// will detect sampling intervals that have changed, and insert appropriate escape values into the resulting data.
    /// </remarks>
    public class ExerciseSamplesData: OtherItemDataCsv
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExerciseSamplesData"/> class. 
        /// </summary>
        public ExerciseSamplesData()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ExerciseSamplesData"/> class 
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
        public ExerciseSamplesData(
            string data,
            string contentEncoding,
            string contentType)
            : base(data, contentEncoding, contentType)
        {
        }

        // Handle unpacking/decompressing data if it is in that format.
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream and GZipStream can be disposed multiple times. Usings block makes code more readable")]
        private void ProcessCompressedAndEncodedData()
        {
            if (ContentEncoding == null)
            {
                return;
            }

                // Only valid encoding here is base64, so we go with that...
            byte[] buffer = null;

            try
            {
                buffer = Convert.FromBase64String(Data);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Data is not a Base64 string");
            }

            if (buffer == null)
            {
                return;
            }

            if (buffer.Length < 2)
            {
                return;
            }

                // look at the bytes to see if they are the proper gzip header...
            if ((buffer[0] != 31) ||
                (buffer[1] != 139))
            {
                return;    // nothing we can do
            }

                // gzip decompress...


            using (MemoryStream bufferStream = new MemoryStream(buffer))
            {
                using (GZipStream decompress = new GZipStream(bufferStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(decompress))
                    {
                        string result = reader.ReadToEnd();
                        Data = result;
                        ContentEncoding = null;
                        ContentType = "text/csv";
                    }
                }
            }
        }

        // Convert an OtherData instance to one of this type.
        // This is used to convert the OtherItemData instance that was deserialized when an ExerciseSamples
        // instance was created to an instance of this type. 
        internal ExerciseSamplesData(OtherItemData otherItemData)
        {
            Data = otherItemData.Data;
            ContentEncoding = otherItemData.ContentEncoding;
            ContentType = otherItemData.ContentType;
        }

        private double _samplingInterval;

        /// <summary>
        /// Gets or sets the initial sampling interval for the set of samples. 
        /// </summary>
        /// <remarks>
        /// The sampling interval may change between two samples.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the sampling interval is less than or equal to zero.
        /// </exception>
        public double SamplingInterval
        {
            get { return _samplingInterval; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value <= 0,
                    "SamplingInterval",
                    "SamplingIntervalMustBePositive");
                _samplingInterval = value;
            }
        }

        Collection<ExerciseSampleOneValue> _singleValuedSamples;

        /// <summary>
        /// Gets the sample data as a collection of single values.
        /// </summary>
        /// <remarks>
        /// The majority of sample types contain only a single value per sample. This property
        /// is used to access samples for those sample types. 
        /// </remarks>
        public Collection<ExerciseSampleOneValue> SingleValuedSamples
        {
            get
            {
                if (_singleValuedSamples == null)
                {
                        // If there is no OtherData here, we're creating a new set of samples...
                    if (Data == null)
                    {
                        _singleValuedSamples = new Collection<ExerciseSampleOneValue>();
                    }
                    else
                    {
                        CreateSingleValuedSamples();
                    }
                }

                return _singleValuedSamples;
            }
        }

        // Get all the samples as doubles, and create the collection that we want to return to the user.
        // We set the time offset from the initial sampling interval and any escapes that are present to 
        // reset it. 
        private void CreateSingleValuedSamples()
        {
            ProcessCompressedAndEncodedData();

            _singleValuedSamples = new Collection<ExerciseSampleOneValue>();

            Collection<OtherItemDataCsvItem> rawSamples = GetAsDouble();

            double currentSampleInterval = _samplingInterval;
            double offsetInSeconds = -currentSampleInterval;

            for (int sampleIndex = 0; sampleIndex < rawSamples.Count; sampleIndex++)
            {
                OtherItemDataCsvItem item = rawSamples[sampleIndex];

                OtherItemDataCsvDouble itemDouble = item as OtherItemDataCsvDouble;

                if (itemDouble != null)
                {
                    offsetInSeconds += currentSampleInterval;

                    ExerciseSampleOneValue sample = new ExerciseSampleOneValue(offsetInSeconds, itemDouble.Value);
                    _singleValuedSamples.Add(sample);
                }

                OtherItemDataCsvEscape itemEscape = item as OtherItemDataCsvEscape;
                if (itemEscape != null)
                {
                    if (itemEscape.Name == "i")
                    {
                        currentSampleInterval = Double.Parse(itemEscape.Value);
                    }
                }
            }
        }

        Collection<ExerciseSampleTwoValue> _twoValuedSamples = null;

        /// <summary>
        /// Gets the sample data as a collection of two-valued samples.
        /// </summary>
        /// <remarks>
        /// Some samples types (such as GPS location) consist not of a single value but of two separate values
        /// This property is used to access samples for those sample types. 
        /// </remarks>
        public Collection<ExerciseSampleTwoValue> TwoValuedSamples
        {
            get
            {
                if (_twoValuedSamples == null)
                {
                    if (Data == null)
                    {
                        _twoValuedSamples = new Collection<ExerciseSampleTwoValue>();
                    }
                    else
                    {
                        CreateTwoValuedSamples();
                    }
                }

                return _twoValuedSamples;
            }
        }

        // Get all the samples as doubles, and create the collection that we want to return to the user.
        // We set the time offset from the initial sampling interval and any escapes that are present to 
        // reset it. We also go from a single-valued list to the two-valued list.  
        private void CreateTwoValuedSamples()
        {
            ProcessCompressedAndEncodedData();

            _twoValuedSamples = new Collection<ExerciseSampleTwoValue>();

            Collection<OtherItemDataCsvItem> rawSamples = GetAsDouble();

            double currentSampleInterval = _samplingInterval;
            double offsetInSeconds = -currentSampleInterval;

            for (int sampleIndex = 0; sampleIndex < rawSamples.Count; sampleIndex ++) 
            {
                OtherItemDataCsvItem item = rawSamples[sampleIndex];

                OtherItemDataCsvDouble itemDouble = item as OtherItemDataCsvDouble;
                if (itemDouble != null)
                {
                    OtherItemDataCsvDouble itemSecond = rawSamples[sampleIndex + 1] as OtherItemDataCsvDouble;

                    offsetInSeconds += currentSampleInterval;

                    ExerciseSampleTwoValue sample =
                        new ExerciseSampleTwoValue(offsetInSeconds, itemDouble.Value, itemSecond.Value);
                    _twoValuedSamples.Add(sample);

                    sampleIndex++;  // skip an extra item
                }
                OtherItemDataCsvEscape itemEscape = item as OtherItemDataCsvEscape;
                if (itemEscape != null)
                {
                    if (itemEscape.Name == "i")
                    {
                        currentSampleInterval = Double.Parse(itemEscape.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the exercise samples to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the height data to.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If both the <see cref="SingleValuedSamples"/> and <see cref="TwoValuedSamples"/> properties have data in them
        /// or
        /// if neither the <see cref="SingleValuedSamples"/> nor <see cref="TwoValuedSamples"/> properties have data in them.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            if (_singleValuedSamples != null && _twoValuedSamples != null)
            {
                throw Validator.InvalidOperationException("OneAndTwoValuedSamples");
            }
            else if (_singleValuedSamples == null && _twoValuedSamples == null)
            {
                throw Validator.InvalidOperationException("OneAndTwoValuedSamples");
            }

                // Convert the samples to the text format and put them in the other data section...

            if (_singleValuedSamples != null)
            {
                StoreSingle();
            }

            if (_twoValuedSamples != null)
            {
                StoreTwo();
            }

                // The base class takes the other data string and puts it in the proper xml format. 
            base.WriteXml(writer); 
        }

        // Take the collection of samples the user gave us, and convert them into the underlying format.
        private void StoreSingle()
        {
            List<OtherItemDataCsvItem> rawSamples = new List<OtherItemDataCsvItem>();

            double currentSamplingInterval = SamplingInterval;

            double lastOffset = -SamplingInterval;
            for (int sampleNumber = 0; sampleNumber < _singleValuedSamples.Count; sampleNumber++)
            {
                ExerciseSampleOneValue sample = _singleValuedSamples[sampleNumber];

                // Is this sample coming when it's expected? 
                // if not, we need to put in an escape...
                if (lastOffset + currentSamplingInterval != sample.OffsetInSeconds)
                {
                    currentSamplingInterval = sample.OffsetInSeconds - lastOffset;

                    OtherItemDataCsvEscape escape = new OtherItemDataCsvEscape("i", currentSamplingInterval.ToString());
                    rawSamples.Add(escape);
                }

                rawSamples.Add(new OtherItemDataCsvDouble(sample.Value));
                lastOffset = sample.OffsetInSeconds;
            }

            SetOtherData(rawSamples);
        }

        // Take the collection of samples the user gave us, and convert them into the underlying format.
        private void StoreTwo()
        {
            List<OtherItemDataCsvItem> rawSamples = new List<OtherItemDataCsvItem>();

            double currentSamplingInterval = SamplingInterval;

            double lastOffset = -SamplingInterval;
            for (int sampleNumber = 0; sampleNumber < _twoValuedSamples.Count; sampleNumber++)
            {
                ExerciseSampleTwoValue sample = _twoValuedSamples[sampleNumber];

                // Is this sample coming when it's expected? 
                // if not, we need to put in an escape...
                if (lastOffset + currentSamplingInterval != sample.OffsetInSeconds)
                {
                    currentSamplingInterval = sample.OffsetInSeconds - lastOffset;

                    OtherItemDataCsvEscape escape = new OtherItemDataCsvEscape("i", currentSamplingInterval.ToString());
                    rawSamples.Add(escape);
                }

                rawSamples.Add(new OtherItemDataCsvDouble(sample.Value1));
                rawSamples.Add(new OtherItemDataCsvDouble(sample.Value2));
                lastOffset = sample.OffsetInSeconds;
            }

            SetOtherData(rawSamples);
        }
    }
}
