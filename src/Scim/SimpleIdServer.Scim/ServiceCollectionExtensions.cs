// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using AspNetCore.Authentication.ApiKey;
using MassTransit;
using MassTransit.MessageData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Api;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using SimpleIdServer.Scim.Queries;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ScimBuilder AddScim(this IServiceCollection services, Action<ScimHostOptions> options = null, bool skipMasstransitRegistration = false, bool skipAuth = false)
    {
        var builder = new ScimBuilder(services);
        if(options != null)
        {
            services.Configure(options);
        }
        else
        {
            services.Configure<ScimHostOptions>((o) => { });
        }

        services.Configure<RouteOptions>(opt =>
        {
            opt.ConstraintMap.Add("realmPrefix", typeof(RealmRoutePrefixConstraint));
        });
        services.AddMvc(o =>
        {
            o.EnableEndpointRouting = false;
            o.AddScimValueProviders();
        }).AddNewtonsoftJson();
        if(!skipMasstransitRegistration)
        {
            var repository = new InMemoryMessageDataRepository();
            services.AddSingleton<IMessageDataRepository>(repository);
            services.AddMassTransit((o) =>
            {
                o.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageData(repository);
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
        services.ConfigureCommands()
            .ConfigureQueries()
            .ConfigureRepositories()
            .ConfigureHelpers()
            .ConfigureInfrastructures();
        if(!skipAuth)
        {
            services.ConfigureAuth();
        }

        return builder;
    }

    private static IServiceCollection ConfigureCommands(this IServiceCollection services)
    {
        services.AddTransient<IAddRepresentationCommandHandler, AddRepresentationCommandHandler>();
        services.AddTransient<IDeleteRepresentationCommandHandler, DeleteRepresentationCommandHandler>();
        services.AddTransient<IReplaceRepresentationCommandHandler, ReplaceRepresentationCommandHandler>();
        services.AddTransient<IPatchRepresentationCommandHandler, PatchRepresentationCommandHandler>();
        services.AddTransient<IRepresentationHelper, RepresentationHelper>();
        return services;
    }

    private static IServiceCollection ConfigureQueries(this IServiceCollection services)
    {
        services.AddTransient<ISearchRepresentationsQueryHandler, SearchRepresentationsQueryHandler>();
        services.AddTransient<IGetRepresentationQueryHandler, GetRepresentationQueryHandler>();
        return services;
    }

    private static IServiceCollection ConfigureRepositories(this IServiceCollection services)
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
        services.TryAddSingleton<IRealmRepository>(new DefaultRealmRepository(SCIMConstants.StandardRealms));
        return services;
    }

    private static IServiceCollection ConfigureHelpers(this IServiceCollection services)
    {
        services.AddTransient<IRepresentationVersionBuilder, IncrementalRepresentationVersionBuilder>();
        services.AddTransient<IAttributeReferenceEnricher, AttributeReferenceEnricher>();
        services.AddTransient<IRepresentationReferenceSync, RepresentationReferenceSync>();
        services.AddTransient<IResourceTypeResolver, ResourceTypeResolver>();
        services.AddTransient<IRepresentationHelper, RepresentationHelper>();
        services.AddHttpContextAccessor();
        services.AddTransient<IUriProvider, UriProvider>();
        services.AddTransient<IBusHelper, BusHelper>();
        return services;
    }

    private static IServiceCollection ConfigureInfrastructures(this IServiceCollection services)
    {
        foreach (var assm in AppDomain.CurrentDomain.GetAssemblies())
        {
            services.RegisterAllAssignableType(typeof(BaseApiController), assm, true);
        }

        services.AddSingleton<IScimEndpointStore, ScimEndpointStore>();
        return services;
    }

    private static IServiceCollection ConfigureAuth(this IServiceCollection services)
    {
        services.AddSingleton(ApiKeysConfiguration.Default);
        services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
            .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
            {
                options.Realm = "Sample Web API";
                options.KeyName = "Authorization";
            });
        services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
        return services;
    }
}
