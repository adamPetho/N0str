using Avalonia.Media.Imaging;
using Avalonia.Threading;
using N0str.Models;
using N0str.Static;
using NNostr.Client;
using System.Collections.ObjectModel;


namespace N0str.ViewModels.Pages.Model
{
    public class EventViewModel : ViewModelBase
    {
        public NostrEvent NostrEvent { get; }
        public List<EventReference> References { get; }

        // Third party root event replied to
        public NostrEvent? RootEvent { get; }
        // The Event the Pubkey replied to
        public NostrEvent? ReplyEvent { get; }
        public string? DisplayContent { get; }

        public ObservableCollection<ImageViewModel> Images { get; } = [];

        public bool IsMediaLoading { get; set; }

        public bool HasMedia => Images.Count > 0;
        public EventType EventType { get; }

        public EventViewModel(NostrEventWithReferences ev)
        {
            NostrEvent = ev.NostrEvent;
            References = ev.References;
            RootEvent = ExtractRootEventIfExists(ev);
            ReplyEvent = ExtractReplyEventIfExists(ev);
            EventType = SetEventType();
            DisplayContent = NostrEvent.Content;

            if (DisplayContent is null)
                return;

            var imageURLs = MediaExtractor.ExtractImageUrls(ev.NostrEvent);
            foreach (var imageURL in imageURLs)
            {
                DisplayContent = DisplayContent.Replace(imageURL, "");
            }
        }

        private EventType SetEventType()
        {
            return RootEvent is not null ? EventType.Reply : EventType.Root;
        }

        private NostrEvent? ExtractRootEventIfExists(NostrEventWithReferences ev)
        {
            if (ev.References.Count == 0) 
                return null;
            
            var rootEventID = ev.References.Where(ev => ev.Type == EventType.Root).Select(ev => ev.EventId).FirstOrDefault();
            return ev.ReferencedEvents.Where(ev => ev.Id == rootEventID).FirstOrDefault();
        }

        private NostrEvent? ExtractReplyEventIfExists(NostrEventWithReferences ev)
        {
            if (ev.References.Count == 0)
                return null;

            var replyEventID = ev.References.Where(ev => ev.Type == EventType.Reply).Select(ev => ev.EventId).FirstOrDefault();
            return ev.ReferencedEvents.Where(ev => ev.Id == replyEventID).FirstOrDefault();
        }

        public async Task AddImageToEvent(string imgURL, byte[] imageBytes)
        {
            var bitmap = ConvertBytesToBitmap(imageBytes);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Images.Add(new(imgURL, bitmap));
            });
        }

        private Bitmap ConvertBytesToBitmap(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}
