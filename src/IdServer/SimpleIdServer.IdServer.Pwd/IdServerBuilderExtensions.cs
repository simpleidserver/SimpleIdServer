﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Pwd.Fakers;
using SimpleIdServer.IdServer.Pwd.Migrations;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Adds password authentication services to the IdServerBuilder.
    /// Optionally configures in-memory stores for forms and workflows.
    /// </summary>
    /// <param name="idServerBuilder">The IdServerBuilder instance.</param>
    /// <param name="isDefaultAuthMethod">Indicates whether the password authentication method is used by default.</param>
    /// <returns>The updated IdServerBuilder instance.</returns>
    public static IdServerBuilder AddPwdAuthentication(this IdServerBuilder idServerBuilder, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.MvcBuilder.AddApplicationPart(typeof(SimpleIdServer.IdServer.UI.AuthenticateController).Assembly);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, PwdAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IPasswordAuthenticationService, PasswordAuthenticationService>();
        idServerBuilder.Services.AddTransient<IUserAuthenticationService, PasswordAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, PwdAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, ConfirmResetPwdWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, PwdRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, ResetPwdWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitPwdAuthDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitPwdConfigurationDefDataseeder>();
        idServerBuilder.Services.AddTransient<IFakerDataService, PwdAuthFakerService>();
        idServerBuilder.AutomaticConfigurationOptions.Add(typeof(IdServerPasswordOptions));
        if (isDefaultAuthMethod)
        {
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAcrValue = SimpleIdServer.IdServer.Config.DefaultAcrs.FirstLevelAssurance.Name;
            });
            idServerBuilder.SidAuthCookie.LoginPath = $"/{SimpleIdServer.IdServer.Constants.AreaPwd}/Authenticate";
        }

        return idServerBuilder;
    }
}