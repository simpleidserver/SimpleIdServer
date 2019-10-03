using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System.Collections.Generic;

namespace SimpleIdServer.Scim
{
    public class SimpleIdServerSCIMBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SimpleIdServerSCIMBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get => _serviceCollection; }

        public SimpleIdServerSCIMBuilder AddSchemas(List<SCIMSchema> schemas)
        {
            _serviceCollection.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
            _serviceCollection.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
            return this;
        }
    }
}
