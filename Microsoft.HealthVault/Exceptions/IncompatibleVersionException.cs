// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Indicates version incompatibility.
    /// </summary>
    ///
    [Serializable]
    public class IncompatibleVersionException : Exception
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
            this.CompatibleVersions = compatibleVersions;
            this.IncompatibleVersion = incompatibleVersion;
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
        /// class with default values.
        /// </summary>
        ///
        public IncompatibleVersionException()
        {
        }

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
                string.Join(" ", base.ToString(), this.GetType().ToString(), ":CompatibleVersions =", this.CompatibleVersions ?? string.Empty, ":IncompatibleVersion =", this.IncompatibleVersion ?? string.Empty);
            return result;
        }
    }
}
