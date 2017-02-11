// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Health
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
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString(
                            "IncompatibleVersionExceptionMessageFormatString"),
                        compatibleVersions == null
                            ? String.Empty : compatibleVersions,
                        incompatibleVersion == null
                            ? String.Empty : incompatibleVersion))
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
        public string CompatibleVersions
        {
            get { return _compatibleVersions; }
        }
        private string _compatibleVersions;

        /// <summary>
        /// Gets the incompatible version.
        /// </summary>
        /// 
        public string IncompatibleVersion
        {
            get { return _incompatibleVersion; }
        }
        private string _incompatibleVersion;

        #region FxCop required ctors

        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/> 
        /// class with default values.
        /// </summary>
        /// 
        public IncompatibleVersionException()
            : base()
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

        #region Serialization

        /// <summary>
        /// Creates a new instance of the <see cref="IncompatibleVersionException"/> 
        /// class with the specified serialization information.
        /// </summary>
        /// 
        /// <param name="info">
        /// Serialized information about this exception.
        /// </param>
        /// 
        /// <param name="context">
        /// The stream context of the serialized information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        protected IncompatibleVersionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(
                    "info",
                    ResourceRetriever.GetResourceString(
                        "ExceptionSerializationInfoNull"));
            }

            info.AddValue("compatibleVersions", _compatibleVersions);
            info.AddValue("incompatibleVersion", _incompatibleVersion);
        }

        /// <summary>
        /// Serializes the exception.
        /// </summary>
        /// 
        /// <param name="info">
        /// The serialization information.
        /// </param>
        /// 
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is <b>null</b>.
        /// </exception>
        [SecurityCritical]
        [SecurityPermission(
            SecurityAction.LinkDemand,
            SerializationFormatter = true)]
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(
                    "info",
                    ResourceRetriever.GetResourceString(
                        "ExceptionSerializationInfoNull"));
            }

            base.GetObjectData(info, context);
            info.AddValue("compatibleVersions", _compatibleVersions);
            info.AddValue("incompatibleVersion", _incompatibleVersion);
        }

        #endregion Serialization

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
                String.Join(" ",
                new string[] {
                    base.ToString(),
                    GetType().ToString(),
                    ":CompatibleVersions =",
                    CompatibleVersions == null 
                        ? String.Empty : CompatibleVersions,
                    ":IncompatibleVersion =",
                    IncompatibleVersion == null
                        ? String.Empty : IncompatibleVersion
                });
            return result;
        }
    }
}

