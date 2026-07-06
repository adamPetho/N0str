using DynamicData;
using N0str.Models;
using N0str.Services.Relay;
using NNostr.Client;
using System.Collections.Concurrent;
using System.Diagnostics;

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
        private ConcurrentDictionary<string, ConcurrentBag<string>> EventsByAuthor { get; } = new();

        // Event ID - List of Event References to map relations (root - reply - mentions)
        private ConcurrentDictionary<string, List<EventReference>> EventReferences { get; } = new();

        public event Action<NostrEventWithReferences>? RelevantEventReceived;
        public event Action<string>? EoseReceived;

        public void OnNostrEventsReceived((string subscriptionId, NostrEvent nostrEvent) e)
        {
            if (!SubscriptionIDs.ContainsKey(e.subscriptionId))
            {
                return;
            }

            ProcessRelevantEvent(e.nostrEvent);
        }

        public IEnumerable<NostrEventWithReferences> GetEventsByAuthor(string pubkey)
        {
            if (EventsByAuthor.TryGetValue(pubkey, out var eventIds))
            {
                var existingEventsInMemory = eventIds.Select(id => ReceivedEvents[id]);

                List<NostrEventWithReferences> nostrEventWithReferences = new List<NostrEventWithReferences>();
                foreach (var existingEvent in existingEventsInMemory)
                {
                    if (!EventReferences.TryGetValue(existingEvent.Id, out List<EventReference>? references))
                    {
                        // If no references (root), then add with empty collections.
                        nostrEventWithReferences.Add(new(existingEvent, [], []));
                        continue;
                    }
                    var referencedEvents = references.Select(reference => ReceivedEvents[reference.EventId]).ToList();
                    nostrEventWithReferences.Add(new(existingEvent, references, referencedEvents));
                }

                return nostrEventWithReferences;
            }

            return [];
        }
        
        private async void ProcessRelevantEvent(NostrEvent nostrEvent)
        {
            if (ReceivedEvents.TryAdd(nostrEvent.Id, nostrEvent))
            {
                EventsByAuthor.GetOrAdd(nostrEvent.PublicKey, _ => []).Add(nostrEvent.Id);

                var references = CheckTagsForReferences(nostrEvent.Tags);

                List<NostrEvent> referencedNostrEvents = new();

                // Add maximum to avoid too many downloads?
                if (references.Count != 0)
                {
                    EventReferences.TryAdd(nostrEvent.Id, references);

                    var eventsInMemory = references
                        .Where(r => ReceivedEvents.ContainsKey(r.EventId))
                        .Select(r => (Event: ReceivedEvents[r.EventId], RelayUrl: r.RelayUrl))
                        .Distinct()
                        .ToList();

                    referencedNostrEvents.AddRange(eventsInMemory.Select(ev => ev.Event));

                    var missingEvents = references
                         .Where(r => !ReceivedEvents.ContainsKey(r.EventId))
                         .Select(r => (r.EventId, r.RelayUrl))
                         .Distinct()
                         .ToArray();

                    if (missingEvents.Length != 0)
                    {
                        var events = await _relayService.FetchIndividualEventsAsync(missingEvents);
                        foreach (NostrEvent receivedEvent in events)
                        {
                            ReceivedEvents.TryAdd(receivedEvent.Id, receivedEvent);
                            referencedNostrEvents.Add(receivedEvent);
                        }
                    }
                }

                RelevantEventReceived?.Invoke(new NostrEventWithReferences(nostrEvent, references, referencedNostrEvents));
            }
        }

        public void RegisterNewSubscriptionID(string subscriptionID)
        {
            SubscriptionIDs.TryAdd(subscriptionID, 0);
        }

        public List<EventReference> CheckTagsForReferences(List<NostrEventTag> tags)
        {
            List<EventReference> references = [];

            foreach (var tag in tags)
            {
                if (tag.TagIdentifier != "e")
                    continue;

                references.Add(new EventReference
                {
                    EventId = tag.Data[0],
                    RelayUrl = tag.Data.ElementAtOrDefault(1),
                    Type = ParseMarker(tag.Data.ElementAtOrDefault(2))
                });
            }

            return references;
        }

        private EventType ParseMarker(string? v)
        {
            return v switch
            {
                "root" => EventType.Root,
                "reply" => EventType.Reply,
                "mention" => EventType.Mention,
                _ => EventType.Unknown,
            };
        }

        public void Dispose()
        {
            _relayService.EventReceived -= OnNostrEventsReceived;
            _relayService.EoseReceived -= OnEoseReceived;
        }
    }
}
