using N0str.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Media
{
    public interface IMediaPipelineService
    {
        Task EnqueueURL(MediaRequest url);

        Task ReadChannelLoop(CancellationToken ct);
    }
}
