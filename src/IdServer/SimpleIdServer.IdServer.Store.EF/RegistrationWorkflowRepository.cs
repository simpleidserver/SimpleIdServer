// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class RegistrationWorkflowRepository : IRegistrationWorkflowRepository
{
	private readonly StoreDbContext _dbContext;

	public RegistrationWorkflowRepository(StoreDbContext dbContext)
	{
		_dbContext = dbContext;
    }

    public Task<RegistrationWorkflow> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return _dbContext.RegistrationWorkflows
            .SingleOrDefaultAsync(r => r.RealmName == realm && r.Id == id, cancellationToken);
    }

    public Task<RegistrationWorkflow> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        return _dbContext.RegistrationWorkflows
            .SingleOrDefaultAsync(r => r.RealmName == realm && r.Name == name, cancellationToken);
    }

    public Task<RegistrationWorkflow> GetDefault(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.RegistrationWorkflows
            .SingleOrDefaultAsync(r => r.RealmName == realm && r.IsDefault, cancellationToken);
    }

    public Task<List<RegistrationWorkflow>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.RegistrationWorkflows
            .Where(r => r.RealmName == realm)
            .OrderByDescending(r => r.UpdateDateTime)
            .ToListAsync(cancellationToken);
    }

    public void Delete(RegistrationWorkflow record) => _dbContext.RegistrationWorkflows.Remove(record);

	public void Add(RegistrationWorkflow record) => _dbContext.RegistrationWorkflows.Add(record);

    public void Update(RegistrationWorkflow record)
    {

    }
}
