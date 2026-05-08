using NNostr.Client;
using System.Net;
using System.Net.WebSockets;

namespace N0str.Factory
{
    public static class NostrClientFactory
    {
        public static INostrClient Create(Uri[] relays, EndPoint? torEndpoint = null)
        {
            WebProxy? webProxy = CreateProxy(torEndpoint);
            return Create(relays, webProxy);
        }

        public static WebProxy? CreateProxy(EndPoint? torEndpoint)
        {
            return torEndpoint switch
            {
                IPEndPoint endpoint => new WebProxy($"socks5://{endpoint.Address}:{endpoint.Port}"),
                DnsEndPoint endpoint => new WebProxy($"socks5://{endpoint.Host}:{endpoint.Port}"),
                null => null,
                _ => throw new ArgumentException("Endpoint type is not supported.")
            };
        }

        public static INostrClient Create(Uri[] relays, WebProxy? proxy)
        {
            return relays.Length switch
            {
                0 => throw new ArgumentException("At least one relay is required.", nameof(relays)),
                1 => new NostrClient(relays.First(), ConfigureSocket),
                _ => new CompositeNostrClient(relays, ConfigureSocket)
            };

            void ConfigureSocket(WebSocket socket)
            {
                if (socket is ClientWebSocket clientWebSocket)
                {
                    clientWebSocket.Options.Proxy = proxy;
                }
            }
        }
    }
}
