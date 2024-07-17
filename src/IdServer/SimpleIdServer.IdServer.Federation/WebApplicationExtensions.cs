// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Federation;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.OpenidFederation;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseOpenidFederation(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            var federationOpts = webApplication.Services.GetRequiredService<IOptions<OpenidFederationOptions>>().Value;
            var usePrefix = opts.UseRealm;
            webApplication.SidMapControllerRoute("openidFederationMetadata",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + OpenidFederationConstants.EndPoints.OpenidFederation,
                defaults: new { controller = "OpenidFederation", action = "Get" });
            webApplication.SidMapControllerRoute("federationRegistration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + OpenidFederationConstants.EndPoints.FederationRegistration,
                defaults: new { controller = "FederationRegistration", action = "Post" });
            if (federationOpts.IsFederationEnabled)
            {
                webApplication.SidMapControllerRoute("federationFetch",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + OpenidFederationConstants.EndPoints.FederationFetch,
                    defaults: new { controller = "FederationFetch", action = "Get" });
                webApplication.SidMapControllerRoute("federationList",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + OpenidFederationConstants.EndPoints.FederationList,
                    defaults: new { controller = "FederationList", action = "Get" });
            }

            return webApplication;
        }
    }
}
