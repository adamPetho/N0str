using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using N0str.Nostr;
using N0str.Services;
using N0str.Services.Relay;
using N0str.Services.Tor;
using N0str.ViewModels;
using N0str.ViewModels.Pages;
using N0str.Views.Pages;
using System.ComponentModel;
using System.Diagnostics;
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

                loadingVm.StatusMessage = "Starting Tor...";

                var torService = services.GetRequiredService<ITorService>();
                await torService.InitializeAsync();

                loadingVm.StatusMessage = "Connecting to relays...";

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
            catch (WebSocketException)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Could not connect to Nostr relays.";
            }
            catch (SocketException)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Network connection failed.";
            }
            catch (Win32Exception)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Tor executable could not be started.";
            }
            catch (TimeoutException)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Tor startup timed out.";
            }
            catch (OperationCanceledException)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = "Startup cancelled.";
            }
            catch (Exception ex)
            {
                if (desktop.MainWindow is LoadingWindow loading && loading.DataContext is LoadingViewModel vm)
                    vm.StatusMessage = $"Unexpected error: {ex}";
            }
        }
    }
}