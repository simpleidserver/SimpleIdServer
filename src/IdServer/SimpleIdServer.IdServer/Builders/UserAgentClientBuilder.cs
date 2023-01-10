// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class UserAgentClientBuilder
    {
        private readonly Client _client;

        internal UserAgentClientBuilder(Client client) { _client = client; }

        public UserAgentClientBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(new Scope { Name = scope });
            return this;
        }

        public Client Build() => _client;
    }
}
