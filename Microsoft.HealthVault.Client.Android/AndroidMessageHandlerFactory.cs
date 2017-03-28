using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.HealthVault.Connection;
using ModernHttpClient;

namespace Microsoft.HealthVault.Client
{
    internal class AndroidMessageHandlerFactory : IMessageHandlerFactory
    {
        public HttpClientHandler Create()
        {
            return new NativeMessageHandler();
        }
    }
}