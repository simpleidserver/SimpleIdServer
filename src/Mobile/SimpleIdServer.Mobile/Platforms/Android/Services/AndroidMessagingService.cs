using Android.App;
using Firebase.Messaging;
using Plugin.Firebase.CloudMessaging.Platforms.Android.Extensions;
using Plugin.Firebase.CloudMessaging;

namespace SimpleIdServer.Mobile.Platforms.Android.Services;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class AndroidMessagingService : FirebaseMessagingService
{
    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        CrossFirebaseCloudMessaging.Current.OnNotificationReceived(message.ToFCMNotification());
    }

    public override async void OnNewToken(string token)
    {
        await CrossFirebaseCloudMessaging.Current.OnTokenRefreshAsync();
    }
}
