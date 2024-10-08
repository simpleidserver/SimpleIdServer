using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class WalletViewModel : INotifyPropertyChanged
{
    private bool _isLoading = false;
    private bool _atLeastOneVerifiableCredential = false;
    private readonly VerifiableCredentialListState _vcListState;

    public WalletViewModel(VerifiableCredentialListState vcListState)
    {
        _vcListState = vcListState;
        DeleteCommand = new Command(async () =>
        {
            IsLoading = true;
            var vc = VerifiableCredentials.Single(d => d.IsSelected);
            await _vcListState.RemoveVerifiableCredentialRecord(vc);
            IsLoading = false;
            RefreshAtLeastOneVerifiableCredential();
            RefreshCommands();
        }, () =>
        {
            return VerifiableCredentials.Any() && VerifiableCredentials.Any(d => d.IsSelected);
        });
        SelectCommand = new Command<VerifiableCredentialRecord>((d) =>
        {
            foreach (var vc in VerifiableCredentials)
                vc.IsSelected = false;
            d.IsSelected = true;
            OnPropertyChanged(nameof(VerifiableCredentials));
            RefreshCommands();
        });
        RefreshAtLeastOneVerifiableCredential();
    }

    public Command<VerifiableCredentialRecord> SelectCommand { get; private set; }

    public ICommand DeleteCommand { get; private set; }

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

    public bool AtLeastOneVerifiableCredential
    {
        get { return _atLeastOneVerifiableCredential; }
        set
        {
            if (_atLeastOneVerifiableCredential != value)
            {
                _atLeastOneVerifiableCredential = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void RefreshCommands()
    {
        ((Command)DeleteCommand).ChangeCanExecute();
    }

    private void RefreshAtLeastOneVerifiableCredential()
        => AtLeastOneVerifiableCredential = VerifiableCredentials?.Any() ?? false;
}