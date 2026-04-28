using N0str.ViewModels;


namespace N0str.Services
{
    public interface INavigation
    {
        void NavigateTo(ViewModelBase viewModel);
        void NavigateBack();
        bool CanNavigateBack();

        event Action<ViewModelBase>? CurrentViewModelChanged;
        event Action<ViewModelBase>? ModalViewModelChanged;

        void OpenModal(ViewModelBase viewModel);
        void CloseModal();
    }
}
