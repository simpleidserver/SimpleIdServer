// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Stores;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IdServerBuilder AddMutualAuthentication(string authenticationSchema = SimpleIdServer.OAuth.Constants.CertificateAuthenticationScheme, Action<CertificateAuthenticationOptions> callback = null)
        {
            _serviceCollection.Configure<OAuthHostOptions>(o =>
            {
                o.MtlsEnabled = true;
                o.CertificateAuthenticationScheme = authenticationSchema;
            });
            _authenticationBuilder.AddCertificate(authenticationSchema, callback != null ? callback : o =>
            {

            });
            return this;
        }

        public IdServerBuilder AddMutualAuthenticationSelfSigned(string authenticationSchema = SimpleIdServer.OAuth.Constants.CertificateAuthenticationScheme)
        {
            _serviceCollection.Configure<OAuthHostOptions>(o =>
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
    }
}
