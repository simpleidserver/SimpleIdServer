// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Federation.Builders;

public interface IFederationEntityBuilder
{
    Task<OpenidFederationResult> BuildSelfIssued(BuildFederationEntityRequest request, CancellationToken cancellationToken);
}

public class FederationEntityBuilder : BaseFederationEntityBuilder, IFederationEntityBuilder
{
    private readonly IOpenidConfigurationRequestHandler _openidConfigurationRequestHandler;

    public FederationEntityBuilder(IOpenidConfigurationRequestHandler openidConfigurationRequestHandler)
    {
        _openidConfigurationRequestHandler = openidConfigurationRequestHandler;
    }

    protected override async Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken)
    {
        var openidProvider = await _openidConfigurationRequestHandler.Handle(federationEntity.Iss, request.Realm, cancellationToken);
        federationEntity.Metadata = new OpenidFederationMetadataResult
        {
            OtherParameters = new Dictionary<string, JsonObject>
            {
                { "openid_provider", openidProvider }
            }
        };
    }
}
