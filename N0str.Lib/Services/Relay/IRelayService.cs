using NNostr.Client;

namespace N0str.Services.Relay
{
    public interface IRelayService
    {
        Task ConnectAsync(IEnumerable<string> relayUrls, CancellationToken ct = default);
        Task PublishEventAsync(NostrEvent nostrEvent, CancellationToken ct = default);
        Task CreateSubscriptionAsync(string pubkey, string subscriptionID, CancellationToken ct = default);

        event Action<(string, NostrEvent)>? EventReceived;
    }
}
