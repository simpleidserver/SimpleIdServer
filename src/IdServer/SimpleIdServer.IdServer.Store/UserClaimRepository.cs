// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface IUserClaimRepository
{
    IQueryable<UserClaim> Query();
    void Add(UserClaim userClaim);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class UserClaimRepository : IUserClaimRepository
{
    private readonly StoreDbContext _dbContext;

    public UserClaimRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<UserClaim> Query() => _dbContext.UserClaims;

    public void Add(UserClaim userClaim) => _dbContext.UserClaims.Add(userClaim);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
