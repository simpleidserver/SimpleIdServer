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

    public IQueryable<RegistrationWorkflow> Query() => _dbContext.RegistrationWorkflows;

    public void Delete(RegistrationWorkflow record) => _dbContext.RegistrationWorkflows.Remove(record);

	public void Add(RegistrationWorkflow record) => _dbContext.RegistrationWorkflows.Add(record);

	public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
