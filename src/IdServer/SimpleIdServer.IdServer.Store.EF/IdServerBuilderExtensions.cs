// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store.EF;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder UseEfStore(this IdServerBuilder builder, Action<DbContextOptionsBuilder> idserverCb, Action<DbContextOptionsBuilder> formbuilderCb)
    {
        builder.Services.AddEfStore(idserverCb);
        builder.FormBuilder.UseEf(formbuilderCb);
        if (builder.DataProtectionBuilder != null)
        {
            builder.DataProtectionBuilder.PersistKeysToDbContext<StoreDbContext>();
        }

        return builder;
    }
}
