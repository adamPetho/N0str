using N0str.Factory;
using N0str.Nostr;
using N0str.Services.Tor;
using N0str.Views;
using NNostr.Client;
using NNostr.Client.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Relay
{
    public class RelayService : IRelayService
    {
        private readonly ITorService _torService;
        private INostrClient NostrClient {  get; set; }

        public List<string> SubscriptionIDs { get; } = [];
        private Dictionary<string, NostrEvent> ReceivedEvents { get; } = new();

        public RelayService(ITorService torService)
        {
            _torService = torService;
        }

        public async Task ConnectAsync(IEnumerable<string> relayUrls, CancellationToken ct = default)
        {
            var relayUris = relayUrls.Select(x => new Uri(x));
            INostrClient nostrClient = NostrClientFactory.Create([.. relayUris], _torService.TorSettings.SocksEndpoint);

            await nostrClient.ConnectAndWaitUntilConnected(ct);

            NostrClient = nostrClient;
            NostrClient.EventsReceived += OnNostrEventsReceived;
        }

        private void OnNostrEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) e)
        {
            if (SubscriptionIDs.Contains(e.subscriptionId))
            {
                foreach (NostrEvent nostrEvent in e.events)
                {
                    if (ReceivedEvents.TryAdd(nostrEvent.Id, nostrEvent))
                    {
                        // Handle received event, probably fire new event for N0strClient
                    }
                }
            }
        }

        public async Task SendAsync(NostrEvent nostrEvent, CancellationToken ct = default)
        {
            await NostrClient.SendEventsAndWaitUntilReceived([nostrEvent], ct);
        }

        public async Task CreateSubscriptionAsync(string pubkey, CancellationToken ct = default)
        {
            string pubKeyHex = NIP19.FromNIP19Npub(pubkey).ToHex();
            string subscriptionID = Guid.NewGuid().ToString();
            await NostrClient.CreateSubscription(subscriptionID, [new() { Kinds = [1], Authors = [pubKeyHex] }], ct).ConfigureAwait(false);

            SubscriptionIDs.Add(subscriptionID);
        }
    }
}
