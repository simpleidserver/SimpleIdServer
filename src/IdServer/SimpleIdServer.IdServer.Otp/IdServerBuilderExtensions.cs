// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Otp;
using SimpleIdServer.IdServer.Otp.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Configures OTP authentication for the IdServer by registering the required services.
    /// </summary>
    /// <param name="idServerBuilder">The IdServer builder instance used to set up authentication.</param>
    /// <param name="isInMemory">If true, seeds in-memory OTP forms and workflows.</param>
    /// <param name="isDefaultAuthMethod">If true, configures OTP as the default authentication method.</param>
    /// <returns>The updated IdServerBuilder with OTP authentication configured.</returns>
    public static IdServerBuilder AddOtpAuthentication(this IdServerBuilder idServerBuilder, bool isInMemory = false, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, OtpAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IOtpAuthenticationService, OtpAuthenticationService>();
        idServerBuilder.Services.AddTransient<IUserAuthenticationService, OtpAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, OtpAuthWorkflowLayout>();
        if (isInMemory)
        {
            Seed(idServerBuilder, isDefaultAuthMethod);
        }

        return idServerBuilder;
    }

    private static void Seed(IdServerBuilder idServerBuilder, bool isDefaultAuthMethod)
    {
        using (var serviceProvider = idServerBuilder.Services.BuildServiceProvider())
        {
            var formStore = serviceProvider.GetService<IFormStore>();
            formStore.Add(StandardOtpAuthForms.OtpForm);
            formStore.SaveChanges(CancellationToken.None).Wait();

            var workflowStore = serviceProvider.GetService<IWorkflowStore>();
            workflowStore.Add(StandardOtpAuthWorkflows.DefaultWorkflow);
            workflowStore.SaveChanges(CancellationToken.None).Wait();

            if (isDefaultAuthMethod)
            {
                idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
                {
                    o.DefaultAuthenticationWorkflowId = StandardOtpAuthWorkflows.workflowId;
                });
                idServerBuilder.SidAuthCookie.Callback = (o) =>
                {
                    o.LoginPath = $"/{SimpleIdServer.IdServer.Otp.Constants.Amr}/Authenticate";
                };
            }
        }
    }
}
