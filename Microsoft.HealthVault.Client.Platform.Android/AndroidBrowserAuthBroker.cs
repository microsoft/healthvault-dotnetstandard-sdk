using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client.Platform.Android
{
    public class AndroidBrowserAuthBroker : IBrowserAuthBroker
    {
        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl)
        {
            throw new NotImplementedException();
        }
    }
}