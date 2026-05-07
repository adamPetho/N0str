using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.ViewModels.Pages
{
    public class LoadingViewModel : ViewModelBase
    {
        private string _statusMessage = string.Empty;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }
}
