using Microsoft.Extensions.Options;
using Plugin.Firebase.CloudMessaging;
using SimpleIdServer.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class NotificationViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;
    private readonly IPromptService _promptService;
    private readonly IUrlService _urlService;
    private readonly MobileOptions _options;
    private NotificationTypes? _notificationType = null;
    private IDictionary<string, string> _data = null;
    private bool _isLoading = false;
    private string _displayMessage = null;

    public NotificationViewModel(INavigationService navigationService, IPromptService promptService, IUrlService urlService, IOptions<MobileOptions> options)
    {
        _navigationService = navigationService;
        _promptService = promptService;
        _urlService = urlService;
        _options = options.Value;
        CloseCommand = new Command(async () =>
        {
            await _navigationService.GoBack();
        });
        RejectCommand = new Command(async () =>
        {
            if (IsLoading) return;
            await HandleConsent(false);
        }, () =>
        {
            return !IsLoading;
        });
        AcceptCommand = new Command(async () =>
        {
            if (IsLoading) return;
            await HandleConsent(true);
        }, () =>
        {
            return !IsLoading;
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
        IsLoading = true;
        try
        {
            switch (_notificationType.Value)
            {
                case NotificationTypes.CIBA:
                    await HandleCIBANotification();
                    var verb = isConfirmed ? "confirmed" : "rejected";
                    await _promptService.ShowAlert("Success", $"The consent is {verb}");
                    await _navigationService.GoBack();
                    break;
            }
        }
        catch
        {
            var verb = isConfirmed ? "confirm" : "reject";
            await _promptService.ShowAlert("Error", $"An error occured while trying to {verb} the consent");
        }
        finally
        {
            IsLoading = false;
        }

        async Task HandleCIBANotification()
        {
            var authReqId = _data["auth_req_id"];
            var bcChannelUrl = _data["bc_channel"];
            var parameter = new BCCallbackParameter
            {
                Action = isConfirmed ? 0 : 1,
                AuthReqId = authReqId
            };
            var handler = new HttpClientHandler();
            if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) =>
            {
                return true;
            };
            using (var httpClient = new HttpClient(handler))
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_urlService.GetUrl(bcChannelUrl)),
                    Content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                httpResult.EnsureSuccessStatusCode();
            }
        }
    }

    private bool TryDisplayCIBANotification(FCMNotification notification)
    {
        if (!notification.Data.ContainsKey("auth_req_id")) return false;
        var bindingMessage = notification.Data["binding_message"].ToString();
        DisplayMessage = bindingMessage;
        _data = notification.Data;
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