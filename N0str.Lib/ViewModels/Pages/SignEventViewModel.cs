using N0str.Nostr;
using N0str.Services;
using NNostr.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace N0str.ViewModels.Pages
{
    public class SignEventViewModel : ViewModelBase
    {
        private readonly INavigation _navigationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IN0strClient _nostrClient;
        private string _privateKey = string.Empty;

        public SignEventViewModel(INavigation navigationService, IServiceProvider services, IN0strClient nostrClient)
        {
            _navigationService = navigationService;
            _serviceProvider  = services;
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

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }

        public void Initialize(NostrEvent unsignedEvent)
        {
            UnsignedEvent = unsignedEvent;
            PrivateKey = string.Empty; // clear any previous keys
        }

        private async Task SignAndPublish()
        {
            if (PrivateKey is null || UnsignedEvent is null)
            {
                return;
            }

            NostrEvent signedEvent = await _nostrClient.SignEvent(PrivateKey, UnsignedEvent);
            await _nostrClient.PublishEventAsync(signedEvent);
        } 
    }
}
