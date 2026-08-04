namespace N0str.ViewModels.Pages.Model.ContentSegments
{
    public abstract class ContentSegment
    {
    }

    public sealed class TextSegment : ContentSegment
    {
        public string Text { get; init; }
    }

    public sealed class MentionSegment : ContentSegment
    {
        public string NProfile { get; init; }

        public string DisplayName { get; set; }
    }

    public sealed class ImageSegment : ContentSegment
    {
        public string Url { get; init; }
    }

    public sealed class LinkSegment : ContentSegment
    {
        public string Url { get; init; }
    }
}
