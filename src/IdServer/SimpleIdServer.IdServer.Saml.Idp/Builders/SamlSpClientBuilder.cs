// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;

namespace SimpleIdServer.IdServer.Builders
{
    public class SamlSpClientBuilder
    {
        private readonly Client _client;

        private SamlSpClientBuilder(Client client)
        {
            _client = client;
        }

        public static SamlSpClientBuilder BuildSamlSpClient(string clientId, string metadataUrl, Domains.Realm? realm = null)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientSecret = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = Saml.Idp.Constants.CLIENT_TYPE,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            if (realm == null) client.Realms.Add(Constants.StandardRealms.Master);
            else client.Realms.Add(realm);
            client.Scopes.Add(Constants.StandardScopes.SAMLProfile);
            client.SetSaml2SpMetadataUrl(metadataUrl);
            client.AddSaml2SigningCertificate();
            return new SamlSpClientBuilder(client);
        }

        public Client Build() => _client;
    }
}
