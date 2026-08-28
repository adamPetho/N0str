using Microsoft.Extensions.Logging;
using N0str.Factory;
using N0str.Logging;
using N0str.Services.Tor;
using N0str.Services.Tor.Settings;
using NNostr.Client;

namespace N0str.Services.Relay
{
    public class RelayService : IRelayService, IDisposable
    {
        private readonly ITorService _torService;
        private readonly ITorSettings _torSettings;
        private readonly INostrClientFactory _nostrClientFactory;
        private INostrClient? _nostrClient;
        private INostrClient NostrClient => _nostrClient ?? throw new InvalidOperationException("NostrClient is null. Not connected to relays.");

        public event Action<(string, NostrEvent)>? EventReceived;
        public event Action<string>? EoseReceived;

        public RelayService(ITorService torService, ITorSettings torSettings, INostrClientFactory nostrClientFactory)
        {
            _torService = torService;
            _torSettings = torSettings;
            _nostrClientFactory = nostrClientFactory;
        }

        public async Task ConnectAsync(IEnumerable<string> relayUrls, CancellationToken ct = default)
        {
            var relayUris = relayUrls.Select(x => new Uri(x));
            INostrClient nostrClient;

            var successfullyConnectedRelays = new List<Uri>();

            foreach (var relay in relayUris)
            {
                try
                {
                    var client = _nostrClientFactory.Create([relay], _torSettings.GetSocksEndpoint());
                    await client.ConnectAndWaitUntilConnected(ct);

                    successfullyConnectedRelays.Add(relay);

                    if (successfullyConnectedRelays.Count >= 3)
                        break;
                }
                catch (Exception ex)
                {
                    // Log and continue with next relay.
                    Logger.LogWarning($"Failed to connect to relay: {relay.ToString()}. Exception: {ex} ");
                }
            }

            if (successfullyConnectedRelays.Count == 0)
            {
                // Fallback to clearnet
                var _clearnetClient = _nostrClientFactory.Create([new Uri("wss://relay.primal.net")], null);
                await _clearnetClient.ConnectAndWaitUntilConnected(ct);

                _nostrClient = _clearnetClient;
            }
            else
            {
                // Have to reconnect again, NNostr doesn't let us keep the connections, so we build it again.
                nostrClient = _nostrClientFactory.Create([.. successfullyConnectedRelays], _torSettings.GetSocksEndpoint());
                await nostrClient.ConnectAndWaitUntilConnected(ct);

                _nostrClient = nostrClient;
            }

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
            await NostrClient.CreateSubscription(subscriptionID, [new() { Kinds = [1], Authors = [pubkey] }], ct);
        }

        public async Task<List<NostrEvent>> FetchIndividualEventsAsync((string EventId, string? RelayUrl)[] missingEvents, CancellationToken ct = default)
        {
            var eventsWithRelays = missingEvents.Where(ev => ev.RelayUrl is not null);
            var eventsWithoutRelays = missingEvents.Where(ev => ev.RelayUrl is null);

            var task1 = FetchEventsThroughSpecificRelays([.. eventsWithRelays.Select(ev => (ev.EventId, ev.RelayUrl))], ct);
            var task2 = NostrClient.FetchEvents([new() { Ids = [.. eventsWithoutRelays.Select(ev => ev.EventId)] }], ct);

            await Task.WhenAll(task1, task2);

            var combinedResults = task1.Result.Concat(task2.Result).ToList();
            return combinedResults;
        }

        public async Task<List<NostrEvent>> FetchEventsThroughSpecificRelays((string eventId, string? relayUrl)[] events, CancellationToken ct = default)
        {
            var fetchTasks = events.Select(async ev =>
            {
                var relay = string.IsNullOrEmpty(ev.relayUrl) ? "wss://relay.primal.net" : ev.relayUrl;
                return await FetchEvent(ev.eventId, relay, ct);
            });

            var resultsArray = await Task.WhenAll(fetchTasks);
            var combinedResults = resultsArray.SelectMany(eventsList => eventsList).ToList();
            return combinedResults;

            async Task<List<NostrEvent>> FetchEvent(string eventId, string relayUrl, CancellationToken ct = default)
            {
                // Immediate fallback if relayUrl is missing or doesn't contain wss://.
                if (string.IsNullOrWhiteSpace(relayUrl) || !relayUrl.Contains("wss://"))
                {
                    return await NostrClient.FetchEvents([new() { Ids = [eventId] }], ct);
                }

                try
                {
                    using var client = _nostrClientFactory.Create([new Uri(relayUrl)], _torSettings.GetSocksEndpoint());
                    await client.ConnectAndWaitUntilConnected(ct);

                    return await client.FetchEvents([new() { Ids = [eventId] }], ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Fallback to default relays
                    Logger.LogWarning($"Couldn't fetch '{eventId}' with specified relay '{relayUrl}'. Fallback to default relays. Exception: {ex}");
                    return await NostrClient.FetchEvents([new() { Ids = [eventId] }], ct);
                }
            }
        }

        public void Dispose()
        {
            NostrClient.EventsReceived -= OnNostrEventsReceived;
            NostrClient.EoseReceived -= OnEoseReceived;
            NostrClient.Dispose();
        }
    }
}
