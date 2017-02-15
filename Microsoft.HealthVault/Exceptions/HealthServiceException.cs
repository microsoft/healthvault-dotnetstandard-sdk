// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Represents the base class for all HealthVault exceptions
    /// thrown by the SDK.
    /// </summary>
    ///
    [Serializable]
    public class HealthServiceException : Exception
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceException"/>
        /// class with the specified error (status) code.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type.
        /// </remarks>
        ///
        /// <param name="errorCode">
        /// The status code representing the error that occurred.
        ///</param>
        ///
        internal HealthServiceException(HealthServiceStatusCode errorCode)
            : this((int)errorCode, null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceException"/>
        /// class with the specified error (status) code and error information.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type.
        /// </remarks>
        ///
        /// <param name="errorCode">
        /// The status code representing the error.
        ///</param>
        ///
        /// <param name="error">
        /// Information about an error that occurred while processing
        /// the request.
        /// </param>
        ///
        internal HealthServiceException(HealthServiceStatusCode errorCode,
            HealthServiceResponseError error)
            : this((int)errorCode, error)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceException"/>
        /// class with the specified error identifier and error information.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type.
        /// </remarks>
        ///
        /// <param name="errorCodeId">
        /// An integer representing the identifier of the status code
        /// of the error.
        ///</param>
        ///
        /// <param name="error">
        /// Information about an error that occurred while processing
        /// the request.
        /// </param>
        ///
        internal HealthServiceException(int errorCodeId,
            HealthServiceResponseError error)
            : base(GetMessage(errorCodeId, error))
        {
            _error = error;
            _errorCodeId = errorCodeId;
        }

        private static string GetMessage(
            int errorCodeId,
            HealthServiceResponseError error)
        {
            return error != null ? error.Message : ResourceRetriever.FormatResourceString(
                        "HealthServiceExceptionNoResponseError",
                        errorCodeId);
        }

        #region FxCop required ctors

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceException"/>
        /// class with default values.
        /// </summary>
        ///
        public HealthServiceException()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceException"/>
        /// class having the specified error message.
        /// </summary>
        ///
        /// <param name="message">
        /// A string representing the error message.
        /// </param>
        ///
        public HealthServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="HealthServiceException"/>
        /// class having the specified error message and inner exception.
        /// </summary>
        ///
        /// <param name="message">
        /// A string representing the error message.
        /// </param>
        ///
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        ///
        public HealthServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion FxCop required ctors

        #region public properties

        /// <summary>
        /// Gets the status code returned by HealthVault to indicate the type
        /// of error that causes this exception.
        /// </summary>
        ///
        public HealthServiceStatusCode ErrorCode => HealthServiceStatusCodeManager.GetStatusCode(
            _errorCodeId);

        /// <summary>
        /// Gets the identifier of the status code in the HealthVault response.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the identifier.
        /// </value>
        ///
        /// <remarks>
        /// This property is useful when the SDK is out of sync with the
        /// HealthVault status code set. The actual integer value of the
        /// status code can be looked up for further investigation.
        /// </remarks>
        ///
        public int ErrorCodeId
        {
            get { return _errorCodeId; }
        }

        private int _errorCodeId = 1; // general failure

        /// <summary>
        /// Gets the information about an error that occurred while processing
        /// the request.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceResponseError"/> object representing the
        /// error information.
        /// </value>
        ///
        /// <remarks>
        /// This value can be <b>null</b> for those exceptions that
        /// occurred in the SDK rather than in HealthVault.
        /// </remarks>
        ///
        public HealthServiceResponseError Error
        {
            get
            {
                return _error;
            }
        }
        private HealthServiceResponseError _error;

        /// <summary>
        /// Contains the response information from the HealthVault service after
        /// processing a request.
        /// </summary>
        public HealthServiceResponseData Response
        {
            get
            {
                return _response;
            }
            internal set
            {
                _response = value;
            }
        }
        private HealthServiceResponseData _response;

        #endregion public properties

        /// <summary>
        /// Retrieves the string representing the
        /// <see cref="HealthServiceException"/> object.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the contents of the
        /// <see cref="HealthServiceException"/> object.
        /// </returns>
        ///
        public override string ToString()
        {
            string result =
                String.Join(" ",
                new string[] {
                    base.ToString(),
                    GetType().ToString(),
                    ":StatusCode =",
                    ErrorCode.ToString(),
                    ":StatusCodeId =",
                    _errorCodeId.ToString(CultureInfo.InvariantCulture)
                });
            return result;
        }
    }
}
