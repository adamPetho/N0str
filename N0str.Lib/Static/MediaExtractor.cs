using NNostr.Client;
using System.Text.RegularExpressions;

namespace N0str.Static
{
    public static partial class MediaExtractor
    {
        private static readonly Regex ImageUrlRegex = new(@"https?:\/\/\S+\.(jpg|jpeg|png|webp)(\?\S*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static IReadOnlyList<string> ExtractImageUrls(NostrEvent nostrEvent)
        {
            if (string.IsNullOrWhiteSpace(nostrEvent.Content))
                return [];

            return ImageUrlRegex
                .Matches(nostrEvent.Content)
                .Select(x => x.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static IReadOnlyList<string> ExtractImageUrls(string eventText)
        {
            if (string.IsNullOrWhiteSpace(eventText))
                return [];

            return ImageUrlRegex
                .Matches(eventText)
                .Select(x => x.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static bool TryGetImageUrls(NostrEvent nostrEvent, out IReadOnlyList<string> links)
        {
            links = ExtractImageUrls(nostrEvent);
            if (links.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static bool TryGetImageUrls(string eventText, out IReadOnlyList<string> links)
        {
            links = ExtractImageUrls(eventText);
            if (links.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsImageUrl(string url)
        {
            return ImageUrlRegex.IsMatch(url);
        }
    }
}
