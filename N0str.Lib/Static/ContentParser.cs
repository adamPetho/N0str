using N0str.ViewModels.Pages.Model;
using N0str.ViewModels.Pages.Model.ContentSegments;
using System.Text.RegularExpressions;

namespace N0str.Static
{
    public static partial class ContentParser
    {
        private static readonly Regex _contentRegex = ContentRegex();

        // Characters that are almost never *part of* a URL/mention but commonly trail one in prose.
        private static readonly char[] TrailingTrimChars =
            ['.', ',', '!', '?', ':', ';', ')', ']', '}', '"', '\''];

        public static List<ContentSegment> Parse(string? content)
        {
            List<ContentSegment> result = [];

            if (string.IsNullOrWhiteSpace(content))
                return result;

            int current = 0;

            foreach (Match match in _contentRegex.Matches(content))
            {
                (int start, int length) = TrimTrailingPunctuation(content, match);

                if (start > current)
                {
                    result.Add(new TextSegment
                    {
                        Text = content[current..start]
                    });
                }

                string value = content.Substring(start, length);
                result.Add(CreateSegment(match, value));

                current = start + length;
            }

            if (current < content.Length)
            {
                result.Add(new TextSegment
                {
                    Text = content[current..]
                });
            }

            return result;
        }

        private static (int Start, int Length) TrimTrailingPunctuation(string content, Match match)
        {
            int start = match.Index;
            int end = match.Index + match.Length;

            while (end > start && TrailingTrimChars.Contains(content[end - 1]))
                end--;

            return (start, end - start);
        }

        private static ContentSegment CreateSegment(Match match, string value)
        {
            if (match.Groups["mention"].Success)
            {
                return new MentionSegment
                {
                    NProfile = value,
                    DisplayName = $"@{value}"
                };
            }

            if (MediaExtractor.IsImageUrl(value))
            {
                return new ImageSegment
                {
                    Url = value,
                    ImageVM = new ImageViewModel(value, null)
                };
            }

            return new LinkSegment
            {
                Url = value
            };
        }

        // "mention" only matches known bech32 identifier prefixes, not arbitrary nostr:garbage.
        // "nsec" (private keys) is deliberately excluded so it doesn't get rendered like a normal mention.
        [GeneratedRegex(
            @"(?<mention>nostr:(?:npub|nprofile|note|nevent|naddr)1[a-z0-9]+)|(?<link>https?://[^\s<>""']+)",
            RegexOptions.Compiled)]
        private static partial Regex ContentRegex();
    }
}