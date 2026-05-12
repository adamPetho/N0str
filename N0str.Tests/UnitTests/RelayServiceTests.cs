using Moq;
using N0str.Factory;
using N0str.Services.Relay;
using N0str.Services.Tor;
using NNostr.Client;
using System.Net;
using System.Net.WebSockets;

namespace N0str.Tests.UnitTests
{
    public class RelayServiceTests
    {
        private readonly Mock<ITorService> _torServiceMock;
        private readonly Mock<INostrClientFactory> _nostrClientFactory;
        private readonly RelayService _relayService;

        public RelayServiceTests()
        {
            _torServiceMock = new Mock<ITorService>();
            _nostrClientFactory = new Mock<INostrClientFactory>();
            _relayService = new RelayService(_torServiceMock.Object, _nostrClientFactory.Object);
        }

        [Fact]
        public async Task Connection_Retry_Mechanism_Test()
        {
            // Mock the Nostr Client and it's connection to the relays.
            var mockedNostrClient = new Mock<INostrClient>();
            mockedNostrClient.SetupSequence(x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new WebSocketException())
                .ThrowsAsync(new WebSocketException())
                .Returns(Task.CompletedTask);

            // Mock successful event publishing
            mockedNostrClient
                .Setup(x => x.PublishEvent(
                    It.IsAny<NostrEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _nostrClientFactory.Setup(x => x.Create(It.IsAny<Uri[]>(), It.IsAny<EndPoint?>())).Returns(mockedNostrClient.Object);

            bool eventForwarded = false;

            _relayService.EventReceived += _ =>
            {
                eventForwarded = true;
            };


            await _relayService.ConnectAsync(["wss://relay.primal.net", "wss://nos.lol", "wss://relay.damus.io"]);

            mockedNostrClient.Raise(
                x => x.EventsReceived += null,
                mockedNostrClient.Object,
                ("sub-1", new[]
                {
                    new NostrEvent
                    {
                        Id = "event-1"
                    }
                }));

            mockedNostrClient.Verify(
                x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()),
                Times.Exactly(3));

            Assert.True(eventForwarded);
        }

        [Fact]
        public async Task Throw_If_All_Retries_Falls()
        {
            // Mock the Nostr Client to fail 3 times.
            var mockedNostrClient = new Mock<INostrClient>();
            mockedNostrClient.SetupSequence(x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new WebSocketException())
                .ThrowsAsync(new WebSocketException())
                .ThrowsAsync(new WebSocketException());

            mockedNostrClient
                .Setup(x => x.PublishEvent(
                    It.IsAny<NostrEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _nostrClientFactory.Setup(x => x.Create(It.IsAny<Uri[]>(), It.IsAny<EndPoint?>())).Returns(mockedNostrClient.Object);

            await Assert.ThrowsAsync<RelayConnectionException>(() => _relayService.ConnectAsync(["wss://relay.primal.net", "wss://nos.lol", "wss://relay.damus.io"]));

            mockedNostrClient.Verify(
                x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }
    }
}
