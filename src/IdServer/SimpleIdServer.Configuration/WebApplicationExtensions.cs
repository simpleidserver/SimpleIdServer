// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Linq;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication SeedOptionDefinitions(this WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var options = scope.ServiceProvider.GetService<AutomaticConfigurationOptions>();
            using (var dbContext = scope.ServiceProvider.GetService<StoreDbContext>())
            {
                var names = options.ConfigurationDefinitions.Select(c => c.Name);
                var missingConfigurations = dbContext.Definitions.Where(d => !names.Contains(d.Id));
                // dbContext.Definitions.AddRange(missingConfigurations);
                // dbContext.SaveChanges();
            }
        }

        return app;
    }

    public static WebApplication UseAutomaticConfiguration(this WebApplication webApplication)
    {
        var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
        var usePrefix = opts.UseRealm;
        return webApplication;
    }
}