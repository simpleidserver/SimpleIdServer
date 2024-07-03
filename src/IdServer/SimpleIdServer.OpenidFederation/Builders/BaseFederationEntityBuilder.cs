// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenidFederation.Builders;

public abstract class BaseFederationEntityBuilder
{
    public async Task<OpenidFederationResult> BuildSelfIssued(BuildFederationEntityRequest request, CancellationToken cancellationToken)
    {
        var result = new OpenidFederationResult
        {
            Iss = request.Issuer,
            Sub = request.Issuer
        };
        var jwks = new OpenidFederationJwksResult
        {
            JsonWebKeys = new List<JsonObject>
            {
                ConvertSigningKey(request.Credential)
            }
        };
        result.Jwks = jwks;
        await EnrichSelfIssued(result, request, cancellationToken);
        return result;
    }

    protected abstract Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken);

    private JsonObject ConvertSigningKey(SigningCredentials signingCredentials)
    {
        var publicJwk = signingCredentials.SerializePublicJWK();
        return JsonNode.Parse(JsonSerializer.Serialize(publicJwk)).AsObject();
    }
}

public class BuildFederationEntityRequest
{
    public string Issuer { get; set; }
    public string Realm { get; set; }
    public SigningCredentials Credential { get; set; }
}