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