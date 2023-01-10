// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer
{
    public class AuthBuilder
    {
        private readonly AuthenticationBuilder _builder;

        internal AuthBuilder(AuthenticationBuilder builder)
        {
            _builder = builder;
        }

        public AuthBuilder AddOIDCAuthentication(Action<OpenIdConnectOptions> callback)
        {
            _builder.AddOpenIdConnect(Constants.DefaultOIDCAuthenticationScheme, (opts) =>
            {
                opts.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        if (context.Properties.Items.TryGetValue("prompt", out string prompt))
                            context.ProtocolMessage.Prompt = prompt;
                        return Task.CompletedTask;
                    }
                };
                if (callback != null) callback(opts);
            });
            return this;
        }

        public AuthenticationBuilder Builder => _builder;
    }
}
