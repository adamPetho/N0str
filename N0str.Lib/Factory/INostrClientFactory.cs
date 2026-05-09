using NNostr.Client;
using System.Net;

namespace N0str.Factory
{
    public interface INostrClientFactory
    {
        INostrClient Create(Uri[] relays, EndPoint? torEndpoint = null);
        WebProxy? CreateProxy(EndPoint? torEndpoint);
        INostrClient Create(Uri[] relays, WebProxy? proxy);
    }
}
