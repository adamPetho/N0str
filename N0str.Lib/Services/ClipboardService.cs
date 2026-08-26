using Avalonia.Input.Platform;

namespace N0str.Services
{
    public class ClipboardService : IClipboardService
    {
        private readonly IClipboard _clipboard;

        public ClipboardService(IClipboard clipboard) => _clipboard = clipboard;

        public Task SetTextAsync(string text) => _clipboard.SetTextAsync(text);
    }
}
