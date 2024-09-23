// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.FastFed;

namespace Microsoft.AspNetCore.Builder;

public static class FastFedWebApplicationBuilderExtensions
{
    public static FastFedWebApplicationBuilder UseIdentityProvider(this FastFedWebApplicationBuilder builder)
    {
        var reccuringJobManager = builder.WebApplication.Services.GetRequiredService<IRecurringJobManager>();
        // reccuringJobManager.AddOrUpdate<ProvisionRepresentationsJob>(nameof(ProvisionRepresentationsJob), j => builder.WebApplication.Services.GetRequiredService<ProvisionRepresentationsJob>().Execute(CancellationToken.None), "*/10 * * * * *");
        builder.WebApplication.MapControllerRoute(name: "StartFastFed",
            pattern: RouteNames.Start,
            defaults: new { controller = "FastFed", action = "Start" });
        builder.WebApplication.MapControllerRoute(name: "GetJwks",
            pattern: RouteNames.Jwks,
            defaults: new { controller = "Jwks", action = "Get" });
        return builder;
    }
}
