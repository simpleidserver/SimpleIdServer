// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
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
    private readonly OpenidFederationOptions _options;
    private readonly IOpenidConfigurationRequestHandler _openidConfigurationRequestHandler;

    public FederationEntityBuilder(
        IOpenidConfigurationRequestHandler openidConfigurationRequestHandler, 
        IOptions<OpenidFederationOptions> options)
    {
        _openidConfigurationRequestHandler = openidConfigurationRequestHandler;
        _options = options.Value;
    }

    protected override bool IsFederationEnabled => _options.IsFederationEnabled;

    protected override string OrganizationName => _options.OrganizationName;

    protected override async Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken)
    {
        var openidProvider = await _openidConfigurationRequestHandler.Handle(federationEntity.Iss, request.Realm, cancellationToken);
        if (federationEntity.Metadata.OtherParameters == null)
            federationEntity.Metadata.OtherParameters = new Dictionary<string, JsonObject>();
        federationEntity.Metadata.OtherParameters.Add("openid_provider", openidProvider);
    }
}
