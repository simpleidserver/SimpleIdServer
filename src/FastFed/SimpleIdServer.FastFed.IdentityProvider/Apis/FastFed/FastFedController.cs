// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.Client.Requests;
using SimpleIdServer.IdServer.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.FastFed;

// [Authorize("Authenticated")]
public class FastFedController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FastFedController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Start([FromQuery] StartHandshakeRequest request, CancellationToken cancellationToken)
    {
        // TODO - TRY TO AUTHENTICATE.
        // 7.2.2.1.Identity Provider Authenticates Administrator
        if (User.IsInRole("administrator")) return null; // NOT AUTHORIZED....
        // Check the request.
        // 7.2.2.2. Identity Provider Reads Application Provider Metadata
        // request.AppMetadataUri;
        return null;
    }
}