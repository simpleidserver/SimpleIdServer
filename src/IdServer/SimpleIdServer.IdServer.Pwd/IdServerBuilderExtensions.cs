// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Adds password authentication services to the IdServerBuilder.
    /// Optionally configures in-memory stores for forms and workflows.
    /// </summary>
    /// <param name="idServerBuilder">The IdServerBuilder instance.</param>
    /// <param name="isInMemory">Indicates whether to use in-memory stores.</param
    /// <param name="isDefaultAuthMethod">Indicates whether the password authentication method is used by default.</param>
    /// <returns>The updated IdServerBuilder instance.</returns>
    public static IdServerBuilder AddPwdAuthentication(this IdServerBuilder idServerBuilder, bool isInMemory = false, bool isDefaultAuthMethod = false)
    {
        idServerBuilder.MvcBuilder.AddApplicationPart(typeof(SimpleIdServer.IdServer.UI.AuthenticateController).Assembly);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, PwdAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IPasswordAuthenticationService, PasswordAuthenticationService>();
        idServerBuilder.Services.AddTransient<IUserAuthenticationService, PasswordAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, PwdAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, ConfirmResetPwdWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, PwdRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, ResetPwdWorkflowLayout>();
        idServerBuilder.AutomaticConfigurationOptions.Add(typeof(IdServerPasswordOptions));        
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
            var acrStore = serviceProvider.GetService<IAuthenticationContextClassReferenceRepository>();
            SimpleIdServer.IdServer.Config.DefaultAcrs.FirstLevelAssurance.AuthenticationWorkflow = StandardPwdAuthWorkflows.completePwdAuthWorkflowId;
            acrStore.Add(SimpleIdServer.IdServer.Config.DefaultAcrs.FirstLevelAssurance);

            var formStore = serviceProvider.GetService<IFormStore>();
            formStore.Add(StandardPwdAuthForms.PwdForm);
            formStore.Add(StandardPwdAuthForms.ResetForm);
            formStore.Add(StandardPwdAuthForms.ConfirmResetForm);
            formStore.Add(StandardPwdRegisterForms.PwdForm);
            formStore.SaveChanges(CancellationToken.None).Wait();

            var workflowStore = serviceProvider.GetService<IWorkflowStore>();
            workflowStore.Add(StandardPwdAuthWorkflows.DefaultCompletePwdAuthWorkflow);
            workflowStore.Add(StandardPwdAuthWorkflows.DefaultConfirmResetPwdWorkflow);
            workflowStore.Add(StandardPwdRegistrationWorkflows.DefaultWorkflow);
            workflowStore.SaveChanges(CancellationToken.None).Wait();

            var registrationStore = serviceProvider.GetService<IRegistrationWorkflowRepository>();
            registrationStore.Add(RegistrationWorkflowBuilder.New(SimpleIdServer.IdServer.Constants.AreaPwd, StandardPwdRegistrationWorkflows.workflowId).Build());

            if(isDefaultAuthMethod)
            {
                idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
                {
                    o.DefaultAcrValue = SimpleIdServer.IdServer.Config.DefaultAcrs.FirstLevelAssurance.Name;
                });
                idServerBuilder.SidAuthCookie.Callback = (o) =>
                {
                    o.LoginPath = $"/{SimpleIdServer.IdServer.Constants.AreaPwd}/Authenticate";
                };
            }
        }
    }
}