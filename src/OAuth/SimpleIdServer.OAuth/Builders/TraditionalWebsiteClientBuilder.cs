// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;

namespace SimpleIdServer.OAuth.Builders
{
    public class TraditionalWebsiteClientBuilder
    {
        private readonly Client _client;

        internal TraditionalWebsiteClientBuilder(Client client) { _client = client; }

        public TraditionalWebsiteClientBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        public Client Build() => _client;
    }
}
