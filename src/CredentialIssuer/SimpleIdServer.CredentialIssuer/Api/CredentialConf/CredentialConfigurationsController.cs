// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialConf;

[Route(Constants.EndPoints.CredentialConfigurations)]
[Authorize("credconfs")]
public class CredentialConfigurationsController : BaseController
{
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;

    public CredentialConfigurationsController(ICredentialConfigurationStore credentialConfigurationStore)
    {
        _credentialConfigurationStore = credentialConfigurationStore;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var credentialConfigurations = await _credentialConfigurationStore.GetAll(cancellationToken);
        return new OkObjectResult(credentialConfigurations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        return new OkObjectResult(credentialConfiguration);
    }
}
