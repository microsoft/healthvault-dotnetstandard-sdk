// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// Helper class that allows the SDK to throw the appropriate
    /// HealthServiceException based on the status code returned by
    /// HealthVault.
    /// </summary>
    ///
    internal static class HealthServiceExceptionHelper
    {
        /// <summary>
        /// Helper method that allows the SDK to throw the appropriate
        /// HealthServiceException based on the status code returned by
        /// HealthVault.
        /// </summary>
        ///
        /// <param name="responseData">
        /// The HealthServiceResponseData object created by parsing response returned by HealthVault.
        /// </param>
        ///
        internal static HealthServiceException GetHealthServiceException(HealthServiceResponseData responseData)
        {
            HealthServiceStatusCode errorCode =
                HealthServiceStatusCodeManager.GetStatusCode(responseData.CodeId);

            var e = errorCode != HealthServiceStatusCode.UnmappedError ? GetHealthServiceException(errorCode, responseData.Error) : new HealthServiceException(responseData.CodeId, responseData.Error);

            e.Response = responseData;

            return e;
        }

        /// <summary>
        /// Helper method that allows the SDK to throw the appropriate
        /// HealthServiceException based on the status code indicating the error
        /// type.
        /// </summary>
        ///
        /// <param name="errorCode">
        /// The status code representing the error which occurred.
        /// </param>
        ///
        /// <param name="error">
        /// Information about an error that occurred while processing
        /// the request.
        /// </param>
        ///
        internal static HealthServiceException GetHealthServiceException(
            HealthServiceStatusCode errorCode,
            HealthServiceResponseError error)
        {
            HealthServiceException e;
            switch (errorCode)
            {
                case HealthServiceStatusCode.CredentialTokenExpired:
                    e = new HealthServiceCredentialTokenExpiredException(error);
                    break;
                case HealthServiceStatusCode.AuthenticatedSessionTokenExpired:
                    e = new HealthServiceAuthenticatedSessionTokenExpiredException(error);
                    break;
                case HealthServiceStatusCode.InvalidPerson:
                    e = new HealthServiceInvalidPersonException(error);
                    break;
                case HealthServiceStatusCode.InvalidRecord:
                    e = new HealthServiceInvalidRecordException(error);
                    break;
                case HealthServiceStatusCode.AccessDenied:
                    e = new HealthServiceAccessDeniedException(error);
                    break;
                case HealthServiceStatusCode.InvalidApplicationAuthorization:
                    e = new HealthServiceInvalidApplicationAuthorizationException(error);
                    break;
                case HealthServiceStatusCode.DuplicateCredentialFound:
                    e = new HealthServiceApplicationDuplicateCredentialException(error);
                    break;
                case HealthServiceStatusCode.MailAddressMalformed:
                    e = new HealthServiceMailAddressMalformedException(error);
                    break;
                case HealthServiceStatusCode.PasswordNotStrong:
                    e = new HealthServicePasswordNotStrongException(error);
                    break;
                case HealthServiceStatusCode.RecordQuotaExceeded:
                    e = new HealthServiceRecordQuotaExceededException(error);
                    break;
                default:
                    e = new HealthServiceException(errorCode, error);
                    break;
            }

            return e;
        }
    }
}
