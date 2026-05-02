using NNostr.Client;

namespace N0str.Services.Events
{
    public interface IEventService
    {
        event Action<NostrEvent>? RelevantEventReceived;
        IEnumerable<NostrEvent> GetEventsByAuthor(string pubkey);
    }
}
