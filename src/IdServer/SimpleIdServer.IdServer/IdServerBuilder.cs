// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Consumers;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Notifications;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.UI.AuthProviders;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IdServerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly AuthenticationBuilder _authBuilder;

        public IdServerBuilder(IServiceCollection serviceCollection, AuthenticationBuilder authBuilder)
        {
            _serviceCollection = serviceCollection;
            _authBuilder = authBuilder;
        }

        public IServiceCollection Services => _serviceCollection;

        #region Authentication & Authorization

        public IdServerBuilder EnableConfigurableAuthentication()
        {
            _serviceCollection.AddTransient<IAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
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

        #region Console notification

        public IdServerBuilder AddConsoleNotification()
        {
            _serviceCollection.AddTransient<IAuthenticationMethodService, ConsoleAuthenticationService>();
            _serviceCollection.AddTransient<IUserNotificationService, ConsoleNotificationService>();
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

        /// <summary>
        /// Authorization server accepts authorization request data only via PAR.
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder PARIsRequired()
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.RequiredPushedAuthorizationRequest = true;
            });
            return this;
        }

        /// <summary>
        /// Use in memory implementation of mass transit.
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder UseInMemoryMassTransit()
        {
            _serviceCollection.AddMassTransit((o) =>
            {
                o.AddConsumer<IdServerEventsConsumer>();
                o.UsingInMemory((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                });
            });
            return this;
        }

        /// <summary>
        /// Configure and use mass transit.
        /// </summary>
        /// <param name="massTransitOptions"></param>
        /// <returns></returns>
        public IdServerBuilder UseMassTransit(Action<IBusRegistrationConfigurator> massTransitOptions)
        {
            _serviceCollection.AddMassTransit(massTransitOptions != null ? massTransitOptions : (o) =>
            {
                o.UsingInMemory();
            });
            return this;
        }

        #endregion
    }
}
