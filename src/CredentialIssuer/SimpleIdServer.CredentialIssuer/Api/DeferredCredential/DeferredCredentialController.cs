// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.DeferredCredential;

[Route(Constants.EndPoints.Credential)]
public class DeferredCredentialController : BaseController
{
    public DeferredCredentialController()
    {
        
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] DeferredCredentialRequest request, CancellationToken cancellationToken)
    {
        return null;
    }

    private async Task Validate(
        DeferredCredentialRequest request,
        CancellationToken cancellationToken)
    {

    }
}