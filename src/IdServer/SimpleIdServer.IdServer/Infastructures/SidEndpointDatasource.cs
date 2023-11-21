// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Infastructures;

public interface ISidEndpointStore
{
    List<SidConventionalRouteEntry> Routes { get; }

    void AddRoute(string routeName,
        string pattern,
        RouteValueDictionary? defaults,
        IDictionary<string, object?>? constraints,
        RouteValueDictionary? dataTokens);
}

public class SidEndpointStore : ISidEndpointStore
{
    public List<SidConventionalRouteEntry> Routes { get; private set; } = new List<SidConventionalRouteEntry>();

    public void AddRoute(string routeName,
        string pattern,
        RouteValueDictionary? defaults,
        IDictionary<string, object?>? constraints,
        RouteValueDictionary? dataTokens)
    {
        var conventions = new List<Action<EndpointBuilder>>();
        var finallyConventions = new List<Action<EndpointBuilder>>();
        Routes.Add(new SidConventionalRouteEntry(routeName, pattern, defaults, constraints, dataTokens, 0, conventions, finallyConventions));
    }
}