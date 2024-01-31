// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleIdServer.CredentialIssuer;

public class CredentialIssuerServer
{
    private readonly IServiceCollection _services;

    public CredentialIssuerServer(IServiceCollection services)
    {
        _services = services;
    }

    public CredentialIssuerServer UseEfStore(Action<DbContextOptionsBuilder> dbCallback)
    {
        _services.AddStore(dbCallback);
        return this;
    }

    public CredentialIssuerServer UseInMemoryStore()
    {
        return this;
    }
}
