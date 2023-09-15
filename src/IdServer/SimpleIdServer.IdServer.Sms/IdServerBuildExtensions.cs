// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Api;

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
            return idServerBuilder;
        }
    }
}
