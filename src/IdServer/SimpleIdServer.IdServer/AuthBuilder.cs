// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleIdServer.IdServer
{
    public class AuthBuilder
    {
        private readonly AuthenticationBuilder _builder;

        internal AuthBuilder(AuthenticationBuilder builder)
        {
            _builder = builder;
        }

        public AuthBuilder AddSelfAuthentication(Action<OpenIdConnectOptions> callback, string selfAuthenticationScheme = Constants.SelfAuthenticationScheme)
        {
            _builder.AddOpenIdConnect(selfAuthenticationScheme, callback);
            return this;
        }
    }
}
