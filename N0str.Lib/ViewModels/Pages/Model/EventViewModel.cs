using Avalonia.Media.Imaging;
using Avalonia.Threading;
using N0str.Static;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Runtime.CompilerServices;


namespace N0str.ViewModels.Pages.Model
{
    public class EventViewModel : ViewModelBase
    {
        public string ID { get; }
        public string? Content { get; }
        public string? DisplayContent { get; }

        public string AuthorPubKey { get; }

        public DateTimeOffset? CreatedAt { get; }

        public int Kind { get; }
        public List<NostrEventTag> Tags { get; }

        public ObservableCollection<Bitmap> Images { get; } = [];

        public bool IsMediaLoading { get; set; }

        public bool HasMedia => Images.Count > 0;

        public EventViewModel(NostrEvent ev)
        {
            ID = ev.Id;
            Content = ev.Content;
            AuthorPubKey = ev.PublicKey;
            CreatedAt = ev.CreatedAt;
            Kind = ev.Kind;
            Tags = ev.Tags;

            DisplayContent = Content;

            if (DisplayContent is null)
                return;

            var imageURLs = MediaExtractor.ExtractImageUrls(ev);
            foreach (var imageURL in imageURLs)
            {
                DisplayContent = DisplayContent.Replace(imageURL, "");
            }
        }

        public async Task AddImageToEvent(byte[] imageBytes)
        {
            var bitmap = ConvertBytesToBitmap(imageBytes);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Images.Add(bitmap);
            });
        }

        private Bitmap ConvertBytesToBitmap(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}
