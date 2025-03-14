// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Email.Services;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;
using static SimpleIdServer.IdServer.Constants;
using Constants = SimpleIdServer.IdServer.Email.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Registers the necessary services for Email Authentication. Registers email notification, authentication,
    /// password reset, and workflow layout services. Optionally seeds in-memory stores for forms and workflows.
    /// </summary>
    public static IdServerBuilder AddEmailAuthentication(this IdServerBuilder idServerBuilder, bool isInMemory = false, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.Services.AddTransient<IUserNotificationService, EmailUserNotificationService>();
        idServerBuilder.Services.AddTransient<IEmailUserNotificationService, EmailUserNotificationService>();
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, EmailAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IUserEmailAuthenticationService, UserEmailAuthenticationService>();
        idServerBuilder.Services.AddTransient<IResetPasswordService, UserEmailResetPasswordService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, EmailAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, EmailRegisterWorkflowLayout>();
        idServerBuilder.AutomaticConfigurationOptions.Add(typeof(IdServerEmailOptions));
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
            formStore.Add(StandardEmailAuthForms.EmailForm);
            formStore.Add(StandardEmailRegistrationForms.EmailForm);
            formStore.SaveChanges(CancellationToken.None).Wait();

            var workflowStore = serviceProvider.GetService<IWorkflowStore>();
            workflowStore.Add(StandardEmailAuthWorkflows.DefaultWorkflow);
            workflowStore.SaveChanges(CancellationToken.None).Wait();

            if(isDefaultAuthMethod)
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
        Name = "email",
        DisplayName = "Email",
        UpdateDateTime = DateTime.UtcNow,
        Realms = new List<Realm>
        {
            StandardRealms.Master
        }
    };
};
