// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;

namespace SimpleIdServer.IdServer.CredentialIssuer
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCredentialIssuer(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            bool usePrefix = opts.UseRealm;

            webApplication.SidMapControllerRoute("credentialIssuer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialIssuer,
                defaults: new { controller = "CredentialIssuer", action = "Get" });

            webApplication.SidMapControllerRoute("credential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Credential,
                defaults: new { controller = "Credential", action = "Get" });

            webApplication.SidMapControllerRoute("shareCredentialOfferQR",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/shareqr",
                defaults: new { controller = "CredentialOffer", action = "ShareQR" });
            webApplication.SidMapControllerRoute("clientShareCredentialOfferQR",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/shareqr/{id}",
                defaults: new { controller = "CredentialOffer", action = "ClientShareQR" });
            webApplication.SidMapControllerRoute("shareCredentialOffer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/share",
                defaults: new { controller = "CredentialOffer", action = "Share" });
            webApplication.SidMapControllerRoute("clientShareCredentialOffer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/share/{id}",
                defaults: new { controller = "CredentialOffer", action = "ClientShare" });
            webApplication.SidMapControllerRoute("getCredentialOffer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/{id}",
                defaults: new { controller = "CredentialOffer", action = "Get" });
            webApplication.SidMapControllerRoute("getCredentialOfferQRCode",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/{id}/qr",
                defaults: new { controller = "CredentialOffer", action = "GetQRCode" });


            webApplication.SidMapControllerRoute("searchCredentialTemplates",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/.search",
                defaults: new { controller = "CredentialTemplates", action = "Search" });
            webApplication.SidMapControllerRoute("removeCredentialTemplate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/{id}",
                defaults: new { controller = "CredentialTemplates", action = "Remove" });
            webApplication.SidMapControllerRoute("addW3CCredentialTemplate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/w3c",
                defaults: new { controller = "CredentialTemplates", action = "AddW3CredentialTemplate" });
            webApplication.SidMapControllerRoute("getCredentialTemplate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/{id}",
                defaults: new { controller = "CredentialTemplates", action = "Get" });
            webApplication.SidMapControllerRoute("removeCredentialTemplateDisplay",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/{id}/displays/{displayId}",
                defaults: new { controller = "CredentialTemplates", action = "RemoveDisplay" });
            webApplication.SidMapControllerRoute("addCredentialTemplateDisplay",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/{id}/displays",
                defaults: new { controller = "CredentialTemplates", action = "AddDisplay" });
            webApplication.SidMapControllerRoute("removeCredentialTemplateParameter",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/{id}/parameters/{parameterId}",
                defaults: new { controller = "CredentialTemplates", action = "RemoveParameter" });
            webApplication.SidMapControllerRoute("updateCredentialTemplateParameters",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialTemplates + "/{id}/parameters",
                defaults: new { controller = "CredentialTemplates", action = "UpdateParameters" });

            return webApplication;
        }
    }
}
