// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents the version information for a health record item type.
    /// </summary>
    public class HealthRecordItemTypeVersionInfo
    {
        private HealthRecordItemTypeVersionInfo() { }

        internal HealthRecordItemTypeVersionInfo(
            Guid versionTypeId,
            string versionName,
            int versionSequence,
            HealthRecordItemTypeOrderByProperties orderByProperties)
        {
            _versionTypeId = versionTypeId;
            _versionName = versionName;
            _versionSequence = versionSequence;
            _orderByProperties = orderByProperties;
        }

        /// <summary>
        /// Gets the unique identifier for the versioned health record item type.
        /// </summary>
        public Guid VersionTypeId
        {
            get { return _versionTypeId; }
        }
        private readonly Guid _versionTypeId;

        /// <summary>
        /// Gets the name for this version of the health record item type.
        /// </summary>
        public string Name
        {
            get { return _versionName; }
        }
        private readonly string _versionName;

        /// <summary>
        /// Gets the sequence number for the health record item type version.
        /// </summary>
        /// <remarks>
        /// The sequence number starts at one and is incremented for each new version
        /// of the type that gets added.
        /// </remarks>
        public int VersionSequence
        {
            get { return _versionSequence; }
        }
        private readonly int _versionSequence;

        /// <summary>
        /// The set of properties that the thing-type can be 
        /// ordered by in the result.
        /// </summary>
        public HealthRecordItemTypeOrderByProperties OrderByProperties
        {
            get { return _orderByProperties; }
        }
        private readonly HealthRecordItemTypeOrderByProperties _orderByProperties;
    }
}
