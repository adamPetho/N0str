using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Models
{
    public class NostrEventWithReferences
    {
        public NostrEvent NostrEvent { get; }
        public List<EventReference> References { get; } = [];
        public List<NostrEvent> ReferencedEvents { get;  } = [];

        public NostrEventWithReferences(NostrEvent nostrEvent, List<EventReference> references, List<NostrEvent> referencedEvents)
        {
            NostrEvent = nostrEvent;
            References = references;
            ReferencedEvents = referencedEvents;
        }

    }
}
