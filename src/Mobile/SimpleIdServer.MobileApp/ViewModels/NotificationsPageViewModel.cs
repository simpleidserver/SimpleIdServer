using SimpleIdServer.MobileApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleIdServer.MobileApp.ViewModels
{
    public class NotificationsPageViewModel : BaseViewModel
    {
        private readonly ITokenStorage _tokenStorage;

        public NotificationsPageViewModel()
        {
            _tokenStorage = DependencyService.Get<ITokenStorage>();
            Consents = new ObservableCollection<ConsentPageViewModel>();
            DisconnectCommand = new Command(async() => await ExecuteDisconnect(), IsDisconnectEnabled);
            var notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += HandleConsentAdded;
        }

        public ObservableCollection<ConsentPageViewModel> Consents { get; set; }
        public ICommand DisconnectCommand { get; private set; }

        public void AddConsent(ConsentPageViewModel notification)
        {
            Consents.Add(notification);
        }

        public void RemoveConsent(ConsentPageViewModel notification)
        {
            Consents.Remove(notification);
        }

        private void HandleConsentAdded(object sender, NotificationEventArgs e)
        {
            Consents.Add(new ConsentPageViewModel
            {
                Title = e.Title,
                Description = e.Description,
                AuthReqId = e.AuthReqId,
                Permissions = new ObservableCollection<PermissionViewModel>(e.Permissions.Select(p => new PermissionViewModel
                {
                    DisplayName = p.DisplayName,
                    PermissionId = p.PermissionId,
                    IsSelected = true
                }))
            });
        }

        private async Task ExecuteDisconnect()
        {
            _tokenStorage.Remove();
            await Shell.Current.GoToAsync("//home");
        }

        private bool IsDisconnectEnabled()
        {
            return true;
        }
    }
}
