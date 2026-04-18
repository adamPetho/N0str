using Microsoft.Extensions.DependencyInjection;
using N0str.ViewModels;

namespace N0str.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddTransient<MainViewModel>();
        }
    }
}
