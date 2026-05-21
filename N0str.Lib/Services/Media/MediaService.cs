using N0str.Services.Tor.Settings;
using OnionSharp.Microservices;
using OnionSharp.Tor;

namespace N0str.Services.Media
{
    public class MediaService : IMediaService
    {
        private readonly ITorSettings _torSettings;
        private readonly OnionHttpClientFactory _torHttpClientFactory;

        public MediaService(ITorSettings torSettings)
        {
            _torSettings = torSettings;
            _torHttpClientFactory = new OnionHttpClientFactory(_torSettings.GetSocksEndpoint().ToUri("socks5"));
        }

        public async Task<byte[]> FetchImageBytesFromUrl(string url)
        {
            using var onionHttpClient = _torHttpClientFactory.CreateClient($"image:{url}");
            return await onionHttpClient.GetByteArrayAsync(url);
        }
    }
}
