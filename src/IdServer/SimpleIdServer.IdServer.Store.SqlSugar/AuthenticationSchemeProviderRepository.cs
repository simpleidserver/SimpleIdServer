// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class AuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
{
    private readonly DbContext _dbContext;

    public AuthenticationSchemeProviderRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(AuthenticationSchemeProvider idProvider)
    {
        _dbContext.Client.InsertNav(Transform(idProvider))
            .Include(c => c.Realms)
            .Include(c => c.Mappers)
            .ExecuteCommand();
    }

    public async Task<AuthenticationSchemeProvider> Get(string realm, string name, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarAuthenticationSchemeProvider>()
            .Includes(a => a.Realms)
            .Includes(a => a.AuthSchemeProviderDefinition)
            .Includes(a => a.Mappers)
            .FirstAsync(a => a.Realms.Any(r => r.RealmsName == realm) && a.Name == name, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<AuthenticationSchemeProvider>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarAuthenticationSchemeProvider>()
            .Includes(a => a.Realms)
            .Includes(a => a.AuthSchemeProviderDefinition)
            .Where(c => c.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public void Remove(AuthenticationSchemeProvider idProvider)
    {
        _dbContext.Client.Deleteable(Transform(idProvider))
            .ExecuteCommand();
    }

    public async Task<SearchResult<AuthenticationSchemeProvider>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarAuthenticationSchemeProvider>()
                .Includes(p => p.Realms)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm));
        query = query.OrderByDescending(r => r.CreateDateTime);
        /*
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        */

        var nb = query.Count();
        var idProviders = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(cancellationToken);
        return new SearchResult<AuthenticationSchemeProvider>
        {
            Count = nb,
            Content = idProviders.Select(i => i.ToDomain()).ToList()
        };
    }

    public void Update(AuthenticationSchemeProvider idProvider)
    {
        _dbContext.Client.UpdateNav(Transform(idProvider))
            .Include(i => i.Mappers)

            .ExecuteCommand();
    }

    private SugarAuthenticationSchemeProvider Transform(AuthenticationSchemeProvider authenticationSchemeProvider)
    {
        return new SugarAuthenticationSchemeProvider
        {
            Id = authenticationSchemeProvider.Id,
            AuthSchemeProviderDefinitionName = authenticationSchemeProvider.AuthSchemeProviderDefinition?.Name,
            Name = authenticationSchemeProvider.Name,
            UpdateDateTime = authenticationSchemeProvider.UpdateDateTime,
            Description = authenticationSchemeProvider.Description,
            DisplayName = authenticationSchemeProvider.DisplayName,
            CreateDateTime = authenticationSchemeProvider.CreateDateTime,
            Realms = authenticationSchemeProvider.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList(),
            Mappers = authenticationSchemeProvider.Mappers.Select(m => new SugarAuthenticationSchemeProviderMapper
            {
                Id = m.Id,
                Name = m.Name,
                SourceClaimName = m.SourceClaimName,
                MapperType = m.MapperType,
                TargetUserAttribute = m.TargetUserAttribute,
                TargetUserProperty = m.TargetUserProperty
            }).ToList()
        };
    }
}
