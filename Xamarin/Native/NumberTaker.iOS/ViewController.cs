using System;
using Foundation;
using NumberTaker.Core;
using UIKit;

namespace NumberTaker.iOS
{
    public partial class ViewController : UIViewController
    {
        PhotoViewModel viewModel;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start ();
#endif

            viewModel = new PhotoViewModel();
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PhotoViewModel.Photo))
                {
                    var stream = viewModel.Photo.GetStreamWithImageRotatedForExternalStorage();
                    var imageData = NSData.FromStream(stream);
                    ImageView.Image = UIImage.LoadFromData(imageData);
                }
            };
        }

        partial void TakePhotoButton_TouchUpInside(UIButton sender)
        {
            viewModel.TakePhotoCommand.Execute(null);
        }

        partial void SendPhotoButton_TouchUpInside(UIButton sender)
        {
            if (viewModel.Photo == null)
            {
                var okAlertController = UIAlertController.Create("Error", "No photo", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);
            }
            else
            {
                viewModel.SendPhotoCommand.Execute(null);
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
