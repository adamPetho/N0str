using N0str.Factory;
using N0str.Nostr;
using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Tests.UnitTests
{
    public class NostrClientFactoryTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenNoRelaysProvided()
        {
            Uri[] relays = [];

            Action act = () => NostrClientFactory.Create(relays);

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("At least one relay is required.", exception.Message);
        }

        [Fact]
        public void Create_ShouldReturnNostrClient_WhenSingleRelayProvided()
        {
            Uri[] relays =
            [
                new Uri("wss://relay.damus.io")
            ];

            var client = NostrClientFactory.Create(relays);

            Assert.IsType<NostrClient>(client);
        }

        [Fact]
        public void Create_ShouldReturnCompositeNostrClient_WhenMultipleRelaysProvided()
        {
            Uri[] uris = DefaultRelayURLs.URLs.Select(x => new Uri(x)).ToArray();
            var client = NostrClientFactory.Create(uris);

            Assert.IsType<CompositeNostrClient>(client);
        }

        [Fact]
        public void Create_ShouldThrow_WhenEndpointTypeUnsupported()
        {
            Uri[] relays =
            [
                new Uri("wss://relay.damus.io")
            ];

            var endpoint = new FakeEndpoint();

            Action act = () => NostrClientFactory.Create(relays, endpoint);

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("Endpoint type is not supported", exception.Message);
        }

        [Fact]
        public void CreateProxy_ShouldReturnSocks5Proxy_ForIPEndPoint()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, 9050);

            var proxy = NostrClientFactory.CreateProxy(endpoint);

            Assert.NotNull(proxy);
            Assert.Contains("9050", proxy.GetProxy(new Uri("http://example.com"))!.ToString());
        }

        private class FakeEndpoint() : EndPoint
        {
        }
    }
}
