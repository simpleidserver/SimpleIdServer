// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace SimpleIdServer.Scim.Infrastructure
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void ScimMapControllerRoute(
            this IEndpointRouteBuilder builder,
            string name,
            [StringSyntax("Route")] string pattern,
            object? defaults = null,
            object? constraints = null,
            object? dataTokens = null)
        {
            builder.MapControllerRoute(name, pattern, defaults, constraints, dataTokens);
            var edpsStore = GetEdpsStore(builder);
            edpsStore.AddRoute(
                name,
                pattern,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new RouteValueDictionary(dataTokens));
        }

        private static IScimEndpointStore GetEdpsStore(IEndpointRouteBuilder builder) => builder.ServiceProvider.GetRequiredService<IScimEndpointStore>();
    }
}
