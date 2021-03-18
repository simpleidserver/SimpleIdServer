using SimpleIdServer.MobileApp.Services;
using System;

namespace SimpleIdServer.MobileApp.Droid.Services
{
    public class AndroidNotificationManager : INotificationManager
    {
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public void ReceiveNotification(string title, string description, string clickAction)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Description = description,
                ClickAction = clickAction
            };
            NotificationReceived?.Invoke(null, args);
        }
    }
}