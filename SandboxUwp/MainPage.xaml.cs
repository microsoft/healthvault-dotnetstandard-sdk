using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SandboxUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IHealthVaultSodaConnection connection;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            this.OutputBlock.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            this.connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
            await this.connection.AuthenticateAsync();

            this.OutputBlock.Text = "Connected.";
        }

        private async void Get_BP_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = this.connection.CreateThingClient();

            var bloodPressures = await thingClient.GetThingsAsync<BloodPressure>(recordInfo.Id);
            BloodPressure firstBloodPressure = bloodPressures.FirstOrDefault();
            if (firstBloodPressure == null)
            {
                this.OutputBlock.Text = "No blood pressures.";
            }
            else
            {
                this.OutputBlock.Text = firstBloodPressure.Systolic + "/" + firstBloodPressure.Diastolic;
            }
        }

        private async void SetBP_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = this.connection.CreateThingClient();

            await thingClient.CreateNewThingsAsync(recordInfo.Id, new List<BloodPressure> { new BloodPressure(new HealthServiceDateTime(DateTime.Now), 117, 70) });

            this.OutputBlock.Text = "Created blood pressure.";
        }

        private async void DeleteConnectionInfo_OnClick(object sender, RoutedEventArgs e)
        {
            await this.connection.DeauthorizeApplicationAsync();
            this.OutputBlock.Text = "Deleted connection information.";
        }
    }
}
