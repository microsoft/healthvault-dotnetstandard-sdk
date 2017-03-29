// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SandboxIos
{
    [Register("BloodPressureCell")]
    partial class BloodPressureCell
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UILabel dateLabel { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UILabel diastolicLabel { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UILabel pulseLabel { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UILabel systolicLabel { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (dateLabel != null){
                dateLabel.Dispose();
                dateLabel = null;
            }

            if (diastolicLabel != null){
                diastolicLabel.Dispose();
                diastolicLabel = null;
            }

            if (pulseLabel != null){
                pulseLabel.Dispose();
                pulseLabel = null;
            }

            if (systolicLabel != null){
                systolicLabel.Dispose();
                systolicLabel = null;
            }
        }
    }
}
