// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Fido2NetLib;
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Fido.Migrations;
using SimpleIdServer.IdServer.Fido.Services;
using SimpleIdServer.IdServer.Options;
using Constants = SimpleIdServer.IdServer.Fido.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddMobileAuthentication(this IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoCallback = null, bool isDefaultAuthMethod = false)
    {
        AddFido(idServerBuilder, fidoCallback);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, MobileAuthenticationService>();
        idServerBuilder.Services.AddTransient<IMobileAuthenticationService, UserMobileAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, MobileAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, MobileRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitMobileAuthDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitMobileConfigurationDefDataseeder>();
        idServerBuilder.AutomaticConfigurationOptions.Add<MobileOptions>();
        if (isDefaultAuthMethod)
        {
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAcrValue = Constants.MobileAMR;
            });
            idServerBuilder.SidAuthCookie.Callback = (o) =>
            {
                o.LoginPath = $"/{Constants.MobileAMR}/Authenticate";
            };
        }

        return idServerBuilder;
    }

    public static IdServerBuilder AddWebauthnAuthentication(this IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoCallback = null, bool isDefaultAuthMethod = false)
    {
        AddFido(idServerBuilder, fidoCallback);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, WebauthnAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWebauthnAuthenticationService, UserWebauthnAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, WebauthRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, WebauthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitWebauthnAuthDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitWebauthnConfigurationDefDataseeder>();
        idServerBuilder.AutomaticConfigurationOptions.Add<WebauthnOptions>();
        if (isDefaultAuthMethod)
        {
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAcrValue = Constants.AMR;
            });
            idServerBuilder.SidAuthCookie.Callback = (o) =>
            {
                o.LoginPath = $"/{Constants.AMR}/Authenticate";
            };
        }

        return idServerBuilder;
    }

    private static void AddFido(IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoCallback = null)
    {
        if (!idServerBuilder.Services.Any(s => s.ServiceType == typeof(IFido2)))
        {
            if (fidoCallback == null)
            {
                idServerBuilder.Services.AddFido2(o =>
                {
                    o.ServerName = "SimpleIdServer";
                    o.ServerDomain = "localhost";
                    o.Origins = new HashSet<string> { "https://localhost:5001" };
                });
            }
            else
            {
                idServerBuilder.Services.AddFido2(fidoCallback);
            }

            idServerBuilder.AddRoute("beginQRRegister", Constants.EndPoints.BeginQRCodeRegister, new { controller = "U2FRegister", action = "BeginQRCode" });
            idServerBuilder.AddRoute("readRegisterQRCode", Constants.EndPoints.ReadRegisterQRCode + "/{sessionId}", new { controller = "U2FRegister", action = "ReadQRCode" });
            idServerBuilder.AddRoute("U2FStatusRegistration", Constants.EndPoints.RegisterStatus + "/{sessionId}", new { controller = "U2FRegister", action = "GetStatus" });
            idServerBuilder.AddRoute("beginRegister", Constants.EndPoints.BeginRegister, new { controller = "U2FRegister", action = "Begin" });
            idServerBuilder.AddRoute("endRegister", Constants.EndPoints.EndRegister, new { controller = "U2FRegister", action = "End" });

            idServerBuilder.AddRoute("beginQRLogin", Constants.EndPoints.BeginQRCodeLogin, new { controller = "U2FLogin", action = "BeginQRCode" });
            idServerBuilder.AddRoute("U2FStatusLogin", Constants.EndPoints.LoginStatus + "/{sessionId}", new { controller = "U2FLogin", action = "GetStatus" });
            idServerBuilder.AddRoute("beginLogin", Constants.EndPoints.BeginLogin, new { controller = "U2FLogin", action = "Begin" });
            idServerBuilder.AddRoute("endLogin", Constants.EndPoints.EndLogin, new { controller = "U2FLogin", action = "End" });
            idServerBuilder.AddRoute("readLoginQRCode", Constants.EndPoints.ReadLoginQRCode + "/{sessionId}", new { controller = "U2FLogin", action = "ReadQRCode" });
        }
    }
}
