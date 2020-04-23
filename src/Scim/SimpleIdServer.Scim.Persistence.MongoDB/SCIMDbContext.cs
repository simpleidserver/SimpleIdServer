// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMDbContext : IDisposable
	{
		private readonly MongoDbOptions _options;

		public SCIMDbContext(IMongoDatabase database, IOptions<MongoDbOptions> options)
		{
            Database = database;
			_options = options.Value;
		}

        internal IMongoDatabase Database { get; private set; }

        public IMongoCollection<SCIMRepresentationModel> SCIMRepresentationLst =>
            Database.GetCollection<SCIMRepresentationModel>(_options.CollectionRepresentations);

		public IMongoCollection<SCIMSchemaModel> SCIMSchemaLst =>
            Database.GetCollection<SCIMSchemaModel>(_options.CollectionSchemas);
		
		public IMongoCollection<SCIMAttributeMappingModel> SCIMAttributeMappingLst =>
            Database.GetCollection<SCIMAttributeMappingModel>(_options.CollectionMappings);

		public void Dispose() { }
	}
}
