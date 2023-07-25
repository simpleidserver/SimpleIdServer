// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Email;

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
        public static IdServerBuilder AddEmailAuthentication(this IdServerBuilder idServerBuilder, Action<IdServerEmailOptions>? callback = null)
        {
            if (callback != null) idServerBuilder.Services.Configure(callback);
            else idServerBuilder.Services.Configure<IdServerEmailOptions>(o => { });
            idServerBuilder.Services.AddTransient<IUserNotificationService, EmailUserNotificationService>();
            idServerBuilder.Services.AddTransient<IEmailUserNotificationService, EmailUserNotificationService>();
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, EmailAuthenticationMethodService>();
            return idServerBuilder;
        }
    }
}
