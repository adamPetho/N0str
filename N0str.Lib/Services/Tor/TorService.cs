using N0str.Logging;
using N0str.Services.Tor.Settings;
using OnionSharp.Tor;

namespace N0str.Services.Tor
{
    public class TorService : ITorService
    {
        private TorProcessManager? _torProcessManager;
        private ITorSettings _torSettings;

        public TorService(ITorSettings torSettings)
        {
            _torSettings = torSettings;
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            const int maxAttempts = 3;

            for (int i = 1; i <= maxAttempts; i++) 
            {
                try
                {
                    _torProcessManager = new TorProcessManager(_torSettings.GetTorSettings());
                    await _torProcessManager.StartAsync(attempts: 3, ct);
                    return;
                }
                catch (Exception ex) when (i < maxAttempts)
                {
                    Logger.LogWarning($"Failed to start Tor. Remaining tries: {maxAttempts - i}. Exception: {ex} ");
                    await Task.Delay(500, ct);
                }
            }
        }

    }
}
