using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.WalletClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class ViewCredentialOfferViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    private (IVerifiableCredentialsService service, string credentialOffer)? _service = null;
    private readonly ObservableCollection<CredentialOfferRecord> _credentialOffers = new ObservableCollection<CredentialOfferRecord>();

    public ViewCredentialOfferViewModel(IVcService vcService, INavigationService navigationService)
    {
        ConfirmCommand = new Command(async () =>
        {
            IsLoading = true;
            await vcService.RegisterVc(_service.Value, CancellationToken.None);
            IsLoading = false;
        }, () =>
        {
            return _service != null;
        });
    }

    public ObservableCollection<CredentialOfferRecord> CredentialOffers
    {
        get
        {
            return _credentialOffers;
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
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public ICommand ConfirmCommand { get; private set; }

    public async Task Set((IVerifiableCredentialsService service, string credentialOffer) service)
    {
        _service = service;
        await App.Current.Dispatcher.DispatchAsync(async () =>
        {
            var cmd = (Command)ConfirmCommand;
            cmd.ChangeCanExecute();
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
