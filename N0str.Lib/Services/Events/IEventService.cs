using NNostr.Client;

namespace N0str.Services.Events
{
    public interface IEventService
    {
        event Action<NostrEvent>? RelevantEventReceived;
        event Action<string>? EoseReceived;
        IEnumerable<NostrEvent> GetEventsByAuthor(string pubkey);
        void RegisterNewSubscriptionID(string subscriptionID);
    }
}
