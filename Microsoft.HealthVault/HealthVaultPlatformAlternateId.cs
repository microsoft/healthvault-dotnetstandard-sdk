// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Exceptions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformPrimitives
{
    /// <summary>
    /// Provides low-level access to the HealthVault alternate id operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformAlternateId.Current to a derived class to intercept all message calls.
    /// </remarks>

    public class HealthVaultPlatformAlternateId
    {
        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        ///
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked.
        /// </remarks>
        ///
        /// <param name="mock">The mocking class.</param>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        ///
        public static void EnableMock(HealthVaultPlatformAlternateId mock)
        {
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        ///
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }

        internal static HealthVaultPlatformAlternateId Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformAlternateId _current = new HealthVaultPlatformAlternateId();
        private static HealthVaultPlatformAlternateId _saved;

        /// <summary>
        /// Associates an alternate ID with a record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="alternateId">
        /// The alternate ID.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The connection, accessor, or alternateId parameters are null
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// If the alternate ID is already associated by this application, the ErrorCode property
        /// will be set to DuplicateAlternateId.
        /// If the number of alternate IDs associated with a record exceeds the limit, the ErrorCode
        /// property will be set to AlternateIdsLimitExceeded.
        /// </exception>
        /// 
        public virtual async Task AssociateAlternateIdAsync(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            string alternateId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "AlternateIdConnectionNull");
            Validator.ThrowIfArgumentNull(accessor, "accessor", "AccessorNull");
            Validator.ThrowIfArgumentNull(alternateId, "alternateId", "AlternateIdNull");

            Validator.ThrowIfStringIsEmptyOrWhitespace(alternateId, "alternateId");
            Validator.ThrowArgumentExceptionIf(alternateId.Length > 255, "alternateId", "AlternateIdTooLong");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "AssociateAlternateId", 1, accessor);

            request.Parameters = "<alternate-id>" + alternateId + "</alternate-id>";
            await request.ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Disassociates an alternate ID with a record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="alternateId">
        /// The alternate ID.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The connection, accessor, or alternateId parameters are null.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public virtual async Task DisassociateAlternateIdAsync(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            string alternateId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "AlternateIdConnectionNull");
            Validator.ThrowIfArgumentNull(accessor, "accessor", "AccessorNull");
            Validator.ThrowIfArgumentNull(alternateId, "alternateId", "AlternateIdNull");

            Validator.ThrowIfStringIsEmptyOrWhitespace(alternateId, "alternateId");
            Validator.ThrowArgumentExceptionIf(alternateId.Length > 255, "alternateId", "AlternateIdTooLong");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "DisassociateAlternateId", 1, accessor);

            request.Parameters = "<alternate-id>" + alternateId + "</alternate-id>";
            await request.ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Disassociates an alternate ID with a record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="alternateId">
        /// The alternate ID.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The connection, or alternateId parameters are null.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public virtual async Task DisassociateAlternateIdAsync(
            ApplicationConnection connection,
            string alternateId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "AlternateIdConnectionNull");
            Validator.ThrowIfArgumentNull(alternateId, "alternateId", "AlternateIdNull");

            Validator.ThrowIfStringIsEmptyOrWhitespace(alternateId, "alternateId");
            Validator.ThrowArgumentExceptionIf(alternateId.Length > 255, "alternateId", "AlternateIdTooLong");

            HealthServiceRequest request = new HealthServiceRequest(connection, "DisassociateAlternateId", 1);

            request.Parameters = "<alternate-id>" + alternateId + "</alternate-id>";
            await request.ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list of alternate IDs that are associated with a record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        ///
        public virtual async Task<Collection<string>> GetAlternateIdsAsync(
            ApplicationConnection connection,
            HealthRecordAccessor accessor)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetAlternateIds", 1, accessor);

            HealthServiceResponseData responseData = await request.ExecuteAsync().ConfigureAwait(false);

            Collection<string> alternateIds = new Collection<string>();

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(
                        SDKHelper.GetInfoXPathExpressionForMethod(
                                responseData.InfoNavigator,
                                "GetAlternateIds"));

            // Get the alternate ids that came back
            XPathNodeIterator alternateIdsNav = infoNav.Select("alternate-ids/alternate-id");

            foreach (XPathNavigator alternateIdNode in alternateIdsNav)
            {
                alternateIds.Add(alternateIdNode.Value);
            }

            return alternateIds;
        }

        /// <summary>
        /// Gets the person and record IDs that were previosly associated
        /// with an alternate ID.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="alternateId">
        /// The alternate ID.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The connection, accessor, or alternateId parameters are null
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public virtual async Task<PersonInfo> GetPersonAndRecordForAlternateIdAsync(
            ApplicationConnection connection,
            string alternateId)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "AlternateIdConnectionNull");
            Validator.ThrowIfArgumentNull(alternateId, "alternateId", "AlternateIdNull");

            Validator.ThrowIfStringIsEmptyOrWhitespace(alternateId, "alternateId");
            Validator.ThrowArgumentExceptionIf(alternateId.Length > 255, "alternateId", "AlternateIdTooLong");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetPersonAndRecordForAlternateId", 1);

            request.Parameters = "<alternate-id>" + alternateId + "</alternate-id>";

            HealthServiceResponseData responseData = await request.ExecuteAsync().ConfigureAwait(false);

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(
                        SDKHelper.GetInfoXPathExpressionForMethod(
                                responseData.InfoNavigator,
                                "GetPersonAndRecordForAlternateId"));

            XPathNavigator personInfoNav = infoNav.SelectSingleNode("person-info");

            return PersonInfo.CreateFromXml(connection, personInfoNav);
        }
    }
}
