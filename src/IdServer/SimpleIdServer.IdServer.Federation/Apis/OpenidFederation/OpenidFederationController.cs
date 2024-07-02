// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Federation.Apis.OpenidFederation;

public class OpenidFederationController : BaseOpenidFederationController
{
    private readonly IKeyStore _keyStore;
    private readonly IOpenidConfigurationRequestHandler _openidConfigurationRequestHandler;

    public OpenidFederationController(IKeyStore keyStore, IOpenidConfigurationRequestHandler openidConfigurationRequestHandler)
    {
        _keyStore = keyStore;
        _openidConfigurationRequestHandler = openidConfigurationRequestHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        var realm = prefix ?? Constants.DefaultRealm;
        var result = await Get(_keyStore.GetAllSigningKeys(realm), cancellationToken);
        var openidProvider = await _openidConfigurationRequestHandler.Handle(result.Iss, realm, cancellationToken);
        result.Metadata = new OpenidFederationMetadataResult
        {
            OtherParameters = new Dictionary<string, JsonObject>
            {
                { "openid_provider", openidProvider }
            }
        };
        return new OkObjectResult(result);
    }
}
