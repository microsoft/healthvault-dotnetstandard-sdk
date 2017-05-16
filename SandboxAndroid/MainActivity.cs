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

            statusView = FindViewById<TextView>(Resource.Id.StatusText);
            controlsLayout = FindViewById<LinearLayout>(Resource.Id.ControlLayout);
            listView = FindViewById<ListView>(Resource.Id.ListView);

            // Get our buttons from the layout resource,
            connectionButton = FindViewById<Button>(Resource.Id.ConnectButton);
            disconnectButton = FindViewById<Button>(Resource.Id.DisconnectButton);
            getButton = FindViewById<Button>(Resource.Id.GetButton);
            createButton = FindViewById<Button>(Resource.Id.CreateButton);

            // attach the actions
            connectionButton.Click += ConnectToHealthVaultAsync;
            disconnectButton.Click += DisconnectButtonOnClick;
            getButton.Click += GetBloodPressures;
            createButton.Click += CreateBloodPressure;
        }

        private async void ConnectToHealthVaultAsync(object sender, EventArgs eventArgs)
        {
            statusView.Text = "Connecting...";

            // create a configuration for our HealthVault application
            var configuration = GetPpeConfiguration();

            try
            {
                // get a connection to HealthVault
                connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
                await connection.AuthenticateAsync();
            }
            catch (Exception e)
            {
                statusView.Text = $"Error connecting... {e.ToString()}";
            }

            // get a thing client

            thingClient = connection.CreateThingClient();

            PersonInfo personInfo = await connection.GetPersonInfoAsync();

            // update visual state
            connectionButton.Visibility = ViewStates.Gone;
            controlsLayout.Visibility = ViewStates.Visible;
            disconnectButton.Visibility = ViewStates.Visible;
            statusView.Text = $"Hello {personInfo.Name}";
        }

        private static HealthVaultConfiguration GetPpeConfiguration()
        {
            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
            };
            return configuration;
        }

        private async void DisconnectButtonOnClick(object sender, EventArgs eventArgs)
        {
            await connection.DeauthorizeApplicationAsync();
            statusView.Text = "Disconnected.";

            connectionButton.Visibility = ViewStates.Visible;
            disconnectButton.Visibility = ViewStates.Gone;
            controlsLayout.Visibility = ViewStates.Gone;
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
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            await thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure>() { bp });
        }

        private async void GetBloodPressures(object sender, EventArgs eventArgs)
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);
            listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, new List<BloodPressure>(bloodPressures));
        }
    }
}
