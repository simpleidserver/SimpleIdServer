// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace SimpleIdServer.IdServer.Migrations.Openiddict;

public class ConfigurationDbcontext : DbContext
{
    public ConfigurationDbcontext(DbContextOptions<ConfigurationDbcontext> options)
        : base(options)
    {
    }

    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications => Set<OpenIddictEntityFrameworkCoreApplication>();
    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes => Set<OpenIddictEntityFrameworkCoreScope>();
}