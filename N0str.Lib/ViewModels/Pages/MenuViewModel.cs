using Metsys.Bson;
using N0str.Services;
using NBitcoin;
using NBitcoin.RPC;
using ReactiveUI;
using System.Windows.Input;


namespace N0str.ViewModels.Pages
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly INavigation _navigationService;

        public MenuViewModel(INavigation navigation)
        {
            _navigationService = navigation;
            GoToPublishCommand = ReactiveCommand.Create(NavigateToPublish);
            GoToFeedCommand = ReactiveCommand.Create(NavigateToFeed);
        }


        public ICommand GoToPublishCommand { get; }
        public ICommand GoToFeedCommand { get; }

        private void NavigateToPublish()
        {
            _navigationService.OpenModal(new CreateEventViewModel(_navigationService));
        }

        private void NavigateToFeed()
        {
            _navigationService.NavigateTo(new FetchPostsViewModel());
        }
    }
}
