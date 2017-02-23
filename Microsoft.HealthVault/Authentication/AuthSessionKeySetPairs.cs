// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Authentication
{
    internal class AuthSessionKeySetPairs
    {
        private readonly Dictionary<Guid, AuthenticationTokenKeySetPair> pairs =
            new Dictionary<Guid, AuthenticationTokenKeySetPair>();

        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        private void AcquireWriterLock()
        {
            this.@lock.TryEnterWriteLock(
                HealthApplicationConfiguration.Current.RetryOnInternal500SleepSeconds
                * HealthApplicationConfiguration.Current.RetryOnInternal500Count
                * 1000);
        }

        private void ReleaseWriterLockIfHeld()
        {
            if (this.@lock.IsWriteLockHeld)
            {
                this.@lock.ExitWriteLock();
            }
        }

        private void AcquireReaderLock()
        {
            this.@lock.TryEnterReadLock(
                HealthApplicationConfiguration.Current.RetryOnInternal500SleepSeconds
                * 1000);
        }

        private void ReleaseReaderLockIfHeld()
        {
            if (this.@lock.IsReadLockHeld)
            {
                this.@lock.ExitReadLock();
            }
        }

        internal AuthenticationTokenKeySetPair GetPair(Guid applicationId)
        {
            try
            {
                this.AcquireReaderLock();

                if (!this.pairs.ContainsKey(applicationId))
                {
                    return null;
                }

                return this.pairs[applicationId];
            }
            catch (Exception)
            {
                throw Validator.HealthServiceException(
                        "WebApplicationCredentialInternalTimeout");
            }
            finally
            {
                this.ReleaseReaderLockIfHeld();
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
                this.AcquireWriterLock();

                AuthenticationTokenKeySetPair pair;

                if (this.pairs.ContainsKey(applicationId))
                {
                    pair = this.pairs[applicationId];
                }
                else
                {
                    pair = new AuthenticationTokenKeySetPair();

                    this.pairs[applicationId] = pair;
                }

                return pair;
            }
            catch (Exception)
            {
                throw Validator.HealthServiceException("WebApplicationCredentialInternalTimeout");
            }
            finally
            {
                this.ReleaseWriterLockIfHeld();
            }
        }
    }
}
