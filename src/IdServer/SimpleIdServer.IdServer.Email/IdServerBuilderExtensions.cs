// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Email.Migrations;
using SimpleIdServer.IdServer.Email.Services;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.UI.Services;
using Constants = SimpleIdServer.IdServer.Email.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Registers the necessary services for Email Authentication. Registers email notification, authentication,
    /// password reset, and workflow layout services. Optionally seeds in-memory stores for forms and workflows.
    /// </summary>
    public static IdServerBuilder AddEmailAuthentication(this IdServerBuilder idServerBuilder, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.Services.AddTransient<IUserNotificationService, EmailUserNotificationService>();
        idServerBuilder.Services.AddTransient<IEmailUserNotificationService, EmailUserNotificationService>();
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, EmailAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IUserEmailAuthenticationService, UserEmailAuthenticationService>();
        idServerBuilder.Services.AddTransient<IResetPasswordService, UserEmailResetPasswordService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, EmailAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, EmailRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitEmailAuthDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitEmailConfigurationDefDataseeder>();
        idServerBuilder.AutomaticConfigurationOptions.Add(typeof(IdServerEmailOptions));
        if (isDefaultAuthMethod)
        {
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAcrValue = "email";
            });
            idServerBuilder.SidAuthCookie.Callback = (o) =>
            {
                o.LoginPath = $"/{Constants.AMR}/Authenticate";
            };
        }

        return idServerBuilder;
    }
};
