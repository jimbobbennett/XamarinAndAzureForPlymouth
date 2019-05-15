// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace NumberTaker.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton Button { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SendPhotoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TakePhotoButton { get; set; }

        [Action ("SendPhotoButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendPhotoButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("TakePhotoButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TakePhotoButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }

            if (SendPhotoButton != null) {
                SendPhotoButton.Dispose ();
                SendPhotoButton = null;
            }

            if (TakePhotoButton != null) {
                TakePhotoButton.Dispose ();
                TakePhotoButton = null;
            }
        }
    }
}