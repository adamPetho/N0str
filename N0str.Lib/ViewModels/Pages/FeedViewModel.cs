using Avalonia.Threading;
using N0str.Nostr;
using N0str.Services.Events;
using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.ViewModels.Pages
{
    public class FeedViewModel : ViewModelBase
    {
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

        public FeedViewModel(IN0strClient nostrClient, IEventService eventService)
        {
            _nostrClient = nostrClient;
            _eventService = eventService;
        }

        public async Task InitializeAsync(string pubkey)
        {
            _pubkey = pubkey;
            IsLoading = true;

            await _nostrClient.SubscribeToPubkey(pubkey);

            // Load whatever we already have (may be empty)
            var existing = _nostrClient.FetchAllByAuthor(pubkey);

            foreach (var ev in existing.OrderByDescending(e => e.CreatedAt))
            {
                Events.Add(ev);
            }

            // Subscribe to live updates
            _eventService.RelevantEventReceived += OnEventReceived;

            // TODO: handle EOSE for loading state
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
            //_nostrClient.EOSEReceived -= OnEOSEReceived;
        }
    }
}
