using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class EnrollViewModel : INotifyPropertyChanged
{
    public EnrollViewModel(SettingsPageViewModel settings, INavigationService navigationService)
    {
        Settings = settings;
        ScanQRCodeCommand = new Command(async () =>
        {
            await navigationService.DisplayModal<QRCodeScannerPage>();
        });
        ViewOTPCommand = new Command((async () =>
        {
            await navigationService.DisplayModal<ViewOtpListPage>();
        }));
        ViewCredentialListCommand = new Command((async () =>
        {
            await navigationService.DisplayModal<ViewCredentialListPage>();
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
