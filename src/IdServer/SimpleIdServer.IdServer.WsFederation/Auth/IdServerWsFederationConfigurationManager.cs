// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsFederation;
using SimpleIdServer.IdServer.Auth;
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.WsFederation.Auth;

public class IdServerWsFederationConfigurationManager : BaseIdServerConfigurationManager<WsFederationConfiguration>
{
    private readonly IServiceProvider _serviceProvider;

    public IdServerWsFederationConfigurationManager(IServiceProvider serviceProvider, string metadataAddress, IConfigurationRetriever<WsFederationConfiguration> configRetriever, IDocumentRetriever documentRetriever) : base(metadataAddress, configRetriever, documentRetriever)
    {
        _serviceProvider = serviceProvider;
    }

    protected override string GetAddress()
    {
        var realmStore = _serviceProvider.GetRequiredService<IRealmStore>();
        var address = MetadataAddress;
        var realm = realmStore.Realm;
        if (string.IsNullOrWhiteSpace(realm))
            return address;

        if (!address.EndsWith("/"))
            address += "/";
        address += realm;
        address += "/FederationMetadata/2007-06/FederationMetadata.xml";
        return address;
    }
}
