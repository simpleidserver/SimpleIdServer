// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseAutomaticConfiguration(this WebApplication webApplication)
    {
        var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
        var usePrefix = opts.UseRealm;

        webApplication.SidMapControllerRoute("getAllConfDefs",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.Configuration.Constants.RouteNames.ConfigurationDefs,
            defaults: new { controller = "ConfigurationDefs", action = "GetAll" });

        return webApplication;
    }
}