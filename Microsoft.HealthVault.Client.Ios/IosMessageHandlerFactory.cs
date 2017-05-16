using System;
using System.Net.Http;
using Microsoft.HealthVault.Connection;
using ModernHttpClient;

namespace Microsoft.HealthVault.Client
{
    internal class IosMessageHandlerFactory : IMessageHandlerFactory
    {
        public HttpClientHandler Create()
        {
            return new NativeMessageHandler();
        }
    }
}