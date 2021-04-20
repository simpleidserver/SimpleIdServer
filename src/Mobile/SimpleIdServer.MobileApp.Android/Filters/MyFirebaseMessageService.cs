using Android.App;
using Firebase.Messaging;
using Newtonsoft.Json;
using SimpleIdServer.MobileApp.Services;
using System.Collections.Generic;
using System.Linq;
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
            var authReqId = GetRecord(message.Data, "authReqId");
            var permissions = JsonConvert.DeserializeObject<IEnumerable<NotificationPermission>>(GetRecord(message.Data, "permissions"));
            var notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.ReceiveNotification(title, body, authReqId, permissions.ToList());
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