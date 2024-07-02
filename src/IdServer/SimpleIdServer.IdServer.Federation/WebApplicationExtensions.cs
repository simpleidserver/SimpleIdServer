// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            var usePrefix = opts.UseRealm;
            webApplication.SidMapControllerRoute("wsfederationMetadata",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + OpenidFederationConstants.EndPoints.OpenidFederation,
                defaults: new { controller = "OpenidFederation", action = "Get" });
            return webApplication;
        }
    }
}
