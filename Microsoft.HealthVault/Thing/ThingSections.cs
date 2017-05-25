// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The section that will be or were retrieved when accessing
    /// a <see cref="ThingBase"/>.
    /// </summary>
    ///
    /// <remarks>
    /// To enable efficient use of the network, the HealthVault service
    /// retrieves only those portions of a <see cref="ThingBase"/> that
    /// the application needs.
    /// These sections are identified in the <see cref="HealthRecordView"/>
    /// when performing a search, and are specified on a <see cref="ThingBase"/>
    /// when the item is retrieved.
    /// When <see cref="HealthRecordAccessor.UpdateItemsAsync"/> is called,
    /// only specified sections are updated.
    /// </remarks>
    ///
    [Flags]
    public enum ThingSections
    {
        /// <summary>
        /// No sections are retrieved or represented in the
        /// <see cref="ThingBase"/> object.
        /// </summary>
        ///
        None = 0x0,

        /// <summary>
        /// The Core section of the <see cref="ThingBase"/>
        /// is retrieved.
        /// </summary>
        ///
        Core = 0x1,

        /// <summary>
        /// The Audits section of the <see cref="ThingBase"/>
        /// is retrieved.
        /// </summary>
        ///
        Audits = 0x2,

        /// <summary>
        /// The EffectivePermissions section is retrieved, showing the
        /// permission granted to the caller of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        EffectivePermissions = 0x4,

        /// <summary>
        /// The BlobPayload section of the <see cref="ThingBase"/>
        /// is retrieved.
        /// </summary>
        ///
        BlobPayload = 0x8,

        /// <summary>
        /// The Signature section of the <see cref="ThingBase"/>
        /// is retrieved.
        /// </summary>
        ///
        Signature = 0x10,

        /// <summary>
        /// The Tags section of the <see cref="ThingBase"/>
        /// is retrieved.
        /// </summary>
        ///
        Tags = 0x20,

        /// <summary>
        /// Gets the base XML section.
        /// </summary>
        ///
        /// <private>
        /// This is here even though it is not exposed by HealthVault
        /// to make it easier for SDK developers to use.
        /// </private>
        ///
        Xml = 0x1000000,

        /// <summary>
        /// All sections of the <see cref="ThingBase"/>
        /// are retrieved.
        /// </summary>
        ///
        All = Core | Audits | EffectivePermissions | BlobPayload | Xml | Signature | Tags,

        /// <summary>
        /// Default sections <see cref="ThingBase"/>
        /// are retrieved.
        /// </summary>
        ///
        Default = Core | Xml
    }
}
