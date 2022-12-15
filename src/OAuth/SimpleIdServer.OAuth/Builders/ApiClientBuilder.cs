// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Token.Handlers;

namespace SimpleIdServer.OAuth.Builders
{
    public class ApiClientBuilder
    {
        private readonly Client _client;

        internal ApiClientBuilder(Client client)
        {
            _client = client;
        }

        public ApiClientBuilder AddPasswordGrantType()
        {
            _client.GrantTypes.Add(PasswordHandler.GRANT_TYPE);
            return this;
        }

        public ApiClientBuilder UseOnlyPasswordGrantType()
        {
            _client.GrantTypes.Clear();
            return AddPasswordGrantType();
        }

        public ApiClientBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        public Client Build() => _client;
    }
}
