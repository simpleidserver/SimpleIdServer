using SimpleIdServer.Mobile.Models;
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

    public ViewCredentialListViewModel(CredentialListState credentialListState)
    {
        _credentialListState = credentialListState;
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

    public async Task Load()
    {
        await _credentialListState.Load();
    }
}
