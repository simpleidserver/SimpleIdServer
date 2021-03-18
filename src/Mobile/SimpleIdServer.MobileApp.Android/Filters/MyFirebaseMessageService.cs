using Android.App;
using Firebase.Messaging;
using SimpleIdServer.MobileApp.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.Droid.Filters
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessageService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            var title = GetRecord(message.Data, "title");
            var body = GetRecord(message.Data, "body");
            var clickAction = GetRecord(message.Data, "clickAction");
            var notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.ReceiveNotification(title, body, clickAction);
        }

        private static string GetRecord(IDictionary<string, string> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }

            return string.Empty;
        }
    }
}