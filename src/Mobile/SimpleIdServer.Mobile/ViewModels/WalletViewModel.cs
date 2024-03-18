using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleIdServer.Mobile.ViewModels;

public class WalletViewModel : INotifyPropertyChanged
{
    private bool _isLoading = false;
    private readonly VerifiableCredentialListState _vcListState;

    public WalletViewModel(VerifiableCredentialListState vcListState)
    {
        _vcListState = vcListState;
    }

    public ObservableCollection<VerifiableCredentialRecord> VerifiableCredentials
    {
        get
        {
            return _vcListState.VerifiableCredentialRecords;
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsLoading
    {
        get { return _isLoading; }
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
}