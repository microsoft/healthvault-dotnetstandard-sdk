// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


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
        ///</param>
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
                case HealthServiceStatusCode.OtherDataItemSizeLimitExceeded :
                    e = new HealthServiceOtherDataSizeLimitExceededException(error);
                    break;
                default:
                    e = new HealthServiceException(errorCode, error);
                    break;
            }
            return e;
        }
    }

}
