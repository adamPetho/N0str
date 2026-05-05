using Microsoft.Extensions.DependencyInjection;
using N0str.Services;
using ReactiveUI;
using System.Windows.Input;


namespace N0str.ViewModels.Pages
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly INavigation _navigationService;
        private readonly IServiceProvider _services;

        public MenuViewModel(INavigation navigation, IServiceProvider services)
        {
            _navigationService = navigation;
            _services = services;
            GoToPublishCommand = ReactiveCommand.Create(NavigateToPublish);
            GoToFeedCommand = ReactiveCommand.Create(NavigateToFeed);
        }


        public ICommand GoToPublishCommand { get; }
        public ICommand GoToFeedCommand { get; }

        private void NavigateToPublish()
        {
            var vm = _services.GetRequiredService<CreateEventViewModel>();
            _navigationService.OpenModal(vm);
        }

        private void NavigateToFeed()
        {
            var vm = _services.GetRequiredService<PubKeyToFetchViewModel>();
            _navigationService.OpenModal(vm);
        }
    }
}
