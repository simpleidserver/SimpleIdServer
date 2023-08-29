// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Api;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using SimpleIdServer.Scim.Queries;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register SCIM dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerSCIMBuilder AddSIDScim(this IServiceCollection services, Action<IBusRegistrationConfigurator> massTransitOptions = null)
        {
            var builder = new SimpleIdServerSCIMBuilder(services);
            services.AddMassTransit(massTransitOptions != null ? massTransitOptions : (o) =>
            {
                o.UsingInMemory();
            });
            services.AddCommandHandlers()
                .AddQueryHandlers()
                .AddSCIMRepository()
                .AddHelpers()
                .AddInfrastructure();
            return builder;
        }

        /// <summary>
        /// Register SCIM dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SimpleIdServerSCIMBuilder AddSIDScim(this IServiceCollection services, Action<SCIMHostOptions> options, Action<IBusRegistrationConfigurator> massTransitOptions = null)
        {
            services.Configure(options);
            return services.AddSIDScim(massTransitOptions);
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<IAddRepresentationCommandHandler, AddRepresentationCommandHandler>();
            services.AddTransient<IDeleteRepresentationCommandHandler, DeleteRepresentationCommandHandler>();
            services.AddTransient<IReplaceRepresentationCommandHandler, ReplaceRepresentationCommandHandler>();
            services.AddTransient<IPatchRepresentationCommandHandler, PatchRepresentationCommandHandler>();
            services.AddTransient<ISCIMRepresentationHelper, SCIMRepresentationHelper>();
            return services;
        }

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            services.AddTransient<ISearchRepresentationsQueryHandler, SearchRepresentationsQueryHandler>();
            services.AddTransient<IGetRepresentationQueryHandler, GetRepresentationQueryHandler>();
            return services;
        }

        private static IServiceCollection AddSCIMRepository(this IServiceCollection services)
        {
            var representations = new List<SCIMRepresentation>();
            var representationAttributes = new List<SCIMRepresentationAttribute>();
            var schemas = new List<SCIMSchema>();
            schemas.AddRange(new List<SCIMSchema> 
            { 
                StandardSchemas.UserSchema, 
                StandardSchemas.GroupSchema 
            });
            var provisioningConfigurations = new List<ProvisioningConfiguration>();
            services.TryAddSingleton<ISCIMRepresentationCommandRepository>(new DefaultSCIMRepresentationCommandRepository(representations, representationAttributes));
            services.TryAddSingleton<ISCIMRepresentationQueryRepository>(new DefaultSCIMRepresentationQueryRepository(representations, representationAttributes));
            services.TryAddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
            services.TryAddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
            services.TryAddSingleton<ISCIMAttributeMappingQueryRepository>(new DefaultAttributeMappingQueryRepository(SCIMConstants.StandardAttributeMapping));
            services.TryAddSingleton<IProvisioningConfigurationRepository>(new DefaultProvisioningConfigurationRepository(provisioningConfigurations));
            return services;
        }

        private static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddTransient<IAttributeReferenceEnricher, AttributeReferenceEnricher>();
            services.AddTransient<IRepresentationReferenceSync, RepresentationReferenceSync>();
            services.AddTransient<IResourceTypeResolver, ResourceTypeResolver>();
            services.AddTransient<IRepresentationHelper, RepresentationHelper>();
            services.AddHttpContextAccessor();
            services.AddTransient<IUriProvider, UriProvider>();
            return services;
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            foreach (var assm in AppDomain.CurrentDomain.GetAssemblies())
            {
                services.RegisterAllAssignableType(typeof(BaseApiController), assm, true);
            }

            return services;
        }
    }
}
