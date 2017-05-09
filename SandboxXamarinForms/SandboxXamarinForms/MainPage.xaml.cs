using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Configuration;
using Xamarin.Forms;

namespace SandboxXamarinForms
{
    public partial class MainPage : ContentPage
    {
        private IHealthVaultSodaConnection connection;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            this.OutputLabel.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            this.connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);

            await this.connection.AuthenticateAsync();

            this.OutputLabel.Text = "Connected.";
        }
    }
}
