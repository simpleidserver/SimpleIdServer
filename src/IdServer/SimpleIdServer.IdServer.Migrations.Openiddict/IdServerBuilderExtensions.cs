// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Migrations.Openiddict;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddOpeniddictMigration(this IdServerBuilder idServerBuilder, Action<DbContextOptionsBuilder> action)
    {
        idServerBuilder.Services.AddDbContext<ConfigurationDbcontext>(options =>
        {
            action(options);
            options.UseOpenIddict();
        });

        idServerBuilder.Services.AddDbContext<ApplicationDbContext>(action);
        idServerBuilder.Services.AddTransient<IMigrationService, OpeniddictMigrationService>();
        return idServerBuilder;
    }
}
