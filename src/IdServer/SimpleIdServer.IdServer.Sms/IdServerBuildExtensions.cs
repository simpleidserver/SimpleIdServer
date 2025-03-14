// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Sms.Services;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Sms;

public static class IdServerBuildExtensions
{
    public static IdServerBuilder AddSmsAuthentication(this IdServerBuilder idServerBuilder, bool isInMemory = false, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.Services.AddTransient<IUserNotificationService, SmsUserNotificationService>();
        idServerBuilder.Services.AddTransient<ISmsUserNotificationService, SmsUserNotificationService>();
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, SmsAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IUserSmsAuthenticationService, UserSmsAuthenticationService>();
        idServerBuilder.Services.AddTransient<IResetPasswordService, UserSmsResetPasswordService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, SmsRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, SmsAuthWorkflowLayout>();
        idServerBuilder.AutomaticConfigurationOptions.Add<IdServerSmsOptions>();
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
            var newAcr = BuildAcr();
            var acrStore = serviceProvider.GetService<IAuthenticationContextClassReferenceRepository>();
            acrStore.Add(newAcr);

            var formStore = serviceProvider.GetService<IFormStore>();
            formStore.Add(StandardSmsAuthForms.SmsForm);
            formStore.Add(StandardSmsRegisterForms.SmsForm);
            formStore.SaveChanges(CancellationToken.None).Wait();

            var workflowStore = serviceProvider.GetService<IWorkflowStore>();
            workflowStore.Add(StandardSmsAuthWorkflows.DefaultWorkflow);
            workflowStore.SaveChanges(CancellationToken.None).Wait();

            if (isDefaultAuthMethod)
            {
                idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
                {
                    o.DefaultAcrValue = newAcr.Name;
                });
                idServerBuilder.SidAuthCookie.Callback = (o) =>
                {
                    o.LoginPath = $"/{Constants.AMR}/Authenticate";
                };
            }
        }
    }

    private static AuthenticationContextClassReference BuildAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "sms",
        DisplayName = "sms",
        UpdateDateTime = DateTime.UtcNow,
        Realms = new List<Realm>
        {
            StandardRealms.Master
        }
    };
}