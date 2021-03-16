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

        private async void HandleNotification(object sender, System.EventArgs e)
        {
            var tappedEvtArgs = e as TappedEventArgs;
            var frame = sender as Frame;
            var scale = frame.Scale;
            await frame.ScaleTo(scale * 1.02, 200);
            await frame.ScaleTo(scale, 200);
            // Redirect the user to ...
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

        private void HandleAddNotification(object sender, System.EventArgs e)
        {
            _viewModel.AddNotification(new Models.Notification
            {
                ClickAction = "clickAction",
                Description = "description",
                Title = "title"
            });
        }
    }
}