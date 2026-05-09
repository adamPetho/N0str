using Moq;
using N0str.Services.Events;
using N0str.Services.Relay;
using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Tests.UnitTests
{
    public class EventServiceTests
    {
        private readonly Mock<IRelayService> _relayServiceMock;

        private readonly IEventService _eventService;

        public EventServiceTests()
        {
            _relayServiceMock = new Mock<IRelayService>();

            _eventService = new EventService(_relayServiceMock.Object);
        }

        [Fact]
        public void Ignore_Unsubscribed_Events_Test()
        {
            bool relevantEventReceived = false;
            const string registeredSubscriptionId = "sub-1";
            const string authorPubKey = "npub123456";

            _eventService.RegisterNewSubscriptionID(registeredSubscriptionId);

            // If EventService raises RelevantEventReceived (Processed the event) turn the bool to true.
            _eventService.RelevantEventReceived += _ => relevantEventReceived = true;

            var nostrEvent = new NostrEvent
            {
                Id = "Event-1",
                Content = "Hello",
                Kind = 1,
                PublicKey = authorPubKey
            };

            // Raise event with different subID to test if we ignore it. (we should ignore it)
            _relayServiceMock.Raise(x => x.EventReceived += null, ("other-subscription", nostrEvent));

            Assert.False(relevantEventReceived);

            // Wrong subscriptionID --> Didn't process --> Should be empty
            var eventsOfPubKey = _eventService.GetEventsByAuthor(authorPubKey);
            Assert.Empty(eventsOfPubKey);
        }

        [Fact]
        public void Process_Event_Test()
        {
            bool relevantEventReceived = false;
            const string registeredSubscriptionId = "sub-1";
            const string authorPubKey = "npub123456";

            var nostrEvent = new NostrEvent
            {
                Id = "Event-1",
                Content = "Hello",
                Kind = 1,
                PublicKey = authorPubKey
            };

            _eventService.RegisterNewSubscriptionID(registeredSubscriptionId);

            // If EventService raises RelevantEventReceived (Processed the event) turn the bool to true.
            _eventService.RelevantEventReceived += _ => relevantEventReceived = true;

            _relayServiceMock.Raise(x => x.EventReceived += null, (registeredSubscriptionId, nostrEvent));

            Assert.True(relevantEventReceived);

            var eventsOfPubKey = _eventService.GetEventsByAuthor(authorPubKey);
            Assert.NotEmpty(eventsOfPubKey);
            var ev = Assert.Single(eventsOfPubKey);
            Assert.Equal(ev, nostrEvent);
        }

        [Fact]
        public void Handle_Duplicate_Events_Test()
        {
            int processedEventCounter = 0;
            const string registeredSubscriptionId = "sub-1";
            const string authorPubKey = "npub123456";


            var nostrEvent = new NostrEvent
            {
                Id = "Event-1",
                Content = "Hello",
                Kind = 1,
                PublicKey = authorPubKey
            };

            _eventService.RegisterNewSubscriptionID(registeredSubscriptionId);

            // If EventService raises RelevantEventReceived (Processed the event) increase counter.
            _eventService.RelevantEventReceived += _ => processedEventCounter++;

            _relayServiceMock.Raise(x => x.EventReceived += null, (registeredSubscriptionId, nostrEvent));
            _relayServiceMock.Raise(x => x.EventReceived += null, (registeredSubscriptionId, nostrEvent));
            _relayServiceMock.Raise(x => x.EventReceived += null, (registeredSubscriptionId, nostrEvent));
            
            Assert.Equal(1, processedEventCounter);

            var eventsOfPubKey = _eventService.GetEventsByAuthor(authorPubKey);
            Assert.NotEmpty(eventsOfPubKey);
            var ev = Assert.Single(eventsOfPubKey);
            Assert.Equal(ev, nostrEvent);

        }
    }
}
