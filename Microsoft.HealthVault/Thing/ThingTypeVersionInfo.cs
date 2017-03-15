// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents the version information for a thing type.
    /// </summary>
    public class ThingTypeVersionInfo
    {
        private ThingTypeVersionInfo()
        {
        }

        internal ThingTypeVersionInfo(
            Guid versionTypeId,
            string versionName,
            int versionSequence,
            ThingTypeOrderByProperties orderByProperties)
        {
            this.VersionTypeId = versionTypeId;
            this.Name = versionName;
            this.VersionSequence = versionSequence;
            this.OrderByProperties = orderByProperties;
        }

        /// <summary>
        /// Gets the unique identifier for the versioned thing type.
        /// </summary>
        public Guid VersionTypeId { get; }

        /// <summary>
        /// Gets the name for this version of the thing type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the sequence number for the thing type version.
        /// </summary>
        /// <remarks>
        /// The sequence number starts at one and is incremented for each new version
        /// of the type that gets added.
        /// </remarks>
        public int VersionSequence { get; }

        /// <summary>
        /// The set of properties that the thing-type can be
        /// ordered by in the result.
        /// </summary>
        public ThingTypeOrderByProperties OrderByProperties { get; }
    }
}
