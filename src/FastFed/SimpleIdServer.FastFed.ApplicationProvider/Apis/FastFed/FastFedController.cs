// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Apis.FastFed;

public class FastFedController : Controller
{
    private readonly IFastFedService _fastFedService;

    public FastFedController(IFastFedService fastFedService)
    {
        _fastFedService = fastFedService;
    }

    [HttpPost]
    public async Task<IActionResult> Whitelist([FromBody] WhitelistIdentityProviderRequest request, CancellationToken cancellationToken)
    {
        // TODO - TRY TO AUTHENTICATE.
        var issuer = this.GetAbsoluteUriWithVirtualPath();
        var validationResult = await _fastFedService.StartWhitelist(issuer, request.IdentityProviderUrl, cancellationToken);
        if (validationResult.HasError) return null;
        var builder = new UriBuilder(validationResult.Result.fastFedHandshakeStartUri);
        builder.Query = validationResult.Result.request.ToQueryParameters();
        return Redirect(builder.Uri.ToString());
    }
}
