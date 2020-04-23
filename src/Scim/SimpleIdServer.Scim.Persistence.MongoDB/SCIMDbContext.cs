using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
	public class SCIMDbContext : IDisposable
	{
		private readonly IMongoDatabase _database;
		private readonly MongoDbOptions _options;

		public SCIMDbContext(IMongoDatabase database, IOptions<MongoDbOptions> options)
		{
			_database = database;
			_options = options.Value;
		}

		public IMongoCollection<SCIMSchema> SCIMSchemaLst =>
			_database.GetCollection<SCIMSchema>(_options.CollectionSchemas);

		public IMongoCollection<SCIMRepresentation> SCIMRepresentationLst =>
			_database.GetCollection<SCIMRepresentation>(_options.CollectionRepresentations);
		
		public IMongoCollection<SCIMAttributeMapping> SCIMAttributeMappingLst =>
			_database.GetCollection<SCIMAttributeMapping>(_options.CollectionMappings);

		public void Dispose()
		{
			//this.Dispose();
		}
	}
}
