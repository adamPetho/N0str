using OnionSharp.Tor;
using System.Net;

namespace N0str.Services.Tor.Settings
{
    public interface ITorSettings
    {
        TorSettings GetTorSettings();
        EndPoint GetSocksEndpoint();
    }
}