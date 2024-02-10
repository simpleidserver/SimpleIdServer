// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleIdServer.CredentialIssuer;

public class CredentialIssuerServer
{
    private readonly IServiceCollection _services;

    public CredentialIssuerServer(IServiceCollection services)
    {
        _services = services;
    }

    public void UseEfStore(Action<DbContextOptionsBuilder> dbCallback)
    {
        _services.AddStore(dbCallback);
    }

    public void UseInMemoryStore(Action<CredentialIssuerInMemoryStoreBuilder> callback)
    {
        _services.AddStore();
        var serviceProvider = _services.BuildServiceProvider();
        callback(new CredentialIssuerInMemoryStoreBuilder(serviceProvider));
    }
}

public class CredentialIssuerInMemoryStoreBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private static object _lck = new object();

    public CredentialIssuerInMemoryStoreBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CredentialIssuerInMemoryStoreBuilder AddCredentialConfigurations(ICollection<CredentialConfiguration> credentialConfigurations)
    {
        lock(_lck)
        {
            var dbContext = _serviceProvider.GetService<CredentialIssuerDbContext>();
            if (!dbContext.CredentialConfigurations.Any())
            {
                dbContext.CredentialConfigurations.AddRange(credentialConfigurations);
                dbContext.SaveChanges();
            }

            return this;
        }
    }

    public CredentialIssuerInMemoryStoreBuilder AddCredentialOfferRecords(ICollection<CredentialOfferRecord> credentialOfferRecords)
    {
        lock(_lck)
        {
            var dbContext = _serviceProvider.GetService<CredentialIssuerDbContext>();
            if (!dbContext.CredentialOfferRecords.Any())
            {
                dbContext.CredentialOfferRecords.AddRange(credentialOfferRecords);
                dbContext.SaveChanges();
            }

            return this;
        }
    }

    public CredentialIssuerInMemoryStoreBuilder AddCredentials(ICollection<Domains.Credential> credentials)
    {
        lock (_lck)
        {
            var dbContext = _serviceProvider.GetService<CredentialIssuerDbContext>();
            if (!dbContext.Credentials.Any())
            {
                dbContext.Credentials.AddRange(credentials);
                dbContext.SaveChanges();
            }

            return this;
        }
    }

    public CredentialIssuerInMemoryStoreBuilder AddUserCredentialClaims(ICollection<UserCredentialClaim> userClaims)
    {
        lock(_lck)
        {
            var dbContext = _serviceProvider.GetService<CredentialIssuerDbContext>();
            if (!dbContext.UserCredentialClaims.Any())
            {
                dbContext.UserCredentialClaims.AddRange(userClaims);
                dbContext.SaveChanges();
            }

            return this;
        }
    }
}
