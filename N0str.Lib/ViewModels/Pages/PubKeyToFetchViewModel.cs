using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using N0str.Nostr;
using N0str.Services;
using NBitcoin;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace N0str.ViewModels.Pages
{
    public class PubKeyToFetchViewModel : ViewModelBase
    {
        private string _pubKey = string.Empty;
        private string? _errorMessage;
        private readonly INavigation _navigationService;
        private readonly IServiceProvider _serviceProvider;

        public PubKeyToFetchViewModel(INavigation navigationService, IServiceProvider serviceProvider)
        {
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;

            BackCommand = ReactiveCommand.Create(() => _navigationService.CloseModal());

            var canConfirm = this.WhenAnyValue(
                x => x.PublicKey,
                key => !string.IsNullOrWhiteSpace(key));

            NavigateToFeedCommand = ReactiveCommand.CreateFromTask(NavigateToFeed, canConfirm);
        }

        public string PublicKey
        {
            get => _pubKey;
            set
            {
                SetProperty(ref _pubKey, value);
            }
        }

        //TODO: Check if string is actually npub or not and display error
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> NavigateToFeedCommand { get; }

        private async Task NavigateToFeed()
        {
            var requestedPubKey = PublicKey;

            var feedVM = _serviceProvider.GetRequiredService<FeedViewModel>();

            await feedVM.InitializeAsync(requestedPubKey);

            _navigationService.NavigateTo(feedVM);
        }

    }
}
