// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.Health
{
    /// <summary>
    /// Encapsulates information about an updated record and the person associated with that record.
    /// </summary>
    public class HealthRecordUpdateInfo
    {
        /// <summary>
        /// Create a new instance of the <see cref="HealthRecordUpdateInfo"/> class for testing purposes. 
        /// </summary>
        protected HealthRecordUpdateInfo()
        {
        }

        internal HealthRecordUpdateInfo(
            Guid recordId, 
            DateTime lastUpdateDate, 
            Guid personId,
            long latestOperationSequenceNumber)
        {
            _recordId = recordId;
            _lastUpdateDate = lastUpdateDate;
            _personId = personId;
            LatestOperationSequenceNumber = latestOperationSequenceNumber;
        }

        /// <summary>
        /// Gets or sets the ID of the <see cref="HealthRecordAccessor"/> updated.
        /// </summary>
        public Guid RecordId
        {
            get { return _recordId; }
            protected set { _recordId = value; }
        }
        private Guid _recordId;

        /// <summary>
        /// Gets or sets the timestamp when an addition, deletion or update occured to the 
        /// <see cref="HealthRecordItem"/>s in the <see cref="HealthRecordAccessor"/> 
        /// </summary>
        public DateTime LastUpdateDate
        {
            get { return _lastUpdateDate; }
            protected set { _lastUpdateDate = value; }
        }
        private DateTime _lastUpdateDate;

        /// <summary>
        /// Gets or sets the person ID of the updated record.
        /// </summary>
        public Guid PersonId
        {
            get { return _personId; }
            protected set { _personId = value; }
        }
        private Guid _personId;

        /// <summary>
        /// Gets the record's latest operation sequence number.
        /// </summary>
        /// 
        /// <remarks>
        /// The record's operation sequence number is used when sync'ing data from a 
        /// record. Anytime an operation is performed against a health record item HealthVault
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