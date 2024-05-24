// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class ScopeRepository : IScopeRepository
    {
        private readonly DbContext _dbContext;

        public ScopeRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Scope scope)
        {
            _dbContext.Client.InsertNav(Transform(scope))
                .Include(s => s.Realms)
                .Include(s => s.ApiResources)
                .Include(s => s.ClaimMappers)
                .ExecuteCommand();
        }

        public void Update(Scope scope)
        {
            _dbContext.Client.UpdateNav(Transform(scope))
                .Include(s => s.Realms)
                .Include(s => s.ApiResources)
                .Include(s => s.ClaimMappers)
                .ExecuteCommand();
        }

        public void DeleteRange(IEnumerable<Scope> scopes)
        {
            _dbContext.Client.Deleteable(scopes.Select(s => Transform(s)).ToList())
                .ExecuteCommand();
        }

        public async Task<Scope> Get(string realm, string id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarScope>()
                .Includes(p => p.Realms)
                .Includes(p => p.ClaimMappers)
                .Includes(p => p.ApiResources)
                .FirstAsync(p => p.Realms.Any(r => r.RealmsName == realm) && p.ScopesId == id, cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<Scope>> GetAll(string realm, List<string> scopeNames, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarScope>()
                .Includes(s => s.Realms)
                .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(s => s.ToDomain()).ToList();
        }

        public async Task<List<Scope>> GetAllExposedScopes(string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarScope>()
                .Includes(s => s.Realms)
                .Includes(s => s.ClaimMappers)
                .Where(s => s.IsExposedInConfigurationEdp && s.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(s => s.ToDomain()).ToList();
        }

        public async Task<Scope> GetByName(string realm, string scopeName, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarScope>()
                .Includes(s => s.Realms)
                .FirstAsync(s => s.Name == scopeName && s.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<Scope>> GetByNames(string realm, List<string> scopeNames, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarScope>()
                .Includes(s => s.Realms)
                .Includes(s => s.ClaimMappers)
                .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(s => s.ToDomain()).ToList();
        }

        public async Task<SearchResult<Scope>> Search(string realm, SearchScopeRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Client.Queryable<SugarScope>()
                    .Includes(p => p.Realms)
                    .Includes(p => p.Realms)
                    .Where(p => p.Realms.Any(r => r.RealmsName == realm) && ((request.IsRole && p.Type == ScopeTypes.ROLE) || (!request.IsRole && (p.Type == ScopeTypes.IDENTITY || p.Type == ScopeTypes.APIRESOURCE))));
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            else
                query = query.OrderByDescending(s => s.UpdateDateTime);

            if (request.Protocols != null && request.Protocols.Any())
                query = query.Where(q => request.Protocols.Contains(q.Protocol));
            var nb = query.Count();
            var scopes = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new SearchResult<Scope>
            {
                Count = nb,
                Content = scopes.Select(s => s.ToDomain()).ToList()
            };
        }

        private static SugarScope Transform(Scope scope)
        {
            return new SugarScope
            {
                Name = scope.Name,
                CreateDateTime = scope.CreateDateTime,
                Description = scope.Description,
                IsExposedInConfigurationEdp = scope.IsExposedInConfigurationEdp,
                ScopesId = scope.Id,
                Type = scope.Type,
                UpdateDateTime = scope.UpdateDateTime,
                Protocol = scope.Protocol,
                Realms = scope.Realms.Select(r => new SugarRealm
                {
                    RealmsName = r.Name
                }).ToList(),
                ApiResources = scope.ApiResources == null ? new List<SugarApiResource>() : scope.ApiResources.Select(r => new SugarApiResource
                {
                    Id = r.Id
                }).ToList(),
                ClaimMappers = scope.ClaimMappers == null ? new List<SugarScopeClaimMapper>() : scope.ClaimMappers.Select(c => new SugarScopeClaimMapper
                {
                    IncludeInAccessToken = c.IncludeInAccessToken,
                    IsMultiValued = c.IsMultiValued,
                    MapperType = c.MapperType,
                    Name = c.Name,
                    SAMLAttributeName = c.SAMLAttributeName,
                    ScopeClaimMapperId = c.Id,
                    SourceUserAttribute = c.SourceUserAttribute,
                    SourceUserProperty = c.SourceUserProperty,
                    TargetClaimPath = c.TargetClaimPath,
                    TokenClaimJsonType = c.TokenClaimJsonType
                }).ToList()
            };
        }
    }
}
