// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class EFTransactionBuilder : ITransactionBuilder
{
    private readonly StoreDbContext _dbContext;

    public EFTransactionBuilder(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ITransaction Build()
        => new EFTransaction(_dbContext);
}

public class EFTransaction : ITransaction
{
    private readonly StoreDbContext _dbContext;

    public EFTransaction(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
    }
}
