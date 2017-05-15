using System;
using System.Net.Http;

namespace Microsoft.HealthVault.Connection
{
    internal class MessageHandlerFactory : IMessageHandlerFactory
    {
        public HttpClientHandler Create()
        {
            return new HttpClientHandler();
        }
    }
}
