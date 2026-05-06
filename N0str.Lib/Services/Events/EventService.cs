using N0str.Services.Relay;
using NNostr.Client;
using System.Collections.Concurrent;

namespace N0str.Services.Events
{
    public class EventService : IEventService, IDisposable
    {
        private readonly IRelayService _relayService;

        public EventService(IRelayService relayService)
        {
            _relayService = relayService;

            _relayService.EventReceived += OnNostrEventsReceived;
            _relayService.EoseReceived += OnEoseReceived;
        }

        private void OnEoseReceived(string obj)
        {
            EoseReceived?.Invoke(obj);
        }

        // RandomGuid - one byte placeholder
        public ConcurrentDictionary<string, byte> SubscriptionIDs { get; } = new();

        // eventID - Nostr Event
        private ConcurrentDictionary<string, NostrEvent> ReceivedEvents { get; } = new();

        // Event PubKey (Author) - Bag of eventIDs
        private ConcurrentDictionary<string, ConcurrentBag<string>> EventsByAuthor { get;  } = new();

        public event Action<NostrEvent>? RelevantEventReceived;
        public event Action<string>? EoseReceived;

        public void OnNostrEventsReceived((string subscriptionId, NostrEvent nostrEvent) e)
        {
            if (!SubscriptionIDs.ContainsKey(e.subscriptionId))
            {
                return;
            }

            ProcessRelevantEvent(e.nostrEvent);
        }

        public IEnumerable<NostrEvent> GetEventsByAuthor(string pubkey)
        {
            if (EventsByAuthor.TryGetValue(pubkey, out var ids))
                return [.. ids.Select(id => ReceivedEvents[id])];

            return [];
        }
        private void ProcessRelevantEvent(NostrEvent nostrEvent)
        {
            if (ReceivedEvents.TryAdd(nostrEvent.Id, nostrEvent))
            {
                EventsByAuthor.GetOrAdd(nostrEvent.PublicKey, _ => []).Add(nostrEvent.Id);

                RelevantEventReceived?.Invoke(nostrEvent);
            }
        }

        public void RegisterNewSubscriptionID(string subscriptionID)
        {
            SubscriptionIDs.TryAdd(subscriptionID, 0);
        }

        public void Dispose()
        {
            _relayService.EventReceived -= OnNostrEventsReceived;
            _relayService.EoseReceived -= OnEoseReceived;
        }
    }
}
