
using SimpleIdServer.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleIdServer.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private static HomePageViewModel _viewModel;

        public HomePage()
        {
            InitializeComponent();
            _viewModel = new HomePageViewModel();
            BindingContext = _viewModel;
        }
    }
}