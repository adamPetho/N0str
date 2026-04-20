using OnionSharp.Microservices;
using OnionSharp.Tor;
using OnionSharp.Tor.Models;

namespace N0str.Services.Tor
{
    public class TorService : ITorService
    {
        private TorProcessManager? _torProcessManager;
        private OnionHttpClientFactory? _httpFactory;
        private TorSettings? _torSettings;

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                var dataDir = EnvironmentHelpers.GetDataDir("N0str");

                _torSettings = new TorSettings(
                    dataDir,
                    distributionFolderPath: EnvironmentHelpers.GetFullBaseDirectory(),
                    terminateOnExit: true,
                    TorMode.Enabled,
                    socksPort: 37155,
                    controlPort: 37156
                );

                _torProcessManager = new TorProcessManager(_torSettings);

                await _torProcessManager.StartAsync(attempts: 3, ct);

                _httpFactory = new OnionHttpClientFactory(
                    _torSettings.SocksEndpoint.ToUri("socks5")
                );
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Couldn't initialize Tor. {ex}");
                throw;
            }
        }

        public HttpClient CreateHttpClient(string name = "default")
        {
            return _httpFactory!.CreateClient(name);
        }

        public TorSettings? GetTorSettings() => _torSettings;
    }
}
