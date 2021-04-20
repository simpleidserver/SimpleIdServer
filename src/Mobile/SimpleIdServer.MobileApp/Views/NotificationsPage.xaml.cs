using Newtonsoft.Json.Linq;
using SimpleIdServer.MobileApp.Factories;
using SimpleIdServer.MobileApp.Services;
using SimpleIdServer.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private ConsentPage _consentPage;

        public NotificationsPage()
        {
            InitializeComponent();
            _viewModel = new NotificationsPageViewModel();
            _tokenStorage = DependencyService.Get<ITokenStorage>();
            BindingContext = _viewModel;
        }

        private async void HandleNotification(object sender, EventArgs e)
        {
            if (!_notificationClicked)
            {
                _notificationClicked = true;
                var tappedEvtArgs = e as TappedEventArgs;
                var frame = sender as Frame;
                var scale = frame.Scale;
                await frame.ScaleTo(scale * 1.02, 200);
                await frame.ScaleTo(scale, 200);
                var consentPageViewModel = tappedEvtArgs.Parameter as ConsentPageViewModel;
                consentPageViewModel.IsRejected += async(se, ev) => await RemoveConsent(se, ev);
                consentPageViewModel.IsConfirmed += async(se, ev) => await ConfirmConsent(se, ev);
                _consentPage = new ConsentPage(consentPageViewModel);
                await Navigation.PushModalAsync(_consentPage);
            }
        }

        private async Task ConfirmConsent(object sender, EventArgs e)
        {
            var consent = (ConsentPageViewModel)sender;
            var authInfo = await _tokenStorage.GetAuthInfo();
            await ConfirmAuthReqId(authInfo.IdToken, consent.AuthReqId, consent.Permissions.Where(p => p.IsSelected).Select(p => p.PermissionId));
            await RemoveConsent(sender, e);
        }

        private async Task RemoveConsent(object sender, EventArgs e)
        {
            _viewModel.RemoveConsent((ConsentPageViewModel)sender);
            _notificationClicked = false;
            await Navigation.PopModalAsync();
        }

        private async void HandleViewCellAppearing(object sender, EventArgs e)
        {
            var viewCell = sender as ViewCell;
            var frame = viewCell.FindByName<Frame>("frame");
            var notification = frame.BindingContext as ConsentPageViewModel;
            if (!notification.IsAnimated)
            {
                frame.Opacity = 0;
                await frame.FadeTo(1, 400, Easing.SinOut);
                notification.IsAnimated = true;
            }
        }

        private async Task ConfirmAuthReqId(string idToken, string authReqId, IEnumerable<string> permissionIds)
        {
            using (var client = HttpClientFactory.Build())
            {
                var jObj = new JObject
                {
                    { "id_token_hint", idToken },
                    { "auth_req_id", authReqId },
                    { "permission_ids", JArray.FromObject(permissionIds) }
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