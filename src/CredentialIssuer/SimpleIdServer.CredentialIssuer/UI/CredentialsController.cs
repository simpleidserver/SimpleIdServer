// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.CredentialIssuer.Api;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Commands;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Queries;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.CredentialIssuer.UI.ViewModels;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.UI;

[Authorize("Authenticated")]
public class CredentialsController : BaseController
{
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;
    private readonly ICreateCredentialOfferCommandHandler _createCredentialOfferCommandHandler;
    private readonly IGetCredentialOfferQueryHandler _getCredentialOfferQueryHandler;

    public CredentialsController(
        ICredentialConfigurationStore credentialConfigurationStore,
        ICreateCredentialOfferCommandHandler createCredentialOfferCommandHandler,
        IGetCredentialOfferQueryHandler getCredentialOfferQueryHandler)
    {
        _credentialConfigurationStore = credentialConfigurationStore;
        _createCredentialOfferCommandHandler = createCredentialOfferCommandHandler;
        _getCredentialOfferQueryHandler = getCredentialOfferQueryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub").Value;
        var credentialConfigurations = await _credentialConfigurationStore.GetAll(cancellationToken);
        return View(new CredentialsViewModel
        {
            CredentialConfigurations = credentialConfigurations
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Share(string configurationId, CancellationToken cancellationToken)
    {
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var subject = User.FindFirst("sub").Value;
        var result = await _createCredentialOfferCommandHandler.Handle(new CreateCredentialOfferCommand
        {
            AccessToken = GetAccessToken(),
            CredentialConfigurationIds = new List<string> { configurationId },
            Grants = new List<string>
            {
                CredentialOfferResultNames.PreAuthorizedCodeGrant
            },
            Subject = subject
        }, cancellationToken);
        return File(_getCredentialOfferQueryHandler.GetQrCode(new GetCredentialOfferQuery 
        { 
            CredentialOffer = result.CredentialOffer, 
            Issuer = issuer 
        }), "image/png");
    }
}
