// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using NodaTime;
using UIKit;

namespace SandboxIos
{
    public partial class BloodPressureEntryViewController : UIViewController, IUITextFieldDelegate
    {
        private IHealthVaultSodaConnection _connection;

        public BloodPressureEntryViewController(IHealthVaultSodaConnection connection)
            : base("BloodPressureEntryViewController", null)
        {
            _connection = connection;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Add a cancel button to the navigation bar
            NavigationItem.SetLeftBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Cancel,
                (sender, args) =>
                {
                    Dismiss();
                }),
                false);

            // Add a save button to the navigation bar
            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Save,
                (sender, args) =>
                {
                    SaveAndCloseAsync();
                }),
                false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            systolicTextField.BecomeFirstResponder();
        }

        private async void SaveAndCloseAsync()
        {
            savingView.Hidden = false;

            int diastolic = 0;
            int systolic = 0;
            int pulse = 0;

            int.TryParse(diastolicTextField.Text, out diastolic);
            int.TryParse(systolicTextField.Text, out systolic);
            int.TryParse(pulseTextField.Text, out pulse);

            BloodPressure bp = new BloodPressure
            {
                Diastolic = diastolic,
                Systolic = systolic,
                Pulse = pulse,
                When = new HealthServiceDateTime(SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime)
            };

            IThingClient thingClient = _connection.CreateThingClient();
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            await thingClient.CreateNewThingsAsync(personInfo.SelectedRecord.Id, new List<BloodPressure> { bp });

            Dismiss();
        }

        private void Dismiss()
        {
            View.EndEditing(true);
            DismissViewController(true, null);
        }

        [Export("textField:shouldChangeCharactersInRange:replacementString:")]
        public bool ShouldChangeCharacters(UITextField textField, NSRange range, string text)
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