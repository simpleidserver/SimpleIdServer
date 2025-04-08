// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.EF;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ScimBuilderExtensions
{
    public static ScimBuilder UseEfStore(this ScimBuilder builder, Action<DbContextOptionsBuilder> dbContextOptsCallback = null, Action<ScimEfOptions> optionsCallback = null)
    {
        var services = builder.Services;
        services.AddTransient<ISCIMRepresentationCommandRepository, EFSCIMRepresentationCommandRepository>();
        services.AddTransient<ISCIMRepresentationQueryRepository, EFSCIMRepresentationQueryRepository>();
        services.AddTransient<ISCIMSchemaQueryRepository, EFSCIMSchemaQueryRepository>();
        services.AddTransient<ISCIMSchemaCommandRepository, EFSCIMSchemaCommandRepository>();
        services.AddTransient<ISCIMAttributeMappingQueryRepository, EFSCIMAttributeMappingQueryRepository>();
        services.AddTransient<IProvisioningConfigurationRepository, EFProvisioningConfigurationRepository>();
        services.AddTransient<IRealmRepository, EFRealmRepository>();
        services.AddDbContext<SCIMDbContext>(dbContextOptsCallback);
        if (optionsCallback == null) services.Configure<ScimEfOptions>(o => { });
        else services.Configure(optionsCallback);
        return builder;
    }
}
