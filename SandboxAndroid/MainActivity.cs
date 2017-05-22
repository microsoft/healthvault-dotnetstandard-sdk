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
        private IHealthVaultSodaConnection _connection;
        private IThingClient _thingClient;

        private TextView _statusView;
        private Button _connectionButton;
        private Button _disconnectButton;
        private Button _createButton;
        private Button _getButton;
        private LinearLayout _controlsLayout;
        private ListView _listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _statusView = FindViewById<TextView>(Resource.Id.StatusText);
            _controlsLayout = FindViewById<LinearLayout>(Resource.Id.ControlLayout);
            _listView = FindViewById<ListView>(Resource.Id.ListView);

            // Get our buttons from the layout resource,
            _connectionButton = FindViewById<Button>(Resource.Id.ConnectButton);
            _disconnectButton = FindViewById<Button>(Resource.Id.DisconnectButton);
            _getButton = FindViewById<Button>(Resource.Id.GetButton);
            _createButton = FindViewById<Button>(Resource.Id.CreateButton);

            // attach the actions
            _connectionButton.Click += ConnectToHealthVaultAsync;
            _disconnectButton.Click += DisconnectButtonOnClick;
            _getButton.Click += GetBloodPressures;
            _createButton.Click += CreateBloodPressure;
        }

        private async void ConnectToHealthVaultAsync(object sender, EventArgs eventArgs)
        {
            _statusView.Text = "Connecting...";

            // create a configuration for our HealthVault application
            var configuration = GetPpeConfiguration();

            try
            {
                // get a connection to HealthVault
                _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
                await _connection.AuthenticateAsync();
            }
            catch (Exception e)
            {
                _statusView.Text = $"Error connecting... {e.ToString()}";
            }

            // get a thing client

            _thingClient = _connection.CreateThingClient();

            PersonInfo personInfo = await _connection.GetPersonInfoAsync();

            // update visual state
            _connectionButton.Visibility = ViewStates.Gone;
            _controlsLayout.Visibility = ViewStates.Visible;
            _disconnectButton.Visibility = ViewStates.Visible;
            _statusView.Text = $"Hello {personInfo.Name}";
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
            await _connection.DeauthorizeApplicationAsync();
            _statusView.Text = "Disconnected.";

            _connectionButton.Visibility = ViewStates.Visible;
            _disconnectButton.Visibility = ViewStates.Gone;
            _controlsLayout.Visibility = ViewStates.Gone;
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
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            await _thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure>() { bp });
        }

        private async void GetBloodPressures(object sender, EventArgs eventArgs)
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await _thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);
            _listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, new List<BloodPressure>(bloodPressures));
        }
    }
}
