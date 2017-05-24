// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The base HealthVault thing type
    /// </summary>
    public interface IThing
    {
        /// <summary>
        /// Writes the XML representation of the information into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the contact information should be
        /// written.
        /// </param>
        /// <returns>A bool indicating if an update is required.</returns>
        bool WriteItemXml(XmlWriter writer);

        /// <summary>
        /// Gets the type identifier for the thing type.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the type of the item.
        /// </value>
        ///
        /// <remarks>
        /// The types available can be queried using <see cref="ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(System.Guid,IConnectionInternal)"/>.
        /// </remarks>
        Guid TypeId { get; }

        /// <summary>
        /// Gets the key of the thing.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the item issued when the item is
        /// created and a globally unique version stamp updated every time
        /// the item is changed.
        /// </value>
        ///
        /// <remarks>
        /// This is the only property that
        /// is guaranteed to be available regardless of how
        /// <see cref="ThingBase.Sections"/> is set.
        /// </remarks>
        ///
        ThingKey Key { get; }
    }
}
