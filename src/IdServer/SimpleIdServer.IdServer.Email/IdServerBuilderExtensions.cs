// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Email.UI.Services;

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
            idServerBuilder.Services.AddTransient<IEmailAuthService, EmailAuthService>();
            return idServerBuilder;
        }
    }
}
