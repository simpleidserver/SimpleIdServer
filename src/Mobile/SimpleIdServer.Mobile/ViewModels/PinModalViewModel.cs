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
            if (PinEntered != null) PinEntered(this, new PinEventArgs(Pin));
        }, () =>
        {
            return Pin?.Length != PinLength;
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

    public event EventHandler<PinEventArgs> PinEntered;

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}


public class PinEventArgs : EventArgs
{
    public PinEventArgs(string pin)
    {
        Pin = pin;
    }

    public string Pin { get; set; }
}