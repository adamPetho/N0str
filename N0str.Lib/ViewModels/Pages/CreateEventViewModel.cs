using N0str.Services;
using N0str.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace N0str.ViewModels.Pages
{
    public class CreateEventViewModel : ViewModelBase
    {
        private string _content = string.Empty;
        private int? _kind = 1;
        private bool _tagsExpanded = false;

        private readonly INavigation _navigationService;

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public int? Kind
        {
            get => _kind;
            set => SetProperty(ref _kind, value ?? 1);
        }

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

        public CreateEventViewModel(INavigation navigationService)
        {
            _navigationService = navigationService;

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

            var canPublish = this.WhenAnyValue(
                x => x.Content,
                x => x.Kind,
                (content, kind) => !string.IsNullOrWhiteSpace(content) && kind.HasValue && kind >= 0
            );

            SignAndPublishCommand = ReactiveCommand.Create(() =>
            {
                // Snapshot ready — handle publishing from outside or extend here
                var snapshot = new
                {
                    Content,
                    Kind,
                    Tags = Tags.Select(t => (t.Identifier, t.Data)).ToList()
                };

                //_navigationService.NavigateTo();
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
