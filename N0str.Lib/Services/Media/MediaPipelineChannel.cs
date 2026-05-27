using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace N0str.Services.Media
{
    public class MediaPipelineChannel
    {
        public Channel<string> MediaChannel { get; }
        public MediaPipelineChannel()
        {
            MediaChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public async Task EnqueueLink(string url)
        {
            await MediaChannel.Writer.WriteAsync(url);
        }

        public async IAsyncEnumerable<string> ReadAllAsync([EnumeratorCancellation]CancellationToken ct)
        {
            await foreach (var request in MediaChannel.Reader.ReadAllAsync(ct)) 
            {
                yield return request;
            }
        }
    }
}
