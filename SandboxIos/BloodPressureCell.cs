// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using CoreGraphics;
using Foundation;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using UIKit;

namespace SandboxIos
{
    public partial class BloodPressureCell : UICollectionViewCell, IThingCell
    {
        public static readonly NSString Key = new NSString("BloodPressureCell");
        public static readonly UINib Nib;

        static BloodPressureCell()
        {
            Nib = UINib.FromName("BloodPressureCell", NSBundle.MainBundle);
        }

        protected BloodPressureCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void SetThing(IThing thing)
        {
            BloodPressure bloodPressure = (BloodPressure)thing;

            diastolicLabel.Text = bloodPressure.Diastolic.ToString();
            pulseLabel.Text = bloodPressure.Pulse.ToString();
            systolicLabel.Text = bloodPressure.Systolic.ToString();
            dateLabel.Text = bloodPressure.When.ToDateTime().ToString("g");
        }

        public CGSize GetSize(IThing thing)
        {
            return new CGSize(172, 172);
        }
    }
}