using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal interface IFileStore
    {
        Task<bool> WriteFileAsync(string fileName, byte[] contents);

        Task<byte[]> ReadFileAsync(string fileName);

        Task<bool> DeleteFileAsync(string fileName);
    }
}
