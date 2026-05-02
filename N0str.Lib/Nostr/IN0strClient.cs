using NNostr.Client;

namespace N0str.Nostr
{
    public interface IN0strClient
    {
        Task<NostrEvent> SignEvent(string key, NostrEvent nostrEvent);
        Task<NostrEvent> CreateNostrEvent(string content, int kind, List<(string TagIdentifier, string[] Data)> tags);
        Task PublishEventAsync(NostrEvent ev, CancellationToken ct = default);
    }
}
