using N0str.Static;
using NNostr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Tests.UnitTests
{
    public class MediaExtractorTest
    {
        [Fact]
        public void Extracts_Multiple_Image_Urls()
        {
            var ev = new NostrEvent
            {
                Content = """
                  Hello, This is NostrEvent Test

                  https://site.com/a.jpg
                  https://site.com/b.png
                  http://youtube.com/test
                  https://learn.microsoft.com/en-us/dotnet/csharp/
                  """
            };

            var urls = MediaExtractor.ExtractImageUrls(ev);

            Assert.Equal(2, urls.Count);
        }
    }
}
