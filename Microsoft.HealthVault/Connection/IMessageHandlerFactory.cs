using System;
using System.Net.Http;

namespace Microsoft.HealthVault.Connection
{
    internal interface IMessageHandlerFactory
    {
        HttpClientHandler Create();
    }
}
