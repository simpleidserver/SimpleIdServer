// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.Authority.Federation.Apis.FederationList;

[Route(OpenidFederationConstants.EndPoints.FederationList)]
public class FederationListController
{
    private readonly IFederationEntityStore _federationEntityStore;

    public FederationListController(IFederationEntityStore federationEntityStore)
    {
        _federationEntityStore = federationEntityStore;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _federationEntityStore.GetAllSubordinates(string.Empty, cancellationToken);
        return new OkObjectResult(result.Select(r => r.Sub).ToArray());
    }
}