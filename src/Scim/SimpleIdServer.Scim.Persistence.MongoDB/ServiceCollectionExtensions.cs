// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
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
			List<SCIMAttributeMapping> initialAttributeMapping = null)
		{
			if (mongoDbSetup != null)
			{
				services.Configure(mongoDbSetup);

				var options = new MongoDbOptions();
				mongoDbSetup(options);

				services.AddSingleton<IMongoClient>(_ => new MongoClient(options.ConnectionString));
				services.AddSingleton(provider => provider.GetService<IMongoClient>().GetDatabase(options.Database));

				services.AddSingleton<SCIMDbContext>();

				MongoDbClientExtensions.EnsureMongoDbSCIMDatabaseIsCreated(options, initialSchemas, initialAttributeMapping);
			}

			services.AddSingleton<ISCIMRepresentationCommandRepository, SCIMRepresentationCommandRepository>();
			services.AddSingleton<ISCIMRepresentationQueryRepository, SCIMRepresentationQueryRepository>();
			services.AddSingleton<ISCIMSchemaQueryRepository, SCIMSchemaQueryRepository>();
			services.AddSingleton<ISCIMSchemaCommandRepository, SCIMSchemaCommandRepository>();
			services.AddSingleton<ISCIMAttributeMappingQueryRepository, SCIMAttributeMappingQueryRepository>();
			return services;
		}
	}
}
