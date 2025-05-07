// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Sms.Migrations;
using SimpleIdServer.IdServer.Sms.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuildExtensions
{
    // Adds SMS Authentication services to the IdServer configuration.
    public static IdServerBuilder AddSmsAuthentication(this IdServerBuilder idServerBuilder, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.Services.AddTransient<IUserNotificationService, SmsUserNotificationService>();
        idServerBuilder.Services.AddTransient<ISmsUserNotificationService, SmsUserNotificationService>();
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, SmsAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IUserSmsAuthenticationService, UserSmsAuthenticationService>();
        idServerBuilder.Services.AddTransient<IResetPasswordService, UserSmsResetPasswordService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, SmsRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, SmsAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitSmsAuthDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitSmsConfigurationDefDataseeder>();
        idServerBuilder.AutomaticConfigurationOptions.Add<IdServerSmsOptions>();
        if (isDefaultAuthMethod)
        {
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAcrValue = SimpleIdServer.IdServer.Sms.Constants.AMR;
            });
            idServerBuilder.SidAuthCookie.LoginPath = $"/{SimpleIdServer.IdServer.Sms.Constants.AMR}/Authenticate";
        }

        return idServerBuilder;
    }
}
