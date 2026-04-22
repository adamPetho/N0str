using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Nostr
{
    public interface IN0strClient
    {
        Task<NostrEvent> SignEvent(string key, NostrEvent nostrEvent);
        Task<NostrEvent> CreateNostrEvent(string content, List<(string TagIdentifier, string[] Data)> tags);
        Task PublishAsync(NostrEvent ev, CancellationToken ct = default);
    }
}
