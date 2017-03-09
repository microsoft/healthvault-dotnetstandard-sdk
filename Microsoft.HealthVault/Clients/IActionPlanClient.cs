using Microsoft.HealthVault.Record;

namespace Microsoft.HealthVault.Clients
{
    public interface IActionPlanClient : IClient
    {
        HealthRecordInfo Record { get; set; }
    }
}
