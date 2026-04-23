using N0str.Services;
using N0str.ViewModels.Pages;

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
            
            _navigationService.NavigateTo(new MenuViewModel());
        }

        private void OnCurrentViewModelChanged(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
