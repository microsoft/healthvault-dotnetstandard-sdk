// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Configuration
{
    /// <summary>
    /// Default vaules for HealthVault configuration
    /// </summary>
    public class HealthVaultConfigurationDefaults
    {
        /// <summary>
        /// The default number of internal retries.
        /// </summary>
        public const int RetryOnInternal500Count = 2;

        /// <summary>
        /// Default URL for Shell application
        /// </summary>
        public static readonly Uri ShellUrl = new Uri("https://account.healthvault.com");

        /// <summary>
        /// Default URL for HealthVault application
        /// </summary>
        public static readonly Uri HealthVaultRootUrl = new Uri("https://platform.healthvault.com/platform/");

        /// <summary>
        /// Default sleep duration in seconds.
        /// </summary>
        public static readonly TimeSpan RetryOnInternal500SleepDuration = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Base class for the app, web and soda configurations
        /// </summary>
        /// <summary>
        /// The default request time to live value.
        /// </summary>
        public static readonly TimeSpan RequestTimeToLiveDuration = TimeSpan.FromSeconds(30 * 60);

        /// <summary>
        /// The default request time out value.
        /// </summary>
        public static readonly TimeSpan RequestTimeoutDuration = TimeSpan.FromSeconds(30);
    }
}
