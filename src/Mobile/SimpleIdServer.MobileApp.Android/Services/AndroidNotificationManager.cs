using SimpleIdServer.MobileApp.Services;
using System;

namespace SimpleIdServer.MobileApp.Droid.Services
{
    public class AndroidNotificationManager : INotificationManager
    {
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public void ReceiveNotification(string title, string description, string authReqId)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Description = description,
                AuthReqId = authReqId
            };
            NotificationReceived?.Invoke(null, args);
        }
    }
}