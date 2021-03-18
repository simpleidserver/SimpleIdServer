using SimpleIdServer.MobileApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleIdServer.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private static HomePageViewModel _viewModel;
        private bool _notificationClicked = false;

        public HomePage()
        {
            InitializeComponent();
            _viewModel = new HomePageViewModel();
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
                await Browser.OpenAsync(notification.ClickAction);
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
    }
}