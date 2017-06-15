using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;
using Xamarin.Forms;

namespace SandboxXamarinForms
{
    public partial class MainPage : ContentPage
    {
        private IHealthVaultSodaConnection _connection;
        private IThingClient _thingClient;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Connect_OnClicked(object sender, EventArgs e)
        {
            OutputLabel.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);

            await _connection.AuthenticateAsync();

            _thingClient = _connection.CreateThingClient();

            OutputLabel.Text = "Connected.";
        }

        private async void SetBP_OnClicked(object sender, EventArgs e)
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
            await _thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure> { bp });

            OutputLabel.Text = "Added blood pressure";
        }

        private async void GetBP_OnClicked(object sender, EventArgs e)
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await _thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);
            BloodPressure firstBloodPressure = bloodPressures.FirstOrDefault();
            if (firstBloodPressure == null)
            {
                OutputLabel.Text = "No blood pressures.";
            }
            else
            {
                OutputLabel.Text = firstBloodPressure.Systolic + "/" + firstBloodPressure.Diastolic;
            }
        }

        private async void MultiQuery_OnClicked(object sender, EventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            var resultSet = await _thingClient.GetThingsAsync(
                personInfo.SelectedRecord.Id, 
                new List<ThingQuery>
                {
                    new ThingQuery(BloodPressure.TypeId),
                    new ThingQuery(Weight.TypeId)
                });

            var resultList = resultSet.ToList();

            OutputLabel.Text = $"There are {resultList[0].Count} blood pressure(s) and {resultList[1].Count} weight(s).";
        }
    }
}
