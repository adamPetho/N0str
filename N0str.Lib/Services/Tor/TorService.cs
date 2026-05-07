using OnionSharp.Microservices;
using OnionSharp.Tor;
using System.Net;
using System.Net.Sockets;

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
            const int maxAttempts = 3;

            for (int i = 1; i <= maxAttempts; i++) 
            {
                try
                {
                    _torProcessManager = new TorProcessManager(_torSettings);
                    await _torProcessManager.StartAsync(attempts: 3, ct);
                    return;
                }
                catch (Exception ex) when (i < maxAttempts)
                {
                    Console.WriteLine($"Failed to start Tor. Remaining tries: {maxAttempts - i}. Exception: {ex} ");
                    await Task.Delay(500, ct);
                }
            }
        }

        public EndPoint GetSocksEndpoint() => _torSettings.SocksEndpoint;
    }
}
