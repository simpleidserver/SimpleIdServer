// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using FormBuilder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Fido.Services;

namespace SimpleIdServer.IdServer.Fido
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddFidoAuthentication(this IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoAction = null)
        {
            if (fidoAction == null) idServerBuilder.Services.AddFido2(o =>
            {
                o.ServerName = "SimpleIdServer";
                o.ServerDomain = "localhost";
                o.Origins = new HashSet<string> { "https://localhost:5001" };
            });
            else idServerBuilder.Services.AddFido2(fidoAction);
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, WebauthnAuthenticationService>();
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, MobileAuthenticationService>();
            idServerBuilder.Services.AddTransient<IMobileAuthenticationService, UserMobileAuthenticationService>();
            idServerBuilder.Services.AddTransient<IWebauthnAuthenticationService, UserWebauthnAuthenticationService>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, WebauthRegisterWorkflowLayout>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, WebauthWorkflowLayout>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, MobileAuthWorkflowLayout>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, MobileRegisterWorkflowLayout>();
            return idServerBuilder;
        }
    }
}
