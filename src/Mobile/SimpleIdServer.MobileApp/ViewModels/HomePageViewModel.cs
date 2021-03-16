using SimpleIdServer.MobileApp.Models;
using System.Collections.ObjectModel;

namespace SimpleIdServer.MobileApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public HomePageViewModel()
        {
            Notifications = new ObservableCollection<Notification>();
            // OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        public ObservableCollection<Notification> Notifications { get; set; }

        public void AddNotification(Notification notification)
        {
            Notifications.Add(notification);
            OnPropertyChanged(nameof(Notifications));
        }
    }
}
