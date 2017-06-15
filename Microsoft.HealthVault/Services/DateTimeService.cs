using System;

namespace Microsoft.HealthVault.Services
{
    internal class DateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
