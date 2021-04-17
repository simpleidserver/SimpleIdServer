using SimpleIdServer.MobileApp.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterDependencies();
            Init().Wait();
        }

        private async Task Init()
        {
            var tokenStorage = DependencyService.Get<ITokenStorage>();
            var jwtSecurityToken = await tokenStorage.GetToken();
            if (jwtSecurityToken != null)
            {
                await Shell.Current.GoToAsync("//notifications");
            }
        }

        private void RegisterDependencies()
        {
            DependencyService.Register<ITokenStorage, TokenStorage>();
            DependencyService.Register<IMessageService, MessageService>();
        }
    }
}
