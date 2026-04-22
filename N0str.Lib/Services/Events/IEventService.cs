using NNostr.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Events
{
    public interface IEventService
    {
        event Action<NostrEvent>? RelevantEventReceived;
        IEnumerable<NostrEvent> GetEventsByAuthor(string pubkey);
    }
}
