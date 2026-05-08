using Moq;
using N0str.Nostr;
using N0str.Services.Events;
using N0str.Services.Relay;
using NBitcoin.Secp256k1;
using NNostr.Client.Protocols;


namespace N0str.Tests.UnitTests
{
    public class N0strClientTests
    {
        private readonly Mock<IRelayService> _relayServiceMock;
        private readonly Mock<IEventService> _eventServiceMock;

        private readonly N0strClient _n0strClient;

        public N0strClientTests()
        {
            _relayServiceMock = new Mock<IRelayService>();
            _eventServiceMock = new Mock<IEventService>();

            _n0strClient = new N0strClient(_relayServiceMock.Object, _eventServiceMock.Object);
        }

        [Fact]
        public async Task Create_And_Sign_Nostr_Event_TestAsync()
        {
            string expectedContent = "Test - Content";
            int expectedKind = 1;
            List<(string TagIdentifier, string[] Data)> tags = new();
            tags.Add(("Tag-Identifier-1", new[] { "Tag-1-Data-1", "Tag-1-Data-2" }));
            tags.Add(("Tag-Identifier-2", new[] { "Tag-2-Data-1", "Tag-2-Data-2" }));

            var actualEvent = await _n0strClient.CreateNostrEvent(expectedContent, expectedKind, tags);

            Assert.Equal(expectedContent, actualEvent.Content);
            Assert.Equal(expectedKind, actualEvent.Kind);
            Assert.Equal(2, actualEvent.Tags.Count);
            Assert.Equal("Tag-Identifier-1", actualEvent.Tags[0].TagIdentifier);
            Assert.Equal("Tag-Identifier-2", actualEvent.Tags[1].TagIdentifier);

            Assert.Equal("Tag-1-Data-1", actualEvent.Tags[0].Data[0]);
            Assert.Equal("Tag-1-Data-2", actualEvent.Tags[0].Data[1]);
            Assert.Equal("Tag-2-Data-1", actualEvent.Tags[1].Data[0]);
            Assert.Equal("Tag-2-Data-2", actualEvent.Tags[1].Data[1]);

            //random bytes and private key for testing
            byte[] bytes = Enumerable.Repeat((byte)1, 32).ToArray();
            using ECPrivKey privateKey = Context.Instance.CreateECPrivKey(bytes);
            string nsec = privateKey.ToNIP19();

            await _n0strClient.SignEvent(nsec, actualEvent);
        }
    }
}
