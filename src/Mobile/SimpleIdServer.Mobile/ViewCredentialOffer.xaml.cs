using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.Mobile.ViewModels;
using SimpleIdServer.WalletClient.Services;

namespace SimpleIdServer.Mobile;

public partial class ViewCredentialOffer : ContentPage
{
    private readonly DidRecordState _didState;
    private readonly ViewCredentialOfferViewModel _viewModel;
    private readonly IVcService _vcService;
    private readonly IVerifiableCredentialResolver _verifiableCredentialResolver;

    public ViewCredentialOffer(
        DidRecordState didState,
        ViewCredentialOfferViewModel viewModel,
        IVcService vcService,
        IVerifiableCredentialResolver verifiableCredentialResolver)
	{
        _didState = didState;
        _viewModel = viewModel;
		_vcService = vcService;
        _verifiableCredentialResolver = verifiableCredentialResolver;
        BindingContext = _viewModel;
        InitializeComponent();
	}

	public async Task Load(Dictionary<string, string> parameters)
	{
        _viewModel.IsLoading = true;
        var res = await _verifiableCredentialResolver.BuildService(parameters, CancellationToken.None);
        var credentialOffer = res.service.DeserializeCredentialOffer(res.credentialOffer);
        foreach (var credentialId in credentialOffer.CredentialIds)
            _viewModel.CredentialOffers.Add(new CredentialOfferRecord { Title = credentialId });
        await _viewModel.Set(res);
        _viewModel.IsLoading = false;
    }
}