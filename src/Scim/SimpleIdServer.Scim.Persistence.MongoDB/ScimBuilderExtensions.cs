// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.MongoDB;
using System;

namespace SimpleIdServer.Scim;

public static class ScimBuilderExtensions
{
	public static ScimBuilder UseMongodbStorage(this ScimBuilder builder, Action<MongoDbOptions> mongoDbSetup)
	{
		var services = builder.ServiceCollection;
		SCIMDbContext.RegisterMappings();
		services.Configure(mongoDbSetup);
		var options = new MongoDbOptions();
		mongoDbSetup(options);
		services.AddTransient<IMongoClient>(_ => new MongoClient(options.ConnectionString));
		services.AddTransient(provider => provider.GetService<IMongoClient>().GetDatabase(options.Database));
		services.AddTransient<SCIMDbContext>();
		services.AddTransient<ISCIMRepresentationCommandRepository, SCIMRepresentationCommandRepository>();
		services.AddTransient<ISCIMRepresentationQueryRepository, SCIMRepresentationQueryRepository>();
		services.AddTransient<ISCIMSchemaQueryRepository, SCIMSchemaQueryRepository>();
		services.AddTransient<ISCIMSchemaCommandRepository, SCIMSchemaCommandRepository>();
		services.AddTransient<ISCIMAttributeMappingQueryRepository, SCIMAttributeMappingQueryRepository>();
		services.AddTransient<IProvisioningConfigurationRepository, ProvisioningConfigurationRepository>();
		services.AddTransient<IRealmRepository, RealmRepository>();
		return builder;
	}
}
