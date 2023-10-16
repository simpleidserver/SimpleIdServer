using SimpleIdServer.Mobile.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class EnrollViewModel : INotifyPropertyChanged
{
    public EnrollViewModel(SettingsPageViewModel settings)
    {
        Settings = settings;
        ScanQRCodeCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("enrollscanqrcode");
        });
        ViewOTPCommand = new Command((async () =>
        {
            await Shell.Current.GoToAsync("viewotplist");
        }));
        ViewCredentialListCommand = new Command((async () =>
        {
            await Shell.Current.GoToAsync("viewcredentiallist");
        }));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SettingsPageViewModel Settings { get; private set; }

    public ICommand ScanQRCodeCommand { get; private set; }

    public ICommand ViewOTPCommand { get; private set; }

    public ICommand ViewCredentialListCommand {  get; private set; }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Init()
    {
        MobileSettings mobileSettings = null;
        Task.Run(async () =>
        {
            mobileSettings = await App.Database.GetMobileSettings();
        }).Wait();
    }
}
