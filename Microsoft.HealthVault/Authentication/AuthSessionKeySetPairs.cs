// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.HealthVault.Web.Authentication
{
    internal class AuthSessionKeySetPairs
    {
        internal Dictionary<Guid, AuthenticationTokenKeySetPair> Pairs =
            new Dictionary<Guid, AuthenticationTokenKeySetPair>();

        private ReaderWriterLock _lock = new ReaderWriterLock();

        private void AcquireWriterLock()
        {
            _lock.AcquireWriterLock(
                EasyWebRequest.RetryOnInternal500SleepSeconds
                * EasyWebRequest.RetryOnInternal500Count
                * 1000);
        }

        private void ReleaseWriterLockIfHeld()
        {
            if (_lock.IsWriterLockHeld)
            {
                _lock.ReleaseWriterLock();
            }
        }

        private void AcquireReaderLock()
        {
            _lock.AcquireReaderLock(
                EasyWebRequest.RetryOnInternal500SleepSeconds
                * 1000);
        }

        private void ReleaseReaderLockIfHeld()
        {
            if (_lock.IsReaderLockHeld)
            {
                _lock.ReleaseReaderLock();
            }
        }

        internal AuthenticationTokenKeySetPair GetPair(Guid applicationId)
        {
            try
            {
                AcquireReaderLock();

                if (!Pairs.ContainsKey(applicationId))
                {
                    return null;
                }

                return Pairs[applicationId];
            }
            catch (ApplicationException)
            {
                throw Validator.HealthServiceException(
                        "WebApplicationCredentialInternalTimeout");
            }
            finally
            {
                ReleaseReaderLockIfHeld();
            }
        }

        /// <summary>
        /// Creates an unauthenticated keyset pair for the specified
        /// <paramref name="applicationId"/> with a new keyset.
        /// </summary>
        /// 
        /// <param name="applicationId">
        /// The application id to create the keyset pair for.
        /// </param>
        /// 
        internal AuthenticationTokenKeySetPair CreatePair(Guid applicationId)
        {
            try
            {
                AcquireWriterLock();

                AuthenticationTokenKeySetPair pair;

                if (Pairs.ContainsKey(applicationId))
                {
                    pair = Pairs[applicationId];
                }
                else
                {
                    pair = new AuthenticationTokenKeySetPair();

                    Pairs[applicationId] = pair;
                }

                return pair;
            }
            catch (ApplicationException)
            {
                throw Validator.HealthServiceException("WebApplicationCredentialInternalTimeout");
            }
            finally
            {
                ReleaseWriterLockIfHeld();
            }
        }
    }
}

