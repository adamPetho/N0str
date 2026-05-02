using System.Net;

namespace N0str.Services.Tor
{
    public interface ITorService
    {
        Task InitializeAsync(CancellationToken ct = default);
        EndPoint GetSocksEndpoint();
    }
}
