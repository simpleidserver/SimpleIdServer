// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface IRegistrationWorkflowRepository
{
	IQueryable<RegistrationWorkflow> Query();
}

public class RegistrationWorkflowRepository : IRegistrationWorkflowRepository
{
	private readonly StoreDbContext _dbContext;

	public RegistrationWorkflowRepository(StoreDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public IQueryable<RegistrationWorkflow> Query() => _dbContext.RegistrationWorkflows;
}
