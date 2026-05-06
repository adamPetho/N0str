using Microsoft.Extensions.DependencyInjection;
using N0str.Nostr;
using N0str.Services.Events;
using N0str.Services.Relay;
using N0str.Services.Tor;
using N0str.ViewModels;
using N0str.ViewModels.Pages;

namespace N0str.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<ITorService, TorService>();
            collection.AddSingleton<IRelayService, RelayService>();
            collection.AddSingleton<IEventService, EventService>();
            collection.AddSingleton<IN0strClient, N0strClient>();
            collection.AddSingleton<INavigation, NavigationService>();

            // ViewModels
            collection.AddTransient<MainViewModel>();
            collection.AddTransient<MenuViewModel>();
            collection.AddTransient<CreateEventViewModel>();
            collection.AddTransient<SignEventViewModel>();
            collection.AddTransient<SuccessfulBroadcastViewModel>();
            collection.AddTransient<PubKeyToFetchViewModel>();
            collection.AddTransient<FeedViewModel>();
        }
    }
}
