using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.HealthVault.Client;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using System.Collections.Generic;
using Microsoft.HealthVault.Person;

namespace SandboxAndroid
{
    [Activity(Label = "SandboxAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IClientHealthVaultConnection connection;
        private IThingClient thingClient;

        TextView statusView;
        Button connectionButton;
        Button disconnectButton;
        Button createButton;
        Button getButton;
        LinearLayout controlsLayout;
        ListView listView;

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
            this.connectionButton.Click += delegate { this.ConnectToHealthVaultAsync(); };
            this.disconnectButton.Click += this.DisconnectButtonOnClick;
            this.getButton.Click += delegate { this.GetBloodPressures(); };
            this.createButton.Click += delegate { this.CreateBloodPressure(); };

            // create a configuration for our HealthVault application
            ConnectionHealthVaultFactory.Current.SetConfiguration(new ClientHealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
            });
        }

        private async Task ConnectToHealthVaultAsync()
        {
            this.statusView.Text = "Connecting...";

            try
            {
                // get a connection to HealthVault
                this.connection = ConnectionHealthVaultFactory.Current.GetConnection();
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

        private async void DisconnectButtonOnClick(object sender, EventArgs eventArgs)
        {
            await this.connection.DeauthorizeApplicationAsync();
            this.statusView.Text = "Disconnected.";

            this.connectionButton.Visibility = ViewStates.Visible;
            this.disconnectButton.Visibility = ViewStates.Gone;
            this.controlsLayout.Visibility = ViewStates.Gone;
        }

        private async Task CreateBloodPressure()
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
            await thingClient.CreateNewThingsAsync(personInfo.SelectedRecord, new List<BloodPressure>() { bp });
        }

        private async Task GetBloodPressures()
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await this.thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord);
            this.listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, new List<BloodPressure>(bloodPressures));
        }
    }
}

