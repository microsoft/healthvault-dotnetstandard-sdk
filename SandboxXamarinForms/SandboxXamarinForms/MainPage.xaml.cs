using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace SandboxXamarinForms
{
    public partial class MainPage : ContentPage
    {
        private IHealthVaultSodaConnection connection;
        private IThingClient thingClient;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Connect_OnClicked(object sender, EventArgs e)
        {
            this.OutputLabel.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            this.connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);

            await this.connection.AuthenticateAsync();

            this.thingClient = this.connection.CreateThingClient();

            this.OutputLabel.Text = "Connected.";
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
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            await this.thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure> { bp });

            this.OutputLabel.Text = "Added blood pressure";
        }

        private async void GetBP_OnClicked(object sender, EventArgs e)
        {
            // use our thing client to get all things of type blood pressure
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            IReadOnlyCollection<BloodPressure> bloodPressures = await this.thingClient.GetThingsAsync<BloodPressure>(personInfo.SelectedRecord.Id);
            BloodPressure firstBloodPressure = bloodPressures.FirstOrDefault();
            if (firstBloodPressure == null)
            {
                this.OutputLabel.Text = "No blood pressures.";
            }
            else
            {
                this.OutputLabel.Text = firstBloodPressure.Systolic + "/" + firstBloodPressure.Diastolic;
            }
        }
    }
}
