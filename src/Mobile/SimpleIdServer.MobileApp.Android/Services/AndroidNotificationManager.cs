using SimpleIdServer.MobileApp.Services;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.MobileApp.Droid.Services
{
    public class AndroidNotificationManager : INotificationManager
    {
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public void ReceiveNotification(string title, string description, string authReqId, ICollection<NotificationPermission> permissions)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Description = description,
                AuthReqId = authReqId,
                Permissions = permissions
            };
            NotificationReceived?.Invoke(null, args);
        }
    }
}