using N0str.ViewModels;
using N0str.ViewModels.Pages;

namespace N0str.Services
{
    public class NavigationService : INavigation
    {
        private readonly Stack<ViewModelBase> _navigationStack = new();
        public event Action<ViewModelBase>? CurrentViewModelChanged;
        public event Action<ViewModelBase?>? ModalViewModelChanged;

        public void NavigateTo(ViewModelBase viewModel)
        {
            _navigationStack.Push(viewModel);
            CurrentViewModelChanged?.Invoke(viewModel);
        }

        public void NavigateBack()
        {
            if (_navigationStack.Count > 1)
            {
                var popped = _navigationStack.Pop();
                if (popped is IDisposable disposable)
                    disposable.Dispose();

                var previousViewModel = _navigationStack.Peek();
                CurrentViewModelChanged?.Invoke(previousViewModel);
            }
        }

        public bool CanNavigateBack()
        {
            return _navigationStack.Count > 1;
        }

        public void OpenModal(ViewModelBase viewModel)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                ModalViewModelChanged?.Invoke(viewModel));
        }

        public void CloseModal()
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                ModalViewModelChanged?.Invoke(null));
        }
    }
}
