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

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
        }

        partial void ConnectButtonPressed()
        {
            this.ConnectToHealthVaultAsync();
        }

        private async Task ConnectToHealthVaultAsync()
        {
            try
            {
                ClientHealthVaultFactory.Current.SetConfiguration(new ClientHealthVaultConfiguration
                {
                    MasterApplicationId = Guid.Parse("cf0cb893-d411-495c-b66f-9d72b4fd2b97"),
                    DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com"),
                    DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform")
                });

                this.connection = await ClientHealthVaultFactory.Current.GetConnectionAsync();

                //this.statusView.Text = $"Hello {this.connection.PersonInfo.Name}";
            }
            catch (Exception e)
            {
                var x = e;
            }
        }
    }
}