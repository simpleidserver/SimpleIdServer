using Newtonsoft.Json;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class SettingsPageViewModel : INotifyPropertyChanged
{
    private string _did;
    private bool _isLoading = false;
    private bool _isGotifyServerRunning;
    private readonly MobileSettingsState _mobileSettingsState;
    private readonly DidRecordState _didRecordState;
    private NotificationMode _selectedNotificationMode;
    private CancellationTokenSource _cancellationTokenSource;
    private int _refreshIntervalMs = 2000;

    public SettingsPageViewModel(
        MobileSettingsState mobileSettingsState,
        DidRecordState didRecordState)
    {
        _mobileSettingsState = mobileSettingsState;
        _didRecordState = didRecordState;
        SelectNotificationModeCommand = new Command<EventArgs>(async (c) =>
        {
            await UpdateNotification(c);
        });
        GenerateDidKeyCommand = new Command<EventArgs>(async (c) =>
        {
            await GenerateDidKey();
        }, (a) =>
        {
            return _didRecordState.Did == null;
        });
        _cancellationTokenSource = new CancellationTokenSource();
        var listener = GotifyNotificationListener.New();
        Task.Run(async () =>
        {
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested) break;
                IsGotifyServerRunning = listener.IsStarted;
                await Task.Delay(_refreshIntervalMs);
            }
        });
    }

    public void Start()
    {
        Did = _didRecordState.Did?.Did;
        var notificationMode = _mobileSettingsState.Settings.NotificationMode;
        SelectedNotificationMode = NotificationModes.Single(m => m.Name == notificationMode);
    }

    public List<NotificationMode> NotificationModes { get; set; } = new List<NotificationMode>
    {
        new NotificationMode { DisplayName = "Gotify", Name = "gotify" },
        new NotificationMode { DisplayName = "Firebase", Name = "firebase" }
    };

    public NotificationMode SelectedNotificationMode
    {
        get
        {
            return _selectedNotificationMode;
        }
        set
        {
            if (_selectedNotificationMode != value)
            {
                _selectedNotificationMode = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsGotifyServerRunning
    {
        get
        {
            return _isGotifyServerRunning;
        }
        set
        {
            if(_isGotifyServerRunning != value)
            {
                _isGotifyServerRunning = value;
                OnPropertyChanged();
            }
        }
    }

    public string Did
    {
        get
        {
            return _did;
        }
        set
        {
            if(_did != value)
            {
                _did = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand SelectNotificationModeCommand { get; private set; }

    public ICommand GenerateDidKeyCommand { get; private set; }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task UpdateNotification(EventArgs args)
    {
        if (_isLoading) return;
        _isLoading = true;
        var mobileSettings = _mobileSettingsState.Settings;
        mobileSettings.NotificationMode = SelectedNotificationMode.Name;
        await _mobileSettingsState.Update(mobileSettings);
        _isLoading = false;
    }

    private async Task GenerateDidKey()
    {
        if (_isLoading) return;
        _isLoading = true;
        var exportResult = DidKeyGenerator.New().GenerateRandomES256Key().Export(false, true);
        var export = SignatureKeySerializer.Serialize(exportResult.Key);
        await _didRecordState.Update(new DidRecord { Did = exportResult.Did, SerializedPrivateKey = System.Text.Json.JsonSerializer.Serialize(export) });
        Did = exportResult.Did;
        var cmd = (Command)GenerateDidKeyCommand;
        cmd.ChangeCanExecute();
        _isLoading = false;
    }
}

public class NotificationMode
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
}