using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.HealthVault.Client;
using System.Threading.Tasks;

namespace SampleAndroid
{
    [Activity(Label = "SampleAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IClientHealthVaultConnection connection;

        TextView statusView;
        Button connectionButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this.statusView = FindViewById<TextView>(Resource.Id.StatusText);

            // Get our button from the layout resource,
            // and attach an event to it
            this.connectionButton = FindViewById<Button>(Resource.Id.MyButton);
            this.connectionButton.Click += delegate { this.ConnectToHealthVaultAsync(); };
        }

        private async Task ConnectToHealthVaultAsync()
        {
            try
            {
                this.connectionButton.Text = "Connecting...";

                ClientHealthVaultFactory.Current.SetConfiguration(new ClientHealthVaultConfiguration
                {
                    MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                    DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                    DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
                });

                this.connection = await ClientHealthVaultFactory.Current.GetConnectionAsync();

                this.connectionButton.Text = "Connected.";
                this.statusView.Text = $"Hello {this.connection.PersonInfo.Name}";
            }
            catch (Exception e)
            {

            }
        }
    }
}

