// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class SqlSugarTransactionBuilder : ITransactionBuilder
{
    private readonly DbContext _dbContext;

    public SqlSugarTransactionBuilder(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ITransaction Build()
        => new SqlSugarTransaction(_dbContext);
}

public class SqlSugarTransaction : ITransaction
{
    private readonly DbContext _dbContext;

    public SqlSugarTransaction(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Client.BeginTran();
    }

    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        await _dbContext.Client.CommitTranAsync();
        return 1; 
    }

    public void Dispose()
    {

    }
}
