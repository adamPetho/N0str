using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using N0str.ViewModels.Pages;

namespace N0str.Views.Pages;

public partial class SuccessfulBroadcastView : UserControl
{
    public SuccessfulBroadcastView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is SuccessfulBroadcastViewModel vm)
                vm.StartAutoClose();
        };
    }
}