// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Stores;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenidFederation.Builders;

public abstract class BaseFederationEntityBuilder
{
    private readonly IFederationEntityStore _federationEntityStore;

    protected BaseFederationEntityBuilder(IFederationEntityStore federationEntityStore)
    {
        _federationEntityStore = federationEntityStore;
    }

    public async Task<OpenidFederationResult> BuildSelfIssued(BuildFederationEntityRequest request, CancellationToken cancellationToken)
    {
        var authorities = await _federationEntityStore.GetAllAuthorities(request.Realm, cancellationToken);
        var currentDateTime = DateTime.UtcNow;
        var result = new OpenidFederationResult
        {
            Iss = request.Issuer,
            Sub = request.Issuer,
            Iat = EpochTime.GetIntDate(currentDateTime),
            Exp = EpochTime.GetIntDate(currentDateTime.AddMinutes(5)) // Expiration time - 5 minutes.
        };
        var jwks = new OpenidFederationJwksResult
        {
            JsonWebKeys = new List<JsonObject>
            {
                ConvertSigningKey(request.Credential)
            }
        };
        result.Jwks = jwks;
        result.AuthorityHints = authorities.Select(a => a.Sub).ToList();
        var prefix = request.Realm;
        if (!string.IsNullOrWhiteSpace(prefix))
            prefix = $"{prefix}/";
        if (IsFederationEnabled)
        {
            result.Metadata.FederationEntity = new FederationEntityResult
            {
                FederationFetchEndpoint = $"{request.Issuer}/{prefix}{OpenidFederationConstants.EndPoints.FederationFetch}",
                FederationListEndpoint = $"{request.Issuer}/{prefix}{OpenidFederationConstants.EndPoints.FederationList}",
                OrganizationName = OrganizationName
            };
        }

        await EnrichSelfIssued(result, request, cancellationToken);
        return result;
    }

    protected abstract bool IsFederationEnabled { get; }

    protected abstract string OrganizationName { get; }

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