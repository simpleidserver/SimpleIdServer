// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        /// <summary>
        /// Add pwd authentication method.
        /// The amr is pwd.
        /// </summary>
        /// <param name="idServerBuilder"></param>
        /// <returns></returns>
        public static IdServerBuilder AddPwdAuthentication(this IdServerBuilder idServerBuilder)
        {
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, PwdAuthenticationMethodService>();
            idServerBuilder.Services.AddTransient<IPasswordAuthenticationService, PasswordAuthenticationService>();
            idServerBuilder.Services.AddTransient<IUserAuthenticationService, PasswordAuthenticationService>();
            return idServerBuilder;
        }
    }
}