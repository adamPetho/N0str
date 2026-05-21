namespace N0str.Services.Media
{
    public interface IMediaService
    {
        Task<byte[]> FetchImageBytesFromUrl(string url);
    }
}