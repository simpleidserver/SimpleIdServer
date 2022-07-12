// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultOAuthClientRepository : IOAuthClientRepository
    {
        private readonly List<OAuthClient> _clients;
        private static Dictionary<string, string> MAPPING_CLIENT_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };

        public DefaultOAuthClientRepository(List<OAuthClient> clients)
        {
            _clients = clients;
        }

        public Task<BaseClient> FindOAuthClientById(string clientId, CancellationToken cancellationToken)
        {
            var res = _clients.FirstOrDefault(c => c.ClientId == clientId);
            if (res == null)
            {
                return Task.FromResult((BaseClient)null);
            }

            return Task.FromResult((BaseClient)res.Clone());
        }

        public Task<IEnumerable<BaseClient>> FindOAuthClientByIds(IEnumerable<string> clientIds, CancellationToken token)
        {
            IEnumerable<BaseClient> result = _clients.Where(c => clientIds.Contains(c.ClientId));
            return Task.FromResult(result);
        }

        public Task<SearchResult<BaseClient>> Find(SearchClientParameter parameter, CancellationToken token)
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
            ICollection<BaseClient> content = result.Cast<BaseClient>().ToList();
            return Task.FromResult(new SearchResult<BaseClient>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content
            });
        }

        public Task<bool> Add(BaseClient data, CancellationToken token)
        {
            var client = (OAuthClient)((OAuthClient)data).Clone();
            _clients.Add(client);
            return Task.FromResult(true);
        }

        public Task<bool> Update(BaseClient data, CancellationToken token)
        {
            var client = (OAuthClient)((OAuthClient)data).Clone();
            var rec = _clients.First(c => c.ClientId == data.ClientId);
            _clients.Remove(rec);
            _clients.Add(client);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(BaseClient data, CancellationToken token)
        {
            var rec = _clients.First(c => c.ClientId == data.ClientId);
            _clients.Remove(rec);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return Task.FromResult(1);
        }

        public Task<List<string>> GetResources(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            var result = new List<string>();
            foreach(var client in _clients)
            {
                if(!result.Contains(client.ClientId) && client.Scopes != null && client.Scopes.Any(s => names.Contains(s.Scope))) result.Add(client.ClientId);
            }

            return Task.FromResult(result);
        }

        public void Dispose()
        {
        }
    }
}