// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Sms.UI.Services;

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
        public static IdServerBuilder AddSmsAuthentication(this IdServerBuilder idServerBuilder, Action<IdServerSmsOptions>? callback = null)
        {
            if (callback != null) idServerBuilder.Services.Configure(callback);
            else idServerBuilder.Services.Configure<IdServerSmsOptions>(o => { });
            idServerBuilder.Services.AddTransient<ISmsAuthService, SmsAuthService>();
            idServerBuilder.Services.AddTransient<IUserNotificationService, SmsUserNotificationService>();
            idServerBuilder.Services.AddTransient<ISmsUserNotificationService, SmsUserNotificationService>();
            return idServerBuilder;
        }
    }
}
