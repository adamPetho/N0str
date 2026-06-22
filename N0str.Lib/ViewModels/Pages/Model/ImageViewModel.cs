using Avalonia.Media.Imaging;
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
        public Bitmap Bitmap { get; }
        public int Width => Bitmap.PixelSize.Width;
        public int Height => Bitmap.PixelSize.Height;

        public ImageViewModel(string url, Bitmap bitmap)
        {
            Url = url;
            Bitmap = bitmap;
        }
    }
}
