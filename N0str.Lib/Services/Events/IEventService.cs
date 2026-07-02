using N0str.Models;
using NNostr.Client;

namespace N0str.Services.Events
{
    public interface IEventService
    {
        event Action<NostrEventWithReferences>? RelevantEventReceived;
        event Action<string>? EoseReceived;
        IEnumerable<NostrEventWithReferences> GetEventsByAuthor(string pubkey);
        void RegisterNewSubscriptionID(string subscriptionID);
    }
}
