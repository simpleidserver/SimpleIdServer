// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.EF;
using System.Collections.Generic;
using System.Linq;

namespace System;

public static class ServiceProviderExtensions
{
    public static void EnsureEfStoreMigrated(this IServiceProvider services, List<SCIMSchema> schemas, List<SCIMAttributeMapping> initialAttributeMapping, List<Realm> realms)
    {
        using (var scope = services.CreateScope())
        {
            using (var dbcontext = scope.ServiceProvider.GetRequiredService<SCIMDbContext>())
            {
                var isInMemory = dbcontext.Database.IsInMemory();
                if (!isInMemory)
                {
                    dbcontext.Database.Migrate();
                }

                if (!dbcontext.SCIMSchemaLst.Any())
                {
                    dbcontext.SCIMSchemaLst.AddRange(schemas);
                }

                if (!dbcontext.SCIMAttributeMappingLst.Any())
                {
                    dbcontext.SCIMAttributeMappingLst.AddRange(initialAttributeMapping);
                }

                if (!dbcontext.Realms.Any())
                {
                    dbcontext.Realms.AddRange(realms);
                }

                dbcontext.SaveChanges();
            }
        }
    }
}
