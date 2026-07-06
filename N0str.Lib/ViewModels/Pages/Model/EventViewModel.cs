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
        public EventReferenceViewModel? ReferenceViewModel { get; }
        public bool HasReferences => ReferenceViewModel != null;
        public string? DisplayContent { get; }

        public ObservableCollection<ImageViewModel> Images { get; } = [];

        public bool IsMediaLoading { get; set; }

        public bool HasMedia => Images.Count > 0;
        public EventType EventType { get; }

        public EventViewModel(NostrEventWithReferences ev)
        {
            NostrEvent = ev.NostrEvent;
            References = ev.References;
            ReferenceViewModel = BuildReferences(ev);
            DisplayContent = NostrEvent.Content;

            if (DisplayContent is null)
                return;

            var imageURLs = MediaExtractor.ExtractImageUrls(ev.NostrEvent);
            foreach (var imageURL in imageURLs)
            {
                DisplayContent = DisplayContent.Replace(imageURL, "");
            }
        }

        private EventReferenceViewModel? BuildReferences(NostrEventWithReferences references)
        {
            var rootEvent = ExtractRootEventIfExists(references);
            var replyEvent = ExtractReplyEventIfExists(references);

            if (rootEvent is null && replyEvent is null)
                return null;

            // Start with the deepest link (Root)
            EventReferenceViewModel? currentChain = rootEvent is not null
                ? new EventReferenceViewModel(rootEvent.Content, null)
                : null;


            if (replyEvent is not null && rootEvent is null)
            {
                // If there is a reply event, but we couldn't fetch the root event for some reason.
                currentChain = new EventReferenceViewModel(replyEvent.Content, currentChain);
            }
            else if (replyEvent is not null && rootEvent.Id != replyEvent.Id)
            {
                // If there is a reply event, wrap the root chain inside it
                currentChain = new EventReferenceViewModel(replyEvent.Content, currentChain);
            }

            return currentChain;
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
