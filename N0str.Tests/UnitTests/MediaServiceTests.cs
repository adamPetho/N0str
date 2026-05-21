using N0str.Services.Media;
using N0str.Services.Tor.Settings;
using OnionSharp.Microservices;
using OnionSharp.Tor;

namespace N0str.Tests.UnitTests
{
    public class MediaServiceTests
    {
        [Theory]
        [InlineData("https://r2.primal.net/cache/8/74/2f/8742f1977fcc732a8297f119a0beb3f78171b899cc35b4d378dcc1dcaed1a832.jpg")]
        public async Task Fetch_IMG_URL_Test(string imageURL)
        {
            var customTorSettings = new CustomTorSettings();

            TorProcessManager torProcessManager = new TorProcessManager(customTorSettings.GetTorSettings());
            await torProcessManager.StartAsync(attempts: 3, CancellationToken.None);

            var mediaService = new MediaService(customTorSettings);

            var result = await mediaService.FetchImageBytesFromUrl(imageURL);

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            await torProcessManager.DisposeAsync();
        }
    }
}
