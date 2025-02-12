// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Sms.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Sms
{
    public static class IdServerBuildExtensions
    {
        /// <summary>
        /// Add sms authentication method.
        /// The amr is sms.
        /// </summary>
        /// <param name="idServerBuilder"></param>
        /// <returns></returns>
        public static IdServerBuilder AddSmsAuthentication(this IdServerBuilder idServerBuilder)
        {
            idServerBuilder.Services.AddTransient<IUserNotificationService, SmsUserNotificationService>();
            idServerBuilder.Services.AddTransient<ISmsUserNotificationService, SmsUserNotificationService>();
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, SmsAuthenticationMethodService>();
            idServerBuilder.Services.AddTransient<IUserSmsAuthenticationService, UserSmsAuthenticationService>();
            idServerBuilder.Services.AddTransient<IResetPasswordService, UserSmsResetPasswordService>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, SmsRegisterWorkflowLayout>();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, SmsAuthWorkflowLayout>();
            return idServerBuilder;
        }
    }
}
