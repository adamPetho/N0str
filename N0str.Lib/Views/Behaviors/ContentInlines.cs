using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using N0str.ViewModels.Pages.Model;
using N0str.ViewModels.Pages.Model.ContentSegments;
using System.Collections.Specialized;

namespace N0str.Views.Behaviors
{
    /// <summary>
    /// Renders a collection of ContentSegment items as flowing inline text on a TextBlock.
    /// Plain text becomes Run/LineBreak inlines; mentions and links are embedded as
    /// InlineUIContainer(HyperlinkButton), so they wrap naturally with surrounding text
    /// instead of being laid out as separate block-level items.
    /// </summary>
    public static class ContentInlines
    {
        public static readonly AttachedProperty<IEnumerable<ContentSegment>?> SegmentsProperty =
            AvaloniaProperty.RegisterAttached<TextBlock, IEnumerable<ContentSegment>?>(
                "Segments", typeof(ContentInlines));

        // Stores the handler we subscribed with, so we can unsubscribe the *same*
        // delegate instance later instead of leaking a subscription on every rebuild.
        private static readonly AttachedProperty<NotifyCollectionChangedEventHandler?> HandlerProperty =
            AvaloniaProperty.RegisterAttached<TextBlock, NotifyCollectionChangedEventHandler?>(
                "SegmentsChangeHandler", typeof(ContentInlines));

        public static void SetSegments(TextBlock element, IEnumerable<ContentSegment>? value) =>
            element.SetValue(SegmentsProperty, value);

        public static IEnumerable<ContentSegment>? GetSegments(TextBlock element) =>
            element.GetValue(SegmentsProperty);

        static ContentInlines()
        {
            SegmentsProperty.Changed.AddClassHandler<TextBlock>(OnSegmentsChanged);
        }

        private static void OnSegmentsChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged oldIncc)
            {
                var oldHandler = textBlock.GetValue(HandlerProperty);
                if (oldHandler is not null)
                    oldIncc.CollectionChanged -= oldHandler;
                textBlock.SetValue(HandlerProperty, null);
            }

            var newSegments = e.NewValue as IEnumerable<ContentSegment>;
            Rebuild(textBlock, newSegments);

            if (e.NewValue is INotifyCollectionChanged newIncc)
            {
                NotifyCollectionChangedEventHandler handler = (_, _) => Rebuild(textBlock, newSegments);
                newIncc.CollectionChanged += handler;
                textBlock.SetValue(HandlerProperty, handler);
            }
        }

        private static void Rebuild(TextBlock textBlock, IEnumerable<ContentSegment>? segments)
        {
            textBlock.Inlines ??= new InlineCollection();
            textBlock.Inlines.Clear();

            if (segments is null)
                return;

            foreach (var segment in segments)
            {
                switch (segment)
                {
                    case TextSegment text:
                        AppendTextWithLineBreaks(textBlock.Inlines, text.Text);
                        break;

                    case MentionSegment mention:
                        textBlock.Inlines.Add(new InlineUIContainer(
                            new HyperlinkButton
                            {
                                Margin = new Thickness(0),
                                Padding = new Thickness(0),
                                Content = mention.DisplayName,
                                NavigateUri = TryCreateUri(mention.NProfile)
                            }));
                        break;

                    case LinkSegment link:
                        textBlock.Inlines.Add(new InlineUIContainer(
                            new HyperlinkButton
                            {
                                Margin = new Thickness(0),
                                Padding = new Thickness(0),
                                Content = link.Url,
                                NavigateUri = TryCreateUri(link.Url)
                                
                            }));
                        break;

                    case ImageSegment image:
                        textBlock.Inlines.Add(new InlineUIContainer(
                            new ContentControl
                            {
                                Content = image.ImageVM
                            }));
                        break;
                }
            }
        }

        private static void AppendTextWithLineBreaks(InlineCollection inlines, string? text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 0)
                    inlines.Add(new Run(lines[i]));

                if (i < lines.Length - 1)
                    inlines.Add(new LineBreak());
            }
        }

        private static Uri? TryCreateUri(string? value) =>
            Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;
    }
}