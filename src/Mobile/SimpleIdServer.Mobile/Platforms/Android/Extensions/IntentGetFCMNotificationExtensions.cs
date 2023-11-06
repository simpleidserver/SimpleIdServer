using Android.Content;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.Platforms.Android.Extensions;

namespace SimpleIdServer.Mobile.Platforms.Android.Extensions;

public static class IntentGetFCMNotificationExtensions
{
    public static FCMNotification GetFCMNotification(this Intent intent)
    {
        if (intent.HasExtra("intent_key_fcm_notification")) return intent.GetNotificationFromExtras("intent_key_fcm_notification");
        if(intent.HasExtra("google.message_id"))
        {
            var keys = intent.Extras.KeySet().Where(k => k != "body" && k != "title" && k != "image_url");
            var data = new Dictionary<string, string>();
            foreach(var key in keys) data.Add(key, intent.Extras.GetString(key));
            return new FCMNotification(intent.Extras!.GetString("body"), intent.Extras!.GetString("title"), intent.Extras!.GetString("image_url"), data);
        }

        return null;
    }
}
