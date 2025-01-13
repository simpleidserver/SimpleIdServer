// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
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
        IOptions<FormBuilderOptions> formOptions,
        IDistributedCache distributedCache, 
        IUserRepository userRepository, 
        ITokenRepository tokenRepository, 
        ITransactionBuilder transactionBuilder,
        IJwtBuilder jwtBuilder,
        IAntiforgery antiforgery,
        IFormStore formStore,
        IWorkflowStore workflowStore) : base(options, formOptions, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, antiforgery, formStore, workflowStore)
    {
        _presentationDefinitionStore = presentationDefinitionStore;
    }

    protected override string Amr => Constants.AMR;

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
        var result = await BuildViewModel(userRegistrationProgress, viewModel, prefix);
        result.SetInput(viewModel);
        return View(result);
    }

    protected override void EnrichUser(User user, VerifiablePresentationRegisterViewModel viewModel) { }

    private string GetRealm(string realm) => Options.UseRealm ? $"{realm}/" : string.Empty;
}