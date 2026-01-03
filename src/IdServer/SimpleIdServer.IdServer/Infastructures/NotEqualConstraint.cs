// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Infastructures;

public class NotEqualConstraint : IRouteConstraint
{
    private readonly string[] _excludedValues;

    public NotEqualConstraint(params string[] excludedValues)
    {
        _excludedValues = excludedValues ?? Array.Empty<string>();
    }

    public bool Match(HttpContext httpContext, IRouter route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        var value = values[routeKey]?.ToString();
        return !_excludedValues.Any(excluded => 
            string.Equals(value, excluded, StringComparison.OrdinalIgnoreCase));
    }
}
