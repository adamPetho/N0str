using OnionSharp.Microservices;
using OnionSharp.Tor;
using OnionSharp.Tor.Models;
using System.Net;

namespace N0str.Services.Tor
{
    public class TorService : ITorService
    {
        private TorProcessManager? _torProcessManager;

        public TorSettings _torSettings = new(
            EnvironmentHelpers.GetDataDir("N0str"),
            EnvironmentHelpers.GetFullBaseDirectory(),
            terminateOnExit: true,
            socksPort: 37155,
            controlPort: 37156);

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                _torProcessManager = new TorProcessManager(_torSettings);

                await _torProcessManager.StartAsync(attempts: 3, ct);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Couldn't initialize Tor. {ex}");
                throw;
            }
        }

        public EndPoint GetSocksEndpoint() => _torSettings.SocksEndpoint;
    }
}
