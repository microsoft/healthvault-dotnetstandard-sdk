using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Record;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IClientHealthVaultConnection connection;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            this.OutputBlock.Text = "Connecting...";

            var configuration = new ClientHealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };

            ClientHealthVaultFactory.Current.SetConfiguration(configuration);
            this.connection = await ClientHealthVaultFactory.Current.GetConnectionAsync();

            this.OutputBlock.Text = "Connected.";
        }

        private async void Get_BP_OnClick(object sender, RoutedEventArgs e)
        {
            HealthRecordInfo recordInfo = this.connection.PersonInfo.GetSelfRecord();
            IThingClient thingClient = this.connection.GetThingClient();

            var bloodPressures = await thingClient.GetThingsAsync<BloodPressure>(recordInfo);
            BloodPressure firstBloodPressure = bloodPressures.First();

            this.OutputBlock.Text = firstBloodPressure.Systolic + "/" + firstBloodPressure.Diastolic;
        }

        private async void SetBP_OnClick(object sender, RoutedEventArgs e)
        {
            IThingClient thingClient = this.connection.GetThingClient();

            await thingClient.CreateNewThingsAsync(new List<BloodPressure> { new BloodPressure(new HealthServiceDateTime(DateTime.Now), 117, 70) });

            this.OutputBlock.Text = "Created blood pressure.";
        }
    }
}
