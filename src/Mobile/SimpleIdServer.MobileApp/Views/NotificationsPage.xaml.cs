using Newtonsoft.Json.Linq;
using SimpleIdServer.MobileApp.Factories;
using SimpleIdServer.MobileApp.Resources;
using SimpleIdServer.MobileApp.Services;
using SimpleIdServer.MobileApp.ViewModels;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleIdServer.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPage : ContentPage
    {
        private static NotificationsPageViewModel _viewModel;
        private bool _notificationClicked = false;
        private readonly ITokenStorage _tokenStorage;
        private readonly IMessageService _messageService;

        public NotificationsPage()
        {
            InitializeComponent();
            _viewModel = new NotificationsPageViewModel();
            _tokenStorage = DependencyService.Get<ITokenStorage>();
            _messageService = DependencyService.Get<IMessageService>();
            BindingContext = _viewModel;
        }

        private async void HandleNotification(object sender, System.EventArgs e)
        {
            if (!_notificationClicked)
            {
                _notificationClicked = true;
                var tappedEvtArgs = e as TappedEventArgs;
                var frame = sender as Frame;
                var scale = frame.Scale;
                await frame.ScaleTo(scale * 1.02, 200);
                await frame.ScaleTo(scale, 200);
                var notification = tappedEvtArgs.Parameter as Models.Notification;
                if (await _messageService.Show(Global.Confirmation, Global.ConfirmAuthReqId, Global.Confirm, Global.Reject))
                {
                    var authInfo = await _tokenStorage.GetAuthInfo();
                    await ConfirmAuthReqId(authInfo.IdToken, notification.AuthReqId);
                }

                _viewModel.RemoveNotification(notification);
                _notificationClicked = false;
            }
        }

        private async void HandleViewCellAppearing(object sender, System.EventArgs e)
        {
            var viewCell = sender as ViewCell;
            var frame = viewCell.FindByName<Frame>("frame");
            var notification = frame.BindingContext as Models.Notification;
            if (!notification.IsAnimated)
            {
                frame.Opacity = 0;
                await frame.FadeTo(1, 400, Easing.SinOut);
                notification.IsAnimated = true;
            }
        }

        private async Task ConfirmAuthReqId(string idToken, string authReqId)
        {
            using (var client = HttpClientFactory.Build())
            {
                var jObj = new JObject
                {
                    { "id_token_hint", idToken },
                    { "auth_req_id", authReqId }
                };
                var httpRequestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(Constants.ConfirmAuthReqId),
                    Method = HttpMethod.Post,
                    Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
                };
                await client.SendAsync(httpRequestMessage);
            }
        }
    }
}