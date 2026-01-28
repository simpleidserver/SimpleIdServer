// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseScim(this WebApplication webApp, List<string> additionalRoutes = null)
    {
        webApp.UseAuthentication();
        webApp.UseAuthorization();
        var usePrefix = webApp.Services.GetRequiredService<IOptions<ScimHostOptions>>().Value.EnableRealm;
        webApp.UseMvc(o =>
        {
            ScimApplicationBuilderExtensions.ConfigureScimRoutes(o, usePrefix, additionalRoutes);
        });
        return webApp;
    }
}