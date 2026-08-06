using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.ViewModels.Pages.Model
{
    public class ImageViewModel : ViewModelBase
    {
        public string Url { get; }
        private Bitmap? _bitmap;
        public Bitmap? Bitmap
        {
            get => _bitmap;
            set => SetProperty(ref _bitmap, value);
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ImageViewModel(string url, Bitmap? bitmap)
        {
            Url = url;
            Bitmap = bitmap;
        }

        public async Task AddImageToEvent(byte[] imageBytes)
        {
            var bitmap = ConvertBytesToBitmap(imageBytes);

            await Task.Delay(TimeSpan.FromSeconds(10));

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                IsLoading = false;
                Bitmap = bitmap;
            });
        }

        private Bitmap ConvertBytesToBitmap(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}
