using SimpleIdServer.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleIdServer.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsentPage : ContentPage
    {
        private readonly ConsentPageViewModel _consentPageViewModel;

        public ConsentPage()
        {
            InitializeComponent();
            _consentPageViewModel = new ConsentPageViewModel();
            BindingContext = _consentPageViewModel;
        }

        public ConsentPage(ConsentPageViewModel viewModel)
        {
            InitializeComponent();
            _consentPageViewModel = viewModel;
            BindingContext = viewModel;
        }

        public ConsentPageViewModel ConsentPageViewModel => _consentPageViewModel;
    }
}