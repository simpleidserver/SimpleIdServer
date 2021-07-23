// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Sp;
using SimpleIdServer.Saml.Sp.Apis.Metadata;
using SimpleIdServer.Saml.Sp.Apis.SingleSignOn;
using SimpleIdServer.Saml.Sp.Validators;
using SimpleIdServer.Saml.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamlSp(this IServiceCollection serviceCollection, Action<SamlSpOptions> callback = null)
        {
            serviceCollection.AddMetadataApi()
                .AddSingleSignOnApi()
                .AddValidators()
                .AddSamlInMemoryEF();
            if (callback == null)
            {
                serviceCollection.Configure<SamlSpOptions>((opt) => { });
            } 
            else
            {
                serviceCollection.Configure(callback);
            }

            return serviceCollection;
        }

        private static IServiceCollection AddValidators(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISAMLResponseValidator, SAMLResponseValidator>();
            return serviceCollection;
        }

        private static IServiceCollection AddMetadataApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMetadataHandler, MetadataHandler>();
            return serviceCollection;
        }

        private static IServiceCollection AddSingleSignOnApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuthnRequestGenerator, AuthnRequestGenerator>();
            return serviceCollection;
        }

        private static IServiceCollection AddSamlInMemoryEF(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEntityDescriptorStore, InMemoryEntityDescriptorStore>();
            return serviceCollection;
        }
    }
}
