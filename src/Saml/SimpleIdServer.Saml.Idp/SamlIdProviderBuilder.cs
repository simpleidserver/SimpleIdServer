// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Persistence.InMemory;
using System.Collections.Generic;

namespace SimpleIdServer.Saml.Idp
{
    public class SamlIdProviderBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        internal SamlIdProviderBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public SamlIdProviderBuilder AddRelyingParties(ICollection<RelyingPartyAggregate> relyingParties)
        {
            _serviceCollection.AddSingleton<IRelyingPartyRepository>(new InMemoryRelyingPartyRepository(relyingParties));
            return this;
        }
    }
}
