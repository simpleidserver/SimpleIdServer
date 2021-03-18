using System;

namespace SimpleIdServer.MobileApp.Services
{
    public interface INotificationManager
    {
        event EventHandler<NotificationEventArgs> NotificationReceived;
        void ReceiveNotification(string title, string description, string clickAction);
    }
}
