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
    /// Enumeration used to specify the sections of thing type definition that should be returned.
    /// </summary>
    ///
    [Flags]
    public enum ThingTypeSections
    {
        /// <summary>
        /// Indicates no information about the thing type definition should be
        /// returned.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Indicates the core information about the thing type definition
        /// should be returned.
        /// </summary>
        Core = 0x1,

        /// <summary>
        /// Indicates the schema of the thing type definition should be returned.
        /// </summary>
        Xsd = 0x2,

        /// <summary>
        /// Indicates the columns used by the thing type definition should be returned.
        /// </summary>
        Columns = 0x4,

        /// <summary>
        /// Indicates the transforms supported by the thing type definition
        /// should be returned.
        /// </summary>
        Transforms = 0x8,

        /// <summary>
        /// Indicates the transforms and their XSL source supported by the health record
        /// item type definition should be returned.
        /// </summary>
        TransformSource = 0x10,

        /// <summary>
        /// Indicates the versions of the thing type definition should be returned.
        /// </summary>
        Versions = 0x20,

        /// <summary>
        /// Indicates the effective date XPath of the thing type definition
        /// should be returned.
        /// </summary>
        EffectiveDateXPath = 0x40,

        /// <summary>
        /// Indicates all information for the thing type definition
        /// should be returned.
        /// </summary>
        All = Core | Xsd | Columns | Transforms | TransformSource | Versions | EffectiveDateXPath
    }
}
