// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System;

namespace SimpleIdServer.Scim.Infrastructure;

public class ScimConventionalRouteEntry
{
    public readonly RoutePattern Pattern;
    public readonly string RouteName;
    public readonly RouteValueDictionary? DataTokens;
    public readonly int Order;
    public readonly IReadOnlyList<Action<EndpointBuilder>> Conventions;
    public readonly IReadOnlyList<Action<EndpointBuilder>> FinallyConventions;

    public ScimConventionalRouteEntry(
        string routeName,
        string pattern,
        RouteValueDictionary? defaults,
        IDictionary<string, object?>? constraints,
        RouteValueDictionary? dataTokens,
        int order,
        List<Action<EndpointBuilder>> conventions,
        List<Action<EndpointBuilder>> finallyConventions)
    {
        RouteName = routeName;
        DataTokens = dataTokens;
        Order = order;
        Conventions = conventions;
        FinallyConventions = finallyConventions;
        Pattern = RoutePatternFactory.Parse(pattern, defaults, constraints);
    }
}
