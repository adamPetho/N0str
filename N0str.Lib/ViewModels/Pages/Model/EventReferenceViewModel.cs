using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.ViewModels.Pages.Model
{
    public class EventReferenceViewModel : ViewModelBase
    {
        public EventReferenceViewModel(string? content, EventReferenceViewModel? eventReference)
        {
            DisplayContent = content;
            EventReference = eventReference;
        }

        // One layer down - recusive. A -- B -- C. If we are B, we have the event 'C' as EventReference and we are the EventReference of event 'A'.
        public EventReferenceViewModel? EventReference { get; }
        public bool HasEventReference => EventReference != null;
        public string? DisplayContent { get; }
    }
}
