// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
using SimpleIdServer.IdServer.Federation;
using SimpleIdServer.IdServer.Federation.Apis.OpenidConfiguration;
using SimpleIdServer.IdServer.Federation.Builders;
using SimpleIdServer.IdServer.Federation.Helpers;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Stores;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdBuilderExtensions
{
    public static IdServerBuilder AddOpenidFederation(this IdServerBuilder builder, Action<OpenidFederationOptions> cb = null, bool useInMemory = false)
    {
        if (cb != null)
        {
            builder.Services.Configure(cb);
        }
        else
        {
            builder.Services.Configure<OpenidFederationOptions>((o) => { });
        }

        builder.Services.AddTransient<IOpenidProviderFederationEntityBuilder, OpenidProviderFederationEntityBuilder>();
        builder.Services.RemoveAll<IClientHelper>();
        builder.Services.AddTransient<IClientHelper, FederationClientHelper>();
        builder.Services.RemoveAll<IOpenidConfigurationRequestHandler>();
        builder.Services.AddTransient<IOpenidConfigurationRequestHandler, FederationOpenidConfigurationRequestHandler>();
        builder.Services.AddTransient<IClientRegistrationService, ClientRegistrationService>();
        builder.AddRoute("openidFederationMetadata", OpenidFederationConstants.EndPoints.OpenidFederation, new { controller = "OpenidFederation", action = "Get" });
        builder.AddRoute("federationRegistration", OpenidFederationConstants.EndPoints.FederationRegistration, new { controller = "FederationRegistration", action = "Post" });
        builder.AddRoute("addFederationEntity", OpenidFederationConstants.EndPoints.FederationEntities + "/trustanchors", new { controller = "FederationEntities", action = "AddTrustAnchor" });
        builder.AddRoute("searchFederationEntities", OpenidFederationConstants.EndPoints.FederationEntities + "/.search", new { controller = "FederationEntities", action = "SearchFederationEntities" });
        builder.AddRoute("removeFederationEntity", OpenidFederationConstants.EndPoints.FederationEntities + "/{id}", new { controller = "FederationEntities", action = "Delete" });
        var opts = new OpenidFederationOptions();
        if(cb != null)
        {
            cb(opts);
        }

        if (opts.IsFederationEnabled)
        {
            builder.AddRoute("federationFetch", OpenidFederationConstants.EndPoints.FederationFetch, new { controller = "FederationFetch", action = "Get" });
            builder.AddRoute("federationList", OpenidFederationConstants.EndPoints.FederationList, new { controller = "FederationList", action = "Get" });
        }

        if(useInMemory)
        {
            builder.Services.AddSingleton<IFederationEntityStore>(new DefaultFederationEntityStore(new List<FederationEntity>()));
            Seed(builder);
        }

        return builder;
    }

    public static IdServerBuilder AddInMemoryFederationEntities(this IdServerBuilder builder, List<FederationEntity> federationEntities)
    {
        builder.Services.AddSingleton<IFederationEntityStore>(new DefaultFederationEntityStore(federationEntities));
        return builder;
    }

    private static void Seed(IdServerBuilder builder)
    {
        using (var serviceProvider = builder.Services.BuildServiceProvider())
        {
            var scopeRepository = serviceProvider.GetRequiredService<IScopeRepository>();
            scopeRepository.Add(IdServerFederationConstants.StandardScopes.FederationEntities);
        }
    }
}