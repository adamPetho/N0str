using N0str.Services.Relay;
using NNostr.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Events
{
    public class EventService : IEventService, IDisposable
    {
        private readonly IRelayService _relayService;

        public EventService(IRelayService relayService)
        {
            _relayService = relayService;

            _relayService.EventReceived += OnNostrEventsReceived;
            _relayService.SuccessfulSubscription += OnSuccessfulSubscription;
        }
        public ConcurrentDictionary<string, byte> SubscriptionIDs { get; } = new();
        private ConcurrentDictionary<string, NostrEvent> ReceivedEvents { get; } = new();

        public event Action<NostrEvent>? RelevantEventReceived;

        public void OnNostrEventsReceived((string subscriptionId, NostrEvent nostrEvent) e)
        {
            if (!SubscriptionIDs.ContainsKey(e.subscriptionId))
            {
                return;
            }
            ProcessRelevantEvent(e.nostrEvent);
        }

        public void OnSuccessfulSubscription(string subscriptionID)
        {
            SubscriptionIDs.TryAdd(subscriptionID, 0);
        }
        private void ProcessRelevantEvent(NostrEvent nostrEvent)
        {
            if (ReceivedEvents.TryAdd(nostrEvent.Id, nostrEvent))
            {
                RelevantEventReceived?.Invoke(nostrEvent);
            }
        }
        public void Dispose()
        {
            _relayService.EventReceived -= OnNostrEventsReceived;
            _relayService.SuccessfulSubscription -= OnSuccessfulSubscription;
        }
    }
}
