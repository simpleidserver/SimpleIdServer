// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
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
        private static Dictionary<string, string> MAPPING_CLIENT_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };
        public List<OpenIdClient> _clients;

        public DefaultOpenIdClientQueryRepository(List<OpenIdClient> clients)
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

        public Task<IEnumerable<OAuthClient>> FindOAuthClientByIds(IEnumerable<string> clientIds, CancellationToken token)
        {
            return Task.FromResult(_clients.Where(c => clientIds.Contains(c.ClientId)).Cast<OAuthClient>());
        }

        public Task<SearchResult<OAuthClient>> Find(SearchClientParameter parameter, CancellationToken token)
        {
            var result = _clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(parameter.RegistrationAccessToken))
            {
                result = result.Where(_ => _.RegistrationAccessToken == parameter.RegistrationAccessToken);
            }

            if (MAPPING_CLIENT_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_CLIENT_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new SearchResult<OAuthClient>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = result.ToList().Cast<OAuthClient>().ToList()
            });
        }
    }
}
