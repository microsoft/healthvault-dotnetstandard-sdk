// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Contains error information for a response that has a code other
    /// than <see cref="HealthServiceStatusCode.Ok"/>.
    /// </summary>
    ///
    public class HealthServiceResponseError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        ///
        /// <value>
        /// A string representing the error message.
        /// </value>
        ///
        /// <remarks>
        /// The message contains localized text of why the request failed.
        /// This text should be added to application context information
        /// and suggestions of what to do before displaying it to the user.
        /// </remarks>
        ///
        public string Message
        {
            get { return this.message; }
            internal set { this.message = value; }
        }

        private string message;

        /// <summary>
        /// Gets the context of the server in which the error occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceErrorContext"/> representing the server context.
        /// </value>
        ///
        /// <remarks>
        /// This information is available only when the service is configured
        /// in debugging mode. In all other cases, this property returns
        /// <b>null</b>.
        /// </remarks>
        ///
        internal HealthServiceErrorContext Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        private HealthServiceErrorContext context;

        /// <summary>
        /// Gets the additional information specific to the method failure.
        /// </summary>
        ///
        /// <value>
        /// A string representing the additional error information.
        /// </value>
        ///
        /// <remarks>
        /// The text contains specific actionable information related to the failed request.
        /// It may be used in determining possible actions to circumvent the error condition
        /// before displaying an error to the user.
        /// </remarks>
        ///
        public string ErrorInfo
        {
            get { return this.errorInfo; }
            internal set { this.errorInfo = value; }
        }

        private string errorInfo;

        /// <summary>
        /// Gets the string representation of the <see cref="HealthServiceErrorContext"/>
        /// object.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the contents of the <see cref="HealthServiceErrorContext"/>
        /// object.
        /// </returns>
        ///
        public override string ToString()
        {
            string result =
                string.Join(" ", this.GetType().ToString(), this.message);
            return result;
        }
    }
}
