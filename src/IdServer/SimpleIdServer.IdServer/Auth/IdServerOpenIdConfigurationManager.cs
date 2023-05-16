// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.Realm.Middlewares;

namespace SimpleIdServer.IdServer.Auth
{

    public class IdServerOpenIdConfigurationManager : BaseIdServerConfigurationManager<OpenIdConnectConfiguration>
    {
        public IdServerOpenIdConfigurationManager(string metadataAddress, IConfigurationRetriever<OpenIdConnectConfiguration> configRetriever, IDocumentRetriever documentRetriever) : base(metadataAddress, configRetriever, documentRetriever)
        {
        }

        protected override string GetAddress()
        {
            var address = MetadataAddress;
            if (!address.EndsWith("/"))
                address += "/";

            if (!string.IsNullOrWhiteSpace(RealmContext.Instance().Realm))
                address += RealmContext.Instance().Realm + "/";

            address += ".well-known/openid-configuration";
            return address;
        }
    }
}
