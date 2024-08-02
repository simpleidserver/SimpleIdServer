// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using SimpleIdServer.IdServer.Website.Middlewares;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSidWebsite(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RealmMiddleware>();
        return builder;
    }
}
