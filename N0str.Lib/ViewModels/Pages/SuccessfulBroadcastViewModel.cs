using N0str.Services;

namespace N0str.ViewModels.Pages
{
    public class SuccessfulBroadcastViewModel : ViewModelBase
    {
        private readonly INavigation _navigationService;

        public SuccessfulBroadcastViewModel(INavigation navigationService)
        {
            _navigationService = navigationService;
        }

        public async void StartAutoClose()
        {
            await Task.Delay(4000);
            _navigationService.CloseModal();
        }
    }
}
