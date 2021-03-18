using System;

namespace SimpleIdServer.MobileApp.Services
{
    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ClickAction { get; set; }
    }
}
