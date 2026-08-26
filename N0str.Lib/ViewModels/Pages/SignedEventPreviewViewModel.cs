using Avalonia.Input.Platform;
using Microsoft.Extensions.DependencyInjection;
using N0str.Nostr;
using N0str.Services;
using NBitcoin.Secp256k1;
using NNostr.Client;
using ReactiveUI;
using System.Reactive;

namespace N0str.ViewModels.Pages
{
    public class SignedEventPreviewViewModel : ViewModelBase
    {
        private readonly INavigation _navigationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IN0strClient _noStrClient;
        private readonly IClipboardService _clipboardService;
        private string? _nostrPrivKey;

        public SignedEventPreviewViewModel(INavigation navigationService, IServiceProvider serviceProvider, IN0strClient noStrClient, IClipboardService clipboardService)
        {
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
            _noStrClient = noStrClient;
            _clipboardService = clipboardService;

            NavigateBack = ReactiveCommand.Create(_navigationService.CloseModal);

            PublishCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (NostrEvent is null)
                {
                    throw new NullReferenceException("NostrEvent can't be null");
                }

                await _noStrClient.PublishEventAsync(NostrEvent);
                var successVm = _serviceProvider.GetRequiredService<SuccessfulBroadcastViewModel>();
                _navigationService.OpenModal(successVm);
            });

            CopyPrivKey = ReactiveCommand.CreateFromTask(async () =>
            {
                if (string.IsNullOrEmpty(NostrKey))
                    return;

                await _clipboardService.SetTextAsync(NostrKey);
            });
        }

        public void Initialize(NostrEvent signedEvent)
        {
            NostrEvent = signedEvent;
        }

        public void InitPrivKeys(ECPrivKey key)
        {
            NostrKey = key.ToHex();
        }

        public NostrEvent? NostrEvent { get; set; }
        public string? NostrKey
        {
            get => _nostrPrivKey;
            set => SetProperty(ref _nostrPrivKey, value);
        }
        public bool ShowPrivKey => NostrKey != null;

        public ReactiveCommand<Unit, Unit> PublishCommand { get; }
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }
        public ReactiveCommand<Unit, Unit> CopyPrivKey { get; }


    }
}
