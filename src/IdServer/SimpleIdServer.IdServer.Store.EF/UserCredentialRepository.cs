// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class UserCredentialRepository : IUserCredentialRepository
{
    private readonly StoreDbContext _dbContext;

    public UserCredentialRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<UserCredential> Query() => _dbContext.UserCredential;
}
