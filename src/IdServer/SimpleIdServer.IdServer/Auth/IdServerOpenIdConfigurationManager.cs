// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Helpers;
using System;

namespace SimpleIdServer.IdServer.Auth
{
    public class IdServerOpenIdConfigurationManager : BaseIdServerConfigurationManager<OpenIdConnectConfiguration>
    {
        private readonly IServiceProvider _serviceProvider;

        public IdServerOpenIdConfigurationManager(IServiceProvider serviceProvider, string metadataAddress, IConfigurationRetriever<OpenIdConnectConfiguration> configRetriever, IDocumentRetriever documentRetriever) : base(metadataAddress, configRetriever, documentRetriever)
        {
            _serviceProvider = serviceProvider;
        }

        protected override string GetAddress()
        {
            var realmStore = _serviceProvider.GetRequiredService<IRealmStore>();
            var address = MetadataAddress;
            if (!address.EndsWith("/"))
                address += "/";
            if (!string.IsNullOrWhiteSpace(realmStore.Realm))
                address += realmStore.Realm + "/";

            address += ".well-known/openid-configuration";
            return address;
        }
    }
}
