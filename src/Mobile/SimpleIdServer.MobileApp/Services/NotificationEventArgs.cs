using System;
using System.Collections.Generic;

namespace SimpleIdServer.MobileApp.Services
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs()
        {
            Permissions = new List<NotificationPermission>();
        }

        public string AuthReqId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<NotificationPermission> Permissions { get; set; }
    }

    public class NotificationPermission
    {
        public string ConsentId { get; set; }
        public string Type { get; set; }
        public string PermissionId { get; set; }
        public string DisplayName { get; set; }
    }
}
