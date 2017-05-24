// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Indicates version incompatibility.
    /// </summary>
    ///
    public class IncompatibleVersionException : HealthVaultException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/>
        /// class.
        /// </summary>
        ///
        /// <param name="compatibleVersions">
        /// The compatible versions supported.
        /// </param>
        ///
        /// <param name="incompatibleVersion">
        /// The incompatible version encountered.
        /// </param>
        ///
        public IncompatibleVersionException(
            string compatibleVersions,
            string incompatibleVersion)
            : this(
                compatibleVersions,
                incompatibleVersion,
                Resources.IncompatibleVersionExceptionMessageFormatString.FormatResource(
                    compatibleVersions ?? string.Empty,
                    incompatibleVersion ?? string.Empty))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/>
        /// class with an error message and a nested exception.
        /// </summary>
        ///
        /// <param name="compatibleVersions">
        /// The compatible versions supported.
        /// </param>
        ///
        /// <param name="incompatibleVersion">
        /// The incompatible version encountered.
        /// </param>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The nested exception.
        /// </param>
        ///
        public IncompatibleVersionException(
            string compatibleVersions,
            string incompatibleVersion,
            string message,
            Exception innerException = null)
            : base(message, innerException)
        {
            CompatibleVersions = compatibleVersions;
            IncompatibleVersion = incompatibleVersion;
        }

        /// <summary>
        /// Gets the compatible versions.
        /// </summary>
        ///
        public string CompatibleVersions { get; }

        /// <summary>
        /// Gets the incompatible version.
        /// </summary>
        ///
        public string IncompatibleVersion { get; }

        #region FxCop required ctors

        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/>
        /// class with the specified message.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        public IncompatibleVersionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/>
        /// class with the specified message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        ///
        public IncompatibleVersionException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion FxCop required ctors

        /// <summary>
        /// Gets the string representation of the <see cref="IncompatibleVersionException"/>
        /// object.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the contents of the
        /// <see cref="IncompatibleVersionException"/> object.
        /// </returns>
        ///
        public override string ToString()
        {
            string result =
                string.Join(" ", base.ToString(), GetType().ToString(), ":CompatibleVersions =", CompatibleVersions ?? string.Empty, ":IncompatibleVersion =", IncompatibleVersion ?? string.Empty);
            return result;
        }
    }
}
