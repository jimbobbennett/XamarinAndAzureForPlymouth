using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace NumberTaker.Forms
{
    public class PhotoViewModel : BaseViewModel
    {
        readonly AzureService azureService = new AzureService();

        public PhotoViewModel()
        {
            TakePhotoCommand = new Command(async () => await TakePhoto());
            SendPhotoCommand = new Command(async () => await SendPhoto());
        }

        async Task SendPhoto()
        {
            if (Photo != null)
                await azureService.UploadPhoto(Photo);
        }

        async Task TakePhoto()
        {
            var options = new StoreCameraMediaOptions { PhotoSize = PhotoSize.Medium };
            Photo = await CrossMedia.Current.TakePhotoAsync(options);
        }

        MediaFile mediaFile;
        public MediaFile Photo
        {
            get => mediaFile;
            set
            {
                if (SetProperty(ref mediaFile, value))
                {
                    ImageSource = ImageSource.FromStream(() => mediaFile.GetStreamWithImageRotatedForExternalStorage());
                }
            }
        }

        ImageSource imageSource;
        public ImageSource ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }

        public ICommand TakePhotoCommand { get; }
        public ICommand SendPhotoCommand { get; }
    }
}
