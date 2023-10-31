using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class ViewOtpListViewModel : INotifyPropertyChanged
{
    private readonly OtpListState _otpListState;
    private readonly INavigationService _navigationService;
    private OTPCode _selectedOTPCode;
    private bool _isLoading;
    public event PropertyChangedEventHandler PropertyChanged;

    public ViewOtpListViewModel(OtpListState otpListState,  INavigationService navigationService)
    {
        _otpListState = otpListState;
        _navigationService = navigationService;
        CloseCommand = new Command(async () =>
        {
            await _navigationService.GoBack();
        });
        RemoveSelectedOtpCommand = new Command(async () =>
        {
            await RemoveSelectedOtp();
        }, () =>
        {
            return SelectedOTPCode != null;
        });
    }

    public ICommand RemoveSelectedOtpCommand { get; private set; }
    public ICommand CloseCommand { get; private set; }

    public ObservableCollection<OTPCode> OTPCodes
    {
        get
        {
            return _otpListState.OTPCodes;
        }
    }

    public OTPCode SelectedOTPCode
    {
        get
        {
            return _selectedOTPCode;
        }
        set
        {
            if(_selectedOTPCode != value)
            {
                _selectedOTPCode = value;
                OnPropertyChanged();
                var cmd = (Command)RemoveSelectedOtpCommand;
                cmd.ChangeCanExecute();
            }
        }
    }

    public bool IsLoading
    {
        get
        {
            return _isLoading;
        }
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async Task RemoveSelectedOtp()
    {
        if (SelectedOTPCode == null) return;
        await _otpListState.RemoveOTPCode(SelectedOTPCode);
        SelectedOTPCode = null;
    }

    public void Load()
    {
        IsLoading = false;
    }
}