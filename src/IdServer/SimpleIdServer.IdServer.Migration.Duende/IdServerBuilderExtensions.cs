// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.IdentityServer.EntityFramework.Options;
using Duende.IdentityServer.EntityFramework.Storage;
using SimpleIdServer.IdServer.Migration.Duende;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddDuendeMigration(this IdServerBuilder idServerBuilder, Action<ConfigurationStoreOptions> action)
    {
        idServerBuilder.Services.AddConfigurationDbContext(action);
        idServerBuilder.Services.AddTransient<ConfigurationMigrationService>();
        return idServerBuilder;
    }
}
