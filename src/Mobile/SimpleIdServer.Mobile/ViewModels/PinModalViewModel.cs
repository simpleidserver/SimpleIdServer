using SimpleIdServer.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class PinModalViewModel : INotifyPropertyChanged
{
    private string _pin;
    private int _pinLength;

    public PinModalViewModel(INavigationService navigationService)
    {
        ConfirmPinCommand = new Command(async () =>
        {
            await navigationService.GoBack();
        });
    }

    public string Pin
    {
        get
        {
            return _pin;
        }
        set
        {
            _pin = value;
            OnPropertyChanged();
        }
    }

    public int PinLength
    {
        get
        {
            return _pinLength;
        }
        set
        {
            _pinLength = value;
            OnPropertyChanged();
        }
    }

    public ICommand ConfirmPinCommand { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
