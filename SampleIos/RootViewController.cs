using System;

using UIKit;
using Microsoft.HealthVault.Client;
using System.Threading.Tasks;
using Foundation;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Clients;

namespace SampleIos
{
    public partial class RootViewController : UIViewController
    {
        private IClientHealthVaultConnection connection;
        private IThingClient thingClient;

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
            this.statusLabel.Text = "Connecting...";

            try
            {
                this.connection = ClientHealthVaultFactory.Current.GetConnection();
                await this.connection.AuthenticateAsync();

                this.thingClient = connection.GetThingClient();
                PersonInfo personInfo = await this.connection.GetPersonInfoAsync();

                this.connectButton.SetTitle("Disconnect", UIControlState.Normal);
                this.SetStatusLabelText($"Hello {personInfo.Name}");
            }
            catch (Exception e)
            {
                this.SetStatusLabelText($"Error connecting... {e.ToString()}");
                this.connectButton.SetTitle("Retry", UIControlState.Normal);
            }
        }

        private async Task DisconnectFromHealthVaultAsync()
        {
            this.activityIndicator.StartAnimating();
            this.statusLabel.Text = "Disconnecting...";

            try
            {
                await this.connection.DeauthorizeApplicationAsync();

                this.thingClient = null;
                this.connection = null;

                this.connectButton.SetTitle("Connect", UIControlState.Normal);
                this.SetStatusLabelText("Status: Not Connected");
            }
            catch (Exception e)
            {
                this.SetStatusLabelText($"Error connecting... {e.ToString()}");
                this.connectButton.SetTitle("Retry", UIControlState.Normal);
            }
        }

        private void SetStatusLabelText(string text)
        {
            this.statusLabel.Text = text;
            this.activityIndicator.StopAnimating();
            this.connectButton.Enabled = true;
        }
    }
}