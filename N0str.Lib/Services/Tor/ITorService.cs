using OnionSharp.Tor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Tor
{
    public interface ITorService
    {
        public TorSettings? TorSettings { get; }
        Task InitializeAsync(CancellationToken ct = default);
        HttpClient CreateHttpClient(string name = "N0str");
    }
}
