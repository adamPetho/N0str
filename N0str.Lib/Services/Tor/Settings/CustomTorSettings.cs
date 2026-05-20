using OnionSharp.Microservices;
using OnionSharp.Tor;
using System.Net;

namespace N0str.Services.Tor.Settings
{
    public class CustomTorSettings : ITorSettings
    {
        public TorSettings _torSettings = new(
            EnvironmentHelpers.GetDataDir("N0str"),
            EnvironmentHelpers.GetFullBaseDirectory(),
            terminateOnExit: true,
            socksPort: 37155,
            controlPort: 37156);

        public TorSettings GetTorSettings() => _torSettings;
        public EndPoint GetSocksEndpoint() => _torSettings.SocksEndpoint;
    }
}
