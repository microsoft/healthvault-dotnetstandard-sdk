// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Provides support for lazy initialization of cache objects
    /// with explicit expiration period.
    /// </summary>
    /// <remarks>
    /// All instance methods are thread-safe.
    /// </remarks>
    /// <typeparam name="T">The type of the cache object.</typeparam>
    internal class LazyCacheWithTtl<T>
    {
        private readonly Func<Task<T>> loadValue;
        private readonly Func<T, Task<T>> reloadValue;
        private readonly TimeSpan ttl;
        private readonly object padlock = new object();
        private DateTime timeOfExpiration;
        private T value;

        /// <summary>
        /// Creates a new instance that will load its initial value lazily on the first get call
        /// using <paramref name="loadValue"/>, and then caches that value.
        ///
        /// The cache is invalid after the specified <paramref name="timeToLive"/>.
        ///
        /// <paramref name="reloadValue"/> is invoked on the next get request after the cache has expired,
        /// passing in the previously cached value.
        /// </summary>
        ///
        /// <param name="loadValue">
        /// Function used for the initial loading of the value.
        /// </param>
        ///
        /// <param name="reloadValue">
        /// Function used for reloading the value after it has expired.  The current expired
        /// value is passed in.
        /// </param>
        ///
        /// <param name="timeToLive">
        /// Period of time before the value is considered expired.
        /// </param>
        public LazyCacheWithTtl(Func<Task<T>> loadValue, Func<T, Task<T>> reloadValue, TimeSpan timeToLive)
        {
            this.loadValue = loadValue;
            this.reloadValue = reloadValue;
            this.ttl = timeToLive;
            this.timeOfExpiration = DateTime.MaxValue;
        }

        /// <summary>
        /// Gets the lazily initialized value.
        /// </summary>
        ///
        /// <remarks>
        /// On first call to this getter, we load the value.
        ///
        /// This is thread-safe.
        /// </remarks>
        public T Value()
        {
                lock (this.padlock)
                {
                    if (!this.IsValueCreated)
                    {
                        // first load- running synchronously to avoid potential deadlock
                        this.value = this.loadValue().Result;
                        this.ValueUpdated();
                    }
                    else if (DateTime.Now > this.timeOfExpiration)
                    {
                        // expired- running synchronously to avoid potential deadlock
                        this.value = this.reloadValue(this.value).Result;
                        this.ValueUpdated();
                    }

                    return this.value;
                }
        }

        private void ValueUpdated()
        {
            this.timeOfExpiration = DateTime.Now + this.ttl;
            this.IsValueCreated = true;
        }

        public bool IsValueCreated { get; private set; }
    }
}
