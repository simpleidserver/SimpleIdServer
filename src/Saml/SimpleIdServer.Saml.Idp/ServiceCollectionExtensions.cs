// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp.Apis.SSO;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Persistence.InMemory;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamlIdp(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSSOApi();
            serviceCollection.AddSamlInMemoryEF();
            return serviceCollection;
        }

        public static IServiceCollection AddSSOApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISingleSignOnHandler, SingleSignOnHandler>();
            return serviceCollection;
        }

        public static IServiceCollection AddSamlInMemoryEF(this IServiceCollection serviceCollection)
        {
            var relyingParties = new List<RelyingPartyAggregate>();
            serviceCollection.AddSingleton<IRelyingPartyRepository>(new InMemoryRelyingPartyRepository(relyingParties));
            return serviceCollection;
        }
    }
}
