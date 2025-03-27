// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace SimpleIdServer.IdServer.Infastructures;

public class NotEqualConstraint : IRouteConstraint
{
    private readonly string _excludedValue;

    public NotEqualConstraint(string excludedValue)
    {
        _excludedValue = excludedValue;
    }

    public bool Match(HttpContext httpContext, IRouter route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        return !string.Equals(values[routeKey]?.ToString(),
            _excludedValue,
            StringComparison.OrdinalIgnoreCase);
    }
}
