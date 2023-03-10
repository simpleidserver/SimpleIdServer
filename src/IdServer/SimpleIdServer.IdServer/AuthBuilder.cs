// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Options;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer
{
    public class AuthBuilder
    {
        private readonly IServiceCollection _services;
        private readonly AuthenticationBuilder _authBuilder;

        internal AuthBuilder(IServiceCollection services, AuthenticationBuilder authBuilder)
        {
            _services = services;
            _authBuilder = authBuilder;
        }

        public AuthBuilder AddOIDCAuthentication(Action<OpenIdConnectOptions> callback)
        {
            _authBuilder.AddOpenIdConnect(Constants.DefaultOIDCAuthenticationScheme, (opts) =>
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
            _services.AddSingleton<IPostConfigureOptions<OpenIdConnectOptions>, ConfigureOpenIdOptions>();
            return this;
        }

        public AuthBuilder AddMutualAuthentication(Action<CertificateAuthenticationOptions> callback = null)
        {
            _services.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
            });
            _authBuilder.AddCertificate(Constants.DefaultCertificateAuthenticationScheme, callback != null ? callback : o =>
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
            _authBuilder.AddCertificate(Constants.DefaultCertificateAuthenticationScheme, o =>
            {
                o.AllowedCertificateTypes = CertificateTypes.SelfSigned;
            });
            return this;
        }

        public AuthenticationBuilder Builder => _authBuilder;
    }

    public class ConfigureOpenIdOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        public void PostConfigure(string name, OpenIdConnectOptions options)
        {
            options.ConfigurationManager = new IdServerConfigurationManager(options.Authority, new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(options.Backchannel) { RequireHttps = options.RequireHttpsMetadata })
            {
                RefreshInterval = options.RefreshInterval,
                AutomaticRefreshInterval = options.AutomaticRefreshInterval,
            };
        }
    }
}