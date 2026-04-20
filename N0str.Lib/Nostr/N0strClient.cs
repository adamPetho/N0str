using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Nostr
{
    public class N0strClient
    {
        public INostrClient NostrClient { get; }

        public N0strClient(INostrClient nostrClient)
        {
            NostrClient = nostrClient;
        }

        public async Task InitializeAsync(CancellationToken ct)
        {
            NostrClient.EventsReceived += OnNostrEventsReceived;

            await NostrClient.ConnectAndWaitUntilConnected(ct).ConfigureAwait(false);
        }

        private void OnNostrEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) e)
        {
            throw new NotImplementedException();
        }
    }
}
