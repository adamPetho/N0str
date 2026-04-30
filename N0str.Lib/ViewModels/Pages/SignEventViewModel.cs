using Microsoft.Extensions.DependencyInjection;
using N0str.Nostr;
using N0str.Services;
using NNostr.Client;
using ReactiveUI;
using System.Reactive;

namespace N0str.ViewModels.Pages
{
    public class SignEventViewModel : ViewModelBase
    {
        private readonly INavigation _navigationService;
        private readonly IServiceProvider _services;
        private readonly IN0strClient _nostrClient;
        private string _privateKey = string.Empty;

        private string? _errorMessage;

        public SignEventViewModel(INavigation navigationService, IServiceProvider serviceProvider ,IN0strClient nostrClient)
        {
            _navigationService = navigationService;
            _services = serviceProvider;
            _nostrClient = nostrClient;

            BackCommand = ReactiveCommand.Create(() =>_navigationService.CloseModal());

            var canConfirm = this.WhenAnyValue(
                x => x.PrivateKey,
                key => !string.IsNullOrWhiteSpace(key));

            ConfirmCommand = ReactiveCommand.CreateFromTask(SignAndPublish, canConfirm);
        }

        public NostrEvent? UnsignedEvent { get; private set; }

        public string PrivateKey
        {
            get => _privateKey;
            set => SetProperty(ref _privateKey, value);
        }
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
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }

        public void Initialize(NostrEvent unsignedEvent)
        {
            UnsignedEvent = unsignedEvent;
            PrivateKey = string.Empty;
            ErrorMessage = null;
        }

        private async Task SignAndPublish()
        {
            /*
            if (PrivateKey is null || UnsignedEvent is null)
            {
                return;
            }
            
            NostrEvent? signedEvent = default;
            try
            {
                signedEvent = await _nostrClient.SignEvent(PrivateKey, UnsignedEvent);
            }
            catch (Exception) 
            {
                ErrorMessage = "Invalid private key.";
                return;
            }

            try
            {
                await _nostrClient.PublishEventAsync(signedEvent);
            }
            catch (Exception)
            {
                ErrorMessage = "Couldn't broadcast the event.";
                return;
            }*/

            var successVm = _services.GetRequiredService<SuccessfulBroadcastViewModel>();
            _navigationService.OpenModal(successVm);
        } 
    }
}
