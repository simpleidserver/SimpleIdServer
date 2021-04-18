using SimpleIdServer.MobileApp.Resources;
using SimpleIdServer.MobileApp.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private readonly ILoginProvider _loginProvider;
        private readonly ITokenStorage _tokenStorage;
        private readonly IMessageService _messageService;

        public HomePageViewModel()
        {
            var loginProvider = DependencyService.Get<ILoginProvider>();
            var tokenStorage = DependencyService.Get<ITokenStorage>();
            var messageService = DependencyService.Get<IMessageService>();
            _loginProvider = loginProvider;
            _tokenStorage = tokenStorage;
            _messageService = messageService;
            LoginCommand = new Command(async() => await ExecuteLogin(), IsLoginEnabled);
        }

        public ICommand LoginCommand { get; private set; }

        private async Task ExecuteLogin()
        {
            var authInfo = await _loginProvider.LoginAsync();
            if (string.IsNullOrWhiteSpace(authInfo.AccessToken) || !authInfo.IsAuthorized)
            {
                await _messageService.Show(Global.Error, Global.AppCannotAuthenticate);
            }
            else
            {
                await _tokenStorage.Store(authInfo);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("//notifications");
                });
            }
        }

        private bool IsLoginEnabled()
        {
            return true;
        }
    }
}
