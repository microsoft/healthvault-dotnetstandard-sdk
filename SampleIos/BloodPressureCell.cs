using System;

using Foundation;
using Microsoft.HealthVault.Thing;
using UIKit;
using Microsoft.HealthVault.ItemTypes;
using CoreGraphics;

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

            this.diastolicLabel.Text = bloodPressure.Diastolic.ToString();
            this.pulseLabel.Text = bloodPressure.Pulse.ToString();
            this.systolicLabel.Text = bloodPressure.Systolic.ToString();
            this.dateLabel.Text = bloodPressure.When.ToDateTime().ToString("g");
        }

        public CGSize GetSize(IThing thing)
        {
            return new CGSize(188, 188);
        }
    }
}