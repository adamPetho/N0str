using N0str.Models;

namespace N0str.Services.Media
{
    public class MediaPipelineService : IMediaPipelineService
    {
        private readonly MediaPipelineChannel _mediaChannel;
        private readonly IMediaService _mediaFetcherService;
        private readonly CancellationTokenSource _cts = new();

        private readonly Task _readerThread;

        public MediaPipelineService(MediaPipelineChannel mediaChannel, IMediaService mediaService)
        {
            _mediaChannel = mediaChannel;
            _mediaFetcherService = mediaService;

            _readerThread = Task.Run(async () => ReadChannelLoop(_cts.Token));
        }

        public async Task EnqueueURL(MediaRequest request)
        {
            await _mediaChannel.EnqueueLink(request);
        }

        public async Task ReadChannelLoop(CancellationToken ct)
        {
            await foreach (var request in _mediaChannel.ReadAllAsync(ct))
            {
                try
                {
                    var imageBytes = await _mediaFetcherService.FetchImageBytesFromUrl(request.RequestURL);
                    // decode bitmap
                    // update VM, raise Event?
                }
                catch (Exception ex)
                {
                    //TODO: Better Logging
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
