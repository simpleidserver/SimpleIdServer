using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleIdServer.Mobile.ViewModels;

public class ViewOtpListViewModel : INotifyPropertyChanged
{
    private readonly OtpListState _otpListState;
    private OTPCode _selectedOTPCode;
    private bool _isLoading;
    public event PropertyChangedEventHandler PropertyChanged;

    public ViewOtpListViewModel(OtpListState otpListState)
    {
        _otpListState = otpListState;
    }

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

    public async Task Load()
    {
        IsLoading = true;
        await _otpListState.Load();
        IsLoading = false;
    }
}