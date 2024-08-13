using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Resources;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.WalletClient.Services;

namespace SimpleIdServer.Mobile.Services;

public interface IVcService
{
    Task RegisterVc(Dictionary<string, string> parameters, CancellationToken cancellationToken);
}

public class VcService : IVcService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DidRecordState _didState;
    private readonly VerifiableCredentialListState _verifiableCredentialListState;
    private readonly IVerifiableCredentialResolver _verifiableCredentialResolver;
    private readonly INavigationService _navigationService;
    private readonly IPromptService _promptService;

    public VcService(IServiceProvider serviceProvider,
        DidRecordState didState,
        VerifiableCredentialListState verifiableCredentialListState,
        IVerifiableCredentialResolver verifiableCredentialResolver, 
        IPromptService promptService, 
        INavigationService navigationService)
    {
        _serviceProvider = serviceProvider;
        _didState = didState;
        _verifiableCredentialListState = verifiableCredentialListState;
        _verifiableCredentialResolver = verifiableCredentialResolver;
        _promptService = promptService;
        _navigationService = navigationService;
    }

    public async Task RegisterVc(Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var didRecord = _didState.Did;
        var privateKey = SignatureKeySerializer.Deserialize(didRecord.SerializedPrivateKey);
        var result = await _verifiableCredentialResolver.BuildService(parameters, cancellationToken);
        var credentialOffer = result.service.DeserializeCredentialOffer(result.credentialOffer);
        string pin = null;
        if (credentialOffer.Grants.PreAuthorizedCodeGrant != null)
        {
            var pinLength = credentialOffer.Grants.PreAuthorizedCodeGrant.Transaction?.Length ?? 4;
            var pinModal = _serviceProvider.GetRequiredService<PinModal>();
            pinModal.ViewModel.PinLength = pinLength;
            await _navigationService.DisplayModal(pinModal);
            pin = pinModal.Pin;
        }

        var credResult = await result.service.Request(result.credentialOffer, didRecord.Did, privateKey, pin, cancellationToken);
        if(credResult.Status == CredentialStatus.ERROR)
        {
            await _promptService.ShowAlert(Global.Error, credResult.ErrorMessage);
            return;
        }

        if(credResult.Status == CredentialStatus.PENDING)
        {
            credResult = await Retry(credResult, cancellationToken);
            if (credResult == null) return;
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
            Logo = firstDisplay.Logo?.Uri
        });
        await _promptService.ShowAlert(Global.Success, Global.VerifiableCredentialEnrolled);
    }

    private async Task<RequestVerifiableCredentialResult> Retry(RequestVerifiableCredentialResult credResult, CancellationToken cancellationToken)
    {
        var retry = await _promptService.ShowPrompt(Global.VerifiableCredentialEnrollment, Global.RetryGetDeferredCredential);
        if (retry)
        {
            var res =  await credResult.Retry(cancellationToken);
            if(res.Status == CredentialStatus.ERROR)
            {
                await _promptService.ShowAlert(Global.Error, credResult.ErrorMessage);
                return null;
            }

            if(res.Status == CredentialStatus.PENDING)
            {
                return await Retry(credResult, cancellationToken);
            }

            return credResult;
        }

        return null;
    }
}
