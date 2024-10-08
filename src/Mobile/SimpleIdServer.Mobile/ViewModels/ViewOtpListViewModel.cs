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
    private bool _atLeastOneOtp;
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
            RefreshDeleteCommand();
            RefreshAtLeastOneOtp();
        }, () =>
        {
            return SelectedOTPCode != null;
        });
        RefreshAtLeastOneOtp();
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
                RefreshDeleteCommand();
            }
        }
    }

    public bool AtLeastOneOtp
    {
        get
        {
            return _atLeastOneOtp;
        }
        set
        {
            if (_atLeastOneOtp != value)
            {
                _atLeastOneOtp = value;
                OnPropertyChanged();
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

    private void RefreshDeleteCommand()
    {
        var cmd = (Command)RemoveSelectedOtpCommand;
        cmd.ChangeCanExecute();
    }

    private void RefreshAtLeastOneOtp()
        => AtLeastOneOtp = OTPCodes?.Any() ?? false;

    public void Load()
    {
        IsLoading = false;
    }
}