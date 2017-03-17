using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Transport
{
    internal interface IHealthWebRequestFactory
    {
        IHealthWebRequest CreateWebRequest(byte[] utf8EncodedXml, int length);
    }
}
