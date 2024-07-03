// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.OpenidFederation.Apis.FederationList;

public class FederationListController : Controller
{
    private readonly IFederationEntityStore _federationEntityStore;

    public FederationListController(IFederationEntityStore federationEntityStore)
    {
        _federationEntityStore = federationEntityStore;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? "master";
        var result = await _federationEntityStore.GetAll(prefix, cancellationToken);
        return new OkObjectResult(result);
    }
}
