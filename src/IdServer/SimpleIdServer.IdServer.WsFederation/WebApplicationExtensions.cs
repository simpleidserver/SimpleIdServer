// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.WsFederation;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseWsFederation(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;

            var usePrefix = opts.UseRealm;
            webApplication.SidMapControllerRoute("wsfederationMetadata",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + WsFederationConstants.EndPoints.FederationMetadata,
                defaults: new { controller = "Metadata", action = "Get" });

            webApplication.SidMapControllerRoute("ssoLogin",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + WsFederationConstants.EndPoints.SSO,
                defaults: new { controller = "SSO", action = "Login" });

            return webApplication;
        }
    }
}
