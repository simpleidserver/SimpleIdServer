// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class MongoDbOptions
    {
		public MongoDbOptions()
		{
			ConnectionString = "mongodb://localhost:27017";
			Database = "scim";

			CollectionRepresentations = "representations";
			CollectionSchemas = "schemas";
			CollectionMappings = "mappings";
			SupportTransaction = true;
		}

		public string ConnectionString { get; set; }
		public string Database { get; set; }
		public string CollectionRepresentations { get; set; }
		public string CollectionSchemas { get; set; }
		public string CollectionMappings { get; set; }
		public bool SupportTransaction { get; set; }
	}
}
