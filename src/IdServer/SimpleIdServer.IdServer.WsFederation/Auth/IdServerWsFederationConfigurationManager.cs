// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsFederation;
using SimpleIdServer.IdServer.Auth;
using SimpleIdServer.IdServer.Middlewares;
using SimpleIdServer.Realm.Middlewares;

namespace SimpleIdServer.IdServer.WsFederation.Auth
{
    public class IdServerWsFederationConfigurationManager : BaseIdServerConfigurationManager<WsFederationConfiguration>
    {
        public IdServerWsFederationConfigurationManager(string metadataAddress, IConfigurationRetriever<WsFederationConfiguration> configRetriever, IDocumentRetriever documentRetriever) : base(metadataAddress, configRetriever, documentRetriever)
        {
        }

        protected override string GetAddress()
        {
            var address = MetadataAddress;
            var realm = RealmContext.Instance().Realm;
            if (string.IsNullOrWhiteSpace(realm))
                return address;

            if (!address.EndsWith("/"))
                address += "/";
            address += realm;
            address += "/FederationMetadata/2007-06/FederationMetadata.xml";
            return address;
        }
    }
}
