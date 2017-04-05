using Foundation;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using System;
using System.Collections.Generic;
using UIKit;

namespace SandboxIos
{
    public partial class BloodPressureEntryViewController : UIViewController, IUITextFieldDelegate
    {
        private IHealthVaultSodaConnection connection;

        public BloodPressureEntryViewController(IHealthVaultSodaConnection connection)
            : base("BloodPressureEntryViewController", null)
        {
            this.connection = connection;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Add a cancel button to the navigation bar
            this.NavigationItem.SetLeftBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Cancel,
                (sender, args) =>
                {
                    this.Dismiss();
                }),
                false);

            // Add a save button to the navigation bar
            this.NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Save,
                (sender, args) =>
                {
                    this.SaveAndCloseAsync();
                }),
                false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.systolicTextField.BecomeFirstResponder();
        }

        private async void SaveAndCloseAsync()
        {
            this.savingView.Hidden = false;

            int diastolic = 0;
            int systolic = 0;
            int pulse = 0;

            int.TryParse(this.diastolicTextField.Text, out diastolic);
            int.TryParse(this.systolicTextField.Text, out systolic);
            int.TryParse(this.pulseTextField.Text, out pulse);

            BloodPressure bp = new BloodPressure
            {
                Diastolic = diastolic,
                Systolic = systolic,
                Pulse = pulse,
                When = new HealthServiceDateTime(DateTime.Now)
            };

            IThingClient thingClient = ClientHealthVaultFactory.GetThingClient(this.connection);
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            await thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure>() { bp });

            this.Dismiss();
        }

        private void Dismiss()
        {
            this.View.EndEditing(true);
            this.DismissViewController(true, null);
        }

        [Export("textField:shouldChangeCharactersInRange:replacementString:")]
        public bool ShouldChangeCharacters(UITextField textField, NSRange range, String text)
        {
            // Allow Deleting Characters-
            if (text.Length == 0)
            {
                return true;
            }

            int n;
            // Allow only 3 characters per field, and allow only numbers
            if (range.Location + range.Length >= 3 || !int.TryParse(text, out n))
            {
                return false;
            }

            return true;
        }
    }
}