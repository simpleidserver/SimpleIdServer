// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseFIDO(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            var usePrefix = opts.UseRealm;

            webApplication.SidMapControllerRoute("beginQRRegister",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BeginQRCodeRegister,
                defaults: new { controller = "U2FRegister", action = "BeginQRCode" });
            webApplication.SidMapControllerRoute("readRegisterQRCode",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ReadRegisterQRCode + "/{sessionId}",
                defaults: new { controller = "U2FRegister", action = "ReadQRCode" });
            webApplication.SidMapControllerRoute("U2FStatusRegistration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegisterStatus + "/{sessionId}",
                defaults: new { controller = "U2FRegister", action = "GetStatus" });
            webApplication.SidMapControllerRoute("beginRegister",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BeginRegister,
                defaults: new { controller = "U2FRegister", action = "Begin" });
            webApplication.SidMapControllerRoute("endRegister",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.EndRegister,
                defaults: new { controller = "U2FRegister", action = "End" });

            webApplication.SidMapControllerRoute("beginQRLogin",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BeginQRCodeLogin,
                defaults: new { controller = "U2FLogin", action = "BeginQRCode" });
            webApplication.SidMapControllerRoute("U2FStatusLogin",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.LoginStatus + "/{sessionId}",
                defaults: new { controller = "U2FLogin", action = "GetStatus" });
            webApplication.SidMapControllerRoute("beginLogin",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BeginLogin,
                defaults: new { controller = "U2FLogin", action = "Begin" });
            webApplication.SidMapControllerRoute("endLogin",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.EndLogin,
                defaults: new { controller = "U2FLogin", action = "End" });
            webApplication.SidMapControllerRoute("readLoginQRCode",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ReadLoginQRCode + "/{sessionId}",
                defaults: new { controller = "U2FLogin", action = "ReadQRCode" });

            return webApplication;
        }
    }
}
