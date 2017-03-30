// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SandboxIos
{
    [Register("ThingListViewController")]
    partial class ThingListViewController<TThing>
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UICollectionView collectionView { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView activityIndicator { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UILabel messageLabel { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (collectionView != null){
                collectionView.Dispose();
                collectionView = null;
            }

            if (activityIndicator != null){
                activityIndicator.Dispose();
                activityIndicator = null;
            }

            if (messageLabel != null){
                messageLabel.Dispose();
                messageLabel = null;
            }
        }
    }
}