// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Infrastructure;

public interface IScimEndpointStore
{
    List<ScimConventionalRouteEntry> Routes { get; }

    void AddRoute(string routeName,
        string pattern,
        RouteValueDictionary? defaults,
        IDictionary<string, object?>? constraints,
        RouteValueDictionary? dataTokens);
}

public class ScimEndpointStore : IScimEndpointStore
{
    public List<ScimConventionalRouteEntry> Routes { get; private set; } = new List<ScimConventionalRouteEntry>();

    public void AddRoute(string routeName,
        string pattern,
        RouteValueDictionary? defaults,
        IDictionary<string, object?>? constraints,
        RouteValueDictionary? dataTokens)
    {
        var conventions = new List<Action<EndpointBuilder>>();
        var finallyConventions = new List<Action<EndpointBuilder>>();
        Routes.Add(new ScimConventionalRouteEntry(routeName, pattern, defaults, constraints, dataTokens, 0, conventions, finallyConventions));
    }
}
