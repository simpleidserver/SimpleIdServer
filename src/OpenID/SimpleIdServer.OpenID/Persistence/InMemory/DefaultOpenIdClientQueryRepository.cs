// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultOpenIdClientQueryRepository : IOAuthClientQueryRepository
    {
        public List<OpenIdClient> _clients;

        public DefaultOpenIdClientQueryRepository(List<OpenIdClient> clients)
        {
            _clients = clients;
        }

        public Task<SearchResult<OAuthClient>> Find(SearchClientParameter parameter, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<OAuthClient> FindOAuthClientById(string clientId)
        {
            var res = _clients.FirstOrDefault(c => c.ClientId == clientId);
            if (res == null)
            {
                return Task.FromResult((OAuthClient)null);
            }
            
            return Task.FromResult((OAuthClient)res.Clone());
        }

        public Task<IEnumerable<OAuthClient>> FindOAuthClientByIds(IEnumerable<string> clientIds)
        {
            return Task.FromResult(_clients.Where(c => clientIds.Contains(c.ClientId)).Cast<OAuthClient>());
        }
    }
}
