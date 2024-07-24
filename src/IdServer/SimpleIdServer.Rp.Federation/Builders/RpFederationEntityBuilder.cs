// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Stores;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Rp.Federation.Builders;

public interface IRpFederationEntityBuilder
{
    Task<OpenidFederationResult> BuildSelfIssued(BuildFederationEntityRequest request, CancellationToken cancellationToken);
}

public class RpFederationEntityBuilder : BaseFederationEntityBuilder, IRpFederationEntityBuilder
{
    private readonly RpFederationOptions _options;

    public RpFederationEntityBuilder(IOptions<RpFederationOptions> options, IFederationEntityStore federationEntityStore) : base(federationEntityStore)
    {
        _options = options.Value;
    }

    protected override bool IsFederationEnabled => _options.IsFederationEnabled;

    protected override string OrganizationName => _options.OrganizationName;

    protected override Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken)
    {
        if (federationEntity.Metadata.OtherParameters == null)
            federationEntity.Metadata.OtherParameters = new Dictionary<string, JsonObject>();
        var client = JsonObject.Parse(JsonSerializer.Serialize(_options.Client)).AsObject();
        federationEntity.Metadata.OtherParameters.Add(EntityStatementTypes.OpenidRelyingParty, client);
        return Task.CompletedTask;
    }
}