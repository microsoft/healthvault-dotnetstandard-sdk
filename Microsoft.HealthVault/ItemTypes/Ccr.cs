// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml.XPath;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores a Continuity of Care Record (CCR).
    /// </summary>
    ///
    /// <remarks>
    /// The CCR XML can be accessed through the TypeSpecificXml property.
    /// </remarks>
    ///
    public class CCR : ThingBase
    {
        /// <summary>
        /// Initializes an instance of the <see cref="CCR"/> class,
        /// with default values.
        /// </summary>
        ///
        public CCR()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="CCR"/> class,
        /// with specific data.
        /// </summary>
        ///
        public CCR(IXPathNavigable typeSpecificData)
            : base(TypeId, typeSpecificData)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("1e1ccbfc-a55d-4d91-8940-fa2fbf73c195");
    }
}
