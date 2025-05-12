// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityServer.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Migrations.Duende;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddDuendeMigration(this IdServerBuilder idServerBuilder, Action<DbContextOptionsBuilder> action)
    {
        idServerBuilder.Services.AddConfigurationDbContext((c) =>
        {
            c.ConfigureDbContext = action;
        });
        idServerBuilder.Services.AddDbContext<ApplicationDbContext>(action);
        idServerBuilder.Services.AddTransient<IMigrationService, DuendeMigrationService>();
        return idServerBuilder;
    }
}
