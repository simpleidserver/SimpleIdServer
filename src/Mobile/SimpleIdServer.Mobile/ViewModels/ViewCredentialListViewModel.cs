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
    private bool _atLeastOneCredential;
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
            RefreshAtLeastOneCredential();
        });
        RefreshAtLeastOneCredential();
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

    public bool AtLeastOneCredential
    {
        get { return _atLeastOneCredential; }
        set
        {
            if (_atLeastOneCredential != value)
            {
                _atLeastOneCredential = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void RefreshAtLeastOneCredential()
        => AtLeastOneCredential = Credentials?.Any() ?? false;

    public void Load()
    {
        IsLoading = false;
    }
}
