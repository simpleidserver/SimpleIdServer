// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.EF;
using System;
using SimpleIdServer.Scim.Persistence.EF.Sqlite;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScimStoreEF(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptsCallback = null, Action<SCIMEFOptions> optionsCallback = null, bool supportSqlite = false)
        {
            if (supportSqlite)
            {
                services.AddTransient<ISCIMRepresentationCommandRepository, SqliteCompatible_EFSCIMRepresentationCommandRepository>();
            }
            else
            {
                services.AddTransient<ISCIMRepresentationCommandRepository, EFSCIMRepresentationCommandRepository>();
            }
            
            services.AddTransient<ISCIMRepresentationQueryRepository, EFSCIMRepresentationQueryRepository>();
            services.AddTransient<ISCIMSchemaQueryRepository, EFSCIMSchemaQueryRepository>();
            services.AddTransient<ISCIMSchemaCommandRepository, EFSCIMSchemaCommandRepository>();
            services.AddTransient<ISCIMAttributeMappingQueryRepository, EFSCIMAttributeMappingQueryRepository>();
            services.AddTransient<IProvisioningConfigurationRepository, EFProvisioningConfigurationRepository>();
            services.AddTransient<IRealmRepository, EFRealmRepository>();
            services.AddDbContext<SCIMDbContext>(dbContextOptsCallback);
            if (optionsCallback == null) services.Configure<SCIMEFOptions>(o => { });
            else services.Configure(optionsCallback);
            return services;
        }
    }
}
