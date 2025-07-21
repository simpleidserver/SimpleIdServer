// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Otp;
using SimpleIdServer.IdServer.Otp.Migrations;
using SimpleIdServer.IdServer.Otp.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Configures OTP authentication for the IdServer by registering the required services.
    /// </summary>
    /// <param name="idServerBuilder">The IdServer builder instance used to set up authentication.</param>
    /// <param name="isDefaultAuthMethod">If true, configures OTP as the default authentication method.</param>
    /// <returns>The updated IdServerBuilder with OTP authentication configured.</returns>
    public static IdServerBuilder AddOtpAuthentication(this IdServerBuilder idServerBuilder, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, OtpAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IOtpAuthenticationService, OtpAuthenticationService>();
        idServerBuilder.Services.AddTransient<IUserAuthenticationService, OtpAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, OtpAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IDataSeeder, InitOtpAuthDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, UpdateOtpTranslationsDataseeder>();
        idServerBuilder.Services.AddTransient<IDataSeeder, UpdateTargetsOtpWorkflowsDataseeder>();
        if (isDefaultAuthMethod)
        {
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAcrValue = "otp";
            });
            idServerBuilder.SidAuthCookie.LoginPath = $"/{SimpleIdServer.IdServer.Otp.Constants.Amr}/Authenticate";
        }

        return idServerBuilder;
    }
}
