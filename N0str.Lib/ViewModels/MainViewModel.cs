using N0str.Services;
using N0str.ViewModels.Pages;

namespace N0str.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private ViewModelBase _currentViewModel;
        private ViewModelBase? _modalViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ViewModelBase? ModalViewModel
        {
            get => _modalViewModel;
            set 
            {
                this.SetProperty(ref _modalViewModel, value);
                this.OnPropertyChanged(nameof(HasModal));
            } 
        }

        public bool HasModal => _modalViewModel != null;

        public MainViewModel()
        {
            _navigationService = new NavigationService();
            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationService.ModalViewModelChanged += OnCurrentModalViewChanged;

            _navigationService.NavigateTo(new MenuViewModel(_navigationService));
        }

        private void OnCurrentViewModelChanged(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private void OnCurrentModalViewChanged(ViewModelBase? viewModel) 
        { 
            ModalViewModel = viewModel;
        }
    }
}
