// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialInstance;

[Route(Constants.EndPoints.CredentialInstances)]
[Authorize("credinstances")]
public class CredentialInstancesController : BaseController
{
    private readonly ICredentialStore _credentialStore;

    public CredentialInstancesController(ICredentialStore credentialStore)
    {
        _credentialStore = credentialStore;
    }

    [HttpPost(".search")]
    public async Task<IActionResult> Search([FromBody] SearchCredentialInstancesRequest request, CancellationToken cancellationToken)
    {
        var credentials = await _credentialStore.GetByCredentialConfigurationId(request.CredentialConfigurationId, cancellationToken);
        return new OkObjectResult(credentials);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var credential = await _credentialStore.Get(id, cancellationToken);
        if (credential == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_ID, id)));
        _credentialStore.Remove(credential);
        await _credentialStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }
}
