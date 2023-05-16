// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.CredentialIssuer
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCredentialIssuer(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<CredentialIssuerOptions>>().Value;
            bool usePrefix = opts.UseRealm;
            webApplication.MapControllerRoute("credentialIssuer",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CredentialIssuer,
                defaults: new { controller = "CredentialIssuer", action = "Get" });
            return webApplication;
        }
    }
}
