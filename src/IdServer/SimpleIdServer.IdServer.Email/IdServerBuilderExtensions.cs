// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Email.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        /// <summary>
        /// Add email authentication method.
        /// The amr is email.
        /// </summary>
        /// <param name="idServerBuilder"></param>
        /// <returns></returns>
        public static IdServerBuilder AddEmailAuthentication(this IdServerBuilder idServerBuilder)
        {
            idServerBuilder.Services.AddTransient<IUserNotificationService, EmailUserNotificationService>();
            idServerBuilder.Services.AddTransient<IEmailUserNotificationService, EmailUserNotificationService>();
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, EmailAuthenticationMethodService>();
            idServerBuilder.Services.AddTransient<IUserEmailAuthenticationService, UserEmailAuthenticationService>();
            idServerBuilder.Services.AddTransient<IResetPasswordService, UserEmailResetPasswordService>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, EmailAuthWorkflowLayout>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, EmailRegisterWorkflowLayout>();
            return idServerBuilder;
        }
    }
}
