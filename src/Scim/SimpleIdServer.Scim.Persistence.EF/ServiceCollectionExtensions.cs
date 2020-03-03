// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.EF;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScimStoreEF(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            services.AddTransient<ISCIMRepresentationCommandRepository, EFSCIMRepresentationCommandRepository>();
            services.AddTransient<ISCIMRepresentationQueryRepository, EFSCIMRepresentationQueryRepository>();
            services.AddTransient<ISCIMSchemaQueryRepository, EFSCIMSchemaQueryRepository>();
            services.AddTransient<ISCIMSchemaCommandRepository, EFSCIMSchemaCommandRepository>();
            services.AddDbContext<SCIMDbContext>(optionsAction);
            return services;
        }
    }
}
