using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class ProfileViewModel : INotifyPropertyChanged
{
    private readonly MobileSettingsState _mobileSettingsState;
    private bool _isLoading = false;
    private bool _isGotifyServerRunning = false;
    private NotificationMode _selectedNotificationMode;
    private CancellationTokenSource _cancellationTokenSource;
    private int _refreshIntervalMs = 2000;

    public ProfileViewModel(MobileSettingsState mobileSettingsState)
    {
        _mobileSettingsState = mobileSettingsState;
        SelectNotificationModeCommand = new Command<EventArgs>(async (c) =>
        {
            await UpdateNotification(c);
        });
        _cancellationTokenSource = new CancellationTokenSource();
        /*
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
        */
    }

    public ICommand SelectNotificationModeCommand { get; private set; }

    public List<NotificationMode> NotificationModes { get; set; } = new List<NotificationMode>
    {
        new NotificationMode { DisplayName = "Gotify", Name = "gotify" },
        new NotificationMode { DisplayName = "Firebase", Name = "firebase" }
    };

    public bool IsLoading
    {
        get
        {
            return _isLoading;
        }
        set
        {
            _isLoading = value;
            OnPropertyChanged();
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
            if (_isGotifyServerRunning != value)
            {
                _isGotifyServerRunning = value;
                OnPropertyChanged();
            }
        }
    }

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

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task UpdateNotification(EventArgs args)
    {
        if (IsLoading) return;
        IsLoading = true;
        var mobileSettings = _mobileSettingsState.Settings;
        mobileSettings.NotificationMode = SelectedNotificationMode.Name;
        await _mobileSettingsState.Update(mobileSettings);
        IsLoading = false;
    }
}

public class NotificationMode
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
}