// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains.Clients
{
    public class DefaultOAuthClientRepository : IOAuthClientQueryRepository
    {
        public List<OAuthClient> _clients;

        public DefaultOAuthClientRepository(List<OAuthClient> clients)
        {
            _clients = clients;
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
            return Task.FromResult(_clients.Where(c => clientIds.Contains(c.ClientId)));
        }
    }
}
