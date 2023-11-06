using Android.App;
using Firebase.Messaging;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.Platforms.Android.Extensions;
using SimpleIdServer.Mobile.Services;

namespace SimpleIdServer.Mobile.Platforms.Android.Services;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class AndroidMessagingService : FirebaseMessagingService
{
    private readonly INavigationService _navigationService;

    public AndroidMessagingService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        HandleFCMNotification(message.ToFCMNotification());
    }

    public override async void OnNewToken(string token)
    {
        await CrossFirebaseCloudMessaging.Current.OnTokenRefreshAsync();
    }

    private async void HandleFCMNotification(FCMNotification fcmNotification)
    {
        var notificationPage = await _navigationService.DisplayModal<NotificationPage>();
        fcmNotification.Display(fcmNotification);
    }
}