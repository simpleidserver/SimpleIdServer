// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System.Collections.Generic;
using System.IO;

namespace SimpleIdServer.Scim
{
    public class SimpleIdServerSCIMBuilder
    {
        public SimpleIdServerSCIMBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        private IServiceCollection ServiceCollection { get; }

        public SimpleIdServerSCIMBuilder AddSchemas(List<SCIMSchema> schemas)
        {
            ServiceCollection.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
            ServiceCollection.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
            return this;
        }

        public SimpleIdServerSCIMBuilder ImportSchemas(Dictionary<string, string> dic)
        {
            var schemaLst = new List<SCIMSchema>();
            foreach (var kvp in dic)
            {
                schemaLst.Add(SCIMSchemaExtractor.Extract(kvp.Value, kvp.Key));
            }

            ServiceCollection.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemaLst));
            ServiceCollection.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemaLst));
            return this;
        }

        public SimpleIdServerSCIMBuilder AddAttributeMapping(List<SCIMAttributeMapping> attributeMappingLst)
        {
            ServiceCollection.AddSingleton<ISCIMAttributeMappingQueryRepository>(new DefaultAttributeMappingQueryRepository(attributeMappingLst));
            return this;
        }
    }
}
