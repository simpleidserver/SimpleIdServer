// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Fido.Services;
using SimpleIdServer.IdServer.Options;
using Constants = SimpleIdServer.IdServer.Fido.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddMobileAuthentication(this IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoCallback = null, bool isInMemory = false, bool isDefaultAuthMethod = false)
    {
        AddFido(idServerBuilder, fidoCallback);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, MobileAuthenticationService>();
        idServerBuilder.Services.AddTransient<IMobileAuthenticationService, UserMobileAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, MobileAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, MobileRegisterWorkflowLayout>();
        idServerBuilder.AutomaticConfigurationOptions.Add<MobileOptions>();
        if (isInMemory)
        {
            SeedMobile(idServerBuilder, isDefaultAuthMethod);
        }

        return idServerBuilder;
    }

    public static IdServerBuilder AddWebauthnAuthentication(this IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoCallback = null, bool isInMemory = false, bool isDefaultAuthMethod = false)
    {
        AddFido(idServerBuilder, fidoCallback);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, WebauthnAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWebauthnAuthenticationService, UserWebauthnAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, WebauthRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, WebauthWorkflowLayout>();
        idServerBuilder.AutomaticConfigurationOptions.Add<WebauthnOptions>();
        if (isInMemory)
        {
            SeedWebauthn(idServerBuilder, isDefaultAuthMethod);
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

    private static void SeedMobile(IdServerBuilder idServerBuilder, bool isDefaultAuthMethod)
    {
        using (var serviceProvider = idServerBuilder.Services.BuildServiceProvider())
        {
            var formStore = serviceProvider.GetService<IFormStore>();
            formStore.Add(StandardFidoAuthForms.MobileForm);
            formStore.Add(StandardFidoRegisterForms.MobileForm);
            formStore.SaveChanges(CancellationToken.None).Wait();

            var workflowStore = serviceProvider.GetService<IWorkflowStore>();
            workflowStore.Add(StandardFidoAuthWorkflows.DefaultMobileWorkflow);
            workflowStore.SaveChanges(CancellationToken.None).Wait();

            if (isDefaultAuthMethod)
            {
                idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
                {
                    o.DefaultAuthenticationWorkflowId = StandardFidoAuthWorkflows.mobileWorkflowId;
                });
                idServerBuilder.SidAuthCookie.Callback = (o) =>
                {
                    o.LoginPath = $"/{Constants.MobileAMR}/Authenticate";
                };
            }
        }
    }

    private static void SeedWebauthn(IdServerBuilder idServerBuilder, bool isDefaultAuthMethod)
    {
        using (var serviceProvider = idServerBuilder.Services.BuildServiceProvider())
        {
            var formStore = serviceProvider.GetService<IFormStore>();
            formStore.Add(StandardFidoAuthForms.WebauthnForm);
            formStore.Add(StandardFidoRegisterForms.WebauthnForm);
            formStore.SaveChanges(CancellationToken.None).Wait();

            var workflowStore = serviceProvider.GetService<IWorkflowStore>();
            workflowStore.Add(StandardFidoAuthWorkflows.DefaultWebauthnWorkflow);
            workflowStore.SaveChanges(CancellationToken.None).Wait();

            if (isDefaultAuthMethod)
            {
                idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
                {
                    o.DefaultAuthenticationWorkflowId = StandardFidoAuthWorkflows.webauthnWorkflowId;
                });
                idServerBuilder.SidAuthCookie.Callback = (o) =>
                {
                    o.LoginPath = $"/{Constants.AMR}/Authenticate";
                };
            }
        }
    }
}
