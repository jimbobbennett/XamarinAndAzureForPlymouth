using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace NumberTaker.Core
{
    public class PhotoViewModel : BaseViewModel
    {
        readonly AzureService azureService = new AzureService();

        public PhotoViewModel()
        {
            TakePhotoCommand = new RelayCommand(async () => await TakePhoto());
            SendPhotoCommand = new RelayCommand(async () => await SendPhoto());
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
            set => SetProperty(ref mediaFile, value);
        }

        public ICommand TakePhotoCommand { get; }
        public ICommand SendPhotoCommand { get; }
    }
}
