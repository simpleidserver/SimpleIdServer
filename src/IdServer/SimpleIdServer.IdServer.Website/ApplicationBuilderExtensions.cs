// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Website;
using SimpleIdServer.IdServer.Website.Middlewares;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseIdserverAdmin(this WebApplication builder)
    {
        var options = builder.Services.GetRequiredService<IOptions<IdServerWebsiteOptions>>().Value;
        builder.UseMiddleware<RealmMiddleware>();
        if (options.ForceHttps)
        {
            builder.UseMiddleware<HttpsMiddleware>();
        }

        builder.UseCookiePolicy();
        builder.UseAuthentication();
        builder.UseAuthorization();
        builder.MapControllers();
        return builder;
    }
}
