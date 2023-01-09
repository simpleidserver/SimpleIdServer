// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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

        public IdServerBuilder AddMutualAuthentication(string authenticationSchema = Constants.CertificateAuthenticationScheme, Action<CertificateAuthenticationOptions> callback = null)
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
                o.CertificateAuthenticationScheme = authenticationSchema;
            });
            _authenticationBuilder.AddCertificate(authenticationSchema, callback != null ? callback : o =>
            {

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

        /// <summary>
        /// Add back channel authentication.
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

        public IdServerBuilder AddMutualAuthenticationSelfSigned(string authenticationSchema = Constants.CertificateAuthenticationScheme)
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
                o.CertificateAuthenticationScheme = authenticationSchema;
            });
            _authenticationBuilder.AddCertificate(authenticationSchema, o =>
            {
                o.AllowedCertificateTypes = CertificateTypes.SelfSigned;
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
