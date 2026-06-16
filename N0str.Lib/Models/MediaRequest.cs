using N0str.ViewModels.Pages.Model;

namespace N0str.Models
{
    public record MediaRequest(IReadOnlyCollection<string> ImageURLs, EventViewModel ViewModel);
}
