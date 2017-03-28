using System;

namespace Microsoft.HealthVault.Services
{
    internal interface IDateTimeService
    {
        DateTimeOffset UtcNow { get; }
    }
}