// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Contains the information about an application needed to construct a user's credential for
    /// that application.
    /// </summary>
    ///
    public class ApplicationTokenCreationInfo
    {
        /// <summary>
        /// Constructs an <see cref="ApplicationTokenCreationInfo"/> instance with the specified
        /// application identifier and whether the application supports multiple records.
        /// </summary>
        ///
        /// <param name="appId">
        /// The unique application identifier.
        /// </param>
        ///
        /// <param name="isMra">
        /// States whether the application is a multi-record app.
        /// </param>
        ///
        public ApplicationTokenCreationInfo(Guid appId, bool isMra)
        {
            _appId = appId;
            _isMra = isMra;
        }

        /// <summary>
        /// Gets the unique application identifier.
        /// </summary>
        ///
        public Guid ApplicationId
        {
            get { return _appId; }
        }
        private Guid _appId;

        /// <summary>
        /// Gets whether the application supports multiple records.
        /// </summary>
        ///
        public bool IsMRA
        {
            get { return _isMra; }
        }
        private bool _isMra;
    }
}
