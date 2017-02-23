// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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
