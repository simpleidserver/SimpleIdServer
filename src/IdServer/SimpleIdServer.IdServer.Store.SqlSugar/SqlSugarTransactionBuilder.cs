// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Stores;

using Transac = SqlSugar.SqlSugarTransaction;
namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class SqlSugarTransactionBuilder : ITransactionBuilder
{
    private readonly DbContext _dbContext;

    public SqlSugarTransactionBuilder(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ITransaction Build()
        => new SqlSugarTransactionInternal(_dbContext);
}

public class SqlSugarTransactionInternal : ITransaction
{
    private readonly DbContext _dbContext;
    private readonly Transac _trans;

    public SqlSugarTransactionInternal(DbContext dbContext)
    {
        _dbContext = dbContext;
        _trans = _dbContext.Client.UseTran();
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        _trans.CommitTran();
        return Task.FromResult(1);
    }

    public void Dispose()
    {

    }
}
