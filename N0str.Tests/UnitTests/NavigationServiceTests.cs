using N0str.Services;
using N0str.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Tests.UnitTests
{
    public class NavigationServiceTests
    {
        [Fact]
        public void Can_Navigate_Back_Test()
        {
            var navigationService = new NavigationService();


            Assert.False(navigationService.CanNavigateBack());

            var firstVM = new ViewModelBase();
            var secondVM = new DisposableViewModel();
            var thirdVM = new ViewModelBase();

            navigationService.NavigateTo(firstVM);

            Assert.False(navigationService.CanNavigateBack());

            navigationService.NavigateTo(secondVM);
            Assert.True(navigationService.CanNavigateBack());

            navigationService.NavigateTo(thirdVM);
            Assert.True(navigationService.CanNavigateBack());
        }

        [Fact]
        public void VM_Disposed_On_Navigate_Back()
        {
            var navigationService = new NavigationService();

            ViewModelBase? current = null;

            navigationService.CurrentViewModelChanged += vm =>
                current = vm;

            var firstVM = new ViewModelBase();
            navigationService.NavigateTo(firstVM);

            var secondVM = new DisposableViewModel();
            navigationService.NavigateTo(secondVM);

            Assert.False(secondVM.IsDisposed);

            navigationService.NavigateBack();

            Assert.True(secondVM.IsDisposed);
            Assert.Equal(firstVM, current);
        }

    }

    public class DisposableViewModel : ViewModelBase, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
