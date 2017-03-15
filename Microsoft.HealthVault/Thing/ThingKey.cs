// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Uniquely identifies a thing in the system.
    /// </summary>
    ///
    public class ThingKey
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ThingBase"/>
        /// class with the specified globally unique ID for the
        /// <see cref="ThingBase"/> and globally unique version stamp.
        /// </summary>
        ///
        /// <param name="id">
        /// A globally unique identifier for the <see cref="ThingBase"/>
        /// in the system.
        /// </param>
        ///
        /// <param name="versionStamp">
        /// A globally unique identifier for the version of the <see cref="ArgumentException"/>
        /// in the system.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="versionStamp"/> or <paramref name="versionStamp"/>
        /// parameter is Guid.Empty.
        /// </exception>
        ///
        public ThingKey(Guid id, Guid versionStamp)
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
        /// Creates a new instance of the <see cref="ThingBase"/>
        /// class with the specified globally unique ID for the
        /// <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <param name="id">
        /// A globally unique identifier for the <see cref="ArgumentException"/>
        /// in the system.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is Guid.Empty.
        /// </exception>
        ///
        public ThingKey(Guid id)
        {
            Validator.ThrowArgumentExceptionIf(
                id == Guid.Empty,
                "id",
                "ThingIdInvalid");

            this.thingId = id;
        }

        /// <summary>
        /// Gets the unique identifier of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the <see cref="ThingBase"/>,
        /// issued when the item is created.
        /// </value>
        ///
        public Guid Id => this.thingId;

        private Guid thingId;

        /// <summary>
        /// Gets the unique version stamp of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier that represents the version of the
        /// <see cref="ThingBase"/>. A new version stamp is issued each
        /// time the item is changed.
        /// </value>
        ///
        /// <remarks>
        /// The version stamp of the current version of a <see cref="ThingBase"/>
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
        /// Compares one <see cref="ThingKey"/> to another.
        /// </summary>
        ///
        /// <param name="obj">
        /// The <see cref="ThingKey"/> to compare against this.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if both the thing keys have
        /// the same ID and version stamp; otherwise, <b>false</b>.
        /// </returns>
        ///
        public override bool Equals(object obj)
        {
            bool result = false;
            ThingKey rVal = obj as ThingKey;

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
