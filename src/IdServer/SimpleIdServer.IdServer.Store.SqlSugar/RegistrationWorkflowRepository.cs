// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class RegistrationWorkflowRepository : IRegistrationWorkflowRepository
{
    private readonly DbContext _dbContext;

    public RegistrationWorkflowRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(RegistrationWorkflow record)
    {
        _dbContext.Client.InsertNav(SugarRegistrationWorkflow.Transform(record))
            .Include(a => a.Realm)
            .Include(a => a.Acrs)
            .ExecuteCommand();
    }

    public void Update(RegistrationWorkflow record)
    {
        _dbContext.Client.UpdateNav(SugarRegistrationWorkflow.Transform(record))
            .Include(a => a.Realm)
            .Include(a => a.Acrs)
            .ExecuteCommand();
    }

    public void Delete(RegistrationWorkflow record)
    {
        _dbContext.Client.Deleteable(SugarRegistrationWorkflow.Transform(record)).ExecuteCommand();
    }

    public async Task<RegistrationWorkflow> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarRegistrationWorkflow>()
            .FirstAsync(r => r.RealmName == realm && r.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<RegistrationWorkflow>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarRegistrationWorkflow>()
            .Where(r => r.RealmName == realm)
            .OrderByDescending(r => r.UpdateDateTime)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<RegistrationWorkflow> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarRegistrationWorkflow>()
            .FirstAsync(r => r.RealmName == realm && r.Name == name, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<RegistrationWorkflow> GetDefault(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarRegistrationWorkflow>()
            .FirstAsync(r => r.RealmName == realm && r.IsDefault, cancellationToken);
        return result?.ToDomain();
    }
}
