// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.Resolvers;

namespace SimpleIdServer.FastFed.Apis.FastFedMetadata;

public interface IGetProviderMetadataQuery
{
    Domains.ProviderMetadata Get();
}

public class GetProviderMetadataQuery : IGetProviderMetadataQuery
{
    private readonly IIssuerResolver _issuerResolver;
    private readonly FastFedOptions _options;

    public GetProviderMetadataQuery(IIssuerResolver issuerResolver, IOptions<FastFedOptions> options)
    {
        _issuerResolver = issuerResolver;
        _options = options.Value;
    }

    public virtual SimpleIdServer.FastFed.Domains.ProviderMetadata Get()
    {
        var result = new SimpleIdServer.FastFed.Domains.ProviderMetadata();
        var issuer = _issuerResolver.Get();
        if(_options.IdProvider != null)
        {
            result.IdentityProvider = new SimpleIdServer.FastFed.Domains.IdentityProviderMetadata
            {
                EntityId = issuer,
                ProviderDomain = _options.ProviderDomain,
                Capabilities = _options.IdProvider.Capabilities,
                DisplaySettings = _options.IdProvider.DisplaySettings,
                ContactInformation = _options.IdProvider.ContactInformation,
                FastFedHandshakeStartUri = $"{issuer}{RouteNames.Start}",
                JwksUri = _options.IdProvider.JwksUri
            };
        }

        if(_options.AppProvider != null)
        {
            result.ApplicationProvider = new SimpleIdServer.FastFed.Domains.ApplicationProviderMetadata
            {
                EntityId = issuer,
                FastfedHandshakeRegisterUri = $"{issuer}{RouteNames.Register}",
                ProviderDomain = _options.ProviderDomain,
                Capabilities = _options.AppProvider.Capabilities,
                DisplaySettings = _options.AppProvider.DisplaySettings,
                ContactInformation = _options.AppProvider.ContactInformation
            };
        }
        return result;
    }
}
