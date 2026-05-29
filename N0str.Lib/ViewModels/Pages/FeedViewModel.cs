using N0str.Nostr;
using N0str.Services;
using N0str.Services.Events;
using N0str.ViewModels.Pages.Model;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace N0str.ViewModels.Pages
{
    public class FeedViewModel : ViewModelBase, IDisposable
    {
        private readonly INavigation _navigateService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IN0strClient _nostrClient;
        private readonly IEventService _eventService;
        private List<string> _pubkeys;

        public ObservableCollection<EventViewModel> Events { get; } = new();

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public List<string> Pubkeys
        {
            get => _pubkeys;
            set => SetProperty(ref _pubkeys, value);
        }

        public FeedViewModel(INavigation navigateService, IServiceProvider serviceProvider, IN0strClient nostrClient, IEventService eventService)
        {
            _navigateService = navigateService;
            _serviceProvider = serviceProvider;
            _nostrClient = nostrClient;
            _eventService = eventService;

            _pubkeys = new();

            _eventService.RelevantEventReceived += OnEventReceived;
            _eventService.EoseReceived += OnEOSEReceived;

            BackCommand = ReactiveCommand.Create(() => _navigateService.NavigateBack());
            AddNewSubCommand = ReactiveCommand.Create(OpenModalForNewSubscription);
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> AddNewSubCommand { get; }

        public async Task InitializeAsync(string pubkey)
        {
            Pubkeys.Add(pubkey);

            await SubscribeToPubkey(pubkey);

            foreach (var pubk in Pubkeys) 
            {
                LoadExistingEventsFromMemory(pubk);
            }
        }

        private void OpenModalForNewSubscription()
        {
            var modalInput = new PubKeyToFetchViewModel(_navigateService, _serviceProvider);
            _navigateService.OpenModal(modalInput);
        }

        private async Task SubscribeToPubkey(string pubkey)
        {
            try
            {
                await _nostrClient.SubscribeToPubkey(pubkey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void LoadExistingEventsFromMemory(string pubkey)
        {
            // Load whatever we already have (empty at first)
            var existing = _nostrClient.FetchAllByAuthorFromMemory(pubkey);

            foreach (var ev in existing.OrderByDescending(e => e.CreatedAt))
            {
                if (Events.Any(e => e.ID == ev.Id))
                {
                    return;
                }

                InsertSorted(ev);
            }
        }

        private void OnEventReceived(NostrEvent ev)
        {
            // Prevent duplicates in UI
            if (Events.Any(e => e.ID == ev.Id))
                return;

            InsertSorted(ev);

        }

        private void InsertSorted(NostrEvent ev)
        {
            int index = 0;

            while (index < Events.Count &&
                   Events[index].CreatedAt > ev.CreatedAt)
            {
                index++;
            }

            Events.Insert(index, new(ev));
        }

        private void OnEOSEReceived(string subscriptionId)
        {
            if (IsLoading)
            {
                IsLoading = false;
            }
        }

        public void Dispose()
        {
            _eventService.RelevantEventReceived -= OnEventReceived;
            _eventService.EoseReceived -= OnEOSEReceived;
            // cancel any pending subscriptions
            // _nostrClient.Unsubscribe(_pubkey);
        }
    }
}
