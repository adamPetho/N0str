using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using N0str.Helpers;
using N0str.Logging;
using N0str.Nostr;
using N0str.Services;
using N0str.Services.Relay;
using N0str.Services.Tor;
using N0str.ViewModels;
using N0str.ViewModels.Pages;
using N0str.Views.Pages;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace N0str.Views
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                InitializeAsync(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async void InitializeAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                var collection = new ServiceCollection();
                collection.AddCommonServices();

                var services = collection.BuildServiceProvider();

                var loadingVm = new LoadingViewModel();
                var loadingWindow = new LoadingWindow
                {
                    DataContext = loadingVm
                };

                desktop.MainWindow = loadingWindow;

                loadingWindow.Show();

                string dataDir = EnvironmentHelpers.GetDataDir(Path.Combine("N0str", "Client"));
                Logger.Initialize(dataDir);

                loadingVm.StatusMessage = "Starting Tor...";
                Logger.LogInfo("Starting Tor.");

                var torService = services.GetRequiredService<ITorService>();
                await torService.InitializeAsync();

                loadingVm.StatusMessage = "Connecting to relays...";
                Logger.LogInfo("Connecting to relays.");

                var relayService = services.GetRequiredService<IRelayService>();
                await relayService.ConnectAsync(DefaultRelayURLs.URLs);

                loadingVm.StatusMessage = "Launching UI...";

                var mainVM = services.GetRequiredService<MainViewModel>();
                var mainWindow = new MainWindow
                {
                    DataContext = mainVM
                };

                desktop.MainWindow = mainWindow;

                mainWindow.Show();
                loadingWindow.Close();
            }
            catch (WebSocketException ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Could not connect to Nostr relays.";

                Logger.LogCritical($"Could not connect to Nostr relays. {ex}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw;
            }
            catch (SocketException ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Network connection failed.";

                Logger.LogCritical($"Network error: {ex}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw;
            }
            catch (Win32Exception ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Tor executable could not be started.";

                Logger.LogCritical($"Tor executable could not be started. {ex}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw;
            }
            catch (TimeoutException ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Tor startup timed out.";

                Logger.LogCritical($"Tor startup timed out. {ex}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw;
            }
            catch (OperationCanceledException ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Startup cancelled.";

                Logger.LogCritical($"Startup Cancelled. {ex}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw;
            }
            catch (Exception ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = $"Unexpected error: {ex}";

                Logger.LogCritical($"Unexpected error: {ex}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw;
            }
        }
    }
}