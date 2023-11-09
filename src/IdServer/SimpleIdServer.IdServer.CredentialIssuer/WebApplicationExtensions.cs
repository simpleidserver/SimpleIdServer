// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Options;

namespace SimpleIdServer.IdServer.CredentialIssuer
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCredentialIssuer(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            bool usePrefix = opts.UseRealm;

            webApplication.MapControllerRoute("credentialIssuer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialIssuer,
                defaults: new { controller = "CredentialIssuer", action = "Get" });

            webApplication.MapControllerRoute("credential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Credential,
                defaults: new { controller = "Credential", action = "Get" });

            webApplication.MapControllerRoute("shareCredentialOfferQR",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/shareqr",
                defaults: new { controller = "CredentialOffer", action = "ShareQR" });
            webApplication.MapControllerRoute("clientShareCredentialOfferQR",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/shareqr/{id}",
                defaults: new { controller = "CredentialOffer", action = "ClientShareQR" });
            webApplication.MapControllerRoute("shareCredentialOffer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/share",
                defaults: new { controller = "CredentialOffer", action = "Share" });
            webApplication.MapControllerRoute("clientShareCredentialOffer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/share/{id}",
                defaults: new { controller = "CredentialOffer", action = "ClientShare" });
            webApplication.MapControllerRoute("getCredentialOffer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/{id}",
                defaults: new { controller = "CredentialOffer", action = "Get" });
            webApplication.MapControllerRoute("getCredentialOfferQRCode",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/{id}/qr",
                defaults: new { controller = "CredentialOffer", action = "GetQRCode" });


            webApplication.MapControllerRoute("searchCredentialTemplates",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/.search",
                defaults: new { controller = "CredentialTemplates", action = "Search" });
            webApplication.MapControllerRoute("removeCredentialTemplate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer,
                defaults: new { controller = "CredentialTemplates", action = "Remove" });
            webApplication.MapControllerRoute("addW3CCredentialTemplate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/w3c",
                defaults: new { controller = "CredentialTemplates", action = "AddW3CredentialTemplate" });
            webApplication.MapControllerRoute("getCredentialTemplate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/{id}",
                defaults: new { controller = "CredentialTemplates", action = "Get" });
            webApplication.MapControllerRoute("removeCredentialTemplateDisplay",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/displays/{displayId}",
                defaults: new { controller = "CredentialTemplates", action = "RemoveDisplay" });
            webApplication.MapControllerRoute("addCredentialTemplateDisplay",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/displays",
                defaults: new { controller = "CredentialTemplates", action = "AddDisplay" });
            webApplication.MapControllerRoute("removeCredentialTemplateParameter",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/parameters/{parameterId}",
                defaults: new { controller = "CredentialTemplates", action = "RemoveParameter" });
            webApplication.MapControllerRoute("updateCredentialTemplateParameters",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialOffer + "/parameters",
                defaults: new { controller = "CredentialTemplates", action = "UpdateParameters" });

            return webApplication;
        }
    }
}
