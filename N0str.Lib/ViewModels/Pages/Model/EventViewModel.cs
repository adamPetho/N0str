using Avalonia.Media.Imaging;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;


namespace N0str.ViewModels.Pages.Model
{
    public class EventViewModel : ViewModelBase
    {
        public string ID { get; }
        public string? Content { get; }

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
        }
    }
}
