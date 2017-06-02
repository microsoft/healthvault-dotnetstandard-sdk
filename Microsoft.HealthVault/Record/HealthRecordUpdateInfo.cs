// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Thing;
using NodaTime;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Encapsulates information about an updated record and the person associated with that record.
    /// </summary>
    internal class HealthRecordUpdateInfo
    {
        /// <summary>
        /// Create a new instance of the <see cref="HealthRecordUpdateInfo"/> class for testing purposes.
        /// </summary>
        protected HealthRecordUpdateInfo()
        {
        }

        internal HealthRecordUpdateInfo(
            Guid recordId,
            Instant lastUpdateDate,
            Guid personId,
            long latestOperationSequenceNumber)
        {
            RecordId = recordId;
            LastUpdateDate = lastUpdateDate;
            PersonId = personId;
            LatestOperationSequenceNumber = latestOperationSequenceNumber;
        }

        /// <summary>
        /// Gets or sets the ID of the <see cref="HealthRecordAccessor"/> updated.
        /// </summary>
        public Guid RecordId { get; protected set; }

        /// <summary>
        /// Gets or sets the timestamp when an addition, deletion or update occurred to the
        /// <see cref="ThingBase"/>s in the <see cref="HealthRecordAccessor"/>
        /// </summary>
        public Instant LastUpdateDate { get; protected set; }

        /// <summary>
        /// Gets or sets the person ID of the updated record.
        /// </summary>
        public Guid PersonId { get; protected set; }

        /// <summary>
        /// Gets the record's latest operation sequence number.
        /// </summary>
        ///
        /// <remarks>
        /// The record's operation sequence number is used when syncing data from a
        /// record. Anytime an operation is performed against a thing HealthVault
        /// stamps it with the next increment of the operation sequence number for the record.
        /// For example, the first item added to the record would be stamped with the sequence
        /// number 1, the next operation would stamp the thing with 2, etc. Applications can
        /// determine all operations that have occurred since a known point by calling
        /// GetRecordOperations and passing the sequence number of the known point.
        /// </remarks>
        ///
        public long LatestOperationSequenceNumber { get; private set; }
    }
}