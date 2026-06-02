using N0str.Models;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace N0str.Services.Media
{
    public class MediaPipelineChannel
    {
        public Channel<MediaRequest> MediaChannel { get; }
        public MediaPipelineChannel()
        {
            MediaChannel = Channel.CreateUnbounded<MediaRequest>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public async Task EnqueueLink(MediaRequest request)
        {
            await MediaChannel.Writer.WriteAsync(request);
        }

        public async IAsyncEnumerable<MediaRequest> ReadAllAsync([EnumeratorCancellation]CancellationToken ct)
        {
            await foreach (var request in MediaChannel.Reader.ReadAllAsync(ct)) 
            {
                yield return request;
            }
        }
    }
}
