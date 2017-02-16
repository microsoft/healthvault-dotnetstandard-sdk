// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// HealthVault's default trace source.
    /// </summary>
    internal class HealthVaultTraceSource : ITraceSource
    {
        private TraceSource _traceSource = new TraceSource("HealthVaultTraceSource");

        /// <summary>
        /// Logs the message as information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void TraceInformation(string message)
        {
            _traceSource.TraceInformation(message);
        }

        /// <summary>
        /// Logs the message as information with string transform with the parameters.
        /// </summary>
        /// <param name="format">The message to log.</param>
        /// <param name="args">String.Format insertion parameters</param>
        public void TraceInformation(string format, params object[] args)
        {
            _traceSource.TraceInformation(format, args);
        }

        /// <summary>
        /// Gets whether the given event type should be logged.
        /// </summary>
        /// <param name="eventType">The event type to check if logging is enabled.</param>
        /// <returns>True if the given event type should be logged, or false.</returns>
        public bool ShouldTrace(TraceEventType eventType)
        {
            return _traceSource.Switch.ShouldTrace(eventType);
        }
    }
}