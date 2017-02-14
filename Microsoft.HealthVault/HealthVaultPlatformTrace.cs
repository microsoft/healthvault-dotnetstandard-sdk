// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Microsoft.HealthVault
{
    internal class HealthVaultPlatformTrace
    {
        private static readonly object _lock = new object();
        private static ITraceSource s_traceSource;

        internal static ITraceSource TraceSource
        {
            get
            {
                if (s_traceSource == null)
                {
                    lock (_lock)
                    {
                        if (s_traceSource == null)
                        {
                            ITraceSource traceSource = new HealthVaultTraceSource();
                            TraceSource = traceSource;
                        }
                    }
                }

                return s_traceSource;
            }
            set
            {
                lock (_lock)
                {
                    Interlocked.Exchange(ref s_traceSource, value);
                }
            }
        }

        internal static void LogRequest(Byte[] utf8Bytes, Guid correlationId)
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
