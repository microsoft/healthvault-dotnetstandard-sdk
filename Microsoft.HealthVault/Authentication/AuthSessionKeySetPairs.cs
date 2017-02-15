// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.HealthVault.Authentication
{
    internal class AuthSessionKeySetPairs
    {
        private readonly Dictionary<Guid, AuthenticationTokenKeySetPair> Pairs =
            new Dictionary<Guid, AuthenticationTokenKeySetPair>();

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private void AcquireWriterLock()
        {
            _lock.TryEnterWriteLock(
                EasyWebRequest.RetryOnInternal500SleepSeconds
                * EasyWebRequest.RetryOnInternal500Count
                * 1000);
        }

        private void ReleaseWriterLockIfHeld()
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
            }
        }

        private void AcquireReaderLock()
        {
            _lock.TryEnterReadLock(
                EasyWebRequest.RetryOnInternal500SleepSeconds
                * 1000);
        }

        private void ReleaseReaderLockIfHeld()
        {
            if (_lock.IsReadLockHeld)
            {
                _lock.ExitReadLock();
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
            catch (Exception)
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
            catch (Exception)
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
