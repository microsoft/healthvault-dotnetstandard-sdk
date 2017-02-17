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
        private readonly Func<Task<T>> _loadValue;
        private readonly Func<T, Task<T>> _reloadValue;
        private readonly TimeSpan _ttl;
        private readonly object _padlock = new object();
        private DateTime _timeOfExpiration;
        private T _value;

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
            _loadValue = loadValue;
            _reloadValue = reloadValue;
            _ttl = timeToLive;
            _timeOfExpiration = DateTime.MaxValue;
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
                lock (_padlock)
                {
                    if (!IsValueCreated)
                    {
                        // first load- running synchronously to avoid potential deadlock
                        _value = _loadValue().Result;
                        ValueUpdated();
                    }
                    else if (DateTime.Now > _timeOfExpiration)
                    {
                        // expired- running synchronously to avoid potential deadlock
                        _value = _reloadValue(_value).Result;
                        ValueUpdated();
                    }

                    return _value;
                }
        }

        private void ValueUpdated()
        {
            _timeOfExpiration = DateTime.Now + _ttl;
            IsValueCreated = true;
        }

        public bool IsValueCreated { get; private set; }
    }
}
