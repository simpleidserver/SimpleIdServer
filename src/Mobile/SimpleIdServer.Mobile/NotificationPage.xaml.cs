using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Plugin.Firebase.CloudMessaging;
using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class NotificationPage : ContentPage
{
    private readonly NotificationViewModel _viewModel;

    public NotificationPage(NotificationViewModel viewModel)
    {
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    public void Display(FCMNotification fcmNotification)
    {
        _viewModel.Display(fcmNotification);
    }
}