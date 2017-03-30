using System;

using UIKit;
using Microsoft.HealthVault.Client;
using System.Threading.Tasks;
using Foundation;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;

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
            this.NavigationItem.BackBarButtonItem = backButton;

            this.statusLabel.Text = notConnectedMessage;
            this.UpdateTitle("Not Connected");
        }

        partial void BloodPressureButtonPressed()
        {
            this.NavigationController.PushViewController(new ThingListViewController<BloodPressure>(this.connection), true);
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
            this.UpdateTitle("Connecting...");
            this.statusLabel.Text = "";

            try
            {
                this.connection = HealthVaultConnectionFactory.Current.GetSodaConnection();
                await this.connection.AuthenticateAsync();

                this.thingClient = ClientHealthVaultFactory.GetThingClient(this.connection);
                PersonInfo personInfo = await this.connection.GetPersonInfoAsync();

                this.connectButton.SetTitle("Disconnect", UIControlState.Normal);
                this.SetStatusLabelText("");
                this.UpdateTitle(personInfo.Name);
                this.controlView.Hidden = false;
            }
            catch (Exception e)
            {
                this.UpdateTitle("Error");
                this.SetStatusLabelText(e.ToString());
                this.connectButton.SetTitle("Retry", UIControlState.Normal);
            }
        }

        private async Task DisconnectFromHealthVaultAsync()
        {
            this.controlView.Hidden = true;
            this.activityIndicator.StartAnimating();
            this.UpdateTitle("Disconnecting...");

            try
            {
                await this.connection.DeauthorizeApplicationAsync();

                this.thingClient = null;
                this.connection = null;

                this.connectButton.SetTitle("Connect", UIControlState.Normal);
                this.UpdateTitle("Not Connected");
                this.SetStatusLabelText(notConnectedMessage);
            }
            catch (Exception e)
            {
                this.UpdateTitle("Error");
                this.SetStatusLabelText(e.ToString());
                this.connectButton.SetTitle("Retry", UIControlState.Normal);
            }
        }

        private void UpdateTitle(string title)
        {
            this.NavigationController.NavigationBar.TopItem.Title = title;
        }

        private void SetStatusLabelText(string text)
        {
            this.statusLabel.Text = text;
            this.activityIndicator.StopAnimating();
            this.connectButton.Enabled = true;
        }
    }
}