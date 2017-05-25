// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines the interface for interacting with sets of things
    /// for authorization purposes. This class is abstract.
    /// </summary>
    ///
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or
    /// effective date, or explicitly by setting the system set).
    /// <see cref="AuthorizationSetDefinition"/> is the base class for the
    /// different types of authorization sets.
    /// The concrete class definitions that define the sets are
    /// <see cref="DateRangeSetDefinition"/> and <see cref="TypeIdSetDefinition"/>.
    /// </remarks>
    ///
    /// <seealso cref="DateRangeSetDefinition"/>
    /// <seealso cref="TypeIdSetDefinition"/>
    ///
    internal abstract class AuthorizationSetDefinition
    {
        internal AuthorizationSetDefinition(SetType setType)
        {
            SetType = setType;
        }

        internal SetType SetType { get; }

        /// <summary>
        /// Retrieves the XML representation of the set. This method is abstract.
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
        public abstract string GetXml();

        /// <summary>
        /// Retrieves the XML representation of the set as a string.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the XML representation of the set.
        /// </returns>
        ///
        /// <remarks>
        /// The XML representation adheres to the schema required by the
        /// HealthVault methods.
        /// </remarks>
        ///
        public override string ToString()
        {
            return GetXml();
        }
    }
}
