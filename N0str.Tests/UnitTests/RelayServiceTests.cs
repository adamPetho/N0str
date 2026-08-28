using Moq;
using N0str.Factory;
using N0str.Services.Relay;
using N0str.Services.Tor;
using N0str.Services.Tor.Settings;
using NNostr.Client;
using System.Net;
using System.Net.WebSockets;

namespace N0str.Tests.UnitTests
{
    public class RelayServiceTests
    {
        private readonly Mock<ITorService> _torServiceMock;
        private readonly Mock<ITorSettings> _torSettings;
        private readonly Mock<INostrClientFactory> _nostrClientFactory;
        private readonly RelayService _relayService;

        public RelayServiceTests()
        {
            _torServiceMock = new Mock<ITorService>();
            _torSettings = new Mock<ITorSettings>();
            _nostrClientFactory = new Mock<INostrClientFactory>();
            _relayService = new RelayService(_torServiceMock.Object, _torSettings.Object, _nostrClientFactory.Object);
        }

        [Fact]
        public async Task Throw_If_All_Retries_Falls()
        {
            // Mock the Nostr Client to fail 3 times.
            var mockedNostrClient = new Mock<INostrClient>();
            mockedNostrClient.SetupSequence(x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RelayConnectionException())
                .ThrowsAsync(new RelayConnectionException())
                .ThrowsAsync(new RelayConnectionException())
                .ThrowsAsync(new RelayConnectionException()); //throw on clearnet as well

            _nostrClientFactory.Setup(x => x.Create(It.IsAny<Uri[]>(), It.IsAny<EndPoint?>())).Returns(mockedNostrClient.Object);

            await Assert.ThrowsAsync<RelayConnectionException>(() => _relayService.ConnectAsync(["wss://relay.primal.net", "wss://nos.lol", "wss://relay.damus.io"]));

            // 4 times = 3 onion connection + 1 fallback clearnet connection failed.
            mockedNostrClient.Verify(
                x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()),
                Times.Exactly(4));
        }

        [Fact]
        public async Task FallBack_To_Clearnet()
        {
            var mockedNostrClient = new Mock<INostrClient>();
            mockedNostrClient.SetupSequence(x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RelayConnectionException())
                .ThrowsAsync(new RelayConnectionException())
                .ThrowsAsync(new RelayConnectionException());

            var mockedFallbackNostrClient = new Mock<INostrClient>();
            mockedFallbackNostrClient.Setup(x => x.ConnectAndWaitUntilConnected(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // 3 relay setup (matching the test case)
            _nostrClientFactory
                .Setup(x => x.Create(
                    It.Is<Uri[]>(uris =>
                        uris.SequenceEqual(new[]
                        {
                            new Uri("wss://relay.primal.net"),
                            new Uri("wss://nos.lol"),
                            new Uri("wss://relay.damus.io")
                        })),
                    It.IsAny<EndPoint?>()))
                .Returns(mockedNostrClient.Object);

            // Specific 1-relay case, called by RelayService on clearnet fallback
            _nostrClientFactory
                .Setup(x => x.Create(
                    It.Is<Uri[]>(uris => uris.SequenceEqual(new[]
                    {
                        new Uri("wss://relay.primal.net")
                    })),
                    It.IsAny<EndPoint?>()))
                .Returns(mockedFallbackNostrClient.Object);


            // After 3 failure, we fall back to clearnet.
            await _relayService.ConnectAsync(["wss://relay.primal.net", "wss://nos.lol", "wss://relay.damus.io"]);
        }
    }
}
