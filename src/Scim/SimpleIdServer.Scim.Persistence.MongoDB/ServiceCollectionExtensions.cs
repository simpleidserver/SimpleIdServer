// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.MongoDB;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddScimStoreMongoDB(this IServiceCollection services, Action<MongoDbOptions> mongoDbSetup = null)
		{
			if (mongoDbSetup != null)
			{
				services.Configure(mongoDbSetup);

				var options = new MongoDbOptions();
				mongoDbSetup(options);

				services.AddSingleton<IMongoClient>(s => new MongoClient(options.ConnectionString));
				services.AddSingleton(provider => provider.GetService<IMongoClient>().GetDatabase(options.Database));
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
