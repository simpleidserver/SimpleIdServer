// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Stores;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Federation.Builders;

public interface IOpenidProviderFederationEntityBuilder
{
    Task<OpenidFederationResult> BuildSelfIssued(BuildFederationEntityRequest request, CancellationToken cancellationToken);
}

public class OpenidProviderFederationEntityBuilder : BaseFederationEntityBuilder, IOpenidProviderFederationEntityBuilder
{
    private readonly OpenidFederationOptions _options;
    private readonly IOpenidConfigurationRequestHandler _openidConfigurationRequestHandler;

    public OpenidProviderFederationEntityBuilder(
        IOpenidConfigurationRequestHandler openidConfigurationRequestHandler, 
        IOptions<OpenidFederationOptions> options,
        IFederationEntityStore federationEntityStore) : base(federationEntityStore)
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
        federationEntity.Metadata.OtherParameters.Add(EntityStatementTypes.OpenidProvider, openidProvider);
    }
}
