// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultOpenIdClientCommandRepository : IOAuthClientCommandRepository
    {
        private readonly List<OpenIdClient> _clients;
        private readonly List<OpenIdScope> _scopes;

        public DefaultOpenIdClientCommandRepository(List<OpenIdClient> clients, List<OpenIdScope> scopes)
        {
            _clients = clients;
            _scopes = scopes;
        }

        public bool Add(OAuthClient data)
        {
            data.AllowedScopes = _scopes.Where(s => data.AllowedScopes.Any(_ => _.Name == s.Name)).ToList();
            _clients.Add((OpenIdClient)data.Clone());
            return true;
        }

        public Task<bool> Update(OAuthClient data, CancellationToken token)
        {
            var client = (OpenIdClient)data;
            _clients.Remove(_clients.First(c => c.ClientId == client.ClientId));
            _clients.Add(client);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(OAuthClient data, CancellationToken cancellationToken)
        {
            _clients.Remove(_clients.First(c => c.ClientId == data.ClientId));
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return Task.FromResult(1);
        }

        public void Dispose() { }
    }
}
