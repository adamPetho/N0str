using N0str.Factory;
using N0str.Services.Events;
using N0str.Services.Tor;
using NNostr.Client;
using NNostr.Client.Protocols;

namespace N0str.Services.Relay
{
    public class RelayService : IRelayService, IDisposable
    {
        private readonly ITorService _torService;
        private INostrClient? _nostrClient;
        private INostrClient NostrClient => _nostrClient ?? throw new InvalidOperationException("NostrClient is null. Not connected to relays.");

        public event Action<(string, NostrEvent)>? EventReceived;
        public event Action<string>? EoseReceived;

        public RelayService(ITorService torService)
        {
            _torService = torService;
        }

        public async Task ConnectAsync(IEnumerable<string> relayUrls, CancellationToken ct = default)
        {
            var relayUris = relayUrls.Select(x => new Uri(x));
            INostrClient nostrClient = NostrClientFactory.Create([.. relayUris], _torService.GetSocksEndpoint());

            await nostrClient.ConnectAndWaitUntilConnected(ct);

            _nostrClient = nostrClient;
            NostrClient.EventsReceived += OnNostrEventsReceived;
            NostrClient.EoseReceived += OnEoseReceived;
        }

        private void OnNostrEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) e)
        {
            foreach (NostrEvent ev in e.events)
            {
                EventReceived?.Invoke((e.subscriptionId, ev));
            }
        }

        private void OnEoseReceived(object? sender, string e)
        {
            EoseReceived?.Invoke(e);
        }

        public async Task PublishEventAsync(NostrEvent nostrEvent, CancellationToken ct = default)
        {
            await NostrClient.SendEventsAndWaitUntilReceived([nostrEvent], ct);
        }

        public async Task CreateSubscriptionAsync(string pubkey, string subscriptionID, CancellationToken ct = default)
        {
            string pubKeyHex = NIP19.FromNIP19Npub(pubkey).ToHex();
            await NostrClient.CreateSubscription(subscriptionID, [new() { Kinds = [1], Authors = [pubKeyHex] }], ct);

        }

        public void Dispose()
        {
            NostrClient.EventsReceived -= OnNostrEventsReceived;
            NostrClient.EoseReceived -= OnEoseReceived;
            NostrClient.Dispose();
        }
    }
}
