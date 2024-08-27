using SimpleIdServer.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class EnrollViewModel : INotifyPropertyChanged
{
    public EnrollViewModel(INavigationService navigationService)
    {
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

    public ICommand ScanQRCodeCommand { get; private set; }

    public ICommand ViewOTPCommand { get; private set; }

    public ICommand ViewCredentialListCommand {  get; private set; }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
