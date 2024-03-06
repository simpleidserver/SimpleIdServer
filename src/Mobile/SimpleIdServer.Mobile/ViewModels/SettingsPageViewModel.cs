using SimpleIdServer.Mobile.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class SettingsPageViewModel : INotifyPropertyChanged
{
    private bool _isLoading = false;
    private MobileSettings _mobileSettings;

    public SettingsPageViewModel()
    {
        Task.Run(async () =>
        {
            _mobileSettings = await App.Database.GetMobileSettings();
        }).Wait();
        var notificationMode = _mobileSettings.NotificationMode ?? "firebase";
        SelectedNotificationMode = NotificationModes.Single(m => m.Name == notificationMode);
        SelectNotificationModeCommand = new Command<EventArgs>(async (c) =>
        {
            await UpdateNotification(c);
        });
    }

    public List<NotificationMode> NotificationModes { get; set; } = new List<NotificationMode>
    {
        new NotificationMode { DisplayName = "Gotify", Name = "gotify" },
        new NotificationMode { DisplayName = "Firebase", Name = "firebase" }
    };

    public NotificationMode SelectedNotificationMode { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand SelectNotificationModeCommand { get; private set; }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async Task UpdateNotification(EventArgs args)
    {
        if (_isLoading) return;
        _isLoading = true;
        _mobileSettings.NotificationMode = SelectedNotificationMode.Name;
        await App.Database.UpdateMobileSettings(_mobileSettings);
        _isLoading = false;
    }
}


public class NotificationMode
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
}