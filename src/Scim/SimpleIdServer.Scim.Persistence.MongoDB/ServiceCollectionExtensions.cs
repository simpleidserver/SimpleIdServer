// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.MongoDB;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddScimStoreMongoDB(
			this IServiceCollection services,
			Action<MongoDbOptions> mongoDbSetup,
			List<SCIMSchema> initialSchemas = null,
			List<SCIMAttributeMapping> initialAttributeMapping = null,
			bool useVersion403 = false)
		{
			if (!useVersion403) SCIMDbContext.RegisterMappings();
			else SCIMDbContext.RegisterMappings403();
			if (mongoDbSetup != null)
			{
				services.Configure(mongoDbSetup);

				var options = new MongoDbOptions();
				mongoDbSetup(options);

				services.AddTransient<IMongoClient>(_ => new MongoClient(options.ConnectionString));
				services.AddTransient(provider => provider.GetService<IMongoClient>().GetDatabase(options.Database));
				services.AddTransient<SCIMDbContext>();
				MongoDbClientExtensions.EnsureMongoDbSCIMDatabaseIsCreated(options, initialSchemas, initialAttributeMapping);
			}

			services.AddTransient<ISCIMRepresentationCommandRepository, SCIMRepresentationCommandRepository>();
			services.AddTransient<ISCIMRepresentationQueryRepository, SCIMRepresentationQueryRepository>();
			services.AddTransient<ISCIMSchemaQueryRepository, SCIMSchemaQueryRepository>();
			services.AddTransient<ISCIMSchemaCommandRepository, SCIMSchemaCommandRepository>();
			services.AddTransient<ISCIMAttributeMappingQueryRepository, SCIMAttributeMappingQueryRepository>();
			services.AddTransient<IProvisioningConfigurationRepository, ProvisioningConfigurationRepository>();
			services.AddTransient<IRealmRepository, RealmRepository>();
			return services;
		}
	}
}
