using System;

using UIKit;
using Microsoft.HealthVault.Client;
using System.Threading.Tasks;
using Foundation;

namespace SampleIos
{
    public partial class RootViewController : UIViewController
    {
        private IClientHealthVaultConnection connection;

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

            this.statusLabel.Text = "Status: Not Connected";
        }

        partial void ConnectButtonPressed()
        {
            this.connectButton.Enabled = false;

            if (this.connection == null)
            {
                this.ConnectToHealthVaultAsync();
            }
            else
            {
                this.DisconnectFromHealthVaultAsync();
            }
        }

        private async Task ConnectToHealthVaultAsync()
        {
            this.activityIndicator.StartAnimating();
            this.statusLabel.Text = "Connecting. Please wait.";

            try
            {
                ClientHealthVaultFactory.Current.SetConfiguration(new ClientHealthVaultConfiguration
                {
                    MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                    DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                    DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
                });

                this.connection = await ClientHealthVaultFactory.Current.GetConnectionAsync();

                this.connectButton.Enabled = true;
                this.connectButton.SetTitle("Disconnect", UIControlState.Normal);
                this.activityIndicator.StopAnimating();

                this.statusLabel.Text = $"Hello {this.connection.PersonInfo.Name}";
            }
            catch (Exception e)
            {
                this.statusLabel.Text = $"Error connecting... {e.ToString()}";
            }
        }

        private async Task DisconnectFromHealthVaultAsync()
        {

        }
    }
}