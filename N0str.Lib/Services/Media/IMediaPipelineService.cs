using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Media
{
    public interface IMediaPipelineService
    {
        Task EnqueueURL(string url);

        Task ReadChannelLoop(CancellationToken ct);
    }
}
