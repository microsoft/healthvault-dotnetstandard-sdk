// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Microsoft.HealthVault.Diagnostics
{
    internal class HealthVaultPlatformTrace
    {
        private static readonly object Lock = new object();
        private static ITraceSource traceSource;

        internal static ITraceSource TraceSource
        {
            get
            {
                if (traceSource == null)
                {
                    lock (Lock)
                    {
                        if (traceSource == null)
                        {
                            ITraceSource newTraceSource = new HealthVaultTraceSource();
                            TraceSource = newTraceSource;
                        }
                    }
                }

                return traceSource;
            }

            set
            {
                lock (Lock)
                {
                    Interlocked.Exchange(ref traceSource, value);
                }
            }
        }

        internal static void LogRequest(byte[] utf8Bytes, Guid correlationId)
        {
            if (LoggingEnabled)
            {
                Log(TraceEventType.Information, Encoding.UTF8.GetString(utf8Bytes));

                if (correlationId != Guid.Empty)
                {
                    Log(TraceEventType.Information, "CorrelationId: {0}", correlationId);
                }
            }
        }

        internal static bool LoggingEnabled
        {
            get
            {
                bool result = TraceSource.ShouldTrace(TraceEventType.Information);
                return result;
            }
        }

        internal static void LogResponse(string responseString)
        {
            Log(TraceEventType.Information, responseString);
        }

        internal static void LogCertLoading(
            string logEntryFormat,
            params object[] parameters)
        {
            Log(TraceEventType.Verbose, logEntryFormat, parameters);
        }

        internal static void Log(TraceEventType level, string message)
        {
            if (TraceSource.ShouldTrace(level))
            {
                TraceSource.TraceInformation(message);
            }
        }

        internal static void Log(TraceEventType level, string format, params object[] args)
        {
            if (TraceSource.ShouldTrace(level))
            {
                TraceSource.TraceInformation(format, args);
            }
        }
    }
}
