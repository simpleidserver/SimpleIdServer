using Newtonsoft.Json.Linq;
using SimpleIdServer.MobileApp.Factories;
using SimpleIdServer.MobileApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.ViewModels
{
    public class ConsentPageViewModel : BaseViewModel
    {
        public ConsentPageViewModel()
        {
            Permissions = new ObservableCollection<PermissionViewModel>();
            RejectCommand = new Command(async() => await HandleReject(), IsRejectEnabled);
            ConfirmCommand = new Command(HandleConfirm, IsConfirmEnabled);
        }

        public string AuthReqId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public bool IsAnimated { get; set; }
        public ObservableCollection<PermissionViewModel> Permissions { get; set; }
        public event EventHandler IsRejected;
        public event EventHandler IsConfirmed;
        public ICommand RejectCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }

        private async Task HandleReject()
        {
            using (var client = HttpClientFactory.Build())
            {
                var tokenStorage = DependencyService.Get<ITokenStorage>();
                var authInfo = await tokenStorage.GetAuthInfo();
                var jObj = new JObject
                {
                    { "auth_req_id", AuthReqId },
                    { "id_token_hint", authInfo.IdToken }
                };
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(Constants.RejectAuthReqId),
                    Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
                };
                await client.SendAsync(request);
                if (IsRejected != null)
                {
                    IsRejected(this, EventArgs.Empty);
                }
            }
        }

        private bool IsRejectEnabled()
        {
            return true;
        }

        private void HandleConfirm()
        {
            if (IsConfirmed != null)
            {
                IsConfirmed(this, EventArgs.Empty);
            }
        }

        private bool IsConfirmEnabled()
        {
            return true;
        }
    }
}
