// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.ApplicationProvider.Resolvers;

namespace SimpleIdServer.FastFed.ApplicationProvider.Apis.FastFed;

public interface IGetApplicationProviderMetadataQuery
{
    SimpleIdServer.FastFed.Models.ProviderMetadata Get();
}

public class GetApplicationProviderMetadataQuery : IGetApplicationProviderMetadataQuery
{
    private readonly IIssuerResolver _issuerResolver;
    private readonly FastFedApplicationProviderOptions _options;

    public GetApplicationProviderMetadataQuery(IIssuerResolver issuerResolver, IOptions<FastFedApplicationProviderOptions> options)
    {
        _issuerResolver = issuerResolver;
        _options = options.Value;
    }

    public SimpleIdServer.FastFed.Models.ProviderMetadata Get()
    {
        var result = new SimpleIdServer.FastFed.Models.ProviderMetadata();
        var issuer = _issuerResolver.Get();
        result.ApplicationProvider = new SimpleIdServer.FastFed.Models.ApplicationProviderMetadata
        {
            EntityId = issuer,
            FastfedHandshakeRegisterUri  = $"{issuer}/{RouteNames.Register}",
            ProviderDomain = _options.ProviderDomain,
            Capabilities = _options.Capabilities,
            DisplaySettings = _options.DisplaySettings,
            ContactInformation = _options.ContactInformation
        };
        return result;
    }
}