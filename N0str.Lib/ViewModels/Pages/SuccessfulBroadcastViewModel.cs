using N0str.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
