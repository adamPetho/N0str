using N0str.Services.Tor.Settings;
using OnionSharp.Microservices;
using OnionSharp.Tor;
using System.Collections.Concurrent;

namespace N0str.Services.Media
{
    public class MediaService : IMediaService
    {
        private readonly ITorSettings _torSettings;
        private readonly OnionHttpClientFactory _torHttpClientFactory;

        private ConcurrentDictionary<string, byte[]> _mediaDict = new();

        public MediaService(ITorSettings torSettings)
        {
            _torSettings = torSettings;
            _torHttpClientFactory = new OnionHttpClientFactory(_torSettings.GetSocksEndpoint().ToUri("socks5"));
        }

        public async Task<byte[]> FetchImageBytesFromUrl(string url)
        {
            if (_mediaDict.TryGetValue(url, out var bytes)) 
            {
                return bytes;
            }

            Uri uri = new Uri(url);
            byte[] imageBytes = await FetchImageBytesFromUrl(uri);

            _mediaDict.TryAdd(url, imageBytes);
            return imageBytes;
        }

        private async Task<byte[]> FetchImageBytesFromUrl(Uri url)
        {
            using var onionHttpClient = _torHttpClientFactory.CreateClient($"image:{url}");
            return await onionHttpClient.GetByteArrayAsync(url);
        }
    }
}
