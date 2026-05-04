using Microsoft.Extensions.DependencyInjection;
using N0str.Nostr;
using N0str.Services;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;

namespace N0str.ViewModels.Pages
{
    public class CreateEventViewModel : ViewModelBase
    {
        private readonly IObservable<bool> _canPublish;

        private string _content = string.Empty;
        private string _kindText = "1";
        private bool _tagsExpanded = false;

        private readonly INavigation _navigationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IN0strClient _n0strClient;

        public string Content
        {
            get => _content;
            set 
            {
                SetProperty(ref _content, value);
            } 
        }

        public string KindText
        {
            get => _kindText;
            set
            {
                // only allow digits
                if (value.All(char.IsDigit) || string.IsNullOrEmpty(value))
                {
                    SetProperty(ref _kindText, value);
                }
            }
        }

        public int Kind => int.TryParse(_kindText, out var k) ? k : 1;

        public bool TagsExpanded
        {
            get => _tagsExpanded;
            set => SetProperty(ref _tagsExpanded, value);
        }

        public ObservableCollection<TagEntryViewModel> Tags { get; } = new();
        public ReactiveCommand<Unit, Unit> ToggleTagsCommand { get; }
        public ReactiveCommand<Unit, Unit> AddTagCommand { get; }
        public ReactiveCommand<TagEntryViewModel, Unit> RemoveTagCommand { get; }
        public ReactiveCommand<Unit, Unit> SignAndPublishCommand { get; }
        public ICommand NavigateBack {  get; }

        public CreateEventViewModel(INavigation navigationService, IServiceProvider serviceProvider, IN0strClient noStrClient)
        {
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
            _n0strClient = noStrClient;

            NavigateBack = ReactiveCommand.Create(_navigationService.CloseModal);

            ToggleTagsCommand = ReactiveCommand.Create(() =>
            {
                TagsExpanded = !TagsExpanded;
            });

            AddTagCommand = ReactiveCommand.Create(() =>
            {
                Tags.Add(new TagEntryViewModel());
            });

            RemoveTagCommand = ReactiveCommand.Create<TagEntryViewModel>(tag =>
            {
                Tags.Remove(tag);
            });

            _canPublish = this.WhenAnyValue(
                x => x.Content,
                x => x.KindText,
                (content, kind) => !string.IsNullOrWhiteSpace(content) && int.TryParse(kind, out var k) && k >= 0);

            SignAndPublishCommand = ReactiveCommand.CreateFromTask(async () =>
            {

                NostrEvent unsignedNostrEvent = await _n0strClient.CreateNostrEvent(
                    Content,
                    Kind,
                    Tags.Select(t => (TagIdentifier: t.Identifier, Data: new[] { t.Data })).ToList());

                var signModal = _serviceProvider.GetRequiredService<SignEventViewModel>();
                signModal.Initialize(unsignedNostrEvent);
                _navigationService.OpenModal(signModal);

            }, _canPublish);

        }
    }

    public class TagEntryViewModel : ReactiveObject
    {
        private string _identifier = string.Empty;
        private string _data = string.Empty;

        public string Identifier
        {
            get => _identifier;
            set => this.RaiseAndSetIfChanged(ref _identifier, value);
        }

        public string Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }
    }

}
