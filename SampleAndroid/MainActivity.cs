using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.HealthVault.Client;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using System.Collections.Generic;
using Microsoft.HealthVault.Record;
using System.Collections;

namespace SampleAndroid
{
    [Activity(Label = "SampleAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IClientHealthVaultConnection connection;
        private IThingClient thingClient;

        TextView statusView;
        Button connectionButton;
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
            this.getButton = FindViewById<Button>(Resource.Id.GetButton);
            this.createButton = FindViewById<Button>(Resource.Id.CreateButton);

            // attach the actions
            this.connectionButton.Click += delegate { this.ConnectToHealthVaultAsync(); };
            this.getButton.Click += delegate { this.GetBloodPressures(); };
            this.createButton.Click += delegate { this.CreateBloodPressure(); };
        }

        private async Task ConnectToHealthVaultAsync()
        {
            this.statusView.Text = "Connecting...";

            try
            {
                // create a configuration for our HealthVault application
                ClientHealthVaultFactory.Current.SetConfiguration(new ClientHealthVaultConfiguration
                {
                    MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                    DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                    DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
                });

                // get a connection to HealthVault
                this.connection = await ClientHealthVaultFactory.Current.GetConnectionAsync();
            }
            catch (Exception e)
            {
                this.statusView.Text = $"Error connecting... {e.ToString()}";
            }

            // get a thing client
            this.thingClient = connection.GetThingClient();

            // update visual state
            this.connectionButton.Visibility = ViewStates.Gone;
            this.controlsLayout.Visibility = ViewStates.Visible;
            this.statusView.Text = $"Hello {this.connection.PersonInfo.Name}";
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
            await thingClient.CreateNewThingsAsync(connection.PersonInfo.SelectedRecord, new List<BloodPressure>() { bp });
        }

        private async Task GetBloodPressures()
        {
            // use our thing client to get all things of type blood pressure
            IReadOnlyCollection<BloodPressure> bloodPressures = await this.thingClient.GetThingsAsync<BloodPressure>(connection.PersonInfo.SelectedRecord);
            this.listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, new List<BloodPressure>(bloodPressures));
        }
    }
}

