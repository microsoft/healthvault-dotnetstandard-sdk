// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Things
{
    /// <summary>
    /// Uniquely identifies a health record item in the system.
    /// </summary>
    ///
    public class HealthRecordItemKey
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItemKey"/>
        /// class with the specified globally unique ID for the
        /// <see cref="HealthRecordItem"/> and globally unique version stamp.
        /// </summary>
        ///
        /// <param name="id">
        /// A globally unique identifier for the <see cref="HealthRecordItem"/>
        /// in the system.
        /// </param>
        ///
        /// <param name="versionStamp">
        /// A globally unique identifier for the version of the <see cref="HealthRecordItem"/>
        /// in the system.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="id"/> or <paramref name="versionStamp"/>
        /// parameter is Guid.Empty.
        /// </exception>
        ///
        public HealthRecordItemKey(Guid id, Guid versionStamp)
        {
            Validator.ThrowArgumentExceptionIf(
                id == Guid.Empty,
                "id",
                "ThingIdInvalid");

            Validator.ThrowArgumentExceptionIf(
                versionStamp == Guid.Empty,
                "versionStamp",
                "ThingVersionInvalid");

            this.thingId = id;
            this.versionStamp = versionStamp;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItemKey"/>
        /// class with the specified globally unique ID for the
        /// <see cref="HealthRecordItem"/>.
        /// </summary>
        ///
        /// <param name="id">
        /// A globally unique identifier for the <see cref="HealthRecordItem"/>
        /// in the system.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is Guid.Empty.
        /// </exception>
        ///
        public HealthRecordItemKey(Guid id)
        {
            Validator.ThrowArgumentExceptionIf(
                id == Guid.Empty,
                "id",
                "ThingIdInvalid");

            this.thingId = id;
        }

        /// <summary>
        /// Gets the unique identifier of the <see cref="HealthRecordItem"/>.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the <see cref="HealthRecordItem"/>,
        /// issued when the item is created.
        /// </value>
        ///
        public Guid Id => this.thingId;

        private Guid thingId;

        /// <summary>
        /// Gets the unique version stamp of the <see cref="HealthRecordItem"/>.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier that represents the version of the
        /// <see cref="HealthRecordItem"/>. A new version stamp is issued each
        /// time the item is changed.
        /// </value>
        ///
        /// <remarks>
        /// The version stamp of the current version of a <see cref="HealthRecordItem"/>
        /// is always equal to the <see cref="Id"/> of that item.
        /// </remarks>
        ///
        public Guid VersionStamp => this.versionStamp;

        private Guid versionStamp;

        /// <summary>
        /// Gets a string representation of the key.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="VersionStamp"/>.ToString().
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.versionStamp != Guid.Empty)
            {
                return this.thingId + "," + this.versionStamp;
            }

            return this.thingId.ToString();
        }

        /// <summary>
        /// Compares one <see cref="HealthRecordItemKey"/> to another.
        /// </summary>
        ///
        /// <param name="obj">
        /// The <see cref="HealthRecordItemKey"/> to compare against this.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if both the health record item keys have
        /// the same ID and version stamp; otherwise, <b>false</b>.
        /// </returns>
        ///
        public override bool Equals(object obj)
        {
            bool result = false;
            HealthRecordItemKey rVal = obj as HealthRecordItemKey;

            if (rVal != null)
            {
                result = (this.versionStamp == rVal.VersionStamp)
                       && (this.thingId == rVal.Id);
            }

            return result;
        }

        /// <summary>
        /// Gets the hashcode value for the object.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="VersionStamp"/>.GetHashCode().
        /// </returns>
        ///
        public override int GetHashCode()
        {
            return this.versionStamp.GetHashCode();
        }
    }
}
