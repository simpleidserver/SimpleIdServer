using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Resources;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.WalletClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class ViewCredentialOfferViewModel : INotifyPropertyChanged
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DidRecordState _didState;
    private readonly VerifiableCredentialListState _verifiableCredentialListState;
    private readonly INavigationService _navigationService;
    private readonly IPromptService _promptService;
    private readonly ObservableCollection<CredentialOfferRecord> _credentialOffers = new ObservableCollection<CredentialOfferRecord>();
    private bool _isLoading;
    private (IVerifiableCredentialsService service, string credentialOffer)? _service = null;
    private Func<Task> _callback;

    public ViewCredentialOfferViewModel(
        IServiceProvider serviceProvider,
        DidRecordState didState,
        VerifiableCredentialListState verifiableCredentialListState,
        IPromptService promptService,
        INavigationService navigationService)
    {
        _serviceProvider = serviceProvider;
        _didState = didState;
        _verifiableCredentialListState = verifiableCredentialListState;
        _promptService = promptService;
        _navigationService = navigationService;
        ConfirmCommand = new Command(async () =>
        {
            if (IsLoading == true) return;
            await RegisterVc();
        }, () =>
        {
            return _service != null && !IsLoading;
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

    public async Task Set((IVerifiableCredentialsService service, string credentialOffer) service, Func<Task> callback)
    {
        _service = service;
        _callback = callback;
        await RefreshCommand();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async Task RegisterVc()
    {
        IsLoading = true;
        var credentialOffer = _service.Value.service.DeserializeCredentialOffer(_service.Value.credentialOffer);
        if (credentialOffer.Grants.PreAuthorizedCodeGrant != null)
        {
            var pinLength = credentialOffer.Grants.PreAuthorizedCodeGrant.Transaction?.Length ?? 4;
            var pinModal = _serviceProvider.GetRequiredService<PinModal>();
            pinModal.ViewModel.PinLength = pinLength;
            await _navigationService.DisplayModal(pinModal);
            pinModal.ViewModel.PinEntered += async (o, e) =>
            {
                await RegisterVc(e.Pin, CancellationToken.None);
                IsLoading = false;
                await this.RefreshCommand();
                await _navigationService.GoBack();
                if (_callback != null)
                    await _callback();
            };
            return;
        }

        await RegisterVc(null, CancellationToken.None);
        IsLoading = false;
        await this.RefreshCommand();
        await _navigationService.GoBack();
        if (_callback != null)
            await _callback();
    }

    private async Task RegisterVc(string pin, CancellationToken cancellationToken)
    {
        var didRecord = _didState.Did;
        var privateKey = SignatureKeySerializer.Deserialize(didRecord.SerializedPrivateKey);
        var credResult = await _service.Value.service.Request(_service.Value.credentialOffer, didRecord.Did, privateKey, pin, cancellationToken);
        if (credResult.Status == CredentialStatus.ERROR)
        {
            await _promptService.ShowAlert(Global.Error, credResult.ErrorMessage);
            return;
        }

        if (credResult.Status == CredentialStatus.PENDING)
        {
            credResult = await Retry(credResult, cancellationToken);
            if (credResult == null) return;
        }

        if (credResult.Status == CredentialStatus.VP_PRESENTED)
        {
            await _promptService.ShowAlert(Global.Success, Global.VpPresented);
            return;
        }
        
        var w3cCred = credResult.VerifiableCredential.W3CCredential;
        var credDef = credResult.VerifiableCredential.CredentialDef;
        var cred = credResult.VerifiableCredential.Credential;
        var firstDisplay = credDef.Display?.FirstOrDefault();
        await _verifiableCredentialListState.AddVerifiableCredentialRecord(new VerifiableCredentialRecord
        {
            Id = Guid.NewGuid().ToString(),
            Format = cred.Format,
            Name = firstDisplay.Name,
            Description = firstDisplay.Description,
            ValidFrom = w3cCred.ValidFrom,
            ValidUntil = w3cCred.ValidUntil,
            Type = w3cCred.Type.Last(),
            BackgroundColor = firstDisplay.BackgroundColor,
            TextColor = firstDisplay.TextColor,
            Logo = firstDisplay.Logo?.Uri,
            SerializedVc = credResult.VerifiableCredential.SerializedVc
        });
        await _promptService.ShowAlert(Global.Success, Global.VerifiableCredentialEnrolled);
    }

    private async Task<RequestVerifiableCredentialResult> Retry(RequestVerifiableCredentialResult credResult, CancellationToken cancellationToken)
    {
        var retry = await _promptService.ShowYesNo(Global.VerifiableCredentialEnrollment, Global.RetryGetDeferredCredential);
        if (retry)
        {
            var res = await credResult.Retry(cancellationToken);
            if (res.Status == CredentialStatus.ERROR)
            {
                await _promptService.ShowAlert(Global.Error, credResult.ErrorMessage);
                return null;
            }

            if (res.Status == CredentialStatus.PENDING)
            {
                return await Retry(credResult, cancellationToken);
            }

            return res;
        }

        return null;
    }

    private async Task RefreshCommand()
    {
        await App.Current.Dispatcher.DispatchAsync(async () =>
        {
            var cmd = (Command)ConfirmCommand;
            cmd.ChangeCanExecute();
        });
    }
}
