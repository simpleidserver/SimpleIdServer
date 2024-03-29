﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ITransaction : IDisposable, IAsyncDisposable
    {
        Task Commit(CancellationToken token = default(CancellationToken));
    }
}
