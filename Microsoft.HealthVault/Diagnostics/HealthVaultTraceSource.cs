// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Diagnostics;

namespace Microsoft.HealthVault.Diagnostics
{
    /// <summary>
    /// HealthVault's default trace source.
    /// </summary>
    internal class HealthVaultTraceSource : ITraceSource
    {
        private readonly TraceSource traceSource = new TraceSource("HealthVaultTraceSource");

        /// <summary>
        /// Logs the message as information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void TraceInformation(string message)
        {
            this.traceSource.TraceInformation(message);
        }

        /// <summary>
        /// Logs the message as information with string transform with the parameters.
        /// </summary>
        /// <param name="format">The message to log.</param>
        /// <param name="args">String.Format insertion parameters</param>
        public void TraceInformation(string format, params object[] args)
        {
            this.traceSource.TraceInformation(format, args);
        }

        /// <summary>
        /// Gets whether the given event type should be logged.
        /// </summary>
        /// <param name="eventType">The event type to check if logging is enabled.</param>
        /// <returns>True if the given event type should be logged, or false.</returns>
        public bool ShouldTrace(TraceEventType eventType)
        {
            return this.traceSource.Switch.ShouldTrace(eventType);
        }
    }
}