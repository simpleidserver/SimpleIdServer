using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class ViewCredentialListViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    private readonly CredentialListState _credentialListState;
    private readonly INavigationService _navigationService;

    public ViewCredentialListViewModel(CredentialListState credentialListState, INavigationService navigationService)
    {
        _credentialListState = credentialListState;
        _navigationService = navigationService;
        CloseCommand = new Command(async () =>
        {
            await _navigationService.GoBack();
        });
        RemoveSelectedCredentialsCommand = new Command(async () =>
        {
            await credentialListState.Remove();
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<SelectableCredentialRecord> Credentials
    {
        get
        {
            return _credentialListState.CredentialRecords;
        }
    }

    public ICommand RemoveSelectedCredentialsCommand { get; private set; }
    public ICommand CloseCommand { get; private set; }

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

    public void Load()
    {
        IsLoading = false;
    }
}
