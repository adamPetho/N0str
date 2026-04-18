using N0str.ViewModels;

namespace N0str.Services
{
    public class NavigationService
    {
        private readonly Stack<ViewModelBase> _navigationStack = new();
        public event Action<ViewModelBase>? CurrentViewModelChanged;

        public void NavigateTo(ViewModelBase viewModel)
        {
            _navigationStack.Push(viewModel);
            CurrentViewModelChanged?.Invoke(viewModel);
        }

        public void NavigateBack()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.Pop();
                var previousViewModel = _navigationStack.Peek();
                CurrentViewModelChanged?.Invoke(previousViewModel);
            }
        }

        public bool CanNavigateBack => _navigationStack.Count > 1;
    }
}
