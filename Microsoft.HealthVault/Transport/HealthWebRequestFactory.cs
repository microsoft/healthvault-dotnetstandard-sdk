using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Transport
{
    internal class HealthWebRequestFactory : IHealthWebRequestFactory
    {
        public IHealthWebRequest CreateWebRequest(byte[] utf8EncodedXml, int length)
        {
            return new EasyWebRequest(utf8EncodedXml, length);
        }
    }
}
