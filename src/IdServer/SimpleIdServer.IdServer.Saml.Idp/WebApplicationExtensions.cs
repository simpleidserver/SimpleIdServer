// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Saml.Idp;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseSamlIdp(this WebApplication webApplication)
    {
        var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
        var usePrefix = opts.UseRealm;

        webApplication.MapControllerRoute("getSamlMetadataIdp",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.RouteNames.Metadata,
            defaults: new { controller = "SamlMetadata", action = "Get" });

        webApplication.MapControllerRoute("ssoHttpRedirect",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.RouteNames.SingleSignOnHttpRedirect,
            defaults: new { controller = "SamlSSO", action = "LoginGet" });
        webApplication.MapControllerRoute("ssoArtifact",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.RouteNames.SingleSignOnArtifact,
            defaults: new { controller = "SamlSSO", action = "LoginArtifact" });

        return webApplication;
    }
}