using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSIDScim(this IServiceCollection services)
        {
            services.AddMvc();
            services.AddCommandHandlers()
                .AddSCIMRepository();
            return services;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<IAddRepresentationCommandHandler, AddRepresentationCommandHandler>();
            services.AddTransient<ISCIMRepresentationHelper, SCIMRepresentationHelper>();
            return services;
        }

        private static IServiceCollection AddSCIMRepository(this IServiceCollection services)
        {
            var representations = new List<SCIMRepresentation>();
            var schemas = new List<SCIMSchema>();
            schemas.AddRange(SCIMConstants.StandardSchemas.UserSchemas);
            services.TryAddSingleton<ISCIMRepresentationCommandRepository>(new DefaultSCIMRepresentationCommandRepository(representations));
            services.TryAddSingleton<ISCIMRepresentationQueryRepository>(new DefaultSCIMRepresentationQueryRepository(representations));
            services.TryAddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
            services.TryAddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
            return services;
        }
    }
}
