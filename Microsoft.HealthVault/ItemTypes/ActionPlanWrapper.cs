// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates action plan
    /// related object.
    /// </summary>
    public class ActionPlanWrapper : MshItemBase
    {
        #region ctor

        /// <summary>
        /// Creates a new instance of the <see cref="ActionPlanWrapper"/>
        /// class with default values
        /// </summary>
        public ActionPlanWrapper()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ActionPlanWrapper"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="wrappedInstance">wrapped instance</param>
        public ActionPlanWrapper(IMshItem wrappedInstance)
            : base(wrappedInstance, TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ActionPlanWrapper"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="wrappedTypeName">Wrapped type name</param>
        /// <param name="base64EncodedJson">Base64-encoded JSON</param>
        public ActionPlanWrapper(
            string wrappedTypeName,
            string base64EncodedJson)
            : base(TypeId, wrappedTypeName, base64EncodedJson, Guid.Empty, Guid.Empty)
        {
        }

        #endregion

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        public static new readonly Guid TypeId =
            new Guid("10291BCD-6C33-4297-86DF-167FEDEFD6D8");

        /// <summary>
        /// Root element name within thing-xml.
        /// </summary>
        protected override string RootElementName => "action-plan";
    }
}
