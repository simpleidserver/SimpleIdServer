// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.MongoDB;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScimStoreMongoDB(this IServiceCollection services, Action<MongoDbOptions> callback = null)
        {
            if (callback != null)
            {
                services.Configure(callback);
            }

            services.AddTransient<ISCIMRepresentationCommandRepository, SCIMRepresentationCommandRepository>();
            services.AddTransient<ISCIMRepresentationQueryRepository, SCIMRepresentationQueryRepository>();
            services.AddTransient<ISCIMSchemaQueryRepository, SCIMSchemaQueryRepository>();
            services.AddTransient<ISCIMSchemaCommandRepository, SCIMSchemaCommandRepository>();
            services.AddTransient<ISCIMAttributeMappingQueryRepository, SCIMAttributeMappingQueryRepository>();
            return services;
        }
    }
}
