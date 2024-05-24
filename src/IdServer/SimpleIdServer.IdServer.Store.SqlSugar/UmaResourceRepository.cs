// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit.Initializers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class UmaResourceRepository : IUmaResourceRepository
{
    private readonly DbContext _dbContext;

    public UmaResourceRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(UMAResource resource)
    {
        _dbContext.Client.InsertNav(Transform(resource))
            .Include(r => r.Translations)
            .Include(r => r.Permissions)
            .ExecuteCommand();
    }

    public void Delete(UMAResource resource)
    {
        _dbContext.Client.Deleteable(Transform(resource))
            .ExecuteCommand();
    }

    public void Update(UMAResource resource)
    {
        _dbContext.Client.UpdateNav(Transform(resource))
            .Include(r => r.Translations)
            .Include(r => r.Permissions)
            .ExecuteCommand();
    }

    public async Task<UMAResource> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUMAResource>()
            .Includes(r => r.Translations)
            .Includes(r => r.Permissions, r => r.Claims)
            .FirstAsync(r => r.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<UMAResource>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUMAResource>().ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<UMAResource>> GetByIds(List<string> resourceIds, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUMAResource>()
            .Includes(r => r.Permissions, p => p.Claims)
            .Where(r => resourceIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static SugarUMAResource Transform(UMAResource resource)
    {
        return new SugarUMAResource
        {
            CreateDateTime = resource.CreateDateTime,
            IconUri = resource.IconUri,
            Id = resource.Id,
            Realm = resource.Realm,
            Scopes = resource.Scopes == null ? string.Empty : resource.Scopes.Join(","),
            Subject = resource.Subject,
            Type = resource.Type,
            UpdateDateTime = resource.UpdateDateTime,
            Permissions = resource.Permissions == null ? new List<SugarUMAResourcePermission>() : resource.Permissions.Select(p => new SugarUMAResourcePermission
            {
                CreateDateTime = p.CreateDateTime,
                Scopes = p.Scopes == null ? string.Empty : p.Scopes.Join(","),
                Claims = p.Claims == null ? new List<SugarUMAResourcePermissionClaim>() : p.Claims.Select(c => new SugarUMAResourcePermissionClaim
                {
                    ClaimType = c.ClaimType,
                    FriendlyName = c.FriendlyName,
                    Name = c.Name,
                    Value = c.Value
                }).ToList()
            }).ToList(),
            Translations = resource.Translations == null ? new List<SugarTranslation>() : resource.Translations.Select(t => new SugarTranslation
            {
                Key = t.Key,
                Value = t.Value,
                Language = t.Language
            }).ToList()
        };
    }
}
