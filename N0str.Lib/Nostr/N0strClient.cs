using N0str.Services.Events;
using N0str.Services.Relay;
using NBitcoin.Secp256k1;
using NNostr.Client;
using NNostr.Client.Protocols;

namespace N0str.Nostr
{
    public class N0strClient : IN0strClient
    {
        private readonly IRelayService _relayService;
        private readonly IEventService _eventService;

        public N0strClient(IRelayService relayService, IEventService eventService)
        {
            _relayService = relayService;
            _eventService = eventService;
        }

        public async Task<NostrEvent> CreateNostrEvent(string content, List<(string TagIdentifier, string[] Data)> tags)
        {
            NostrEvent unsignedEvent = new NostrEvent
            {
                Kind = 1,
                Content = content,
                Tags = CreateTags(tags),
                CreatedAt = DateTime.UtcNow,
            };

            return unsignedEvent;
        }

        public async Task<NostrEvent> SignEvent(string key, NostrEvent nostrEvent)
        {
            using ECPrivKey signingKey = key.FromNIP19Nsec();
            return await nostrEvent.ComputeIdAndSignAsync(signingKey);
        }

        private List<NostrEventTag> CreateTags(List<(string TagIdentifier, string[] Data)> tags)
        {
            List<NostrEventTag> nostrEventTags = new List<NostrEventTag>();

            foreach (var tag in tags) 
            {
                nostrEventTags.Add(new NostrEventTag()
                {
                    TagIdentifier = tag.TagIdentifier,
                    Data = tag.Data.ToList()
                });
            }

            return nostrEventTags;
        }

        public async Task PublishEventAsync(NostrEvent ev, CancellationToken ct = default)
        {
            await _relayService.PublishEventAsync(ev, ct);
        }

        public IEnumerable<NostrEvent> FetchAllByAuthor(string pubkey)
        {
            return _eventService.GetEventsByAuthor(pubkey);
        }
    }
}
