using N0str.Services;

namespace N0str.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel()
        {
            _navigationService = new NavigationService();
            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            
            // TODO: Start with MainMenu
            // _navigationService.NavigateTo();
        }

        private void OnCurrentViewModelChanged(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
