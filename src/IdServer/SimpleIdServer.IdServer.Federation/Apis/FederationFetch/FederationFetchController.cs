// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OpenidFederation.Apis.FederationFetch;

namespace SimpleIdServer.IdServer.Federation.Apis.FederationFetch;

public class FederationFetchController : BaseFederationFetchController
{
    public FederationFetchController()
    {

    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, [FromQuery] FederationFetchRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var validationResult = Validate(request, issuer);
        return null;
    }
}