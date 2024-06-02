// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Store.SqlSugar;

namespace Microsoft.AspNetCore.DataProtection;

public static class DataProtectionBuilderExtensions
{
    public static void PersistKeysToSqlSugar(this IServiceCollection services)
    {
        services.AddSingleton((Func<IServiceProvider, IConfigureOptions<KeyManagementOptions>>)delegate (IServiceProvider services)
        {
            return new ConfigureOptions<KeyManagementOptions>(delegate (KeyManagementOptions options)
            {
                options.XmlRepository = new SqlSugarXmlRepository(services);
            });
        });
    }
}
