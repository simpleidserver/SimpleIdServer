// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Saml.Idp;
using SimpleIdServer.Saml.Idp.Apis.Metadata;
using SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Persistence.InMemory;
using SimpleIdServer.Saml.Stores;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static SamlIdProviderBuilder AddSamlIdp(this IServiceCollection serviceCollection, Action<SamlIdpOptions> callback = null)
        {
            serviceCollection.AddSSOApi();
            serviceCollection.AddMetadataApi();
            serviceCollection.AddRelyingPartyApi();
            serviceCollection.AddSamlInMemoryEF();
            if (callback == null)
            {
                serviceCollection.Configure<SamlIdpOptions>((opt) => { });
            } 
            else
            {
                serviceCollection.Configure(callback);
            }

            return new SamlIdProviderBuilder(serviceCollection);
        }

        private static IServiceCollection AddSSOApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISingleSignOnHandler, SingleSignOnHandler>();
            return serviceCollection;
        }

        private static IServiceCollection AddMetadataApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMetadataHandler, MetadataHandler>();
            return serviceCollection;
        }

        private static IServiceCollection AddRelyingPartyApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISearchRelyingPartiesHandler, SearchRelyingPartiesHandler>();
            serviceCollection.AddTransient<IAddRelyingPartyHandler, AddRelyingPartyHandler>();
            serviceCollection.AddTransient<IGetRelyingPartyHandler, GetRelyingPartyHandler>();
            serviceCollection.AddTransient<IUpdateRelyingPartyHandler, UpdateRelyingPartyHandler>();
            return serviceCollection;
        }

        private static IServiceCollection AddSamlInMemoryEF(this IServiceCollection serviceCollection)
        {
            var relyingParties = new List<RelyingPartyAggregate>();
            var users = new List<User>();
            serviceCollection.AddSingleton<IRelyingPartyRepository>(new InMemoryRelyingPartyRepository(relyingParties));
            serviceCollection.AddSingleton<IUserRepository>(new InMemoryUserRepository(users));
            serviceCollection.AddSingleton<IEntityDescriptorStore, InMemoryEntityDescriptorStore>();
            return serviceCollection;
        }
    }
}
