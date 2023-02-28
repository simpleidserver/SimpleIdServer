// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.AuthProviders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IdServerBuilder
    {
        private readonly InMemoryKeyStore _keyStore = new InMemoryKeyStore();
        private readonly IServiceCollection _serviceCollection;
        private readonly AuthenticationBuilder _authBuilder;

        public IdServerBuilder(IServiceCollection serviceCollection, AuthenticationBuilder authBuilder)
        {
            _serviceCollection = serviceCollection;
            _authBuilder = authBuilder;
            _serviceCollection.AddSingleton<IKeyStore>(_keyStore);
        }

        public IServiceCollection Services => _serviceCollection;
        public InMemoryKeyStore KeyStore => _keyStore;

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
            var rsa = RSA.Create();
            var key = new RsaSecurityKey(rsa)
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
            _authBuilder
                .AddCookie(Constants.DefaultExternalCookieAuthenticationScheme);
            if(callback != null)
                callback(new AuthBuilder(_serviceCollection, _authBuilder));

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
