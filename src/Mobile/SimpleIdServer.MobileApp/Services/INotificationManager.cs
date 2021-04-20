using System;
using System.Collections.Generic;

namespace SimpleIdServer.MobileApp.Services
{
    public interface INotificationManager
    {
        event EventHandler<NotificationEventArgs> NotificationReceived;
        void ReceiveNotification(string title, string description, string clickAction, ICollection<NotificationPermission> permissions);
    }
}
