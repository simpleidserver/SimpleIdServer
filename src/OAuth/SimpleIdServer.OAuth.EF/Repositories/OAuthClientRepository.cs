// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.EF.Repositories
{
    public class OAuthClientRepository : IOAuthClientRepository
    {
        private static Dictionary<string, string> MAPPING_CLIENT_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };
        private readonly OAuthDBContext _dbContext;

        public OAuthClientRepository(OAuthDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SearchResult<BaseClient>> Find(SearchClientParameter parameter, CancellationToken token)
        {
            IQueryable<OAuthClient> result = GetClients();
            if (!string.IsNullOrWhiteSpace(parameter.RegistrationAccessToken))
            {
                result = result.Where(c => c.RegistrationAccessToken == parameter.RegistrationAccessToken);
            }

            if (MAPPING_CLIENT_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_CLIENT_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = await result.CountAsync(token);
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            ICollection<BaseClient> content = (await result.ToListAsync(token)).Cast<BaseClient>().ToList();
            return new SearchResult<BaseClient>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content
            };
        }

        public async Task<BaseClient> FindOAuthClientById(string clientId, CancellationToken cancellationToken)
        {
            IQueryable<OAuthClient> clients = GetClients();
            BaseClient result = await clients.FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
            return result;
        }

        public async Task<IEnumerable<BaseClient>> FindOAuthClientByIds(IEnumerable<string> clientIds, CancellationToken token)
        {
            IQueryable<OAuthClient> clients = GetClients();
            IEnumerable<BaseClient> result = await clients.Where(c => clientIds.Contains(c.ClientId)).ToListAsync(token);
            return result;
        }

        private IQueryable<OAuthClient> GetClients()
        {
            return _dbContext.OAuthClients
                .Include(c => c.OAuthAllowedScopes).ThenInclude(s => s.Claims)
                .Include(c => c.Translations).ThenInclude(c => c.Translation)
                .Include(c => c.JsonWebKeys);
        }

        public Task<bool> Add(BaseClient data, CancellationToken token)
        {
            var openidClient = data as OAuthClient;
            _dbContext.OAuthClients.Add(openidClient);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(BaseClient data, CancellationToken token)
        {
            var openidClient = data as OAuthClient;
            _dbContext.OAuthClients.Remove(openidClient);
            return Task.FromResult(true);
        }

        public Task<bool> Update(BaseClient data, CancellationToken token)
        {
            var openidClient = data as OAuthClient;
            _dbContext.OAuthClients.Update(openidClient);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}
