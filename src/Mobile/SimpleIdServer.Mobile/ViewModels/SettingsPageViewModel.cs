using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class SettingsPageViewModel : INotifyPropertyChanged
{
    private string _did;
    private bool _isLoading = false;
    private bool _isGotifyServerRunning;
    private MobileSettings _mobileSettings;
    private DidRecord _didRecord;
    private CancellationTokenSource _cancellationTokenSource;
    private int _refreshIntervalMs = 2000;

    public SettingsPageViewModel()
    {
        Task.Run(async () =>
        {
            _mobileSettings = await App.Database.GetMobileSettings();
            _didRecord = await App.Database.GetDidRecord();
        }).Wait();
        Did = _didRecord?.Did;
        var notificationMode = _mobileSettings.NotificationMode ?? "firebase";
        SelectedNotificationMode = NotificationModes.Single(m => m.Name == notificationMode);
        SelectNotificationModeCommand = new Command<EventArgs>(async (c) =>
        {
            await UpdateNotification(c);
        });
        GenerateDidKeyCommand = new Command<EventArgs>(async (c) =>
        {
            await GenerateDid();
        }, (a) =>
        {
            return _didRecord == null;
        });
        _cancellationTokenSource = new CancellationTokenSource();
        var listener = GotifyNotificationListener.New();
        Task.Run(async () =>
        {
            while(true)
            {
                if (_cancellationTokenSource.IsCancellationRequested) break;
                IsGotifyServerRunning = listener.IsStarted;
                await Task.Delay(_refreshIntervalMs);
            }
        });
    }

    public List<NotificationMode> NotificationModes { get; set; } = new List<NotificationMode>
    {
        new NotificationMode { DisplayName = "Gotify", Name = "gotify" },
        new NotificationMode { DisplayName = "Firebase", Name = "firebase" }
    };

    public NotificationMode SelectedNotificationMode { get; set; }

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
        _mobileSettings.NotificationMode = SelectedNotificationMode.Name;
        await App.Database.UpdateMobileSettings(_mobileSettings);
        _isLoading = false;
    }

    private async Task GenerateDid()
    {
        if (_isLoading) return;
        _isLoading = true;
        var ed25519 = Ed25519SignatureKey.Generate();
        var generator = DidKeyGenerator.New();
        var did = generator.Generate(ed25519);
        await App.Database.AddDidRecord(new DidRecord { Did = did, PrivaterKey = ed25519.GetPrivateKey() });
        Did = did;
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