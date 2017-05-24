// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines a set of things of the specified type
    /// for authorization purposes.
    /// </summary>
    ///
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or
    /// effective date, or explicitly by setting the system set.) This class
    /// serves as a set of things that have a specified type ID.
    /// Other types of authorization sets include:
    /// <see cref="DateRangeSetDefinition"/>.
    /// </remarks>
    ///
    /// <seealso cref="AuthorizationSetDefinition"/>
    /// <seealso cref="DateRangeSetDefinition"/>
    ///
    internal class TypeIdSetDefinition : AuthorizationSetDefinition
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
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(Guid,IConnectionInternal)"/>
        /// for information on getting the value thing types.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="typeId"/> parameter is Guid.Empty.
        /// </exception>
        ///
        public TypeIdSetDefinition(Guid typeId)
            : base(SetType.TypeIdSet)
        {
            if (typeId == Guid.Empty)
            {
                throw new ArgumentException(Resources.TypeIdSetGuidEmpty, nameof(typeId));
            }

            TypeId = typeId;
        }

        /// <summary>
        /// Gets the type identifier for the set.
        /// </summary>
        ///
        /// <value>
        /// A Guid representing the unique identifier for a thing
        /// type.
        /// </value>
        ///
        /// <remarks>
        /// The value must be the identifier for a thing
        /// type.
        /// <see
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(Guid,IConnectionInternal)"/>
        /// for information on getting the value thing types.
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
                "<type-id>" + TypeId + "</type-id>";
        }
    }
}
