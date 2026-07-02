using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Models
{
    public record EventReference
    {
        public required string EventId { get; init; }

        public string? RelayUrl { get; init; }

        public EventType Type { get; init; }
    }

    public enum EventType
    {
        Root,
        Reply,
        Mention,
        Unknown
    }
}
