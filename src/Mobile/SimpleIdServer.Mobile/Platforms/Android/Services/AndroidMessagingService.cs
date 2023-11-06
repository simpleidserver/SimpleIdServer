using Android.App;
using Firebase.Messaging;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.Platforms.Android.Extensions;

namespace SimpleIdServer.Mobile.Platforms.Android.Services;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class AndroidMessagingService : FirebaseMessagingService
{
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
        await App.Current.Dispatcher.DispatchAsync(async () =>
        {
            var notificationPage = await App.NavigationService.DisplayModal<NotificationPage>();
            notificationPage.Display(fcmNotification);
        });
    }
}