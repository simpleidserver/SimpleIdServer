// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.AuthProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IdServerBuilder
    {
        private readonly InMemoryKeyStore _keyStore = new InMemoryKeyStore();
        private readonly IServiceCollection _serviceCollection;

        public IdServerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serviceCollection.AddSingleton<IKeyStore>(_keyStore);
        }

        public IServiceCollection Services => _serviceCollection;

        public IdServerBuilder SetSigningKeys(params SigningCredentials[] signingCredentials)
        {
            return SetSigningKeys((IEnumerable<SigningCredentials>)signingCredentials);
        }

        public IdServerBuilder SetSigningKeys(IEnumerable<SigningCredentials> signingCredentials)
        {
            _keyStore.SetSigningCredentials(signingCredentials);
            return this;
        }

        public IdServerBuilder SetSigningKey(RsaSecurityKey rsa, string signingAlg = SecurityAlgorithms.RsaSha256)
        {
            var signingCredentials = new SigningCredentials(rsa, signingAlg);
            return SetSigningKeys(new[] { signingCredentials });
        }

        #region Encryption and signing Keys

        public IdServerBuilder SetSigningKey(ECDsaSecurityKey ecdsa, string signingAlg = SecurityAlgorithms.EcdsaSha256)
        {
            var signingCredentials = new SigningCredentials(ecdsa, signingAlg);
            return SetSigningKeys(new[] { signingCredentials });
        }

        public IdServerBuilder SetEncryptedKeys(params EncryptingCredentials[] encryptedCredentials)
        {
            SetEncryptedKeys((IEnumerable<EncryptingCredentials>)encryptedCredentials);
            return this;
        }

        public IdServerBuilder SetEncryptedKeys(IEnumerable<EncryptingCredentials> encryptedCredentials)
        {
            _keyStore.SetEncryptedCredentials(encryptedCredentials);
            return this;
        }

        public IdServerBuilder AddDeveloperSigningCredentials()
        {
            var key = new RsaSecurityKey(new RSACryptoServiceProvider(2048))
            {
                KeyId = "keyid"
            };
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            SetSigningKeys(new[] { signingCredentials });
            return this;
        }

        #endregion

        #region Authentication & Authorization

        public IdServerBuilder EnableConfigurableAuthentication()
        {
            _serviceCollection.AddTransient<ISIDAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
            _serviceCollection.AddTransient<IAuthenticationHandlerProvider, DynamicAuthenticationHandlerProvider>();
            return this;
        }

        public IdServerBuilder AddAuthentication(Action<AuthBuilder> callback = null)
        {
            var auth = _serviceCollection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.Events.OnSigningIn += (CookieSigningInContext ctx) =>
                    {
                        if (ctx.Principal != null && ctx.Principal.Identity != null && ctx.Principal.Identity.IsAuthenticated)
                        {
                            var nameIdentifier = ctx.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                            var ticket = new AuthenticationTicket(ctx.Principal, ctx.Properties, ctx.Scheme.Name);
                            var cookieValue = ctx.Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding(ctx));
                            ctx.Options.CookieManager.AppendResponseCookie(
                                ctx.HttpContext,
                                $"{ctx.Options.Cookie.Name}-{nameIdentifier}",
                                cookieValue,
                                ctx.CookieOptions);
                        }

                        return Task.CompletedTask;
                    };
                    opts.Events.OnSigningOut += (CookieSigningOutContext ctx) =>
                    {
                        if (ctx.HttpContext.User != null && ctx.HttpContext.User.Identity != null && ctx.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var nameIdentifier = ctx.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                            ctx.Options.CookieManager.DeleteCookie(
                                ctx.HttpContext,
                                $"{ctx.Options.Cookie.Name}-{nameIdentifier}",
                                ctx.CookieOptions);
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    };
                });
            if(callback != null)
                callback(new AuthBuilder(_serviceCollection, auth));

            string GetTlsTokenBinding(CookieSigningInContext context)
            {
                var binding = context.HttpContext.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
                return binding == null ? null : Convert.ToBase64String(binding);
            }

            return this;
        }

        public IdServerBuilder SetDefaultRegistrationAuthorizationPolicy()
        {
            _serviceCollection.Configure<AuthorizationOptions>(o =>
            {
                o.AddPolicy(Constants.Policies.Register, o => o.RequireClaim("scope", Constants.ScopeNames.Register));
            });
            return this;
        }

        public IdServerBuilder SetDefaultAuthSchemeAuthorizationPolicy()
        {
            _serviceCollection.Configure<AuthorizationOptions>(o =>
            {
                o.AddPolicy(Constants.Policies.AuthSchemeProvider, o => o.RequireClaim("scope", Constants.ScopeNames.AuthSchemeProvider));
            });
            return this;
        }

        #endregion

        #region CIBA

        /// <summary>
        /// Add back channel authentication (CIBA).
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder AddBackChannelAuthentication(Action<IGlobalConfiguration> callback = null)
        {
            _serviceCollection.AddTransient<BCNotificationJob>();
            _serviceCollection.AddHangfire(callback == null ? (o => {
                o.UseIgnoredAssemblyVersionTypeResolver();
                o.UseInMemoryStorage();
            }) : callback);
            _serviceCollection.AddHangfireServer();
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.IsBCEnabled = true;
            });
            return this;
        }

        #endregion

        #region Other

        /// <summary>
        /// IdentityServer can be hosted in several Realm.
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder UseRealm()
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.UseRealm = true;
            });
            return this;
        }

        #endregion
    }
}
