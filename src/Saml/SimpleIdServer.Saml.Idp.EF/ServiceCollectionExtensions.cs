// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Saml.Idp.EF;
using SimpleIdServer.Saml.Idp.EF.Repositories;
using SimpleIdServer.Saml.Idp.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamlIdpEF(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            serviceCollection.AddDbContext<SamlIdpDBContext>(optionsAction);
            serviceCollection.AddTransient<IUserRepository, UserRepository>();
            serviceCollection.AddTransient<IRelyingPartyRepository, RelyingPartyRepository>();
            return serviceCollection;
        }
    }
}
