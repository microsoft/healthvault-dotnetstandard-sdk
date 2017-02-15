// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;

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
            : this(compatibleVersions,
                    incompatibleVersion,
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString(
                            "IncompatibleVersionExceptionMessageFormatString"),
                        compatibleVersions ?? string.Empty,
                        incompatibleVersion ?? string.Empty))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/>
        /// class with an error message.
        /// </summary>
        ///
        /// <param name="compatibleVersions">
        /// The compatible versions supported.
        /// </param>
        ///
        /// <param name="incompatibleVersion">The incompatible version
        /// encountered.</param>
        ///
        /// <param name="message">
        /// The error message.
        /// </param>
        ///
        public IncompatibleVersionException(
            string compatibleVersions,
            string incompatibleVersion,
            string message)
            : this(compatibleVersions, incompatibleVersion, message, null)
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
            Exception innerException)
            : base(message, innerException)
        {
            _compatibleVersions = compatibleVersions;
            _incompatibleVersion = incompatibleVersion;
        }

        /// <summary>
        /// Gets the compatible versions.
        /// </summary>
        ///
        public string CompatibleVersions => _compatibleVersions;

        private readonly string _compatibleVersions;

        /// <summary>
        /// Gets the incompatible version.
        /// </summary>
        ///
        public string IncompatibleVersion => _incompatibleVersion;

        private readonly string _incompatibleVersion;

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
                string.Join(" ", base.ToString(), GetType().ToString(), ":CompatibleVersions =", CompatibleVersions ?? string.Empty, ":IncompatibleVersion =", IncompatibleVersion ?? string.Empty);
            return result;
        }
    }
}
