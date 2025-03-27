// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation.UI.ViewModels;

namespace SimpleIdServer.IdServer.VerifiablePresentation.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseRegisterController<VerifiablePresentationRegisterViewModel>
{
    private readonly IPresentationDefinitionStore _presentationDefinitionStore;

    public RegisterController(
        IPresentationDefinitionStore presentationDefinitionStore,
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache, 
        IUserRepository userRepository, 
        ITokenRepository tokenRepository, 
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder,
        IRealmStore realmStore) : base(options, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, realmStore)
    {
        _presentationDefinitionStore = presentationDefinitionStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var userRegistrationProgress = await GetRegistrationProgress();
        var presentationDefinitions = await _presentationDefinitionStore.GetAll(prefix, cancellationToken);
        var verifiablePresentations = presentationDefinitions.Select(d => new VerifiablePresentationViewModel
        {
            Id = d.PublicId,
            Name = d.Name,
            VcNames = d.InputDescriptors.Select(id => id.Name).ToList()
        });
        var viewModel = new VerifiablePresentationRegisterViewModel
        {
            VerifiablePresentations = verifiablePresentations,
            QrCodeUrl = $"{issuer}/{GetRealm(prefix)}{Constants.Endpoints.VpAuthorizeQrCode}",
            StatusUrl = $"{issuer}/{GetRealm(prefix)}{Constants.Endpoints.VpRegisterStatus}",
            EndRegisterUrl = $"{issuer}/{GetRealm(prefix)}{Constants.Endpoints.VpEndRegister}",
            RedirectUrl = userRegistrationProgress?.RedirectUrl
        };
        return View(viewModel);
    }

    protected override void EnrichUser(User user, VerifiablePresentationRegisterViewModel viewModel) { }

    private string GetRealm(string realm) => Options.UseRealm ? $"{realm}/" : string.Empty;
}