using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.Mobile.ViewModels;
using SimpleIdServer.WalletClient.Services;

namespace SimpleIdServer.Mobile;

public partial class ViewCredentialOffer : ContentPage
{
    private readonly DidRecordState _didState;
    private readonly ViewCredentialOfferViewModel _viewModel;
    private readonly IVerifiableCredentialResolver _verifiableCredentialResolver;

    public ViewCredentialOffer(
        DidRecordState didState,
        ViewCredentialOfferViewModel viewModel,
        IVerifiableCredentialResolver verifiableCredentialResolver)
	{
        _didState = didState;
        _viewModel = viewModel;
        _verifiableCredentialResolver = verifiableCredentialResolver;
        BindingContext = _viewModel;
        InitializeComponent();
	}

	public async Task Load(Dictionary<string, string> parameters, Func<Task> callback)
	{
        _viewModel.IsLoading = true;
        var res = await _verifiableCredentialResolver.BuildService(parameters, CancellationToken.None);
        var credentialOffer = res.service.DeserializeCredentialOffer(res.credentialOffer);
        foreach (var credentialId in credentialOffer.CredentialIds)
            _viewModel.CredentialOffers.Add(new CredentialOfferRecord { Title = credentialId });
        _viewModel.IsLoading = false;
        await _viewModel.Set(res, callback);
    }
}