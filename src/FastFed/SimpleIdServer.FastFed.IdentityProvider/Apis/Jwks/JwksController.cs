// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.IdentityProvider.Extensions;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.Jwks;

public class JwksController : Controller
{
    private readonly FastFedIdentityProviderOptions _options;

    public JwksController(IOptions<FastFedIdentityProviderOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var result = new JwksResult();
        var signingKeys = _options.SigningCredentials;
        foreach(var key in signingKeys)
        {
            var publicJwk = key.SerializePublicJWK();
            result.JsonWebKeys.Add(JsonNode.Parse(JsonSerializer.Serialize(publicJwk)).AsObject());
        }

        return new OkObjectResult(result);
    }
}
