// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.Requests;
using SimpleIdServer.IdServer.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.FastFed;

// [Authorize("Authenticated")]
public class FastFedController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IGetIdentityProviderMetadataQuery _getIdentityProviderMetadataQuery;

    public FastFedController(IHttpClientFactory httpClientFactory, IGetIdentityProviderMetadataQuery getIdentityProviderMetadataQuery)
    {
        _httpClientFactory = httpClientFactory;
        _getIdentityProviderMetadataQuery = getIdentityProviderMetadataQuery;
    }


    [HttpGet]
    public IActionResult GetMetadata()
    {
        var result = _getIdentityProviderMetadataQuery.Get();
        return Ok(result);
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