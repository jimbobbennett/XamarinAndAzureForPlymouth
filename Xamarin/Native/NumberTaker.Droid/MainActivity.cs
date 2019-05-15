using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using NumberTaker.Core;
using Android.Runtime;
using Android.Graphics;

namespace NumberTaker.Droid
{
    [Activity(Label = "Number Taker", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        PhotoViewModel viewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            viewModel = new PhotoViewModel();

            var takePhotoButton = FindViewById<Button>(Resource.Id.takePhotoButton);
            takePhotoButton.Click += (s, e) => viewModel.TakePhotoCommand.Execute(null);

            var sendPhotoButton = FindViewById<Button>(Resource.Id.sendPhotoButton);
            sendPhotoButton.Click += (s, e) =>
            {
                if (viewModel.Photo == null)
                {
                    var dialog = new AlertDialog.Builder(this);
                    var alert = dialog.Create();
                    alert.SetTitle("Error");
                    alert.SetMessage("No photo");
                    alert.SetButton("OK", (c, ev) =>
                    { 
                    });
                    alert.Show();
                }
                else
                {
                    viewModel.SendPhotoCommand.Execute(null);
                }
            };

            var imageView = FindViewById<ImageView>(Resource.Id.imageView);

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PhotoViewModel.Photo))
                {
                    var stream = viewModel.Photo.GetStreamWithImageRotatedForExternalStorage();
                    var bp = BitmapFactory.DecodeStream(stream);
                    imageView.SetImageBitmap(bp);
                }
            };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

