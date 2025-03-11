// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.Stores.Default;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Add pwd authentication method.
    /// The amr is pwd.
    /// </summary>
    /// <param name="idServerBuilder"></param>
    /// <returns></returns>
    public static IdServerBuilder AddPwdAuthentication(this IdServerBuilder idServerBuilder, bool isInMemory = false)
    {
        idServerBuilder.MvcBuilder.AddApplicationPart(typeof(SimpleIdServer.IdServer.UI.AuthenticateController).Assembly);
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, PwdAuthenticationMethodService>();
        idServerBuilder.Services.AddTransient<IPasswordAuthenticationService, PasswordAuthenticationService>();
        idServerBuilder.Services.AddTransient<IUserAuthenticationService, PasswordAuthenticationService>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, PwdAuthWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, ConfirmResetPwdWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, PwdRegisterWorkflowLayout>();
        idServerBuilder.Services.AddTransient<IWorkflowLayoutService, ResetPwdWorkflowLayout>();
        if(isInMemory)
        {
            idServerBuilder.Services.AddSingleton<IFormStore>(new DefaultFormStore(new List<FormRecord>
            {
                StandardPwdAuthForms.PwdForm,
                StandardPwdAuthForms.ResetForm,
                StandardPwdAuthForms.ConfirmResetForm
            }));
            idServerBuilder.Services.AddSingleton<IWorkflowStore>(new DefaultWorkflowStore(new List<WorkflowRecord>
            {
                StandardPwdAuthWorkflows.DefaultCompletePwdAuthWorkflow
            }));
            idServerBuilder.Services.Configure<IdServerHostOptions>(o =>
            {
                o.DefaultAuthenticationWorkflowId = StandardPwdAuthWorkflows.completePwdAuthWorkflowId;
            });
        }

        return idServerBuilder;
    }
}