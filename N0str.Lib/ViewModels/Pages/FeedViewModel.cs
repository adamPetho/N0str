using Avalonia.Threading;
using N0str.Nostr;
using N0str.Services;
using N0str.Services.Events;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace N0str.ViewModels.Pages
{
    public class FeedViewModel : ViewModelBase, IDisposable
    {
        private readonly INavigation _navigateService;
        private readonly IN0strClient _nostrClient;
        private readonly IEventService _eventService;
        private string? _pubkey;

        public ObservableCollection<NostrEvent> Events { get; } = new();

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public FeedViewModel(INavigation navigateService, IN0strClient nostrClient, IEventService eventService)
        {
            _navigateService = navigateService;
            _nostrClient = nostrClient;
            _eventService = eventService;

            BackCommand = ReactiveCommand.Create(() => _navigateService.NavigateBack());
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; }

        public async Task InitializeAsync(string pubkey)
        {
            _pubkey = pubkey;

            _eventService.RelevantEventReceived += OnEventReceived;
            // TODO: handle EOSE for loading state
            // _eventService.EOSEReceived += OnEOSEReceived

            await _nostrClient.SubscribeToPubkey(pubkey);

            // Load whatever we already have (empty at first)
            var existing = _nostrClient.FetchAllByAuthor(pubkey);

            foreach (var ev in existing.OrderByDescending(e => e.CreatedAt))
            {
                if (Events.Any(e => e.Id == ev.Id))
                    return;

                Events.Insert(0, ev);
            }
        }

        private void OnEventReceived(NostrEvent ev)
        {
            if (ev.PublicKey != _pubkey || ev.Kind != 1)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                // Prevent duplicates in UI
                if (Events.Any(e => e.Id == ev.Id))
                    return;

                Events.Insert(0, ev);
            });
        }

        private void OnEOSEReceived(string subscriptionId)
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsLoading = false;
            });
        }

        public void Dispose()
        {
            _eventService.RelevantEventReceived -= OnEventReceived;
            // _eventService.EOSEReceived -= OnEOSEReceived;
            // cancel any pending subscriptions
            // _nostrClient.Unsubscribe(_pubkey);
        }
    }
}
