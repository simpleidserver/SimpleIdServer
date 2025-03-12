// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public interface ISidRoutesStore
{
    void Add(SidRoute route);
    List<SidRoute> GetAll();
}

public class SidRoutesStore : ISidRoutesStore
{
    private readonly List<SidRoute> _routes;

    public SidRoutesStore()
    {
        _routes = new List<SidRoute>();
    }

    public void Add(SidRoute route)
    {
        _routes.Add(route);
    }

    public List<SidRoute> GetAll()
    {
        return _routes;
    }
}
