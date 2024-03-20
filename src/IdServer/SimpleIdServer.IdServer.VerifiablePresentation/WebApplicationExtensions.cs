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
                defaults: new { controller = "VpAuthorization", action = "Callback" });
            webApplication.SidMapControllerRoute("vpQrCode",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizeQrCode + "/{id}",
                defaults: new { controller = "VpAuthorization", action = "GetQRCode" });

            webApplication.SidMapControllerRoute("vpRegisterStatus",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpRegisterStatus + "/{id}",
                defaults: new { controller = "VpRegister", action = "Status" });
            webApplication.SidMapControllerRoute("vpEndRegister",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpEndRegister,
                defaults: new { controller = "VpRegister", action = "EndRegister" });


            return webApplication;
        }
    }
}