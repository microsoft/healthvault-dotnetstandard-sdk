// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SandboxIos
{
    [Register("BloodPressureEntryViewController")]
    partial class BloodPressureEntryViewController
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UITextField diastolicTextField { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UITextField systolicTextField { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UITextField pulseTextField { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIView savingView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (diastolicTextField != null){
                diastolicTextField.Dispose();
                diastolicTextField = null;
            }

            if (systolicTextField != null){
                systolicTextField.Dispose();
                systolicTextField = null;
            }

            if (pulseTextField != null){
                pulseTextField.Dispose();
                pulseTextField = null;
            }

            if (savingView != null){
                savingView.Dispose();
                savingView = null;
            }
        }
    }
}