// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseVerifiablePresentation(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            var usePrefix = opts.UseRealm;

            webApplication.SidMapControllerRoute("getPresentationDefinition",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.PresentationDefinitions,
                defaults: new { controller = "PresentationDefinitions", action = "Get" });

            webApplication.SidMapControllerRoute("vpAuthorizeCallback",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizeCallback,
                defaults: new { controller = "VpAuthorizationController", action = "Callback" });
            webApplication.SidMapControllerRoute("vpQrCode",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizeQrCode + "/{id}",
                defaults: new { controller = "VpAuthorizationController", action = "GetQRCode" });
            webApplication.SidMapControllerRoute("vpAuthorizeStatus",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizeStatus + "/{id}",
                defaults: new { controller = "VpAuthorizationController", action = "Status" });

            return webApplication;
        }
    }
}