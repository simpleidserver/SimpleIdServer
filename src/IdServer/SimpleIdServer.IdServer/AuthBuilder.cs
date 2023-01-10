// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Options;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer
{
    public class AuthBuilder
    {
        private readonly IServiceCollection _services;
        private readonly AuthenticationBuilder _builder;

        internal AuthBuilder(IServiceCollection services, AuthenticationBuilder builder)
        {
            _services = services;
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

        public AuthBuilder AddMutualAuthentication(Action<CertificateAuthenticationOptions> callback = null)
        {
            _services.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
            });
            _builder.AddCertificate(Constants.DefaultCertificateAuthenticationScheme, callback != null ? callback : o =>
            {

            });
            return this;
        }

        public AuthBuilder AddMutualAuthenticationSelfSigned()
        {
            _services.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
            });
            _builder.AddCertificate(Constants.DefaultCertificateAuthenticationScheme, o =>
            {
                o.AllowedCertificateTypes = CertificateTypes.SelfSigned;
            });
            return this;
        }

        public AuthenticationBuilder Builder => _builder;
    }
}
