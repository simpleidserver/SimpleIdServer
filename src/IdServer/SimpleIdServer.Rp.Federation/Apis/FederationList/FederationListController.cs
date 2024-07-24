// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.Rp.Federation.Apis.FederationList;

[Route(OpenidFederationConstants.EndPoints.FederationList)]
public class FederationListController : BaseOpenidFederationController
{
    private readonly IFederationEntityStore _federationEntityStore;
    private readonly RpFederationOptions _options;

    public FederationListController(
        IFederationEntityStore federationEntityStore, 
        IOptions<RpFederationOptions> options)
    {
        _federationEntityStore = federationEntityStore;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        if (!_options.IsFederationEnabled) return Error(System.Net.HttpStatusCode.BadRequest, ErrorCodes.FEDERATION_IS_NOT_ENABLED, SimpleIdServer.OpenidFederation.Resources.Global.FederationIsNotEnabled);
        var result = await _federationEntityStore.GetAllSubordinates(string.Empty, cancellationToken);
        return new OkObjectResult(result.Select(r => r.Sub).ToArray());
    }
}