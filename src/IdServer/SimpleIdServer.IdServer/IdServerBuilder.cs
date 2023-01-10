// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
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
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceProvider _serviceProvider;
        private readonly AuthenticationBuilder _authenticationBuilder;

        public IdServerBuilder(IServiceCollection serviceCollection, AuthenticationBuilder authenticationBuilder, IServiceProvider serviceProvider)
        {
            _serviceCollection = serviceCollection;
            _authenticationBuilder = authenticationBuilder;
            _serviceProvider = serviceProvider;
            AddInMemoryAcr(new List<AuthenticationContextClassReference> { Constants.StandardAcrs.FirstLevelAssurance });
        }

        public IServiceCollection Services => _serviceCollection;
        public IServiceProvider ServiceProvider => _serviceProvider;

        public IdServerBuilder AddSigningKey(SigningCredentials signingCredentials)
        {
            _serviceCollection.AddSingleton<IKeyStore>(new InMemoryKeyStore(signingCredentials));
            return this;
        }

        public IdServerBuilder AddSigningKey(RsaSecurityKey rsa, string signingAlg = SecurityAlgorithms.RsaSha256)
        {
            var signingCredentials = new SigningCredentials(rsa, signingAlg);
            return AddSigningKey(signingCredentials);
        }

        public IdServerBuilder AddDeveloperSigningCredentials()
        {
            var key = new RsaSecurityKey(new RSACryptoServiceProvider(2048))
            {
                KeyId = "keyid"
            };
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            AddSigningKey(signingCredentials);
            return this;
        }

        public IdServerBuilder AddInMemoryScopes(ICollection<Scope> scopes)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if (!storeDbContext.Scopes.Any())
            {
                storeDbContext.Scopes.AddRange(scopes);
                storeDbContext.SaveChanges();
            }

            return this;
        }

        public IdServerBuilder AddInMemoryClients(ICollection<Client> clients)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if (!storeDbContext.Clients.Any())
            {
                storeDbContext.Clients.AddRange(clients);
                storeDbContext.SaveChanges();
            }

            return this;
        }

        public IdServerBuilder EnableConfigurableAuthentication(ICollection<SimpleIdServer.IdServer.Domains.AuthenticationSchemeProvider> providers)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if(!storeDbContext.AuthenticationSchemeProviders.Any())
            {
                storeDbContext.AuthenticationSchemeProviders.AddRange(providers);
                storeDbContext.SaveChanges();
            }

            _serviceCollection.AddTransient<ISIDAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
            _serviceCollection.AddTransient<IAuthenticationHandlerProvider, DynamicAuthenticationHandlerProvider>();
            return this;
        }

        public IdServerBuilder AddInMemoryAcr(ICollection<AuthenticationContextClassReference> acrs)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if(!storeDbContext.Acrs.Any())
            {
                storeDbContext.Acrs.AddRange(acrs);
                storeDbContext.SaveChanges();
            }

            return this;
        }

        public IdServerBuilder AddInMemoryUsers(ICollection<User> users)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if (!storeDbContext.Users.Any())
            {
                storeDbContext.Users.AddRange(users);
                storeDbContext.SaveChanges();
            }

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

        /// <summary>
        /// Add back channel authentication (CIBA).
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder AddBackChannelAuthentication()
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.IsBCEnabled = true;
            });
            return this;
        }

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
    }
}
