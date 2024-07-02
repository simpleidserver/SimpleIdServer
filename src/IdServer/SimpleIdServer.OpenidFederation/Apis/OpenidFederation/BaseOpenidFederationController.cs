// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenidFederation.Apis.OpenidFederation;

public class BaseOpenidFederationController : Controller
{
    protected async Task<OpenidFederationResult> Get(IEnumerable<SigningCredentials> signingKeys, CancellationToken cancellationToken)
    {
        // TODO : returns application/entity-statement+jwt
        var issuer = GetAbsoluteUriWithVirtualPath(Request);
        var jwks = new OpenidFederationJwksResult();
        foreach (var key in signingKeys)
            jwks.JsonWebKeys.Add(ConvertSigningKey(key));
        var result = new OpenidFederationResult
        {
            Iss = issuer,
            Sub = issuer,
            Jwks = jwks
        };
        return result;

        JsonObject ConvertSigningKey(SigningCredentials signingCredentials)
        {
            var publicJwk = signingCredentials.SerializePublicJWK();
            return JsonNode.Parse(JsonSerializer.Serialize(publicJwk)).AsObject();
        }
    }

    private static string GetAbsoluteUriWithVirtualPath(HttpRequest requestMessage)
    {
        var host = requestMessage.Host.Value;
        var http = "http://";
        if (requestMessage.IsHttps) http = "https://";
        var relativePath = requestMessage.PathBase.Value;
        return http + host + relativePath;
    }
}