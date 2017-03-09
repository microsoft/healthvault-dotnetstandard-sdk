// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines a set of health record items of the specified type
    /// for authorization purposes.
    /// </summary>
    ///
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or
    /// effective date, or explicitly by setting the system set.) This class
    /// serves as a set of health record items that have a specified type ID.
    /// Other types of authorization sets include:
    /// <see cref="DateRangeSetDefinition"/>.
    /// </remarks>
    ///
    /// <seealso cref="AuthorizationSetDefinition"/>
    /// <seealso cref="DateRangeSetDefinition"/>
    ///
    public class TypeIdSetDefinition : AuthorizationSetDefinition
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TypeIdSetDefinition"/> class
        /// with the specified type identifier.
        /// </summary>
        ///
        /// <param name="typeId">
        /// The identifier of the type for the set.
        /// </param>
        ///
        /// <remarks>
        /// The <paramref name="typeId"/> must be the identifier for a health
        /// record item type. See
        /// <see
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(System.Guid,HealthServiceConnection)"/>
        /// for information on getting the value health record item types.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="typeId"/> parameter is Guid.Empty.
        /// </exception>
        ///
        public TypeIdSetDefinition(Guid typeId)
            : base(SetType.TypeIdSet)
        {
            Validator.ThrowArgumentExceptionIf(
                typeId == Guid.Empty,
                "typeId",
                "TypeIdSetGuidEmpty");

            this.TypeId = typeId;
        }

        /// <summary>
        /// Gets the type identifier for the set.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the unique identifier for a health record item
        /// type.
        /// </value>
        ///
        /// <remarks>
        /// The value must be the identifier for a health record item
        /// type.
        /// <see
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(System.Guid,HealthServiceConnection)"/>
        /// for information on getting the value health record item types.
        /// </remarks>
        ///
        public Guid TypeId { get; } = Guid.Empty;

        /// <summary>
        /// Gets the XML representation of the set.
        /// </summary>
        ///
        /// <returns>
        /// The XML representation of the set as a string.
        /// </returns>
        ///
        /// <remarks>
        /// The XML representation adheres to the schema required by the
        /// HealthVault methods.
        /// </remarks>
        ///
        public override string GetXml()
        {
            return
                "<type-id>" + this.TypeId + "</type-id>";
        }
    }
}
