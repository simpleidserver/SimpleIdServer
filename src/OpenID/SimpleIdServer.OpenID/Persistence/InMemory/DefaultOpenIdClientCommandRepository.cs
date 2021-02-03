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
        private List<OpenIdClient> _clients;

        public DefaultOpenIdClientCommandRepository(List<OpenIdClient> clients)
        {
            _clients = clients;
        }

        public bool Add(OAuthClient data)
        {
            _clients.Add((OpenIdClient)data.Clone());
            return true;
        }

        public bool Update(OAuthClient data, CancellationToken token)
        {
            var client = (OpenIdClient)data;
            _clients.Remove(_clients.First(c => c.ClientId == client.ClientId));
            _clients.Add(client);
            return true;
        }

        public bool Delete(OAuthClient data)
        {
            _clients.Remove(_clients.First(c => c.ClientId == data.ClientId));
            return true;
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return Task.FromResult(1);
        }

        public void Dispose() { }
    }
}
