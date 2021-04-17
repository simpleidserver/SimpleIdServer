using SimpleIdServer.MobileApp.Resources;
using System.Threading.Tasks;

namespace SimpleIdServer.MobileApp.Services
{
    public class MessageService : IMessageService
    {
        public async Task Show(string title, string message)
        {
            await App.Current.MainPage.DisplayAlert(title, message, Global.Ok);
        }

        public async Task<bool> Show(string title, string message, string accept, string cancel)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
    }
}
