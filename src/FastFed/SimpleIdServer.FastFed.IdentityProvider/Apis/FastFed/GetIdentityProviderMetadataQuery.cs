// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.IdentityProvider.Resolvers;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.FastFed;

public interface IGetIdentityProviderMetadataQuery
{
    SimpleIdServer.FastFed.Models.ProviderMetadata Get();
}

public class GetIdentityProviderMetadataQuery : IGetIdentityProviderMetadataQuery
{
    private readonly IIssuerResolver _issuerResolver;
    private readonly FastFedIdentityProviderOptions _options;

    public GetIdentityProviderMetadataQuery(IIssuerResolver issuerResolver, IOptions<FastFedIdentityProviderOptions> options)
    {
        _issuerResolver = issuerResolver;
        _options = options.Value;
    }

    public SimpleIdServer.FastFed.Models.ProviderMetadata Get()
    {
        var result = new SimpleIdServer.FastFed.Models.ProviderMetadata();
        var issuer = _issuerResolver.Get();
        result.IdentityProvider = new SimpleIdServer.FastFed.Models.IdentityProviderMetadata
        {
            EntityId = issuer,
            ProviderDomain = _options.ProviderDomain,
            Capabilities = _options.Capabilities,
            DisplaySettings = _options.DisplaySettings,
            ContactInformation = _options.ContactInformation,
            FastFedHandshakeStartUri = $"{issuer}/{RouteNames.Start}",
        };
        return result;
    }
}