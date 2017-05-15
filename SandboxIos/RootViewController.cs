using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using UIKit;

namespace SandboxIos
{
    public partial class RootViewController : UIViewController
    {
        private IHealthVaultSodaConnection connection;
        private IThingClient thingClient;

        private const string notConnectedMessage = "Tap \"Connect\" to log in";

        public RootViewController() :
            base("RootViewController", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIBarButtonItem backButton = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null, null);
            NavigationItem.BackBarButtonItem = backButton;

            statusLabel.Text = notConnectedMessage;
            UpdateTitle("Not Connected");
        }

        partial void BloodPressureButtonPressed()
        {
            NavigationController.PushViewController(new ThingListViewController<BloodPressure>(connection), true);
        }

        partial void ConnectButtonPressed()
        {
            connectButton.Enabled = false;

            if (connection == null)
            {
                ConnectToHealthVaultAsync();
            }
            else
            {
                DisconnectFromHealthVaultAsync();
            }
        }

        private async Task ConnectToHealthVaultAsync()
        {
            activityIndicator.StartAnimating();
            UpdateTitle("Connecting...");
            statusLabel.Text = "";

            try
            {
                var configuration = GetPpeConfiguration();

                connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
                await connection.AuthenticateAsync();

                thingClient = connection.CreateThingClient();
                PersonInfo personInfo = await connection.GetPersonInfoAsync();

                connectButton.SetTitle("Disconnect", UIControlState.Normal);
                SetStatusLabelText("");
                UpdateTitle(personInfo.Name);
                controlView.Hidden = false;
            }
            catch (Exception e)
            {
                UpdateTitle("Error");
                SetStatusLabelText(e.ToString());
                connectButton.SetTitle("Retry", UIControlState.Normal);
            }
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

        private async Task DisconnectFromHealthVaultAsync()
        {
            controlView.Hidden = true;
            activityIndicator.StartAnimating();
            UpdateTitle("Disconnecting...");

            try
            {
                await connection.DeauthorizeApplicationAsync();

                thingClient = null;
                connection = null;

                connectButton.SetTitle("Connect", UIControlState.Normal);
                UpdateTitle("Not Connected");
                SetStatusLabelText(notConnectedMessage);
            }
            catch (Exception e)
            {
                UpdateTitle("Error");
                SetStatusLabelText(e.ToString());
                connectButton.SetTitle("Retry", UIControlState.Normal);
            }
        }

        private void UpdateTitle(string title)
        {
            NavigationController.NavigationBar.TopItem.Title = title;
        }

        private void SetStatusLabelText(string text)
        {
            statusLabel.Text = text;
            activityIndicator.StopAnimating();
            connectButton.Enabled = true;
        }
    }
}