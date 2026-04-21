using OnionSharp.Tor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Services.Tor
{
    public interface ITorService
    {
        Task InitializeAsync(CancellationToken ct = default);
        EndPoint GetSocksEndpoint();
    }
}
