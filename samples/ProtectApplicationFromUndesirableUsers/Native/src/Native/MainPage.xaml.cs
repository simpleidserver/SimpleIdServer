using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Native
{
    public partial class MainPage : ContentPage
    {
        private readonly ILoginProvider _loginProvider;

        public MainPage()
        {
            InitializeComponent();
            _loginProvider = DependencyService.Resolve<ILoginProvider>();
        }

        private void Authenticate(object sender, EventArgs e)
        {
            _loginProvider.LoginAsync().ContinueWith(async(authInfo) =>
            {
                if (!string.IsNullOrWhiteSpace(authInfo.Result.IdToken))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage.DisplayAlert("Auth", $"IdToken is : {authInfo.Result.IdToken}", "Cancel");
                    });
                }
            });
        }
    }
}
