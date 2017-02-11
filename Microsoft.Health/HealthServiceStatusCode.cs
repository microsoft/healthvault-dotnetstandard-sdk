// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health
{
    /// <summary>
    /// Status codes for various conditions occurring in the SDK. Contains 
    /// status codes returned by HealthVault methods as Response.Code
    /// and codes to represent errors in the SDK itself.
    /// </summary>
    /// 
    public enum HealthServiceStatusCode
    {
        /// <summary>
        /// The other data item cannot be larger than the largest size allowed by the 
        /// HealthVault platform.
        /// </summary>
        OtherDataItemSizeLimitExceeded = -11,

        /// <summary>
        /// A record was not authorized for the application.
        /// </summary>
        /// 
        RecordAuthorizationFailure = -10,

        /// <summary>
        /// A required configuration value is missing or malformed for the key 
        /// supplied.
        /// </summary>
        /// 
        ConfigValueMissingOrMalformed = -9,

        /// <summary>
        /// The data element was not set, so it could not be retrieved.
        /// </summary>
        /// 
        CannotGetDataNotSet = -8,

        /// <summary>
        /// The record authorization data wasn't supplied to allow the update
        /// to occur.
        /// </summary>
        /// 
        NoRecordAuthDataForUpdate = -7,

        /// <summary>
        /// A person other than a record custodian tried to perform a 
        /// privileged action.
        /// </summary>
        /// 
        NotRecordCustodian = -6,

        /// <summary>
        /// The application information could not be returned for the 
        /// specified application identifier.
        /// </summary>
        /// 
        NoApplicationInfoReturned = -5,

        /// <summary>
        /// Information about multiple persons was returned when only a single
        /// result was expected.
        /// </summary>
        /// 
        MoreThanOnePersonReturned = -4,

        /// <summary>
        /// The record specified in the request could not be found. 
        /// </summary>
        /// 
        RecordNotFound = -3,

        /// <summary>
        /// Information about multiple record items was returned when only a
        /// single result was expected.
        /// </summary>
        /// 
        MoreThanOneThingReturned = -2,

        /// <summary>
        /// The error condition is not recognized by the client.
        /// </summary>
        /// 
        /// <remarks> 
        /// Any integer error code returned by HealthVault without a mapping
        /// in the enum is mapped to this value. This could indicate new 
        /// error values that HealthVault returns. A newer version of the SDK
        /// might provide an appropriate mapping.
        /// </remarks>
        /// 
        UnmappedError = -1,

        /// <summary>
        /// The request was successful.
        /// </summary>
        /// 
        Ok = 0,

        /// <summary>
        /// Generic failure, due to unknown causes or internal error.
        /// </summary>
        /// 
        Failed = 1,

        /// <summary> 
        /// Http protocol problem.
        /// </summary>
        /// 
        BadHttp = 2,

        /// <summary> 
        /// Request xml cannot be parsed or nonconformant.
        /// </summary>
        /// 
        InvalidXml = 3,

        /// <summary> 
        /// The signature is invalid. 
        /// </summary>
        /// 
        InvalidRequestIntegrity = 4,

        /// <summary>
        /// No such method. 
        /// </summary>
        /// 
        BadMethod = 5,

        /// <summary>
        /// App does not exist, app is invalid, app is not active, 
        /// or calling IP is invalid.
        /// </summary>
        /// 
        InvalidApp = 6,

        /// <summary> 
        /// credntial token has expired, need a new one.
        /// </summary>
        /// 
        CredentialTokenExpired = 7,

        /// <summary>
        /// Auth token malformed or otherwise busted. 
        /// </summary>
        /// 
        InvalidToken = 8,

        /// <summary> 
        /// Person does not exist or is not active.
        /// </summary>
        /// 
        InvalidPerson = 9,

        /// <summary> 
        /// Given record id does not exist. 
        /// </summary>
        /// 
        InvalidRecord = 10,

        /// <summary> 
        /// Person or app does not have sufficient rights.
        /// </summary>
        /// 
        AccessDenied = 11,

        /// <summary>
        /// The functionality being accessed is not yet implemented.
        /// </summary>
        /// 
        NotYetImplemented = 12,

        /// <summary> 
        /// Invalid thing identifier. 
        /// </summary>
        /// 
        InvalidThing = 13,

        /// <summary>
        /// Data table already exists with incompatible units.
        /// </summary>
        /// 
        CantConvertUnits = 14,

        /// <summary> 
        /// Missing or invalid GetThingsFilter.
        /// </summary>
        /// 
        InvalidFilter = 15,

        /// <summary> 
        /// Missing or invalid GetThings format specifier.
        /// </summary>
        /// 
        InvalidFormat = 16,

        /// <summary>
        /// A credential was supplied without a shared secret.
        /// </summary>
        /// 
        MissingSharedSecret = 17,

        /// <summary> 
        /// authorized_applications entry missing.
        /// </summary>
        /// 
        InvalidApplicationAuthorization = 18,

        /// <summary> 
        /// thing type doesn't exist. 
        /// </summary>
        /// 
        InvalidThingType = 19,

        /// <summary>
        /// can't update things of this type.
        /// </summary>
        /// 
        ThingTypeImmutable = 20,

        /// <summary> 
        /// can't create things of this type.
        /// </summary>
        /// 
        ThingTypeNotCreatable = 21,

        /// <summary>
        /// Duplicate Credential found.
        /// </summary>
        /// 
        DuplicateCredentialFound = 22,

        /// <summary> 
        /// Invalid Record name.
        /// </summary>
        /// 
        InvalidRecordName = 23,

        /// <summary> 
        /// Cannot find the drug specified.
        /// </summary>
        /// 
        DrugNotFound = 24,

        /// <summary> 
        /// Invalid person state. 
        /// </summary>
        /// 
        InvalidPersonState = 25,

        /// <summary> 
        /// Requested code set was not found. 
        /// </summary>
        /// 
        InvalidCodeSet = 26,

        /// <summary>
        /// Group does not exist, group is invalid, or group is not active.
        /// </summary>
        /// 
        InvalidGroup = 27,

        /// <summary>
        /// Invalid validation token for contact email validation.
        /// </summary>
        /// 
        InvalidValidationToken = 28,

        /// <summary>
        /// Invalid group/person name to create a group.
        /// </summary>
        /// 
        InvalidAccountName = 29,

        /// <summary> 
        /// Invalid contact email to create a group. 
        /// </summary>
        /// 
        InvalidContactEmail = 30,

        /// <summary> 
        /// Invalid logon name.
        /// </summary>
        /// 
        InvalidLogOnName = 31,

        /// <summary> 
        /// Invalid password.
        /// </summary>
        /// 
        InvalidPassword = 32,

        /// <summary>
        /// Transform cannot be loaded. 
        /// </summary>
        /// 
        InvalidTransform = 34,

        /// <summary> 
        /// Invalid relationship type. 
        /// </summary>
        /// 
        InvalidRelationshipType = 35,

        /// <summary> 
        /// Invalid credential type.
        /// </summary>
        /// 
        InvalidCredentialType = 36,

        /// <summary> 
        /// Invalid record state.
        /// </summary>
        /// 
        InvalidRecordState = 37,

        /// <summary>
        /// Application authorization is not required for this app.
        /// </summary>
        /// 
        ApplicationAuthorizationNotRequired = 38,

        ///<summary>
        /// The request provided has exceeded maximum allowed request length.
        ///</summary>
        ///
        RequestTooLong = 39,

        /// <summary> 
        /// Duplicate authorized record found. 
        /// </summary>
        /// 
        DuplicateAuthorizedRecordFound = 40,

        /// <summary> 
        /// Person email must be validated but it's not
        /// </summary>
        /// 
        EmailNotValidated = 41,

        /// <summary>
        /// A group already exists with the specified name.
        /// </summary>
        /// 
        DuplicateGroupFound = 42,

        /// <summary>
        /// The group already is a member of a group.
        /// </summary>
        /// 
        GroupAlreadyHasParent = 43,

        /// <summary>
        /// Adding the group as a member of the specified group would cause
        /// a cycle in the group membership.
        /// </summary>
        /// 
        GroupMembershipCycleDetected = 44,

        /// <summary>
        /// The email address specified to SendInsecureMessage is malformed.
        /// </summary>
        /// 
        MailAddressMalformed = 45,

        /// <summary>
        /// The password does not meet the complexity requirements.
        /// </summary>
        /// 
        PasswordNotStrong = 46,

        /// <summary>
        /// The last custodian for a record cannot be removed.
        /// </summary>
        /// 
        CannotRemoveLastCustodian = 47,

        /// <summary>
        /// The email address is invalid.
        /// </summary>
        /// 
        InvalidEmailAddress = 48,

        /// <summary>
        /// The request sent to HealthVault reached its time to live and
        /// is now too old to be processed.
        /// </summary>
        /// 
        RequestTimedOut = 49,

        /// <summary>
        /// The sponsor email address is invalid.
        /// </summary>
        /// 
        InvalidSponsorEmail = 50,

        /// <summary>
        /// Promotion token is invalid.
        /// </summary>
        /// 
        InvalidPromotionToken = 51,

        /// <summary>
        /// Record authorization token is invalid.
        /// </summary>
        /// 
        InvalidRecordAuthorizationToken = 52,

        /// <summary>
        /// GetThings Query has too many request groups.
        /// </summary>
        /// 
        TooManyRequestGroupsInQuery = 53,

        /// <summary>
        /// The permissions to be granted exceed the default permissions
        /// available to be granted. e.g. attempt to grant all access when only
        /// read access is available.
        /// </summary>
        /// 
        GrantAuthorizationExceedsDefault = 54,

        /// <summary> 
        /// Requested code set was not found.
        /// </summary>
        /// 
        InvalidVocabulary = 55,

        /// <summary> 
        /// An error occurred loading the vocabulary.
        /// </summary>
        /// 
        VocabularyLoadError = 56,

        /// <summary> 
        /// Record authorization token has expired. 
        /// </summary>
        /// 
        RecordAuthorizationTokenExpired = 57,

        /// <summary> 
        /// Record authorization does not exist. 
        /// </summary>
        /// 
        RecordAuthorizationDoesNotExist = 58,

        /// <summary> 
        /// can't delete things of this type.
        /// </summary>
        /// 
        ThingTypeNotDeletable = 59,

        /// <summary>
        /// Version stamp is missing.
        /// </summary>
        /// 
        VersionStampMissing = 60,

        /// <summary> 
        /// Version stamp does not match.
        /// </summary>
        /// 
        VersionStampMismatch = 61,

        /// <summary>
        /// Public key specified is not valid.
        /// </summary>
        /// 
        InvalidPublicKey = 63,

        /// <summary>
        /// The application's domain name is not set.
        /// </summary>
        /// 
        DomainNameNotSet = 64,

        /// <summary> 
        /// authenticated session token has expired, need a new one.
        /// </summary>
        /// 
        AuthenticatedSessionTokenExpired = 65,

        /// <summary>
        /// Indicates that the credential key could not be found.
        /// </summary>
        /// 
        /// <remarks>
        /// The credential key is different depending on the authentication mechanism being use.
        /// For username/password login the credential key is the username. For Windows Live ID
        /// login the credential key is the Windows Live ID PUID.
        /// </remarks>
        /// 
        InvalidCredentialKey = 66,

        /// <summary> 
        /// Pseudo id for person or group not valid
        /// </summary>
        /// 
        InvalidPersonOrGroupId = 67,

        /// <summary>
        /// The size occupied by the <see cref="HealthRecordItem"/>s in the request will cause the 
        /// <see cref="HealthRecordInfo"/> to exceed the size quota allotted to it. 
        /// </summary>
        /// 
        RecordQuotaExceeded = 68,

        /// <summary> 
        /// the datetime exceeds the maximum or minimum values allowed by System.DateTime
        /// or System.Data.SqlTypes.SqlDateTime
        /// </summary>
        /// 
        InvalidDateTime = 69,

        /// <summary> 
        /// The certificate is not valid. It may be expired or have been revoked. 
        /// </summary>
        /// 
        InvalidCertificate = 70,

        ///<summary>
        /// The response has exceeded maximum size allowed.
        ///</summary>
        ///
        ResponseTooLong = 71,

        /// <summary> 
        /// The verification question for the connect request is invalid. 
        /// </summary>
        /// 
        InvalidVerificationQuestion = 72,

        /// <summary> 
        /// The verification answer for the connect request is invalid. 
        /// </summary>
        /// 
        InvalidVerificationAnswer = 73,

        /// <summary> 
        /// There is no connect request corresponding to the given code.
        /// </summary>
        /// 
        InvalidIdentityCode = 74,

        /// <summary>
        /// Number of retries has been exceeded.
        /// </summary>
        /// 
        RetryLimitExceeded = 75,

        /// <summary> 
        /// The culture specified in the request header is not supported for the attempted operation.
        /// </summary>
        /// 
        CultureNotSupported = 76,

        ///<summary>
        /// The file extension is not supported.
        ///</summary>
        ///
        InvalidFileExtension = 77,

        ///<summary>
        /// The vocabulary item does not exist.
        ///</summary>
        ///
        InvalidVocabularyItem = 78,

        ///<summary>
        /// Duplicate connect request found.
        ///</summary>
        ///
        DuplicateConnectRequestFound = 79,

        /// <summary>
        /// The account type being requested is not valid.
        /// </summary>
        /// 
        InvalidSpecialAccountType = 80,

        /// <summary>
        /// An attempt was made to add a new type with an identifier that already exists.
        /// </summary>
        /// 
        DuplicateTypeFound = 81,

        /// <summary>
        /// The requested credential could not be found.
        /// </summary>
        /// 
        CredentialNotFound = 82,

        /// <summary>
        /// Cannot remove the last credential associated with an account.
        /// </summary>
        /// 
        CannotRemoveLastCredential = 83,

        /// <summary>
        /// The connect request has already been authorized.
        /// </summary>
        /// 
        ConnectRequestAlreadyAuthorized = 84,

        /// <summary>
        /// The type specified to update an instance of a thing is an older version of the type
        /// than the existing instance.
        /// </summary>
        /// 
        InvalidThingTypeVersion = 85,

        /// <summary>
        /// The maximum number of allowed credentials has been exceeded.
        /// </summary>
        /// 
        CredentialsLimitExceeded = 86,

        /// <summary>
        /// One or more invalid methods were specified in the method mask.
        /// </summary>
        /// 
        InvalidMethod = 87,

        /// <summary>
        /// The reference url supplied for the blob streaming API is invalid. 
        /// </summary>
        /// 
        /// <remarks>
        /// This value replaces InvalidOtherDataToken which remains in the enum for binary
        /// compatibility.
        /// </remarks>
        /// 
        InvalidBlobRefUrl = 88,

        /// <summary>
        /// Other data put in to Healthvault via the streaming API cannot be requested as an 
        /// other data string.
        /// </summary>
        CannotGetStreamedOtherData = 89,

        /// <summary>
        /// The type version of the thing cannot be changed without a data xml supplied for validation.
        /// </summary>
        UpdateThingTypeVersionNoDataXml = 90,

        /// <summary>
        /// The content encoding specified for the blob is not supported.
        /// </summary>
        UnsupportedContentEncoding = 91,

        /// <summary>
        /// The content encoding specified for the blob does not match the blob data.
        /// </summary>
        ContentEncodingDataMismatch = 92,

        /// <summary>
        /// The number of applications for the person has been exceeded.
        /// </summary>
        ApplicationLimitExceeded = 93,

        /// <summary>
        /// The unique identifier for the binary content could not be found.
        /// </summary>
        InvalidBinaryContentId = 94,

        /// <summary>
        /// The connect request retrieved is not yet fully created.
        /// </summary>
        IncompleteConnectRequest = 95,

        /// <summary>
        /// The connect package was previously fully populated and a new package
        /// cannot be attached.
        /// </summary>
        CreatePackageExists = 96,

        /// <summary>
        /// The file name extension is not supported.
        /// </summary>
        InvalidFileName = 97,

        /// <summary>
        /// The signup code is invalid.
        /// </summary>
        InvalidSignupCode = 98,

        /// <summary>
        /// The blob is too large and cannot be returned inline.
        /// </summary>
        BlobSizeTooLargeForInline = 99,

        /// <summary>
        /// A blob of this name is already present in the request.
        /// </summary>
        DuplicateBlob = 100,

        /// <summary>
        /// The blob token corresponds to a blob that is already committed.
        /// </summary>
        BlobTokenCommitted = 101,

        /// <summary>
        /// The blob token corresponds to a blob that was not marked completed 
        /// through the streaming interface.
        /// </summary>
        BlobTokenNotCompleted = 102,

        /// <summary>
        /// The thing being updated has data items that cannot be seen in this version, e.g.
        /// signatures with new signature methods or multiple blobs.
        /// </summary>
        ThingPotentiallyIncomplete = 104,

        /// <summary>
        /// The signature algorithm is not valid.
        /// </summary>
        InvalidSignatureAlgorithm = 105,

        /// <summary>
        /// The blob hash algorithm is invalid or not supported.
        /// </summary>
        InvalidBlobHashAlgorithm = 106,

        /// <summary>
        /// The blob hash block size is unsupported.
        /// </summary>
        UnsupportedBlobHashBlockSize = 107,

        /// <summary>
        /// The specified blob hash algorithm does not match the blob's hash algorithm.
        /// </summary>
        BlobHashAlgorithmMismatch = 108,

        /// <summary>
        /// The specified blob hash block size does not match the blob's hash block size.
        /// </summary>
        BlobHashBlockSizeMismatch = 109,

        /// <summary>
        /// The signature method is not supported in the context it is being used.
        /// </summary>        
        UnsupportedSignatureMethod = 110,

        /// <summary>
        /// The specified blob hash is invalid.
        /// </summary>
        InvalidBlobHash = 111,

        /// <summary>
        /// The blob is associated with a connect package that is not yet created.
        /// </summary>
        PackageBlobNotCommitted = 112,

        /// <summary>
        /// Changing the application state from deleted is not supported.
        /// </summary>
        ApplicationStateTransitionNotSupported = 113,

        /// <summary>
        /// Application Manager authorization invite already exists.
        /// </summary>
        AppManagerAuthInviteDuplicate = 114,

        /// <summary>
        /// Application Manager invite is not found for re-send
        /// </summary>
        ExistingAppManagerAuthInviteNotFound = 115,

        /// <summary>
        /// Application Manager invite token is invalid.
        /// </summary>
        InvalidAppManagerAuthToken = 116,

        /// <summary>
        /// Cannot resend application manager invite
        /// </summary>
        ResendAppManagerInviteFailed = 117,

        /// <summary>
        /// Application Manager authorization already exists
        /// </summary>
        AppManagerAuthAlreadyExist = 118,

        /// <summary>
        /// Application Manager authorization specified does not exist.
        /// </summary>
        AppManagerAuthDoesNotExist = 119,

        /// <summary>
        /// Entity Manager authorization invite already exists.
        /// </summary>
        EntityManagerAuthInviteDuplicate = 114,

        /// <summary>
        /// Entity Manager invite is not found for re-send
        /// </summary>
        ExistingEntityManagerAuthInviteNotFound = 115,

        /// <summary>
        /// Entity Manager invite token is invalid.
        /// </summary>
        InvalidEntityManagerAuthToken = 116,

        /// <summary>
        /// Cannot resend entity manager invite
        /// </summary>
        ResendEntityManagerInviteFailed = 117,

        /// <summary>
        /// Entity Manager authorization already exists
        /// </summary>
        EntityManagerAuthAlreadyExist = 118,

        /// <summary>
        /// Entity Manager authorization specified does not exist.
        /// </summary>
        EntityManagerAuthDoesNotExist = 119,

        /// <summary>
        /// The contents of the connect package are not valid xml. 
        /// </summary>
        InvalidPackageContents = 120,

        /// <summary>
        ///  The content type of the file is not supported.
        /// </summary>
        InvalidContentType = 121,

        /// <summary>
        /// The contents of the connect package must be validated before they are put into 
        /// a health record.
        /// </summary>
        ConnectPackageValidationRequired = 122,

        /// <summary>
        /// Invalid thing state.
        /// </summary>
        InvalidThingState = 123,

        /// <summary>
        /// The maximum number of things specified has been exceeded.
        /// </summary>
        TooManyThingsSpecified = 124,

        /// <summary>
        /// Then entity type is not valid.
        /// </summary>
        InvalidEntityType = 125,

        /// <summary>
        /// Then directory item specified does not exist or is invalid.
        /// </summary>
        InvalidDirectoryItem = 126,

        /// <summary>
        /// Then directory item type specified is invalid.
        /// </summary>
        InvalidDirectoryType = 127,

        /// <summary>
        /// Then directory item state specified is invalid.
        /// </summary>
        InvalidDirectoryState = 128,

        /// <summary>
        /// The vocbulary authorization is invalid.
        /// </summary>
        InvalidVocabularyAuthorization = 129,

        /// <summary>
        /// Access to the requested vocabulary is denied.
        /// </summary>
        VocabularyAccessDenied = 130,

        /// <summary>
        /// Setting the personal flag is not supported on the type.
        /// </summary>
        PersonalFlagUnsupported = 131,

        ///
        /// <summary>
        /// The requested subscription was not found.
        /// </summary>
        SubscriptionNotFound = 132,

        /// <summary>
        /// The number of subscriptions for the application was exceeded.
        /// </summary>
        SubscriptionLimitExceeded = 133,

        /// <summary>
        /// The subscription contains invalid data.
        /// </summary>
        SubscriptionInvalid = 134,

        /// <summary>
        /// The application creation token was invalid.
        /// </summary>
        InvalidApplicationCreationToken = 135,

        /// <summary>
        /// An application already exists for the token/application id.
        /// </summary>
        DuplicateApplicationId = 136,

        /// <summary>
        /// The application requesting the sharing invitation was not found.
        /// </summary>
        SharingRequestingApplicationNotFound = 137,

        /// <summary>
        /// The encrypted credential has expired.
        /// </summary>
        EncryptedCredentialExpired = 138,

        /// <summary>
        /// The alternate id already exists.
        /// </summary>
        DuplicateAlternateId = 139,

        /// <summary>
        /// The alternate id was not found.
        /// </summary>
        AlternateIdNotFound = 140,

        /// <summary>
        /// The maximum number of alternate ids was exceeded.
        /// </summary>
        AlternateIdsLimitExceeded = 141,

        /// <summary>
        /// The Record Audit was not found.
        /// </summary>
        RecordAuditNotFound = 143,

        /// <summary>
        /// An incorrect pin was used to accept a record sharing invitation.
        /// </summary>
        RecordSharingInvitationPinMismatch = 144,

        /// <summary>
        /// The maximum number of attempts for accepting/rejecting the sharing 
        /// invitation has been exceeded. 
        /// </summary>
        RecordSharingMaximumNumberOfAttemptsExceeded = 145,

        /// <summary>
        /// Optimistic concurrency violation detected.
        /// </summary>
        UpdateConcurrencyViolation = 146,

        /// <summary>
        /// The record location is not supported by the application.
        /// </summary>
        RecordLocationNotSupported = 147,

        /// <summary>
        /// The number of messages to be enqueued exceeds the config-defined limit.
        /// </summary>
        TooManyMessages = 148,

        /// <summary>
        /// There was an error when trying to add the messages to the queue.
        /// </summary>
        CouldNotEnqueue = 149,

        /// <summary>
        /// The specified location already exists for this application, or a warning
        /// message in the specified language already exists for this location.
        /// </summary>
        DuplicateAppLocationInfo = 150,

        /// <summary>
        /// The auth allowed warning messages are not required in the input for the remove app locations 
        /// method.
        /// </summary>
        RemoveLocationsInvalidWarningMessages = 151,

        /// <summary>
        /// Access is denied temporarily because HealthVault is in maintenance mode.
        /// </summary>
        MaintenanceModeAccessDenied = 152,

        /// <summary>
        /// The specified HealthVault instance identifier is invalid.
        /// </summary>
        InvalidInstanceId = 153,

        /// <summary>
        /// Cannot update immutable thing instance.
        /// </summary>
        CannotUpdateReadOnlyThing = 154,

        /// <summary>
        /// Cannot create immutable thing instance.
        /// </summary>
        CannotCreateReadOnlyThing = 155,

        /// <summary>
        /// Cannot change immutable flag to false if it is true.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms")]
        CannotChangeReadOnlyFlag = 156,

        /// <summary>
        /// Age not supported for new accounts.
        /// </summary>
        InvalidAge = 157,

        /// <summary>
        /// IP Address not supported for method invocation.
        /// </summary>
        InvalidIPAddress = 158,

        /// <summary>
        /// Cannot set immutable flag to true on update.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms")]
        CannotSetReadOnlyFlag = 161,

        /// <summary>
        /// The communication preferences access token has expired.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms")]
        CommunicationPreferencesAccessTokenExpired = 162,

        /// <summary>
        /// Cannot process SAML 2.0 token
        /// </summary>
        InvalidSaml2Token = 163,

        /// <summary>
        /// Invalid operation if Meaningful Use feature is disabled.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms")]
        MeaningfulUseFeatureDisabled = 164,

        /// <summary>
        /// A key was specified more than once in the request in an application
        /// cache request.
        /// </summary>
        ApplicationCacheRequestDuplicateKey = 165,

        /// <summary>
        /// One or more cache items are invalid.  A cache item with force-overwrite
        /// set to true cannot specify an old-version-id.
        /// </summary>
        ApplicationCacheRequestInvalidItem = 166,

        /// <summary>
        /// Cache operation has failed due to capacity limits.
        /// </summary>
        ApplicationCacheRequestTooLarge = 167,

        /// <summary>
        /// Direct Address already exists.
        /// </summary>
        DuplicateDirectAddress = 168,

        /// <summary>
        /// Direct address invalid format.
        /// </summary>
        InvalidDirectAddress = 169,

        /// <summary>
        /// The maximum number of direct addresses has been exceeded.
        /// </summary>
        MaxDirectAddressesExceeded = 170,

        /// <summary>
        /// The direct address could not be found.
        /// </summary>
        DirectAddressNotFound = 171,

        /// <summary>
        /// One or more invalid Meaningful Use sources were specified.
        /// </summary>
        InvalidMeaningfulUseSource = 172,

        /// <summary>
        /// One or more invalid Meaningful Use sources were specified due to duplicates.
        /// </summary>
        DuplicateMeaningfulUseSources = 173,

        /// <summary>
        /// Meaningful Use sources were specified though application is not Meaningful Use enabled.
        /// </summary>
        AppNotEnabledForMeaningfulUse = 174,

        /// <summary>
        /// Meaningful Use sources ceiling were specified though application's method ceiling is not Meaningful Use enabled.
        /// </summary>
        ChildAppsNotEnabledForMeaningfulUse = 175,

        /// <summary>
        /// DataPull:Update/Remove DataConnection - Connection not found.
        /// </summary>
        DataConnectionNotFound = 176,
        
        /// <summary>
        /// DataPull: SourceId is either not active or not a data adapter.
        /// </summary>
        InvalidSourceId = 177,

        /// <summary>
        /// DataConnection is not enabled.
        /// </summary>
        DataConnectionStatusError = 178,
        
        /// <summary>
        /// The specified signal type is invalid.
        /// </summary>
        InvalidSignalType = 179,

        /// <summary>
        /// The specified GetThings order-by specifications are invalid.
        /// </summary>
        InvalidThingOrderSpecs = 180,

        /// <summary>
        /// Thing sort functionality is not enabled.
        /// </summary>
        ThingSortNotEnabled = 181,

        /// <summary>
        /// Record store is not available.
        /// </summary>
        RecordStoreUnavailable = 182,

        /// <summary>
        /// When record is under maintenance, all reads and writes are blocked.
        /// </summary>
        RecordUnderMaintenance = 183,

        /// <summary>
        /// The account is already paired.
        /// </summary>
        AccountPaired = 184,

        /// <summary>
        /// The record does not meet the requirements for pairing.
        /// </summary>
        PairingRecordRequirementsNotMet = 185,

        /// <summary>
        /// The Microsoft Health user ID is already paired with another account.
        /// </summary>
        MshUserIdPaired = 186,

        /// <summary>
        /// Invalid Microsoft Heath User ID
        /// </summary>
        InvalidMshUserId = 187,

        /// <summary> 
        /// Client id value for connect request invalid. 
        /// </summary>
        InvalidVerificationClientId = 188,

        /// <summary>
        /// The client ID references multiple things.
        /// </summary>
        ClientIdReferencesMultipleThings = 189,

        // Add new values here.

        /// <summary>
        /// The maximum value of the status code that the SDK knows about.
        /// This is not a valid status code.
        /// </summary>
        /// 
        Max,
    }
}
