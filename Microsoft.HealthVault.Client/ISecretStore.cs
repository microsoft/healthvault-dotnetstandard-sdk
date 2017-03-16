using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal interface ISecretStore
    {
        Task WriteAsync(string key, string contents);

        Task<string> ReadAsync(string key);

        Task DeleteAsync(string key);
    }
}
