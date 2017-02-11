// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics;

namespace Microsoft.Health
{
    /// <summary>
    /// Interface for trace source used by <see cref="Microsoft.Health.HealthVaultPlatformTrace"/>.
    /// </summary>
    internal interface ITraceSource
    {
        /// <summary>
        /// Logs the message as information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void TraceInformation(string message);

        /// <summary>
        /// Logs the message as information with string transform with the parameters.
        /// </summary>
        /// <param name="format">The message to log.</param>
        /// <param name="args">String.Format insertion parameters</param>
        void TraceInformation(string format, params object[] args);

        /// <summary>
        /// Gets whether the given event type should be logged.
        /// </summary>
        /// <param name="eventType">The event type to check if logging is enabled.</param>
        /// <returns>True if the given event type should be logged, or false.</returns>
        bool ShouldTrace(TraceEventType eventType);
    }
}
