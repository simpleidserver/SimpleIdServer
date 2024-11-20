// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Api;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Commands;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Queries;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.CredentialIssuer.UI.ViewModels;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.UI;

[Authorize("WebsiteAuthenticated")]
public class CredentialsController : BaseController
{
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;
    private readonly ICreateCredentialOfferCommandHandler _createCredentialOfferCommandHandler;
    private readonly IGetCredentialOfferQueryHandler _getCredentialOfferQueryHandler;
    private readonly CredentialIssuerOptions _options;

    public CredentialsController(
        ICredentialConfigurationStore credentialConfigurationStore,
        ICreateCredentialOfferCommandHandler createCredentialOfferCommandHandler,
        IGetCredentialOfferQueryHandler getCredentialOfferQueryHandler,
        IOptions<CredentialIssuerOptions> options)
    {
        _credentialConfigurationStore = credentialConfigurationStore;
        _createCredentialOfferCommandHandler = createCredentialOfferCommandHandler;
        _getCredentialOfferQueryHandler = getCredentialOfferQueryHandler;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var credentialConfigurations = await _credentialConfigurationStore.GetAll(cancellationToken);
        return View(new CredentialsViewModel
        {
            CredentialConfigurations = credentialConfigurations,
            IsDeveloperModeEnabled = _options.IsDeveloperModeEnabled
        });
    }

    [HttpPost]
    public async Task<IActionResult> Share([FromBody] ShareCredentialRequest request, CancellationToken cancellationToken)
    {
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var grants = new List<string>();
        if (request.PreAuthorized)
            grants.Add(CredentialOfferResultNames.PreAuthorizedCodeGrant);
        else
            grants.Add(CredentialOfferResultNames.AuthorizedCodeGrant);
        var result = await _createCredentialOfferCommandHandler.Handle(new CreateCredentialOfferCommand
        {
            AccessToken = await GetAccessToken(),
            CredentialConfigurationIds = new List<string> { request.ConfigurationId },
            Grants = grants,
            Subject = subject
        }, cancellationToken);
        if (result.Error != null)
            return Build(result.Error.Value);
        var json = JsonSerializer.Serialize(result.CredentialOffer);
        Response.Headers.Add("QRCode", json);
        return File(_getCredentialOfferQueryHandler.GetQrCode(new GetCredentialOfferQuery 
        { 
            CredentialOffer = result.CredentialOffer, 
            Issuer = issuer 
        }, _options.AuthorizationServer), "image/png");
    }
}


public class ShareCredentialRequest
{
    public string ConfigurationId { get; set; }
    public bool PreAuthorized { get; set; }
}