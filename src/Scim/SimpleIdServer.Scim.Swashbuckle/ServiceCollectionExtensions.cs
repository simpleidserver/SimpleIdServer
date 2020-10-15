// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SCIMSwashbuckleServiceCollectionExtensions
    {
        public static IServiceCollection AddSCIMSwagger(this IServiceCollection services)
        {
            services.AddTransient<ISchemaGenerator, SCIMSchemaGenerator>();
            return services;
        }
    }
}
