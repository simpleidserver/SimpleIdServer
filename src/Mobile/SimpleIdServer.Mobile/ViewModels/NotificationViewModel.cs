using Plugin.Firebase.CloudMessaging;
using SimpleIdServer.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class NotificationViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;
    private NotificationTypes? _notificationType = null;
    private bool _isLoading = false;
    private string _authReqId = null;
    private string _displayMessage = null;

    public NotificationViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        CloseCommand = new Command(async () =>
        {
            await _navigationService.GoBack();
        });
        RejectCommand = new Command(async () =>
        {

        });
        AcceptCommand = new Command(async () =>
        {

        });
    }

    public bool IsLoading
    {
        get
        {
            return _isLoading;
        }
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public string DisplayMessage
    {
        get
        {
            return _displayMessage;
        }
        set
        {
            if (_displayMessage != value)
            {
                _displayMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand CloseCommand { get; private set; }

    public ICommand RejectCommand { get; private set; }

    public ICommand AcceptCommand {  get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Display(FCMNotification notification)
    {
        if (notification.Data == null || !notification.Data.Any()) return;
        if (TryDisplayCIBANotification(notification)) return;
    }

    private async Task HandleConsent(bool isConfirmed)
    {
        if (_notificationType == null) return;
        switch(_notificationType.Value)
        {
            case NotificationTypes.CIBA:
                var parameter = new BCCallbackParameter
                {
                    Action = isConfirmed ? 0 : 1,
                    AuthReqId = _authReqId
                };

                break;
        }
    }

    private bool TryDisplayCIBANotification(FCMNotification notification)
    {
        if (!notification.Data.ContainsKey("auth_req_id")) return false;
        var bindingMessage = notification.Data["binding_message"].ToString();
        DisplayMessage = bindingMessage;
        _authReqId = notification.Data["auth_req_id"];
        _notificationType = NotificationTypes.CIBA;
        return true;
    }
}

public enum NotificationTypes
{
    CIBA = 0
}

public class BCCallbackParameter
{
    [JsonPropertyName("auth_req_id")]
    public string AuthReqId { get; set; }
    [JsonPropertyName("action")]
    public int Action { get; set; }
}