using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using N0str.Nostr;
using N0str.Services;
using NNostr.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace N0str.ViewModels.Pages
{
    public class CreateEventViewModel : ViewModelBase
    {
        private string _content = string.Empty;
        private string _kindText = string.Empty;
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
                OnPropertyChanged(nameof(IsValid));
            } 
        }

        public string KindText
        {
            get => _kindText;
            set
            {
                SetProperty(ref _kindText, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }
        public bool IsValid => !string.IsNullOrWhiteSpace(Content)
                    && int.TryParse(KindText, out var k)
                    && k >= 0;

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

            var canPublish = this.WhenPropertyChanged(x => x.IsValid)
                     .Select(x => x.Value);

            SignAndPublishCommand = ReactiveCommand.CreateFromTask(async () =>
            {

                NostrEvent unsignedNostrEvent = await _n0strClient.CreateNostrEvent(
                    Content,
                    Kind,
                    Tags.Select(t => (TagIdentifier: t.Identifier, Data: new[] { t.Data })).ToList());

                var signModal = _serviceProvider.GetRequiredService<SignEventViewModel>();
                signModal.Initialize(unsignedNostrEvent);
                _navigationService.OpenModal(signModal);

            }, canPublish);

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
