// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class MongoDbOptions
    {
		public MongoDbOptions()
		{
			ConnectionString = "mongodb://localhost:27017";
			Database = "scim";
			CollectionRepresentations = "representations";
			CollectionRepresentationAttributes = "representationAttributes";
            CollectionSchemas = "schemas";
			CollectionMappings = "mappings";
			CollectionProvisioningLst = "provisioningLst";
			CollectionRealms = "realms";
            SupportTransaction = true;
			BatchSize = 10000;
		}

		public string ConnectionString { get; set; }
		public string Database { get; set; }
		public string CollectionRepresentations { get; set; }
		public string CollectionRepresentationAttributes { get; set; }
		public string CollectionSchemas { get; set; }
		public string CollectionMappings { get; set; }
		public string CollectionProvisioningLst { get; set; }
		public string CollectionRealms { get; set; }

        public bool SupportTransaction { get; set; }
		
		private int _batchSize;
		/// <summary>
		/// MongoDB cursor batch size for large result sets. 
		/// Default is 10000 to optimize performance when loading groups with many members.
		/// Minimum value is 1, maximum recommended is 16MB worth of documents.
		/// </summary>
		public int BatchSize 
		{ 
			get => _batchSize;
			set => _batchSize = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(BatchSize), "BatchSize must be greater than 0");
		}
	}
}
