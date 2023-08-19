using SimpleIdServer.Mobile.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class SettingsPageViewModel : INotifyPropertyChanged
{
    private bool _isDeveloperModeEnabled = false;
    private bool _isLoading = false;
    private MobileSettings _mobileSettings;

    public SettingsPageViewModel()
    {
        Task.Run(async () =>
        {
            _mobileSettings = await App.Database.GetMobileSettings();
        }).Wait();
        IsDeveloperModeEnabled = _mobileSettings.IsDeveloperModeEnabled;
        ToggleDeveloperModeComand = new Command<ToggledEventArgs>(async (c) =>
        {
            await ToggleDeveloperMode(c);
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsDeveloperModeEnabled
    {
        get => _isDeveloperModeEnabled;
        set
        {
            if(_isDeveloperModeEnabled != value)
            {
                _isDeveloperModeEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand ToggleDeveloperModeComand { get; private set; }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async Task ToggleDeveloperMode(ToggledEventArgs args)
    {
        if (_isLoading || args.Value == _mobileSettings.IsDeveloperModeEnabled) return;
        _isLoading = true;
        _mobileSettings.IsDeveloperModeEnabled = args.Value;
        await App.Database.UpdateMobileSettings(_mobileSettings);
        _isLoading = false;
    }
}
