// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultTranslationBuilder : ITransactionBuilder
{
    public ITransaction Build()
    {
        return new DefaultTransaction();
    }
}

public class DefaultTransaction : ITransaction
{
    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }

    public void Dispose()
    {
    }
}