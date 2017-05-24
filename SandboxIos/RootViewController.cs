// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        private IHealthVaultSodaConnection _connection;
        private IThingClient _thingClient;

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
            NavigationController.PushViewController(new ThingListViewController<BloodPressure>(_connection), true);
        }

        partial void ConnectButtonPressed()
        {
            connectButton.Enabled = false;

            if (_connection == null)
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

                _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
                await _connection.AuthenticateAsync();

                _thingClient = _connection.CreateThingClient();
                PersonInfo personInfo = await _connection.GetPersonInfoAsync();

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
                await _connection.DeauthorizeApplicationAsync();

                _thingClient = null;
                _connection = null;

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