// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Swagger;
using SimpleIdServer.IdServer.Swagger.Migrations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddSwagger(
        this IdServerBuilder builder, 
        Action<IdServerSwaggerApiConfiguration> action = null)
    {
        var conf = new IdServerSwaggerApiConfiguration(builder.Services);
        if (action != null)
        {
            action(conf);
        }

        builder.Services.AddSwaggerGen(o =>
        {
            conf.Options = o;
            conf.Configure();
            conf.AddOAuthSecurity();
            o.SchemaFilter<DescribeEnumMemberValues>();
        });
        builder.Services.RemoveAll<IApiDescriptionGroupCollectionProvider>();
        builder.Services.AddSingleton<IApiDescriptionGroupCollectionProvider, SidApiDescriptionGroupCollectionProvider>();
        return builder;
    }

    public static IdServerBuilder SeedSwagger(this IdServerBuilder builder, List<string> clientRedirectUrls)
    {
        builder.Services.AddTransient<IDataSeeder>((s) =>
        {
            var scope = s.CreateScope();
            var transactionBuilder = scope.ServiceProvider.GetRequiredService<ITransactionBuilder>();
            var realmRepository = scope.ServiceProvider.GetRequiredService<IRealmRepository>();
            var scopeRepository = scope.ServiceProvider.GetRequiredService<IScopeRepository>();
            var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
            var dataSeederRepository = scope.ServiceProvider.GetRequiredService<IDataSeederExecutionHistoryRepository>();
            return new ConfigureSwaggerDataseeder(transactionBuilder, clientRedirectUrls, realmRepository, scopeRepository, clientRepository, dataSeederRepository);
        });
        return builder;
    }
}
