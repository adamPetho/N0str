using Avalonia.Controls;

namespace N0str.Views.Pages;

public partial class CreateEvent : UserControl
{
    public CreateEvent()
    {
        if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            throw new Exception("CreateEvent constructor called off UI thread!");

        InitializeComponent();
    }
}