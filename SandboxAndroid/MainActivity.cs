using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;

namespace SandboxAndroid
{
    [Activity(Label = "SandboxAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IHealthVaultSodaConnection connection;
        private IThingClient thingClient;

        private TextView statusView;
        private Button connectionButton;
        private Button disconnectButton;
        private Button createButton;
        private Button getButton;
        private LinearLayout controlsLayout;
        private ListView listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this.statusView = FindViewById<TextView>(Resource.Id.StatusText);
            this.controlsLayout = FindViewById<LinearLayout>(Resource.Id.ControlLayout);
            this.listView = FindViewById<ListView>(Resource.Id.ListView);

            // Get our buttons from the layout resource,
            this.connectionButton = FindViewById<Button>(Resource.Id.ConnectButton);
            this.disconnectButton = FindViewById<Button>(Resource.Id.DisconnectButton);
            this.getButton = FindViewById<Button>(Resource.Id.GetButton);
            this.createButton = FindViewById<Button>(Resource.Id.CreateButton);

            // attach the actions
            this.connectionButton.Click += this.ConnectToHealthVaultAsync;
            this.disconnectButton.Click += this.DisconnectButtonOnClick;
            this.getButton.Click += this.GetBloodPressures;
            this.createButton.Click += this.CreateBloodPressure;
        }

        private async void ConnectToHealthVaultAsync(object sender, EventArgs eventArgs)
        {
            this.statusView.Text = "Connecting...";

            // create a configuration for our HealthVault application
            var configuration = GetPpeConfiguration();

            try
            {
                // get a connection to HealthVault
                this.connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
                await this.connection.AuthenticateAsync();
            }
            catch (Exception e)
            {
                this.statusView.Text = $"Error connecting... {e.ToString()}";
            }

            // get a thing client

            this.thingClient = ClientHealthVaultFactory.GetThingClient(connection);

            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();

            // update visual state
            this.connectionButton.Visibility = ViewStates.Gone;
            this.controlsLayout.Visibility = ViewStates.Visible;
            this.disconnectButton.Visibility = ViewStates.Visible;
            this.statusView.Text = $"Hello {personInfo.Name}";
        }

        private static HealthVaultConfiguration GetPpeConfiguration()
        {
            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                HealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                HealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
            };
            return configuration;
        }

        private async void DisconnectButtonOnClick(object sender, EventArgs eventArgs)
        {
            await this.connection.DeauthorizeApplicationAsync();
            this.statusView.Text = "Disconnected.";

            this.connectionButton.Visibility = ViewStates.Visible;
            this.disconnectButton.Visibility = ViewStates.Gone;
            this.controlsLayout.Visibility = ViewStates.Gone;
        }

        private async void CreateBloodPressure(object sender, EventArgs eventArgs)
        {
            // Create a new blood pressure object with random values
            Random rand = new Random();
            BloodPressure bp = new BloodPressure
            {
                Diastolic = rand.Next(20, 100),
                Systolic = rand.Next(80, 120)
            };

            // use our thing client to creat the new blood pressure
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            await thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure>() { bp });
        }

        private async void GetBloodPressures(object sender, EventArgs eventArgs)
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await this.thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);
            this.listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, new List<BloodPressure>(bloodPressures));
        }
    }
}
