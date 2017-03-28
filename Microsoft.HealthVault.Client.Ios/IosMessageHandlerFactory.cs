using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Foundation;
using Microsoft.HealthVault.Connection;
using ModernHttpClient;
using UIKit;

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