// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using SimpleIdServer.OpenID.Middlewares;

namespace SimpleIdServer.OpenID
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UILocalesMiddleware>();
        }
    }
}
