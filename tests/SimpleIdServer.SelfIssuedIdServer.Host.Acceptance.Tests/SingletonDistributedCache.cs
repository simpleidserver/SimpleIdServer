// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleIdServer.SelfIdServer.Host.Acceptance.Tests;

public class SingletonDistributedCache
{
    private static SingletonDistributedCache _instance;
    private IDistributedCache _cache;

    private SingletonDistributedCache()
    {
        _cache = new MemoryDistributedCache(Microsoft.Extensions.Options.Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
    }

    public static SingletonDistributedCache Instance()
    {
        if (_instance == null)
        {
            _instance = new SingletonDistributedCache();
        }

        return _instance;
    }

    public IDistributedCache Get() => _cache;
}
