using SimpleIdServer.MobileApp.Models;
using SimpleIdServer.MobileApp.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.ViewModels
{
    public class NotificationsPageViewModel : BaseViewModel
    {
        public NotificationsPageViewModel()
        {
            Notifications = new ObservableCollection<Notification>();
            var notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += HandleNotificationAdded;
        }

        public ObservableCollection<Notification> Notifications { get; set; }

        public void AddNotification(Notification notification)
        {
            Notifications.Add(notification);
        }

        public void RemoveNotification(Notification notification)
        {
            Notifications.Remove(notification);
        }

        private void HandleNotificationAdded(object sender, NotificationEventArgs e)
        {
            Notifications.Add(new Notification
            {
                Title = e.Title,
                Description = e.Description,
                AuthReqId = e.AuthReqId
            });
        }
    }
}
